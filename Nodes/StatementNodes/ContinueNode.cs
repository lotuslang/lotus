public class ContinueNode : StatementNode
{
    public ContinueNode(ComplexToken continueToken) : base(continueToken) { }

    public override GraphNode ToGraphNode() {
        var root = base.ToGraphNode();

        root.AddProperty("tooltip", "continue keyword");

        return root;
    }
}