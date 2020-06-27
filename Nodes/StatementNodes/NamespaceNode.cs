public class NamespaceNode : StatementNode
{
    public ValueNode NamespaceName { get; protected set; }

    public NamespaceNode(ValueNode namespaceName, ComplexToken namespaceToken) : base(namespaceToken) {
        NamespaceName = namespaceName;
    }

    public override GraphNode ToGraphNode()
        => new GraphNode(GetHashCode(), "namespace") {
            NamespaceName.ToGraphNode()
                .SetTooltip("namespace name")
        }.SetColor("cornflowerblue")
         .SetTooltip("namespace declaration");
}
