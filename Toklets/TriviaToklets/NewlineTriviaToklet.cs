using System;

public sealed class NewlineTriviaToklet : ITriviaToklet<NewlineTriviaToken>
{
    public Predicate<IConsumer<char>> Condition
        => (input => input.Consume() == '\n');

    public NewlineTriviaToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var startingPosition = input.Position;

        int charCounter = 0;

        while (input.Consume() == '\n') charCounter++;

        input.Reconsume();

        return new NewlineTriviaToken(charCounter, startingPosition, trailing: tokenizer.ConsumeTrivia());
    }
}