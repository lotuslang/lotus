internal class CharToklet : Toklet
{
    private readonly string _matchChrAsStr;
    public readonly char matchChr;
    public readonly TokenKind kind;

    public CharToklet(char match, TokenKind kind = TokenKind.delimiter) : base() {
        matchChr = match;
        this.kind = kind;
        _matchChrAsStr = matchChr.ToString();
        _condition = (input) => input.Current == matchChr;
    }

    public override Predicate<IConsumer<char>> Condition => _condition;
    private readonly Predicate<IConsumer<char>> _condition;

    public override Token Consume(IConsumer<char> input, Tokenizer _) {
        var currChar = input.Consume();
        var charPos = input.Position;

        if (matchChr != currChar)
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, charPos));

        return new Token(_matchChrAsStr, kind, charPos);
    }
}