public class NamespaceNode : TopLevelNode
{
    public new static readonly NamespaceNode NULL = new NamespaceNode(ValueNode.NULL, ComplexToken.NULL, false);

    public ValueNode NamespaceName { get; protected set; }

    public NamespaceNode(ValueNode namespaceName, ComplexToken namespaceToken, bool isValid = true) : base(Token.NULL)
        //: base(namespaceToken, new LocationRange(namespaceToken.Location, namespaceName.Location), isValid)
    {
        NamespaceName = namespaceName;
    }
}
