public class ContinueNode : StatementNode
{
    public ContinueNode(ComplexToken continueToken, bool isValid = true) : base(continueToken, isValid) { }

    public override GraphNode ToGraphNode()
        => base.ToGraphNode()
            .SetTooltip("continue keyword");
}