using System;
using System.Linq;
using System.Collections.Generic;

public abstract class Parser<T> : IConsumer<T> where T : Node
{
    protected readonly Queue<T> reconsumeQueue;

    public IConsumer<Token> Tokenizer { get; }

    public LocationRange Position {
        get => Current.Token.Location;
    }

    /// <summary>
    /// Gets the last StatementNode object consumed by this instance.
    /// </summary>
    /// <value>The last StatementNode consumed.</value>
    public abstract T Current { get; protected set; }

    /// <summary>
    /// Contrary to <see cref="Parser.Default"/>, this variable is constant, and just returns <see cref="StatementNode.NULL"/>
    /// </summary>
    public static readonly T ConstantDefault = (Node.NULL as T)!;

    /// <summary>
    /// Returns the value of <see cref="Parser.ConstantDefault"/> BUT adjusted for the current position. <br/>
    /// Most of the time, this is the variable you want, because when comparing nodes, position is important,
    /// and the parser will always return a node with relevant position, even if it is EOF and other things that
    /// prompt for the use of <see cref="StatementNode.NULL"/>
    /// </summary>
    public virtual T Default {
        get {
            var output = ConstantDefault;

            output.Location = Position;

            return output;
        }
    }

    public ReadOnlyGrammar Grammar { get; protected set; }

#pragma warning disable CS8618
    protected Parser() {
        reconsumeQueue = new Queue<T>();

        Current = ConstantDefault;

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
    }

    public Parser(IConsumer<T> nodeConsumer, ReadOnlyGrammar grammar) : this(grammar) {
        foreach (var node in nodeConsumer) {
            reconsumeQueue.Enqueue(node);
        }
    }

    public Parser(IEnumerable<Token> tokens, ReadOnlyGrammar grammar) : this(grammar) {
        Tokenizer = new Consumer<Token>(tokens, Token.NULL);
    }

    public Parser(StringConsumer consumer, ReadOnlyGrammar grammar) : this(new Tokenizer(consumer, grammar), grammar) { }

    public Parser(IEnumerable<char> collection, ReadOnlyGrammar grammar) : this(new Tokenizer(collection, grammar), grammar) { }

    public Parser(System.IO.FileInfo file, ReadOnlyGrammar grammar) : this(new Tokenizer(file, grammar), grammar) { }

    public Parser(Parser<T> parser) : this(parser.Grammar) {
        reconsumeQueue = parser.reconsumeQueue;
        Tokenizer = parser.Tokenizer;
        Current = parser.Current;
    }

    public Parser(Parser<T> parser, ReadOnlyGrammar grammar) : this(parser.Tokenizer, grammar) {
        reconsumeQueue = parser.reconsumeQueue;
        Current = parser.Current;
    }

    /// <summary>
    /// Reconsumes the last Node object.
    /// </summary>
    public void Reconsume() {
        if (reconsumeQueue.TryPeek(out T? node) && Object.ReferenceEquals(node, Current)) return;
    }

    public abstract T Peek();

    public abstract T[] Peek(int n);

    /// <summary>
    /// Consumes a StatementNode object and returns it.
    /// </summary>
    /// <param name="success">True if the operation succeeded, false otherwise.</param>
    /// <returns>The StatementNode object consumed.</returns>
    public virtual bool Consume(out T result) {
        result = Consume(); // consume a StatementNode

        return result != Default;
    }

    /// <summary>
    /// Consume a StatementNode object and returns it.
    /// </summary>
    /// <returns>The StatementNode object consumed.</returns>
    public virtual T Consume() {
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