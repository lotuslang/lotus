namespace Lotus.Syntax;

public sealed partial class Tokenizer
{
    private readonly Stack<Token> _reconsumeStack;
    private readonly TextStream _input;
    private Token _lastTok = Token.NULL;

    public Token Current { get; private set; } = Token.NULL;

    public Token Default => Token.NULL with { Location = Position };

    public LocationRange Position => Current.Location;

    public bool EndOfStream { get; private set; }

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
        Current = Current with { Location = _input.Position };
        EndOfStream = tokenizer.EndOfStream;
    }

    public Tokenizer(TextStream stream) : this() {
        _input = stream.Clone();
        Current = Current with { Location = _input.Position };
    }

    public void Reconsume() {
        if (_reconsumeStack.TryPeek(out var token)) {
            // check that we're not gonna reconsume the same token twice
            Debug.Assert(!Object.ReferenceEquals(token, Current));
        }

        _reconsumeStack.Push(Current);
        Current = _lastTok;

        EndOfStream = Current.Kind == TokenKind.EOF;
    }

    public Token Peek(bool preserveTrivia = false) {
        var oldLastTok = _lastTok;
        var eos = EndOfStream;

        var output = Consume(preserveTrivia);

        EndOfStream = eos;
        _reconsumeStack.Push(output);

        Current = _lastTok;
        _lastTok = oldLastTok;

        return output;
    }

    public bool TryConsume(out Token result) {
        result = Consume();

        return !EndOfStream;
    }

    public Token Consume(bool preserveTrivia = false) {
        _lastTok = Current;

        // If we are instructed to reconsume the last token, then dequeue a token from the reconsumeQueue and return it
        if (_reconsumeStack.Count != 0) {
            Current = _reconsumeStack.Pop();
            EndOfStream = Current.Kind == TokenKind.EOF;
            return Current;
        }

        // If there is nothing left to consume, return an EOF token
        if (_input.EndOfStream) {
            Current = Default;
            EndOfStream = true;
            return Current;
        }

        var currChar = _input.PeekNextChar();

        if (!preserveTrivia && currChar != ',') {
            var leadingTrivia = ConsumeTrivia();

            Current = ConsumeTokenCore();

            if (leadingTrivia != null)
                Current.AddLeadingTrivia(leadingTrivia);

            if (_input.PeekNextChar() != '\n') {
                var trailingTrivia = ConsumeTrivia();

                if (trailingTrivia != null) {
                    Current.AddTrailingTrivia(trailingTrivia);
                }
            }
        } else {
            Current = ConsumeTokenCore();
        }

        if (Current.Kind == TokenKind.EOF)
            EndOfStream = true;

        return Current;
    }

    public Tokenizer Clone() => new(this);
    internal static Tokenizer FromExtractedTokens(IEnumerable<Token> tokens, string originalFile)
        => new(tokens, originalFile);
}