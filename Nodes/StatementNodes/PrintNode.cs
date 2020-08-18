public class PrintNode : StatementNode
{
    public ValueNode Value { get; }

    public PrintNode(ComplexToken printToken, ValueNode node) : base(printToken) {
        Value = node;
    }

    public override GraphNode ToGraphNode()
        =>  new GraphNode(GetHashCode(), "print") {
                Value.ToGraphNode()
            }.SetColor("")
             .SetTooltip(""); // FIXME: find color
}