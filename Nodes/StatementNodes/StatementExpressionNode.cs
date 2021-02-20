public class StatementExpressionNode : StatementNode
{
    public ValueNode Value { get; }

    public StatementExpressionNode(ValueNode value) : base(value.Representation, value.Token, value.Location) {
        Value = value;
    }

    public static implicit operator ValueNode(StatementExpressionNode node) => node.Value;
}