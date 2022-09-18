namespace Lotus.Syntax;

public sealed class WhitespaceTriviaToklet : ITriviaToklet<WhitespaceTriviaToken>
{
    public static readonly WhitespaceTriviaToklet Instance = new();

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
	private static readonly Func<char, Func<IConsumer<char>>, bool> _condition =
        ((currChar, _) =>
            currChar != '\n' && Char.IsWhiteSpace(currChar)
        );

    public WhitespaceTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var startingPosition = input.Position;

        var whitespaceChar = input.Consume();

        Debug.Assert(Char.IsWhiteSpace(whitespaceChar));

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