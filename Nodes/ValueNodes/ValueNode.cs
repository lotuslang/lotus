[System.Diagnostics.DebuggerDisplay("{Token.Representation}")]
public record ValueNode(Token Token, LocationRange Location, bool IsValid = true) : Node(Token, Location, IsValid)
{ // FIXME: make base nodes abstract
    public new static readonly ValueNode NULL = new Dummy();

    protected ValueNode(Token token, bool isValid = true) : this(token, token.Location, isValid) { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);

    public static explicit operator StatementExpressionNode(ValueNode node) => new(node);

    public static explicit operator StatementNode(ValueNode node) => (StatementExpressionNode)node;

    private record Dummy() : ValueNode(Token.NULL, false);
}