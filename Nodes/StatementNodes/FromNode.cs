public class FromNode : StatementNode
{
    public new static readonly FromNode NULL = new FromNode(ValueNode.NULL, ComplexToken.NULL, false);

    public ValueNode OriginName { get; protected set; }

    internal FromNode(ValueNode originName, ComplexToken fromToken, bool isValid = true)
    : base(fromToken, new LocationRange(fromToken.Location, originName.Location), isValid)
    {
        OriginName = originName;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
