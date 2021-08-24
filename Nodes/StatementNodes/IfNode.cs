using System.Diagnostics.CodeAnalysis;


public record IfNode(
    ParenthesizedValueNode Condition,
    SimpleBlock Body,
    ElseNode? ElseNode,
    Token Token,
    bool IsValid = true
) : StatementNode(Token, new LocationRange(Token.Location, ElseNode?.Location ?? Body.Location), IsValid)
{
    public new static readonly IfNode NULL = new(ParenthesizedValueNode.NULL, SimpleBlock.NULL, null, Token.NULL, false);

    [MemberNotNullWhen(true, nameof(ElseNode))]
    public bool HasElse => ElseNode != null;

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}