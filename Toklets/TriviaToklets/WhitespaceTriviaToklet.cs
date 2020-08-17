using System;

public sealed class WhitespaceTriviaToklet : ITriviaToklet<WhitespaceTriviaToken>
{
    public Predicate<IConsumer<char>> Condition
        => (input =>  {

                if (input.Consume() == '\n') return false;

                if (Char.IsWhiteSpace(input.Current)) return true;

                return false;
            }
        );

    public WhitespaceTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var startingPosition = input.Position;

        var whitespaceChar = input.Consume();

        if (!Char.IsWhiteSpace(whitespaceChar)) {
            throw Logger.Fatal(new InvalidCallException(input.Position));
        }

        int charCounter = 1;

        while (input.Consume(out char currChar) && currChar == whitespaceChar) charCounter++;

        input.Reconsume();

        return new WhitespaceTriviaToken(whitespaceChar, charCounter, startingPosition, trailing: tokenizer.ConsumeTrivia());
    }
}