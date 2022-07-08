public sealed class CharsToklet : IToklet<Token>
{
    public readonly string matchString;
    public readonly TokenKind kind;

    public Predicate<IConsumer<char>> Condition => _condition;
    private readonly Predicate<IConsumer<char>> _condition;

    public CharsToklet(string match, TokenKind kind = TokenKind.delimiter) : base() {
        if (match.Length == 0) {
            throw Logger.Fatal(new ArgumentException("Cannot create a generic toklet from an empty string"));
        }

        matchString = match;
        this.kind = kind;
        _condition = (input) => {
            foreach (var c in matchString) {
                if (input.Consume() != c)
                    return false;
            }

            return true;
        };
    }


    public Token Consume(IConsumer<char> input, Tokenizer _) {
        var currChar = input.Consume();
        var startPos = input.Position;

        int i = 0;

        do {
            if (currChar != matchString[i++])
                throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, new LocationRange(startPos, input.Position)));
            currChar = input.Consume();
        } while (i < matchString.Length);

        input.Reconsume();

        return new Token(matchString, kind, new LocationRange(startPos, input.Position));
    }
}