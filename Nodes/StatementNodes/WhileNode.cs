using System;

public class WhileNode : StatementNode
{
    public bool IsDoLoop { get; }

    public ComplexToken? DoToken { get; }

    public ParenthesizedValueNode Condition { get; }

    public SimpleBlock Body { get; }

    public WhileNode(ParenthesizedValueNode condition, SimpleBlock body, ComplexToken whileToken, bool isValid = true)
        : base(whileToken, isValid)
    {
        Condition = condition;
        IsDoLoop = false;
        DoToken = null;
        Body = body;
    }

    public WhileNode(ParenthesizedValueNode condition, SimpleBlock body, ComplexToken whileToken, ComplexToken doToken, bool isValid = true)
        : this(condition, body, whileToken, isValid)
    {
        IsDoLoop = true;
        DoToken = doToken;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}