public class ParenthesizedValueNode : TupleNode
{
    public ValueNode Value => Count == 0 ? ValueNode.NULL : Values[0];

    public ParenthesizedValueNode(ValueNode value, Token leftParen, Token rightParen, bool isValid = true)
        : base(new[] {value}, leftParen, rightParen, isValid) { }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}