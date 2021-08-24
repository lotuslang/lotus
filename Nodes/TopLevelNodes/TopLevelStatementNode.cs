public record TopLevelStatementNode(StatementNode Statement)
: TopLevelNode(Statement.Token, Statement.Location, Statement.IsValid)
{
    public static implicit operator StatementNode(TopLevelStatementNode node) => node.Statement;

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}