using System;

public class IfNode : StatementNode
{
    public ValueNode Condition { get; protected set; }

    public SimpleBlock Body { get; protected set; }

    public ElseNode? ElseNode { get; protected set; }

    public bool HasElse { get => ElseNode != null; }

    public IfNode(ValueNode condition, SimpleBlock body, ComplexToken ifToken, bool isValid = true) : base(ifToken, isValid) {
        Condition = condition;
        Body = body;
        ElseNode = null;
    }

    public IfNode(ValueNode condition, SimpleBlock body, ElseNode elseNode, ComplexToken ifToken, bool isValid = true)
        : this(condition, body, ifToken, isValid)
    {
        ElseNode = elseNode;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "if") {
            new GraphNode(HashCode.Combine(this, "condition"), "condition") {
                Condition.ToGraphNode()
            }.SetTooltip("if condition"),
            Body.ToGraphNode()
        }.SetTooltip("if statement"); // FIXME: Choose color

        if (HasElse) {
            root.Add(ElseNode!.ToGraphNode());
        }

        return root;
    }
}