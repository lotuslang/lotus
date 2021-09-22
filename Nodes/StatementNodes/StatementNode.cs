public abstract record StatementNode(Token Token, LocationRange Location, bool IsValid = true) : Node(Token, Location, IsValid)
{
    public new static readonly StatementNode NULL = new Dummy();

    protected StatementNode(Token token, bool isValid = true) : this(token, token.Location, isValid) { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);

    public static explicit operator TopLevelStatementNode(StatementNode node) => new(node);

    public static explicit operator TopLevelNode(StatementNode node) => (TopLevelStatementNode)node;

    private record Dummy() : StatementNode(Token.NULL, false);
}
