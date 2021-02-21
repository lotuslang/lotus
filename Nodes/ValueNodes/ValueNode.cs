[System.Diagnostics.DebuggerDisplay("{Representation}")]
public class ValueNode : Node
{
    /// <summary>
    /// This constant is the equivalent of "null". When a function doesn't return, it will actually set the `#return` variable to this constant.
    /// Variables that are assigned to a non-returning functions will actually be assigned this value.
    /// </summary>
    public new static readonly ValueNode NULL = new ValueNode("", Token.NULL, LocationRange.NULL, false);

    public string Representation { get; protected set; }

    public ValueNode(Token token, LocationRange range, bool isValid = true) : this(token.Representation, token, range, isValid)
    { }

    public ValueNode(string representation, Token token, LocationRange range, bool isValid = true)
        : base(token, range, isValid)
    {
        Representation = representation;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(StatementVisitor<T> visitor) => visitor.Visit(this);

    public static explicit operator StatementExpressionNode(ValueNode node) => new StatementExpressionNode(node);

    public static explicit operator StatementNode(ValueNode node) => (StatementExpressionNode)node;
}