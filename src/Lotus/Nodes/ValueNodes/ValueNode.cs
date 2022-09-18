public abstract record ValueNode(Token Token, LocationRange Location)
: Node(Token, Location)
{
    public new static readonly ValueNode NULL = new Dummy();

    protected ValueNode(Token token) : this(token, token.IsValid) { }
    protected ValueNode(Token token, bool isValid) : this(token, token.Location, isValid) { }
    protected ValueNode(Token token, LocationRange loc, bool isValid) : this(token, loc) {
        IsValid = isValid;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);

    public static explicit operator StatementExpressionNode(ValueNode node) => new(node);

    public static explicit operator StatementNode(ValueNode node) => (StatementExpressionNode)node;

    internal record Dummy() : ValueNode(Token.NULL);
}