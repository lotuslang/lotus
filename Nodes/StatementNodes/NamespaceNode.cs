public class NamespaceNode : StatementNode
{
    public ValueNode NamespaceName { get; protected set; }

    public NamespaceNode(ValueNode namespaceName, ComplexToken namespaceToken) : base(namespaceToken) {
        NamespaceName = namespaceName;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "namespace");

        root.AddProperty("color", "cornflowerblue");
        root.AddProperty("tooltip", "namespace declaration");

        var nameNode = NamespaceName.ToGraphNode();

        nameNode.AddProperty("tooltip", "namespace name");

        root.AddNode(nameNode);

        return root;
    }
}
