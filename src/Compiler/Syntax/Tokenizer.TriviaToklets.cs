using System.Text;

namespace Lotus.Syntax;

public partial class Tokenizer : IConsumer<Token>
{
    public TriviaToken? ConsumeTrivia() {
        if (!_input.Consume(out char currChar))
            return null;

        switch (currChar) {
            case '/':
                if (_input.Peek() is '/' or '*')
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

        currChar = _input.Consume();

        Debug.Assert(currChar is '/' or '*');

        if (currChar is '/') {
            var strBuilder = new StringBuilder("//");

            while (_input.Consume(out currChar) && currChar is not '\n') {
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

            while (_input.Consume(out currChar) && !(currChar is '*' && _input.Peek() is '/')) {
                if (currChar is '/' && _input.Peek() is '*') {
                    inner.Add(ConsumeCommentTrivia(isInner: true));

                    strBuilder.Append(inner.Last().Representation);

                    continue;
                }

                strBuilder.Append(currChar);
            }

            if (_input.Current == _input.Default) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                    In = "a multi-line comment",
                    Expected = "the comment delimiter '*/'",
                    Location = new LocationRange(startingPosition, _input.Position)
                });

                isValid = false;
            }

            // consumes the remaining '/'
            _ = _input.Consume();

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

        Debug.Assert(_input.Current == '\n');

        int charCounter = 1;

        char currChar;

        while (_input.Consume(out currChar) && currChar == '\n') charCounter++;

        // if it's an eof, we shouldn't reconsume cause we would just go back and be stuck otherwise
        if (currChar != _input.Default)
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

        while (_input.Consume(out char currChar) && currChar == whitespaceChar) charCounter++;

        _input.Reconsume();

        return new WhitespaceTriviaToken(
            whitespaceChar,
            charCounter,
            new LocationRange(startingPosition, _input.Position)
        ) { TrailingTrivia = ConsumeTrivia() };

        // TODO: Shouldn't the tokenizer be responsible of consuming further trivia ?
    }
}