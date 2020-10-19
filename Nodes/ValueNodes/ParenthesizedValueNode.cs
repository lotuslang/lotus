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

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}