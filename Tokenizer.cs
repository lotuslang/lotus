using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;


public class Tokenizer : IConsumer<Token>
{

    protected readonly List<Toklet> toklets;

    protected readonly List<TriviaToklet> triviaToklets;

    public Token Current { get; protected set; }

    protected Queue<Token> reconsumeQueue;

    public Location Position {
        get => input.Position;
    }

    protected StringConsumer input;

    private Tokenizer() {
        reconsumeQueue = new Queue<Token>();

        Current = new Token('\0', TokenKind.delim, default(Location));

        triviaToklets = new List<TriviaToklet> {
            new CommentTriviaToklet(),
            new WhitespaceTriviaToklet(),
            new NewlineTriviaToklet(),
            new TriviaToklet(),
        };

        toklets = new List<Toklet> {
            new NumberToklet(),
            new ComplexStringToklet(),
            new StringToklet(),
            new IdentToklet(),
            new OperatorToklet(),
            new Toklet(),
        };
    }

    public Tokenizer(StringConsumer stringConsumer) : this () {
        input = new StringConsumer(stringConsumer);
    }

    public Tokenizer(IConsumer<char> consumer) : this () {
        input = new StringConsumer(consumer);
    }

    public Tokenizer(IConsumer<Token> tokenConsumer) : this(new StringConsumer("")) {
        while (tokenConsumer.Consume(out _)) {
            reconsumeQueue.Enqueue(tokenConsumer.Current);
        }
    }

    public Tokenizer(Tokenizer tokenizer) : this(tokenizer.input) {
        reconsumeQueue = new Queue<Token>(tokenizer.reconsumeQueue);

        Current = tokenizer.Current;
    }

    public Tokenizer(System.IO.FileInfo fileInfo) : this(new StringConsumer(fileInfo))
    { }

    public Tokenizer(IEnumerable<char> collection) : this(new StringConsumer(collection))
    { }

    public Tokenizer(IEnumerable<string> collection) : this(new StringConsumer(collection))
    { }

    public void Reconsume() {
        if (reconsumeQueue.TryPeek(out Token token) && Object.ReferenceEquals(token, Current)) return;

        reconsumeQueue.Enqueue(Current);
    }

    public Token Peek() {
        if (reconsumeQueue.Count != 0) {
            return reconsumeQueue.Peek();
        }

        if (input.Peek() == '\u0003' || input.Peek() == '\0') {
            return Token.NULL;
        }

        return new Tokenizer(this).Consume();
    }

    public Token[] Peek(int n) {
        // create a new (dee-copy of) tokenizer from this one
        var tokenizer = new Tokenizer(this);

        // the output list
        var output = new Token[n];

        // consume `n` tokens and add them to the output
        for (int i = 0; i < n; i++) {
            output[i] = tokenizer.Consume();
        }

        return output;
    }

    public bool Consume(out Token result) {
        result = Consume();

        return result != TokenKind.EOF;
    }

    public Token Consume() {

        // If we are instructed to reconsume the last token, then dequeue a token from the reconsumeQueue and return it
        if (reconsumeQueue.Count != 0) {
            return reconsumeQueue.Dequeue();
        }

        // Consume a character from the LinesConsumer object
        var currChar = input.Consume();

        // if you want to preserve whitespace, you could do an if before the while loop and then return a whitespace token
        // although, you'll also need to modify the parser and parselets because they might not work correctly

        // If the character is U+0003 END OF TRANSMISSION, it means there is nothing left to consume. Return an EOF token
        if (currChar == '\u0003' || currChar == '\0') {
            Current = Token.NULL;

            return Current;
        }

        input.Reconsume();

        var leadingTrivia = ConsumeTrivia();

        Current = toklets.Find(toklet => toklet.Condition(new StringConsumer(input))).Consume(input, this);

        if (!(leadingTrivia is null)) Current.AddLeadingTrivia(leadingTrivia);

        if (input.Peek() != '\n') {
            var trailingTrivia = ConsumeTrivia();

            if (!(trailingTrivia is null)) Current.AddTrailingTrivia(trailingTrivia);
        }

        return Current;
    }

    public TriviaToken ConsumeTrivia() {
        var triviaToklet = triviaToklets.Find(toklet => toklet.Condition(new StringConsumer(input)));

        if (triviaToklet is null) return null;

        return triviaToklet.Consume(input, this) as TriviaToken;
    }
}