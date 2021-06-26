public class TopLevelStatementNode : TopLevelNode
{
    public StatementNode Statement { get; }

    public TopLevelStatementNode(StatementNode node) : base(node.Token) {
        Statement = node;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}