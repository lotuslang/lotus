using System.Text;

public class Toklet : IToklet<Token>
{
    public Predicate<IConsumer<char>> Condition => (_ => true);

    public virtual Token Consume(IConsumer<char> input, Tokenizer _)
        => new(input.Consume().ToString(), TokenKind.delimiter, input.Position);
}