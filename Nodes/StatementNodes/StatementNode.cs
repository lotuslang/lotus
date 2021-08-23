public class StatementNode : Node
{
    public new static readonly StatementNode NULL = new(Token.NULL, LocationRange.NULL, false);

    public StatementNode(Token token, LocationRange range, bool isValid = true) : base(token, range, isValid)
    { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);

    public static explicit operator TopLevelStatementNode(StatementNode node) => new(node);

    public static explicit operator TopLevelNode(StatementNode node) => (TopLevelStatementNode)node;
}
