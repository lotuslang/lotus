public class ReturnNode : StatementNode
{
    public new static readonly ReturnNode NULL = new ReturnNode(ValueNode.NULL, ComplexToken.NULL, false);

    public ValueNode Value { get; protected set; }

    public bool IsReturningValue => Value != ValueNode.NULL;

    public ReturnNode(ValueNode value, ComplexToken returnToken, bool isValid = true)
        : base(returnToken, new LocationRange(returnToken.Location, value.Location), isValid)
    {
        Value = value;
    }

    public ReturnNode(ComplexToken returnToken, bool isValid = true)
        : base(returnToken, returnToken.Location, isValid)
    {
        Value = ValueNode.NULL;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
