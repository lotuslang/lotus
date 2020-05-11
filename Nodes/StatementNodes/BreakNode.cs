public class BreakNode : StatementNode
{
    public BreakNode(ComplexToken breakToken) : base(breakToken) { }

    public override GraphNode ToGraphNode() {
        var root = base.ToGraphNode();

        root.AddProperty("tooltip", "break keyword");

        return root;
    }
}