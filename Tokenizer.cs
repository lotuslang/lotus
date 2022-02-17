public class Tokenizer : IConsumer<Token>
{

    public Token Current { get; protected set; }

    public Token Default {
        get => Token.NULL with { Location = Position };
    }

    protected Queue<Token> reconsumeQueue;

    public LocationRange Position {
        get => Current.Location;
    }

    protected StringConsumer input;

    public ReadOnlyGrammar Grammar { get; protected set; }

    protected Tokenizer() {
        reconsumeQueue = new Queue<Token>();

        input = new StringConsumer(Array.Empty<char>());

        Current = Token.NULL;

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
        input = stringConsumer.Clone();
    }

    public Tokenizer(IConsumer<char> consumer, ReadOnlyGrammar grammar) : this(grammar) {
        input = new StringConsumer(consumer);
    }

    public Tokenizer(IConsumer<Token> tokenConsumer, ReadOnlyGrammar grammar) : this(new StringConsumer(""), grammar) {
        var cloned = tokenConsumer.Clone();
        while (cloned.Consume(out _)) {
            reconsumeQueue.Enqueue(tokenConsumer.Current);
        }
    }

    public Tokenizer(Tokenizer tokenizer) : this(tokenizer, tokenizer.Grammar) { }

    public Tokenizer(Tokenizer tokenizer, ReadOnlyGrammar grammar) : this(tokenizer.input, grammar) {
        reconsumeQueue = new Queue<Token>(tokenizer.reconsumeQueue);

        Current = tokenizer.Current;
    }

    public Tokenizer(Uri fileInfo, ReadOnlyGrammar grammar) : this(new StringConsumer(fileInfo), grammar) { }

    public Tokenizer(IEnumerable<char> collection, ReadOnlyGrammar grammar) : this(new StringConsumer(collection), grammar) { }

    public Tokenizer(IEnumerable<string> collection, ReadOnlyGrammar grammar) : this(new StringConsumer(collection), grammar) { }

    public void Reconsume() {
        if (reconsumeQueue.TryPeek(out var token) && Object.ReferenceEquals(token, Current)) {
            Logger.Warning("Calling reconsume multiple times in a row !", Position);
            return;
        }

        reconsumeQueue.Enqueue(Current);
    }

    // Because of stupid interface rule
    public Token Peek() => Peek(preserveTrivia: false);

    public Token Peek(bool preserveTrivia = false) {
        var oldCurrent = new Token(
            Current.Representation,
            Current.Kind,
            Current.Location,
            Current.IsValid
        ) {LeadingTrivia = Current.LeadingTrivia, TrailingTrivia = Current.TrailingTrivia} ;

        var output = Consume(preserveTrivia);

        reconsumeQueue.Enqueue(output);

        Current = oldCurrent;

        return output;
    }

    public Token[] Peek(int n, bool preserveTrivia = false) {

        var oldCurrent = new Token(
            Current.Representation,
            Current.Kind,
            Current.Location,
            Current.IsValid
        ) { LeadingTrivia = Current.LeadingTrivia, TrailingTrivia = Current.TrailingTrivia };

        var output = new Token[n];

        for (int i = 0; i < n; i++) {
            output[n] = Consume(preserveTrivia);
        }

        Current = oldCurrent;

        foreach (var token in output.Reverse()) {
            // no we don't put it in the same loop as Consume() because then it would just consume the reconsume indefinitely
            reconsumeQueue.Enqueue(token);
        }

        return output;
    }

    public bool Consume(out Token result) {
        result = Consume();

        return result.Kind != TokenKind.EOF;
    }

    // Because of stupid interface rule
    public Token Consume() => Consume(preserveTrivia: false);

    public Token Consume(bool preserveTrivia = false) {

        // If we are instructed to reconsume the last token, then dequeue a token from the reconsumeQueue and return it
        if (reconsumeQueue.Count != 0) {
            return (Current = reconsumeQueue.Dequeue());
        }

        // If there is nothing left to consume, return an EOF token
        if (!input.Consume(out var currChar)) {
            return (Current = Default);
        }

        input.Reconsume();

        if (!preserveTrivia && currChar != ',') {
            var leadingTrivia = ConsumeTrivia();

            Current = Grammar.MatchToklet(input).Consume(input, this);

            if (leadingTrivia != null)
                Current.AddLeadingTrivia(leadingTrivia);

            if (input.Peek() != '\n') {
                var trailingTrivia = ConsumeTrivia();

                if (trailingTrivia != null)
                    Current.AddTrailingTrivia(trailingTrivia);
            }
        } else {
            Current = Grammar.MatchToklet(input).Consume(input, this);
        }

        return Current;
    }

    public TriviaToken? ConsumeTrivia() {
        var triviaToklet = Grammar.MatchTriviaToklet(input);

        if (triviaToklet is null) return null;

        return triviaToklet.Consume(input, this);
    }

    public IConsumer<Token> Clone() => new Tokenizer(this);
    IConsumer<Token> IConsumer<Token>.Clone() => Clone();
}