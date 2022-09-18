using System.Text;

namespace Lotus.Syntax;

public sealed class CommentTriviaToklet : ITriviaToklet<CommentTriviaToken>
{
    public static readonly CommentTriviaToklet Instance = new();

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
	private static readonly Func<char, Func<IConsumer<char>>, bool> _condition =
        ((currChar, getInput) =>
            currChar is '/' && (getInput().Consume() is '/' or '*')
        );

    private CommentTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer, bool isInner) {
        var startingPosition = input.Position;

        var isValid = true;

        var currChar = input.Consume();

        Debug.Assert(currChar is '/');

        currChar = input.Consume();

        Debug.Assert(currChar is '/' or '*');

        if (currChar is '/') {
            var strBuilder = new StringBuilder("//");

            while (input.Consume(out currChar) && currChar is not '\n') {
                strBuilder.Append(currChar);
            }

            strBuilder.Append('\n');

            return new CommentTriviaToken(
                strBuilder.ToString(),
                new LocationRange(startingPosition, input.Position)
            ) { IsValid = true, TrailingTrivia = tokenizer.ConsumeTrivia() };
        }

        if (currChar is '*') {
            var strBuilder = new StringBuilder("/*");

            var inner = ImmutableArray.CreateBuilder<CommentTriviaToken>();

            while (input.Consume(out currChar) && !(currChar is '*' && input.Peek() is '/')) {
                if (currChar is '/' && input.Peek() is '*') {
                    // reconsume the '/'
                    input.Reconsume();

                    inner.Add(Consume(input, tokenizer, isInner: true));

                    strBuilder.Append(inner.Last().Representation);

                    continue;
                }

                strBuilder.Append(currChar);
            }

            if (input.Current == input.Default) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                    In = "a multi-line comment",
                    Expected = "the comment delimiter '*/'",
                    Location = new LocationRange(startingPosition, input.Position)
                });

                isValid = false;
            }

            // consumes the remaining '/'
            _ = input.Consume();

            strBuilder.Append("*/");

            return new CommentTriviaToken(
                strBuilder.ToString(),
                new LocationRange(startingPosition, input.Position),
                inner: inner.ToImmutable()
            ) { IsValid = isValid, TrailingTrivia = isInner ? null : tokenizer.ConsumeTrivia() };
        }

        Debug.Fail("wtf even happened??");
        throw null;
    }

    public CommentTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer)
        => Consume(input, tokenizer, isInner: false);
}