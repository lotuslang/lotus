
public interface IToklet<out T> where T : Token
{
    Predicate<IConsumer<char>> Condition { get; }

    T Consume(IConsumer<char> input, Tokenizer tokenizer);
}