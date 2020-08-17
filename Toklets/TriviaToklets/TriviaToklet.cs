using System;
using System.Diagnostics.CodeAnalysis;

public class TriviaToklet : ITriviaToklet<TriviaToken>
{
    public Predicate<IConsumer<char>> Condition
        => (_ => false);

    //[DoesNotReturn]
    public TriviaToken Consume(IConsumer<char> input, Tokenizer _)
        => throw Logger.Fatal(new InvalidCallException(input.Position));
}