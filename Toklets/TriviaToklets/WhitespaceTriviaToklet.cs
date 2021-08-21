using System;

public sealed class WhitespaceTriviaToklet : ITriviaToklet<WhitespaceTriviaToken>
{
    public Predicate<IConsumer<char>> Condition
        => (input => input.Consume() != '\n' && Char.IsWhiteSpace(input.Current));

    public WhitespaceTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var startingPosition = input.Position;

        var whitespaceChar = input.Consume();

        if (!Char.IsWhiteSpace(whitespaceChar)) {
            throw Logger.Fatal(new InvalidCallException(new LocationRange(startingPosition, input.Position)));
        }

        int charCounter = 1;

        while (input.Consume(out char currChar) && currChar == whitespaceChar) charCounter++;

        input.Reconsume();

        return new WhitespaceTriviaToken(whitespaceChar, charCounter, new LocationRange(startingPosition, input.Position), trailing: tokenizer.ConsumeTrivia());
    }
}