namespace Lotus.Syntax;

public sealed record IfNode(
    ParenthesizedValueNode Condition,
    Tuple<StatementNode> Body,
    ElseNode? ElseNode,
    Token Token
) : StatementNode(Token, new LocationRange(Token.Location, ElseNode?.Location ?? Body.Location))
{
    public new static readonly IfNode NULL = new(ParenthesizedValueNode.NULL, Tuple<StatementNode>.NULL, null, Token.NULL) { IsValid = false };

    [MemberNotNullWhen(true, nameof(ElseNode))]
    public bool HasElse => ElseNode is not null;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IStatementVisitor<T> visitor) => visitor.Visit(this);
}