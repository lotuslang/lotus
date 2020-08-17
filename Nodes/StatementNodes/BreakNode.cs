public class BreakNode : StatementNode
{
    public BreakNode(ComplexToken breakToken, bool isValid = true) : base(breakToken, isValid) { }

    public override GraphNode ToGraphNode()
        => base.ToGraphNode()
            .SetTooltip("break keyword");
}