public class ReturnNode : StatementNode
{
    public ValueNode Value { get; protected set; }

    public bool IsReturningValue => Value != ValueNode.NULL;

    public ReturnNode(ValueNode value, ComplexToken returnToken, bool isValid = true) : base(returnToken, isValid) {
        Value = (value != null && value != ValueNode.NULL) ? value : ValueNode.NULL;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
