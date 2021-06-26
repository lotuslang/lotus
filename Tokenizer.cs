using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class Tokenizer : IConsumer<Token>
{

    public Token Current { get; protected set; }

    public Token Default {
        get {
            var output = Token.NULL;

            output.Location = Position;

            return output;
        }
    }

    protected Queue<Token> reconsumeQueue;

    public LocationRange Position {
        get => Current.Location;
    }

    protected StringConsumer input;

    public ReadOnlyGrammar Grammar { get; protected set; }

    protected Tokenizer() {
        reconsumeQueue = new Queue<Token>();

        input = new StringConsumer(new char[0]);

        Current = Token.NULL;

        Grammar = new ReadOnlyGrammar();
    }

    protected Tokenizer(ReadOnlyGrammar grammar) : this() {
        if (grammar is null) {
            Logger.Warning(new InvalidCallException(
                message: "Something tried to create a new Tokenizer with a null grammar."
                        + "That's not allowed, and might throw in future versions, but for now the grammar will just be empty...",
                range: Position
            ));

            grammar = new ReadOnlyGrammar();
        }

        Grammar = grammar;
    }

    public Tokenizer(StringConsumer stringConsumer, ReadOnlyGrammar grammar) : this(grammar) {
        input = new StringConsumer(stringConsumer);
    }

    public Tokenizer(IConsumer<char> consumer, ReadOnlyGrammar grammar) : this(grammar) {
        input = new StringConsumer(consumer);
    }

    public Tokenizer(IConsumer<Token> tokenConsumer, ReadOnlyGrammar grammar) : this(new StringConsumer(""), grammar) {
        while (tokenConsumer.Consume(out _)) {
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
        if (reconsumeQueue.TryPeek(out Token? token) && Object.ReferenceEquals(token, Current)) {
            Console.WriteLine("Calling reconsume multiple times in a row !");
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
            Current.IsValid,
            Current.LeadingTrivia,
            Current.TrailingTrivia
        );

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
            Current.IsValid,
            Current.LeadingTrivia,
            Current.TrailingTrivia
        );

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

    public bool Consume([MaybeNullWhen(false)] out Token result) {
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

        // Consume a character from the StringConsumer object
        var currChar = input.Consume();

        // if you want to preserve whitespace, you could do an if before the while loop and then return a whitespace token
        // although, you'll also need to modify the parser and parslets because they might not work correctly

        // If the character is U+0003 END OF TRANSMISSION, it means there is nothing left to consume. Return an EOF token
        if (currChar == input.Default) {
            var lastPos = Position;

            Current = Token.NULL;

            Current.Location = lastPos;

            return Current;
        }

        input.Reconsume();

        if (!preserveTrivia && input.Peek() != ',') {
            var leadingTrivia = ConsumeTrivia();

            Current = Grammar.MatchToklet(input).Consume(input, this);

            if (!(leadingTrivia is null)) Current.AddLeadingTrivia(leadingTrivia);

            if (input.Peek() != '\n') {
                var trailingTrivia = ConsumeTrivia();

                if (!(trailingTrivia is null)) Current.AddTrailingTrivia(trailingTrivia);
            }
        } else {
            Current = Grammar.MatchToklet(input).Consume(input, this);
        }

        return Current;
    }

    public TriviaToken ConsumeTrivia() {
        var triviaToklet = Grammar.MatchTriviaToklet(input);

        if (triviaToklet is null) return TriviaToken.NULL;

        return triviaToklet.Consume(input, this);
    }
}