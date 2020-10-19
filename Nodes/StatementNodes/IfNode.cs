using System;

public class IfNode : StatementNode
{
    public ParenthesizedValueNode Condition { get; }

    public SimpleBlock Body { get; }

    public ElseNode? ElseNode { get; }

    public bool HasElse { get => ElseNode != null; }

    public IfNode(ParenthesizedValueNode condition, SimpleBlock body, ComplexToken ifToken, bool isValid = true)
        : base(ifToken, isValid)
    {
        Condition = condition;
        Body = body;
        ElseNode = null;
    }

    public IfNode(ParenthesizedValueNode condition, SimpleBlock body, ElseNode elseNode, ComplexToken ifToken, bool isValid = true)
        : this(condition, body, ifToken, isValid)
    {
        ElseNode = elseNode;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}