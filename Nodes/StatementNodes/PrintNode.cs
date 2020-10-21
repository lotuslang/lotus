public class PrintNode : StatementNode
{
    public ValueNode Value { get; }

    public PrintNode(ComplexToken printToken, ValueNode node) : base(printToken, printToken.Location) {
        Value = node;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}