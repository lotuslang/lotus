public class FromNode : StatementNode
{
    public ValueNode OriginName { get; protected set; }

    internal FromNode(ValueNode originName, ComplexToken fromToken) : base(fromToken) {
        OriginName = originName;
    }

    public override GraphNode ToGraphNode()
        => new GraphNode(GetHashCode(), "from") {
            OriginName.ToGraphNode()
                .SetTooltip("origin name")
        }.SetColor("navy")
         .SetTooltip("from statement");
}
