public class FromNode : StatementNode
{
    public ValueNode OriginName { get; protected set; }

    internal FromNode(ValueNode originName, ComplexToken fromToken) : base(fromToken) {
        OriginName = originName;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "from");

        root.AddProperty("color", "navy");
        root.AddProperty("tooltip", "from statement");

        var nameNode = OriginName.ToGraphNode();

        nameNode.AddProperty("tooltip", "origin name");

        root.AddNode(nameNode);

        return root;
    }
}
