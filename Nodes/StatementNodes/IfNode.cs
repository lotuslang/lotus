using System;
using System.Collections.Generic;

public class IfNode : StatementNode
{
    public ValueNode Condition { get; protected set; }

    public SimpleBlock Body { get; protected set; }

    public ElseNode? ElseNode { get; protected set; }

    public bool HasElse { get => ElseNode != null; }

    public IfNode(ValueNode condition, SimpleBlock body, ComplexToken ifToken) : base(ifToken) {
        Condition = condition;
        Body = body;
        ElseNode = null;
    }

    public IfNode(ValueNode condition, SimpleBlock body, ElseNode elseNode, ComplexToken ifToken)
        : this(condition, body, ifToken)
    {
        ElseNode = elseNode;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "if");

        // FIXME: Choose color
        root.AddProperty("tooltip", "if statement");

        var conditionNode = new GraphNode(HashCode.Combine(this, "condition"), "condition");

        conditionNode.AddNode(Condition.ToGraphNode());
        conditionNode.AddProperty("tooltip", "if condition");

        root.AddNode(conditionNode);

        root.AddNode(Body.ToGraphNode());

        if (HasElse) {
            root.AddNode(ElseNode!.ToGraphNode());
        }

        return root;
    }
}