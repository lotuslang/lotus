public class NamespaceNode : StatementNode
{
    public ValueNode NamespaceName { get; protected set; }

    public NamespaceNode(ValueNode namespaceName, ComplexToken namespaceToken, bool isValid = true) : base(namespaceToken, isValid) {
        NamespaceName = namespaceName;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}