public class FromNode : StatementNode
{
    public ValueNode OriginName { get; protected set; }

    internal FromNode(ValueNode originName, ComplexToken fromToken, bool isValid = true) : base(fromToken, isValid) {
        OriginName = originName;
    }

    public override GraphNode ToGraphNode()
        => new GraphNode(GetHashCode(), "from") {
            OriginName.ToGraphNode()
                .SetTooltip("origin name")
        }.SetColor("navy")
         .SetTooltip("from statement");
}
