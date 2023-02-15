namespace Lotus.Syntax;

// todo(parsing): consolidate all parsers into one
public abstract class Parser<T> where T : Node
{
    protected readonly Stack<T> _reconsumeStack;

    public Tokenizer Tokenizer { get; }

    public LocationRange Position => Current.Location;

    protected T _curr;
    /// <summary>
    /// Gets the last StatementNode object consumed by this instance.
    /// </summary>
    /// <value>The last StatementNode consumed.</value>
    public ref readonly T Current => ref _curr;

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

#pragma warning disable CS8618
    protected Parser() {
        _reconsumeStack = new Stack<T>();
        _curr = ConstantDefault;
    }
#pragma warning restore

    protected Parser(Tokenizer tokenizer) : this() {
        Tokenizer = tokenizer;
    }

    protected Parser(TextStream stream) : this(new Tokenizer(stream)) { }

    protected Parser(Parser<T> parser) : this(parser.Tokenizer) {
        _reconsumeStack = parser._reconsumeStack;
        _curr = parser.Current;
    }

    /// <summary>
    /// Reconsumes the last Node object.
    /// </summary>
    public void Reconsume() {
        if (_reconsumeStack.TryPeek(out var node))
            Debug.Assert(!Object.ReferenceEquals(node, Current));

        _reconsumeStack.Push(Current);
    }

    public abstract T Peek();

    public virtual bool Consume(out T result) {
        result = Consume(); // consume a StatementNode

        return result != Default;
    }

    /// <summary>
    /// Consume a StatementNode object and returns it.
    /// </summary>
    /// <returns>The StatementNode object consumed.</returns>
    public virtual ref readonly T Consume() {
        // If we are instructed to reconsume the last node, then dequeue a node from the reconsumeQueue and return it
        if (_reconsumeStack.Count != 0) {
            _curr = _reconsumeStack.Pop();
            return ref _curr;
        }

        _curr = Default;
        return ref _curr;
    }

    public bool TryConsume<TNode>([NotNullWhen(true)] out TNode? output, out T asNode) where TNode : T {
        asNode = Consume();

        output = asNode as TNode;

        return output is not null;
    }

    public Result<TNode> TryConsume<TNode>(out T asNode) where TNode : T
        => (asNode = Consume()) as TNode ?? Result<TNode>.Error;

    public TNode Consume<TNode>(TNode defaultVal, Action<T> errorHandler) where TNode : T {
        var errCount = Logger.ErrorCount;
        if (!TryConsume<TNode>(out var output, out var val)) {
            while (errCount < Logger.ErrorCount)
                _ = Logger.errorStack.Pop();

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
        where TNode2 : T
    {
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
}