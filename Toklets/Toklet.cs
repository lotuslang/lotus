public abstract class Toklet : IToklet<Token>
{
    public static readonly Toklet Default = new Generic();

	protected Toklet() : base() { }

    public abstract Predicate<IConsumer<char>> Condition { get; }

    public abstract Token Consume(IConsumer<char> input, Tokenizer _);

    public static IToklet<Token> From(char c, TokenKind kind = TokenKind.delimiter)
        => new CharToklet(c, kind);

    public static IToklet<Token> From(string s, TokenKind kind = TokenKind.delimiter)
        => new CharsToklet(s, kind);

    private sealed class Generic : Toklet {
        private static readonly Predicate<IConsumer<char>> _condition = (_ => true);
        public override Predicate<IConsumer<char>> Condition => _condition;

        public override Token Consume(IConsumer<char> input, Tokenizer _) {
            if (!input.Consume(out char currChar)) {
                return new Token(currChar.ToString(), TokenKind.EOF, input.Position, false);
            }

            return new(currChar.ToString(), TokenKind.delimiter, input.Position);
        }
    }
}