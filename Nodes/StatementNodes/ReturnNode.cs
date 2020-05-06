public class ReturnNode : StatementNode
{
    public ValueNode Value { get; protected set; }

    public bool IsReturningValue { get; protected set; }

    public ReturnNode(ValueNode value, ComplexToken returnToken) : base(returnToken) {
        if (value != null && value != ValueNode.NULL) {
            Value = value;
            IsReturningValue = true;
            return;
        }

        Value = ValueNode.NULL;
        IsReturningValue = false;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "return");

        root.AddProperty("color", "brown");
        root.AddProperty("tooltip", "return");

        if (IsReturningValue) {
            root.AddNode(Value.ToGraphNode());
        }

        return root;
    }
}
