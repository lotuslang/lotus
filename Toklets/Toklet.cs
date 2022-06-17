using System;

public abstract class Toklet : IToklet<Token>
{
    private static GenericToklet _instance = new GenericToklet();
    public static Toklet Instance => _instance;

	protected Toklet() : base() { }

    public abstract Predicate<IConsumer<char>> Condition { get; }

    public abstract Token Consume(IConsumer<char> input, Tokenizer _);

    public static Toklet From(char c, TokenKind kind = TokenKind.delimiter)
        => new CharToklet(c, kind);

    public static Toklet From(string s, TokenKind kind = TokenKind.delimiter)
        => new CharsToklet(s, kind);
}