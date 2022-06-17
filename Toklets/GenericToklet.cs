internal sealed class GenericToklet : Toklet
{
    internal GenericToklet() : base() { }

    public override Predicate<IConsumer<char>> Condition => _condition;
    private static readonly Predicate<IConsumer<char>> _condition = (_ => true);

    public override Token Consume(IConsumer<char> input, Tokenizer _) {
        if (!input.Consume(out char currChar)) {
            return new Token(currChar.ToString(), TokenKind.EOF, input.Position, false);
        }

        return new(currChar.ToString(), TokenKind.delimiter, input.Position);
    }
}