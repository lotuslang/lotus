public class NamespaceNode : StatementNode
{
    public new static readonly NamespaceNode NULL = new NamespaceNode(ValueNode.NULL, ComplexToken.NULL, false);

    public ValueNode NamespaceName { get; protected set; }

    public NamespaceNode(ValueNode namespaceName, ComplexToken namespaceToken, bool isValid = true)
        : base(namespaceToken, new LocationRange(namespaceToken.Location, namespaceName.Location), isValid)
    {
        NamespaceName = namespaceName;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
