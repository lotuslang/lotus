using System.Text;

namespace Lotus.Syntax;

public partial class Tokenizer
{
    public TriviaToken? ConsumeTrivia() {
        if (!_input.TryConsumeChar(out char currChar))
            return null;

        switch (currChar) {
            case '/':
                if (_input.PeekNextChar() is '/' or '*')
                    return ConsumeCommentTrivia(false);

                _input.Reconsume();
                return null;
            case '\n':
                return ConsumeNewlineTrivia();
            case ' ':
                return ConsumeWhitespaceTrivia();
            default:
                if (Char.IsWhiteSpace(currChar))
                    return ConsumeWhitespaceTrivia();

                _input.Reconsume();
                return null;
        }
    }

    private CommentTriviaToken ConsumeCommentTrivia(bool isInner) {
        var startingPosition = _input.Position;

        var isValid = true;

        var currChar = _input.Current;

        Debug.Assert(currChar is '/');

        currChar = _input.ConsumeChar();

        Debug.Assert(currChar is '/' or '*');

        if (currChar is '/') {
            var strBuilder = new StringBuilder("//");

            while (_input.TryConsumeChar(out currChar) && currChar is not '\n') {
                strBuilder.Append(currChar);
            }

            strBuilder.Append('\n');

            return new CommentTriviaToken(
                strBuilder.ToString(),
                new LocationRange(startingPosition, _input.Position)
            ) { IsValid = true, TrailingTrivia = ConsumeTrivia() };
        }

        if (currChar is '*') {
            var strBuilder = new StringBuilder("/*");

            var inner = ImmutableArray.CreateBuilder<CommentTriviaToken>();

            while (_input.TryConsumeChar(out currChar) && !(currChar is '*' && _input.PeekNextChar() is '/')) {
                if (currChar is '/' && _input.PeekNextChar() is '*') {
                    inner.Add(ConsumeCommentTrivia(isInner: true));

                    strBuilder.Append(inner.Last().Representation);

                    continue;
                }

                strBuilder.Append(currChar);
            }

            if (_input.EndOfStream) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                    In = "a multi-line comment",
                    Expected = "the comment delimiter '*/'",
                    Location = new LocationRange(startingPosition, _input.Position)
                });

                isValid = false;
            }

            // consumes the remaining '/'
            _ = _input.ConsumeChar();

            strBuilder.Append("*/");

            return new CommentTriviaToken(
                strBuilder.ToString(),
                new LocationRange(startingPosition, _input.Position),
                inner: inner.ToImmutable()
            ) { IsValid = isValid, TrailingTrivia = isInner ? null : ConsumeTrivia() };
        }

        Debug.Fail("wtf even happened??");
        throw null;
    }

    private NewlineTriviaToken ConsumeNewlineTrivia() {
        var startingPosition = _input.Position;

        Debug.Assert(_input.Current is '\n');

        int charCounter = 1;

        while (_input.TryConsumeChar(out char currChar) && currChar == '\n') charCounter++;

        // if it's an eof, we shouldn't reconsume cause we would just go back and be stuck otherwise
        if (!_input.EndOfStream)
            _input.Reconsume();

        return new NewlineTriviaToken(
            charCounter,
            new LocationRange(startingPosition, _input.Position)
        ) { TrailingTrivia = ConsumeTrivia() };
    }

    private WhitespaceTriviaToken ConsumeWhitespaceTrivia() {
        var startingPosition = _input.Position;

        var whitespaceChar = _input.Current;

        Debug.Assert(Char.IsWhiteSpace(whitespaceChar));

        int charCounter = 1;

        while (_input.TryConsumeChar(out char currChar) && currChar == whitespaceChar) charCounter++;

        // same as NewlineTrivia, we don't want to be stuck if there's whitespace at the end of a file
        if (!_input.EndOfStream)
            _input.Reconsume();

        return new WhitespaceTriviaToken(
            whitespaceChar,
            charCounter,
            new LocationRange(startingPosition, _input.Position)
        ) { TrailingTrivia = ConsumeTrivia() };

        // todo(parsing): Shouldn't the tokenizer be responsible of consuming further trivia ?
    }
}