public record StatementExpressionNode(ValueNode Value)
: StatementNode(Value.Token, Value.Location, Value.IsValid)
{
    public static implicit operator ValueNode(StatementExpressionNode node) => node.Value;

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}