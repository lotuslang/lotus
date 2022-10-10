namespace Lotus.Syntax;

public sealed record IfNode(
    ParenthesizedValueNode Condition,
    Tuple<StatementNode> Body,
    ElseNode ElseNode,
    Token Token
) : StatementNode(Token, new LocationRange(Token.Location, ElseNode != ElseNode.NULL ? ElseNode.Location : Body.Location))
{
    public new static readonly IfNode NULL = new(ParenthesizedValueNode.NULL, Tuple<StatementNode>.NULL, ElseNode.NULL, Token.NULL) { IsValid = false };

    public bool HasElse => ElseNode != ElseNode.NULL;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IStatementVisitor<T> visitor) => visitor.Visit(this);
}