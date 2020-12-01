public class PrintNode : StatementNode
{
    public new static readonly PrintNode NULL = new PrintNode(ComplexToken.NULL, ValueNode.NULL, false);

    public ValueNode Value { get; }

    public PrintNode(ComplexToken printToken, ValueNode node, bool isValid = true)
        : base(printToken, new LocationRange(printToken.Location, node.Location), isValid)
    {
        Value = node;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}