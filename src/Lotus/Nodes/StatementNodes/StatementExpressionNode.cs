public sealed record StatementExpressionNode(ValueNode Value)
: StatementNode(Value.Token, Value.Location, Value.IsValid)
{
    public static implicit operator ValueNode(StatementExpressionNode node) => node.Value;

    public new LocationRange Location {
        get => Value.Location;
        init {
            Value = Value with { Location = value };
            base.Location = value;
        }
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}