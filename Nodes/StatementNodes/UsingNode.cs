public class UsingNode : StatementNode
{

    public ValueNode ImportName { get; }

    public UsingNode(ComplexToken usingToken, ValueNode importName, bool isValid = true) : base(usingToken, isValid) {
        ImportName = importName;
    }

    public override GraphNode ToGraphNode()
        =>  new GraphNode(GetHashCode(), "using") {
                ImportName.ToGraphNode()
            }.SetColor("")
             .SetTooltip(""); // FIXME: find color
}