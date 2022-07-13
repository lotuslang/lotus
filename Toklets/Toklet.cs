public abstract class Toklet : IToklet<Token>
{
    public static readonly Toklet Default = new Generic();

	protected Toklet() : base() { }

    public abstract ref readonly Func<char, Func<IConsumer<char>>, bool> Condition { get; }

    public abstract Token Consume(IConsumer<char> input, Tokenizer _);

    public static IToklet<Token> From(char c, TokenKind kind = TokenKind.delimiter)
        => new CharToklet(c, kind);

    public static IToklet<Token> From(string s, TokenKind kind = TokenKind.delimiter)
        => new CharsToklet(s, kind);

    private sealed class Generic : Toklet {
        private static readonly Func<char, Func<IConsumer<char>>, bool> _condition = ((_, _) => true);
        public override ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;

        public override Token Consume(IConsumer<char> input, Tokenizer _) {
            if (!input.Consume(out char currChar)) {
                return new Token(currChar.ToString(), TokenKind.EOF, input.Position, false);
            }

            return new(currChar.ToString(), TokenKind.delimiter, input.Position);
        }
    }
}