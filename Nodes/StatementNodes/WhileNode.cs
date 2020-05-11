using System;

public class WhileNode : StatementNode
{
    public bool IsDoLoop { get; protected set; }

    public ComplexToken? DoToken { get; protected set; }

    public ValueNode Condition { get; protected set; }

    public SimpleBlock Body { get; protected set; }

    public WhileNode(ValueNode condition, SimpleBlock body, ComplexToken whileToken) : base(whileToken) {
        Condition = condition;
        IsDoLoop = false;
        DoToken = null;
        Body = body;
    }

    public WhileNode(ValueNode condition, SimpleBlock body, ComplexToken whileToken, ComplexToken doToken)
        : this(condition, body, whileToken)
    {
        IsDoLoop = true;
        DoToken = doToken;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), IsDoLoop ? "do-while" : "while");

        // FIXME: Choose color
        root.AddProperty("tooltip", IsDoLoop ? "do-while loop" : "while loop");

        var conditionNode = new GraphNode(HashCode.Combine(this, "condition"), "condition");

        // FIXME: Choose color
        conditionNode.AddProperty("tooltip", "loop condition");

        conditionNode.AddNode(Condition.ToGraphNode());

        root.AddNode(conditionNode);

        root.AddNode(Body.ToGraphNode());

        return root;
    }
}