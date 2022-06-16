using System;

public class Toklet : IToklet<Token>
{

    private static Toklet _instance = new();
    public static Toklet Instance => _instance;

	private Toklet() : base() { }

    public Predicate<IConsumer<char>> Condition => _condition;
	private static readonly Predicate<IConsumer<char>> _condition = (_ => true);

    public virtual Token Consume(IConsumer<char> input, Tokenizer _) {
        if (!input.Consume(out char currChar)) {
            return new Token(currChar.ToString(), TokenKind.EOF, input.Position, false);
        }

        return new(currChar.ToString(), currChar == ';' ? TokenKind.semicolon : TokenKind.delimiter, input.Position);
    }
}