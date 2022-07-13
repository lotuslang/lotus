public class Tokenizer : IConsumer<Token>
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
        if (grammar is null) {
            Logger.Warning(new InvalidCallError(ErrorArea.Tokenizer, Position) {
                Message = "Something tried to create a new Tokenizer with a null grammar."
                        + "That's not allowed, and might throw in future versions, but for now the grammar will just be empty...",
            });

            grammar = new ReadOnlyGrammar();
        }

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

        _curr = tokenizer.Current;
    }

    public Tokenizer(Uri fileInfo, ReadOnlyGrammar grammar) : this(new StringConsumer(fileInfo), grammar) { }

    public Tokenizer(IEnumerable<char> collection, ReadOnlyGrammar grammar) : this(new StringConsumer(collection), grammar) { }

    public Tokenizer(IEnumerable<string> collection, ReadOnlyGrammar grammar) : this(new StringConsumer(collection), grammar) { }

    public void Reconsume() {
        if (_reconsumeQueue.TryPeek(out var token) && Object.ReferenceEquals(token, Current)) {
            Logger.Warning("Calling reconsume multiple times in a row !", Position);
            return;
        }

        _reconsumeQueue.Enqueue(Current);
    }

    // Because of stupid interface rule
    public Token Peek() => Peek(preserveTrivia: false);

    public Token Peek(bool preserveTrivia = false) {
        var oldCurrent = Current.ShallowClone();

        var output = Consume(preserveTrivia);

        _reconsumeQueue.Enqueue(output);

        _curr = oldCurrent;

        return output;
    }

    public Token[] Peek(int n, bool preserveTrivia = false) {

        var oldCurrent = Current.ShallowClone();

        var output = new Token[n];

        for (int i = 0; i < n; i++) {
            output[n] = Consume(preserveTrivia);
        }

        _curr = oldCurrent;

        foreach (var token in output.Reverse()) {
            // no we don't put it in the same loop as Consume() because then it would just consume the reconsume indefinitely
            _reconsumeQueue.Enqueue(token);
        }

        return output;
    }

    public bool Consume(out Token result) {
        result = Consume();

        return result.Kind != TokenKind.EOF;
    }

    // Because of stupid interface rule
    public ref readonly Token Consume() => ref Consume(preserveTrivia: false);

    public ref readonly Token Consume(bool preserveTrivia = false) {

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

            _curr = Grammar.MatchToklet(_input).Consume(_input, this);

            if (leadingTrivia != null)
                Current.AddLeadingTrivia(leadingTrivia);

            if (_input.Peek() != '\n') {
                var trailingTrivia = ConsumeTrivia();

                if (trailingTrivia != null)
                    Current.AddTrailingTrivia(trailingTrivia);
            }
        } else {
            _curr = Grammar.MatchToklet(_input).Consume(_input, this);
        }

        return ref _curr;
    }

    public TriviaToken? ConsumeTrivia() {
        var triviaToklet = Grammar.MatchTriviaToklet(_input);

        if (triviaToklet is null) return null;

        return triviaToklet.Consume(_input, this);
    }

    public IConsumer<Token> Clone() => new Tokenizer(this);
    IConsumer<Token> IConsumer<Token>.Clone() => Clone();
}