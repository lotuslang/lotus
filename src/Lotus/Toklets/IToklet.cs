namespace Lotus.Syntax;

public interface IToklet<out T> where T : Token
{
    ref readonly Func<char, Func<IConsumer<char>>, bool> Condition { get; }

    T Consume(IConsumer<char> input, Tokenizer tokenizer);
}