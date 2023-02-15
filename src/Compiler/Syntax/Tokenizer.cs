namespace Lotus.Syntax;

public sealed partial class Tokenizer
{
    private Token _lastTok = Token.NULL;

    private Token _curr = Token.NULL;
    public ref readonly Token Current => ref _curr;

    public Token Default => Token.NULL with { Location = Position };

    public LocationRange Position => Current.Location;

    private readonly Stack<Token> _reconsumeStack;

    private readonly TextStream _input;

    private Tokenizer() {
        _reconsumeStack = new Stack<Token>(2);
        _input = new(ImmutableArray<string>.Empty, "");
    }

    private Tokenizer(IEnumerable<Token> tokens, string filename) {
        _reconsumeStack = new Stack<Token>(tokens.Reverse());
        _input = new TextStream(ImmutableArray<string>.Empty, filename);
    }

    private Tokenizer(Tokenizer tokenizer) {
        _reconsumeStack = tokenizer._reconsumeStack.Clone();
        _input = tokenizer._input.Clone();
    }

    public Tokenizer(TextStream stream) : this() {
        _input = stream.Clone();
    }

    public void Reconsume() {
        if (_reconsumeStack.TryPeek(out var token)) {
            // check that we're not gonna reconsume the same token twice
            Debug.Assert(!Object.ReferenceEquals(token, Current));
        }

        _reconsumeStack.Push(Current);
        _curr = _lastTok;
    }

    public Token Peek(bool preserveTrivia = false) {
        var oldLastTok = _lastTok.ShallowClone();

        var output = Consume(preserveTrivia);

        _reconsumeStack.Push(output);

        _curr = _lastTok;
        _lastTok = oldLastTok;

        return output;
    }

    public bool TryConsume(out Token result) {
        result = Consume();

        return result.Kind != TokenKind.EOF;
    }

    public ref readonly Token Consume(bool preserveTrivia = false) {
        _lastTok = _curr;

        // If we are instructed to reconsume the last token, then dequeue a token from the reconsumeQueue and return it
        if (_reconsumeStack.Count != 0) {
            _curr = _reconsumeStack.Pop();
            return ref _curr;
        }

        // If there is nothing left to consume, return an EOF token
        if (!_input.TryConsumeChar(out var currChar)) {
            _curr = Default;
            return ref _curr;
        }

        _input.Reconsume();

        if (!preserveTrivia && currChar != ',') {
            var leadingTrivia = ConsumeTrivia();

            _curr = ConsumeTokenCore();

            if (leadingTrivia != null)
                _curr.AddLeadingTrivia(leadingTrivia);

            if (_input.PeekNextChar() != '\n') {
                var trailingTrivia = ConsumeTrivia();

                if (trailingTrivia != null)
                    _curr.AddTrailingTrivia(trailingTrivia);
            }
        } else {
            _curr = ConsumeTokenCore();
        }

        return ref _curr;
    }

    public Tokenizer Clone() => new(this);
    internal static Tokenizer FromExtractedTokens(IEnumerable<Token> tokens, string originalFile)
        => new(tokens, originalFile);
}