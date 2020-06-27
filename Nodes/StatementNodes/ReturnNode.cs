public class ReturnNode : StatementNode
{
    public ValueNode Value { get; protected set; }

    public bool IsReturningValue => Value != ValueNode.NULL;

    public ReturnNode(ValueNode value, ComplexToken returnToken) : base(returnToken) {
        Value = (value != null && value != ValueNode.NULL) ? value : ValueNode.NULL;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "return")
            .SetColor("brown")
            .SetTooltip("return");

        if (IsReturningValue) {
            root.Add(Value.ToGraphNode());
        }

        return root;
    }
}
