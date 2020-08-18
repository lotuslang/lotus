public class ParenthesizedValueNode : ValueNode
{
    public Token RightParenthesis { get; }

    public ValueNode Value { get; }

    public ParenthesizedValueNode(Token leftParen, Token rightParen, ValueNode value, bool isValid = true)
        : base(leftParen, isValid)
    {
        RightParenthesis = rightParen;

        Value = value;
    }

    public override GraphNode ToGraphNode() {
        return Value.ToGraphNode();
    }
}