public sealed class CharsToklet : IToklet<Token>
{
    public readonly string matchString;
    public readonly TokenKind kind;

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
    private readonly Func<char, Func<IConsumer<char>>, bool> _condition;

    public CharsToklet(string match, TokenKind kind = TokenKind.delimiter) : base() {
        Debug.Assert(match.Length > 0);

        matchString = match;
        this.kind = kind;

        if (match.Length == 1) {
            _condition = (currChar, _) => currChar == matchString[0];
        } else {
            _condition = (currChar, getInput) => {
                if (currChar != matchString[0])
                    return false;

                var input = getInput();

                foreach (var c in matchString.AsSpan(1)) {
                    if (input.Consume() != c)
                        return false;
                }

                return true;
            };
        }
    }


    public Token Consume(IConsumer<char> input, Tokenizer _) {
        var currChar = input.Consume();
        var startPos = input.Position;

        int i = 0;

        do {
            Debug.Assert(currChar == matchString[i]);

            i++;
            currChar = input.Consume();
        } while (i < matchString.Length);

        input.Reconsume();

        return new Token(matchString, kind, new LocationRange(startPos, input.Position));
    }
}