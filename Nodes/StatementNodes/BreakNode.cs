public class BreakNode : StatementNode
{
    public BreakNode(ComplexToken breakToken) : base(breakToken) { }

    public override GraphNode ToGraphNode()
        => base.ToGraphNode()
            .SetTooltip("break keyword");
}