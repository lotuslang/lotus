namespace Lotus.Syntax;

public sealed partial class Tokenizer : IConsumer<Token>
{
    private Token _curr;
    public ref readonly Token Current => ref _curr;

    public Token Default => Token.NULL with { Location = Position };

    private readonly Queue<Token> _reconsumeQueue;

    public LocationRange Position => Current.Location;

    private readonly StringConsumer _input;

    private Tokenizer() {
        _reconsumeQueue = new Queue<Token>();

        _input = new StringConsumer(Array.Empty<char>());

        _curr = Token.NULL;
    }

    public Tokenizer(StringConsumer stringConsumer) : this() {
        _input = stringConsumer.Clone();
    }

    public Tokenizer(IConsumer<char> consumer) : this() {
        _input = new StringConsumer(consumer);
    }

    public Tokenizer(IConsumer<Token> tokenConsumer) : this(new StringConsumer("")) {
        var cloned = tokenConsumer.Clone();
        while (cloned.Consume(out _)) {
            _reconsumeQueue.Enqueue(tokenConsumer.Current);
        }
    }

    public Tokenizer(Tokenizer tokenizer) : this(tokenizer._input) {
        _reconsumeQueue = new Queue<Token>(tokenizer._reconsumeQueue);

        _curr = tokenizer._curr;
        _lastTok = tokenizer._lastTok;
    }

    public Tokenizer(Uri fileInfo) : this(new StringConsumer(fileInfo)) { }

    public Tokenizer(IEnumerable<char> collection) : this(new StringConsumer(collection)) { }

    private Token _lastTok = Token.NULL;
    public void Reconsume() {
        Debug.Assert(!_reconsumeQueue.TryPeek(out var token) || !Object.ReferenceEquals(token, Current));

        _reconsumeQueue.Enqueue(Current);
        _curr = _lastTok;
    }

    // Because of stupid interface rule
    Token IConsumer<Token>.Peek() => Peek(preserveTrivia: false);

    public Token Peek(bool preserveTrivia = false) {
        var oldLastTok = _lastTok.ShallowClone();

        var output = Consume(preserveTrivia);

        _reconsumeQueue.Enqueue(output);

        _curr = _lastTok;
        _lastTok = oldLastTok;

        return output;
    }

    public bool Consume(out Token result) {
        result = Consume();

        return result.Kind != TokenKind.EOF;
    }

    // Because of stupid interface rule
    ref readonly Token IConsumer<Token>.Consume() => ref Consume(preserveTrivia: false);

    public ref readonly Token Consume(bool preserveTrivia = false) {
        _lastTok = _curr;

        // If we are instructed to reconsume the last token, then dequeue a token from the reconsumeQueue and return it
        if (_reconsumeQueue.Count != 0) {
            _curr = _reconsumeQueue.Dequeue();
            return ref _curr;
        }

        // If there is nothing left to consume, return an EOF token
        if (!_input.Consume(out var currChar)) {
            _curr = Default;
            return ref _curr;
        }

        _input.Reconsume();

        if (!preserveTrivia && currChar != ',') {
            var leadingTrivia = ConsumeTrivia();

            if (_input.Unconsumed == 0) {
                _curr = Default with { LeadingTrivia = leadingTrivia };
                return ref _curr;
            }

            _curr = ConsumeTokenCore();

            if (leadingTrivia != null)
                _curr.AddLeadingTrivia(leadingTrivia);

            if (_input.Peek() != '\n') {
                var trailingTrivia = ConsumeTrivia();

                if (trailingTrivia != null)
                    _curr.AddTrailingTrivia(trailingTrivia);
            }
        } else {
            _curr = ConsumeTokenCore();
        }

        return ref _curr;
    }

    public IConsumer<Token> Clone() => new Tokenizer(this);
    IConsumer<Token> IConsumer<Token>.Clone() => Clone();
}