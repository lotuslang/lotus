public class ContinueNode : StatementNode
{
    public ContinueNode(ComplexToken continueToken) : base(continueToken) { }

    public override GraphNode ToGraphNode()
        => base.ToGraphNode()
            .SetTooltip("continue keyword");
}