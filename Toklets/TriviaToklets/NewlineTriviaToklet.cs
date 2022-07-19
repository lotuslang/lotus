public sealed class NewlineTriviaToklet : ITriviaToklet<NewlineTriviaToken>
{
    public static readonly NewlineTriviaToklet Instance = new();

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
	private static readonly Func<char, Func<IConsumer<char>>, bool> _condition =
        ((currChar, _) => currChar is '\n');

    public NewlineTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var startingPosition = input.Position;

        if (input.Consume() != '\n' && input.Current != '\r')
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Tokenizer, new LocationRange(startingPosition, input.Position)));

        int charCounter = 1;

        char currChar;

        while (input.Consume(out currChar) && currChar == '\n') charCounter++;

        // if it's an eof, we shouldn't reconsume cause we would just go back and be stuck otherwise
        if (currChar != input.Default)
            input.Reconsume();

        return new NewlineTriviaToken(
            charCounter,
            new LocationRange(startingPosition, input.Position)
        ) { TrailingTrivia = tokenizer.ConsumeTrivia() };
    }
}