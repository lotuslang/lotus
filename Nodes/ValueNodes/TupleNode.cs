public record TupleNode
: ValueNode, IEnumerable<ValueNode>
{
    public new static readonly TupleNode NULL = new(Tuple<ValueNode>.NULL);

    private Tuple<ValueNode> _internalTuple;

    public IList<ValueNode> Items => _internalTuple.Items;
    public Token OpeningToken => _internalTuple.OpeningToken;
    public Token ClosingToken => _internalTuple.ClosingToken;
    public int Count => _internalTuple.Count;

    public TupleNode(Tuple<ValueNode> tuple) : base(tuple.OpeningToken, tuple.Location, tuple.IsValid)
        => _internalTuple = tuple;

    public TupleNode(IList<ValueNode> items, Token openingToken, Token closingToken, bool isValid = true)
        : this(new Tuple<ValueNode>(items, openingToken, closingToken, isValid)) { }


    /// <summary>
    /// <strong>TRUNCATES</strong> the tuple to the first element and turns it into a paren expression.
    /// </summary>
    internal ParenthesizedValueNode AsParenthesized()
        => new(
            Items.FirstOrDefault(ValueNode.NULL),
            OpeningToken,
            ClosingToken,
            IsValid
        );

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);

    public IEnumerator<ValueNode> GetEnumerator() => _internalTuple.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public static explicit operator Tuple<ValueNode>(TupleNode node) => node._internalTuple;
}