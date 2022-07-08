using System.Diagnostics.CodeAnalysis;

public abstract class Parser<T> : IConsumer<T> where T : Node
{
    protected readonly Queue<T> reconsumeQueue;

    public IConsumer<Token> Tokenizer { get; }

    public LocationRange Position {
        get => Current.Location;
    }

    /// <summary>
    /// Gets the last StatementNode object consumed by this instance.
    /// </summary>
    /// <value>The last StatementNode consumed.</value>
    public abstract T Current { get; protected set; }

    /// <summary>
    /// Contrary to <see cref="Parser.Default"/>, this variable is constant, and just returns <see cref="Node.NULL"/>
    /// </summary>
    public static readonly T ConstantDefault = (Node.NULL as T)!;

    /// <summary>
    /// Returns the value of <see cref="Parser.ConstantDefault"/> BUT adjusted for the current position. <br/>
    /// Most of the time, this is the variable you want, because when comparing nodes, position is important,
    /// and the parser will always return a node with relevant position, even if it is EOF and other things that
    /// prompt for the use of <see cref="StatementNode.NULL"/>
    /// </summary>
    public virtual T Default => ConstantDefault with { Location = Position };

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
            Logger.Warning(new InvalidCallError(ErrorArea.Parser, Position) {
                Message = "Something tried to create a new Parser with a null grammar."
                        + "That's not allowed, and might throw in future versions, but for now the grammar will just be empty...",
            });

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
        Tokenizer = new Consumer<Token>(tokens, Token.NULL, Position.filename);
    }

    public Parser(StringConsumer consumer, ReadOnlyGrammar grammar) : this(new Tokenizer(consumer, grammar), grammar) { }

    public Parser(IEnumerable<char> collection, ReadOnlyGrammar grammar) : this(new Tokenizer(collection, grammar), grammar) { }

    public Parser(Uri file, ReadOnlyGrammar grammar) : this(new Tokenizer(file, grammar), grammar) { }

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
        if (reconsumeQueue.TryPeek(out var node) && Object.ReferenceEquals(node, Current)) return;
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

        if (Tokenizer is null) {
            throw Logger.Fatal(new InternalError(ErrorArea.Parser) {
                Message = "The parser's tokenizer was null. Something went seriously wrong",
                Location = Position
            });
        }

        return Default;
    }

    public bool TryConsume<TNode>([NotNullWhen(true)] out TNode? output, out T asNode) where TNode : T {
        asNode = Consume();

        output = asNode as TNode;

        return output is not null;
    }

    public Result<TNode> TryConsume<TNode>(out T asNode) where TNode : T
        => (asNode = Consume()) as TNode ?? Result<TNode>.Error;

    public TNode Consume<TNode>(TNode defaultVal, Action<T> errorHandler) where TNode : T {
        if (!TryConsume<TNode>(out var output, out var val)) {
            if (!val.IsValid)
                // if it's not valid, it probably already emitted an error
                Logger.errorStack.Pop();

            errorHandler(val);
        }

        return output ?? defaultVal;
    }

    public TNode Consume<TNode>(TNode defaultVal, string? @in = null, string? @as = null) where TNode : T
        => Consume<TNode>(
            defaultVal,
            val =>
                Logger.Error(new UnexpectedError<T>(ErrorArea.Parser) {
                    Value = val,
                    In = @in,
                    As = @as,
                    Expected = typeof(TNode).Name
                })
        );

    public bool TryConsumeEither<TNode1, TNode2>(Union<TNode1, TNode2> defaultVal, out Union<TNode1, TNode2> res, out T asNode)
        where TNode1 : T
        where TNode2 : T {
        asNode = Consume();

        switch (asNode) {
            case TNode1 t1:
                res = t1;
                return true;
            case TNode2 t2:
                res = t2;
                return true;
            default:
                res = defaultVal;
                return false;
        }
    }

    public Result<Union<T1, T2>> TryConsumeEither<T1, T2>(out T asNode)
        where T1 : T
        where T2 : T
        =>  (asNode = Consume()) switch {
                T1 t1 => new(t1),
                T2 t2 => new(t2),
                _ => Result<Union<T1, T2>>.Error
            };

    public abstract Parser<T> Clone();
    IConsumer<T> IConsumer<T>.Clone() => Clone();
}