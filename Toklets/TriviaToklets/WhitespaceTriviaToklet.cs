public sealed class WhitespaceTriviaToklet : ITriviaToklet<WhitespaceTriviaToken>
{
    public static readonly WhitespaceTriviaToklet Instance = new();

    public Predicate<IConsumer<char>> Condition => _condition;
	private static readonly Predicate<IConsumer<char>> _condition =
        (input =>
            input.Consume() != '\n' && Char.IsWhiteSpace(input.Current)
        );

    public WhitespaceTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var startingPosition = input.Position;

        var whitespaceChar = input.Consume();

        if (!Char.IsWhiteSpace(whitespaceChar)) {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Tokenizer, new LocationRange(startingPosition, input.Position)));
        }

        int charCounter = 1;

        while (input.Consume(out char currChar) && currChar == whitespaceChar) charCounter++;

        input.Reconsume();

        return new WhitespaceTriviaToken(
            whitespaceChar,
            charCounter,
            new LocationRange(startingPosition, input.Position)
        ) { TrailingTrivia = tokenizer.ConsumeTrivia() };

        // TODO: Shouldn't the tokenizer be responsible of consuming further trivia ?
    }
}