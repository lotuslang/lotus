using System;

public class WhileNode : StatementNode
{
    public bool IsDoLoop { get; protected set; }

    public ComplexToken? DoToken { get; protected set; }

    public ValueNode Condition { get; protected set; }

    public SimpleBlock Body { get; protected set; }

    public WhileNode(ValueNode condition, SimpleBlock body, ComplexToken whileToken, bool isValid = true) : base(whileToken, isValid) {
        Condition = condition;
        IsDoLoop = false;
        DoToken = null;
        Body = body;
    }

    public WhileNode(ValueNode condition, SimpleBlock body, ComplexToken whileToken, ComplexToken doToken, bool isValid = true)
        : this(condition, body, whileToken, isValid)
    {
        IsDoLoop = true;
        DoToken = doToken;
    }

    public override GraphNode ToGraphNode()
        => new GraphNode(GetHashCode(), IsDoLoop ? "do-while" : "while") {
            new GraphNode(HashCode.Combine(this, "condition"), "condition") {
                Condition.ToGraphNode()
            }.SetTooltip("loop condition"), // FIXME: Choose color
            Body.ToGraphNode()
        }.SetTooltip(IsDoLoop ? "do-while loop" : "while loop"); // FIXME: Choose color
}