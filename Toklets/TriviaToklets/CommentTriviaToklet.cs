using System.Text;

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

        if (currChar != '/') {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Tokenizer, input.Position));
        }

        currChar = input.Consume();

        if (currChar == '/') {

            var strBuilder = new StringBuilder("//");

            while (input.Consume(out currChar) && currChar != '\n') {
                strBuilder.Append(currChar);
            }

            strBuilder.Append('\n');

            return new CommentTriviaToken(
                strBuilder.ToString(),
                new LocationRange(startingPosition, input.Position)
            ) { TrailingTrivia = tokenizer.ConsumeTrivia() };
        }

        if (currChar == '*') {
            var strBuilder = new StringBuilder("/*");

            var inner = new List<CommentTriviaToken>();

            while (input.Consume(out currChar) && !(currChar == '*' && input.Peek() == '/')) {
                if (currChar == '/' && input.Peek() == '*') {

                    // reconsume the '/'
                    input.Reconsume();

                    inner.Add(Consume(input, tokenizer, isInner: true));

                    strBuilder.Append(inner[^1].Representation);

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
            input.Consume();

            strBuilder.Append("*/");

            return new CommentTriviaToken(
                strBuilder.ToString(),
                new LocationRange(startingPosition, input.Position),
                inner: inner,
                isValid: isValid
            ) { TrailingTrivia = isInner ? null : tokenizer.ConsumeTrivia() };
        }

        throw Logger.Fatal(new InvalidCallError(ErrorArea.Tokenizer, new LocationRange(startingPosition, input.Position)));
    }

    public CommentTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer)
        => Consume(input, tokenizer, isInner: false);
}