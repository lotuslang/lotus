using System.Text;

public class Toklet : IToklet<Token>
{

    private static Toklet _instance = new();
    public static Toklet Instance => _instance;

	private Toklet() : base() { }

    public Predicate<IConsumer<char>> Condition => (_ => true);

    public virtual Token Consume(IConsumer<char> input, Tokenizer _)
        => new(input.Consume().ToString(), TokenKind.delimiter, input.Position);
}