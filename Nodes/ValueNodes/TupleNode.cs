public record TupleNode<TValue>(IList<TValue> Items, Token Token, Token ClosingToken, bool IsValid = true)
: ValueNode(Token, new LocationRange(Token.Location, ClosingToken.Location), IsValid), IEnumerable<TValue>
where TValue : ILocalized
{
    public new static readonly TupleNode<TValue> NULL = new(Array.Empty<TValue>(), Token.NULL, Token.NULL, false);

    public Token OpeningToken { get => Token; init => Token = value; }

    public int Count => Items.Count;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);

    public IEnumerator<TValue> GetEnumerator() => Items.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public record TupleNode(IList<ValueNode> Items, Token Token, Token ClosingToken, bool IsValid = true)
: TupleNode<ValueNode>(Items, Token, ClosingToken, IsValid)
{
    public new static readonly TupleNode NULL = new(Array.Empty<ValueNode>(), Token.NULL, Token.NULL, false);

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

    public TupleNode(TupleNode<ValueNode> tuple)
        : this(
            tuple.Items,
            tuple.OpeningToken,
            tuple.ClosingToken,
            tuple.IsValid
        ) {}
}