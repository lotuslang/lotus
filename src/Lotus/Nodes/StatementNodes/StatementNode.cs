public abstract record StatementNode(Token Token, LocationRange Location)
: Node(Token, Location)
{
    public new static readonly StatementNode NULL = new Dummy();

    protected StatementNode(Token token) : this(token, token.IsValid) { }
    protected StatementNode(Token token, bool isValid) : this(token, token.Location, isValid) { }
    protected StatementNode(Token token, LocationRange loc, bool isValid) : this(token, loc) {
        IsValid = isValid;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);

    public static explicit operator TopLevelStatementNode(StatementNode node) => new(node);

    public static explicit operator TopLevelNode(StatementNode node) => (TopLevelStatementNode)node;

    internal record Dummy() : StatementNode(Token.NULL, false);
}
