public sealed class CharToklet : IToklet<Token>
{
    private readonly string _matchChrAsStr;
    public readonly char matchChr;
    public readonly TokenKind kind;

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
    private readonly Func<char, Func<IConsumer<char>>, bool> _condition;

    public CharToklet(char match, TokenKind kind = TokenKind.delimiter) : base() {
        matchChr = match;
        this.kind = kind;
        _matchChrAsStr = matchChr.ToString();
        _condition = (currChar, _) => currChar == matchChr;
    }

    public Token Consume(IConsumer<char> input, Tokenizer _) {
        var currChar = input.Consume();
        var charPos = input.Position;

        Debug.Assert(currChar == matchChr);

        return new Token(_matchChrAsStr, kind, charPos);
    }
}