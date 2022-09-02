public partial class Tokenizer : IConsumer<Token>
{
    protected Token _curr;
    public ref readonly Token Current => ref _curr;

    public Token Default {
        get => Token.NULL with { Location = Position };
    }

    protected Queue<Token> _reconsumeQueue;

    public LocationRange Position {
        get => Current.Location;
    }

    protected StringConsumer _input;

    public ReadOnlyGrammar Grammar { get; protected set; }

    protected Tokenizer() {
        _reconsumeQueue = new Queue<Token>();

        _input = new StringConsumer(Array.Empty<char>());

        _curr = Token.NULL;

        Grammar = new ReadOnlyGrammar();
    }

    protected Tokenizer(ReadOnlyGrammar grammar) : this() {
        Debug.Assert(grammar is not null);

        Grammar = grammar;
    }

    public Tokenizer(StringConsumer stringConsumer, ReadOnlyGrammar grammar) : this(grammar) {
        _input = stringConsumer.Clone();
    }

    public Tokenizer(IConsumer<char> consumer, ReadOnlyGrammar grammar) : this(grammar) {
        _input = new StringConsumer(consumer);
    }

    public Tokenizer(IConsumer<Token> tokenConsumer, ReadOnlyGrammar grammar) : this(new StringConsumer(""), grammar) {
        var cloned = tokenConsumer.Clone();
        while (cloned.Consume(out _)) {
            _reconsumeQueue.Enqueue(tokenConsumer.Current);
        }
    }

    public Tokenizer(Tokenizer tokenizer) : this(tokenizer, tokenizer.Grammar) { }

    public Tokenizer(Tokenizer tokenizer, ReadOnlyGrammar grammar) : this(tokenizer._input, grammar) {
        _reconsumeQueue = new Queue<Token>(tokenizer._reconsumeQueue);

        _curr = tokenizer._curr;
        _lastTok = tokenizer._lastTok;
    }

    public Tokenizer(Uri fileInfo, ReadOnlyGrammar grammar) : this(new StringConsumer(fileInfo), grammar) { }

    public Tokenizer(IEnumerable<char> collection, ReadOnlyGrammar grammar) : this(new StringConsumer(collection), grammar) { }

    public Tokenizer(IEnumerable<string> collection, ReadOnlyGrammar grammar) : this(new StringConsumer(collection), grammar) { }

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

        TriviaToken? leadingTrivia = null;

        if (!preserveTrivia && currChar != ',') {
            leadingTrivia = ConsumeTrivia();

            if (_input.Count == 0) {
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

    public TriviaToken? ConsumeTrivia() {
        if (!_input.Consume(out char currChar))
            return null;

        switch (currChar) {
            case '/':
                if (_input.Peek() is '/' or '*') {
                    _input.Reconsume();
                    return CommentTriviaToklet.Instance.Consume(_input, this);
                }

                _input.Reconsume();
                return null;
            case '\n':
                _input.Reconsume();
                return NewlineTriviaToklet.Instance.Consume(_input, this);
            case ' ':
                _input.Reconsume();
                return WhitespaceTriviaToklet.Instance.Consume(_input, this);
            default:
                _input.Reconsume();
                if (Char.IsWhiteSpace(currChar))
                    return WhitespaceTriviaToklet.Instance.Consume(_input, this);
                else
                    return null;
        }
    }

    public IConsumer<Token> Clone() => new Tokenizer(this);
    IConsumer<Token> IConsumer<Token>.Clone() => Clone();
}