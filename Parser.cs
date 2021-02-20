using System;
using System.Linq;
using System.Collections.Generic;

public abstract class Parser : IConsumer<Node>
{
    protected readonly Queue<Node> reconsumeQueue;

    public IConsumer<Token> Tokenizer { get; }

    public LocationRange Position {
        get => Current.Token.Location;
    }

    /// <summary>
    /// Gets the last StatementNode object consumed by this instance.
    /// </summary>
    /// <value>The last StatementNode consumed.</value>
    public virtual Node Current { get; protected set; }

    public virtual Node Default {
        get {
            var output = StatementNode.NULL;

            output.Location = Position;

            return output;
        }
    }

    public ReadOnlyGrammar Grammar { get; protected set; }

#pragma warning disable CS8618
    protected Parser() {
        reconsumeQueue = new Queue<Node>();

        Current = StatementNode.NULL;

        Grammar = new ReadOnlyGrammar();
    }
#pragma warning restore

    protected Parser(ReadOnlyGrammar grammar) : this() {
        if (grammar is null) {
            Logger.Warning(new InvalidCallException(
                message: "Something tried to create a new Parser with a null grammar."
                        + "That's not allowed, and might throw in future versions, but for now the grammar will just be empty...",
                range: Position
            ));

            grammar = new ReadOnlyGrammar();
        }

        Grammar = grammar;
    }

    //public Parser(Tokenizer tokenizer) : this(tokenizer as IConsumer<Token>) { }

    public Parser(IConsumer<Token> tokenConsumer, ReadOnlyGrammar grammar) : this(grammar) {
        Tokenizer = tokenConsumer;
        reconsumeQueue = new Queue<Node>();
    }

    public Parser(IConsumer<Node> nodeConsumer, ReadOnlyGrammar grammar) : this(grammar) {
        while (nodeConsumer.Consume(out Node? node)) {
            reconsumeQueue.Enqueue(node);
        }
    }

    public Parser(StringConsumer consumer, ReadOnlyGrammar grammar) : this(new Tokenizer(consumer, grammar), grammar) { }

    public Parser(IEnumerable<char> collection, ReadOnlyGrammar grammar) : this(new Tokenizer(collection, grammar), grammar) { }

    public Parser(System.IO.FileInfo file, ReadOnlyGrammar grammar) : this(new Tokenizer(file, grammar), grammar) { }

    public Parser(Parser parser) : this(parser.Grammar) {
        reconsumeQueue = parser.reconsumeQueue;
        Tokenizer = parser.Tokenizer;
        Current = parser.Current;
    }

    public Parser(Parser parser, ReadOnlyGrammar grammar) : this(parser.Tokenizer, grammar) {
        reconsumeQueue = new Queue<Node>(parser.reconsumeQueue);

        Current = parser.Current;
    }

    /// <summary>
    /// Reconsumes the last Node object.
    /// </summary>
    public void Reconsume() {
        if (reconsumeQueue.TryPeek(out Node? node) && Object.ReferenceEquals(node, Current)) return;
    }

    public abstract Node Peek();

    public abstract Node[] Peek(int n);

    /// <summary>
    /// Consumes a StatementNode object and returns it.
    /// </summary>
    /// <param name="success">True if the operation succeeded, false otherwise.</param>
    /// <returns>The StatementNode object consumed.</returns>
    public virtual bool Consume(out Node result) {
        result = Consume(); // consume a StatementNode

        return result != Default;
    }

    /// <summary>
    /// Consume a StatementNode object and returns it.
    /// </summary>
    /// <returns>The StatementNode object consumed.</returns>
    public virtual Node Consume() {
        // If we are instructed to reconsume the last node, then dequeue a node from the reconsumeQueue and return it
        if (reconsumeQueue.Count != 0) {
            return reconsumeQueue.Dequeue();
        }

        if (Tokenizer == null) {
            throw Logger.Fatal(new InternalErrorException(
                message: "The parser's tokenizer was null. Something went seriously wrong",
                range: Position
            ));
        }

        return Default;
    }
}