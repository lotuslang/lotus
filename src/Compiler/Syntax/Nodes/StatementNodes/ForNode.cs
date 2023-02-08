namespace Lotus.Syntax;

/// <summary>
/// Represents a for-loop statement
/// </summary>
public sealed record ForNode(
    Token Token,
    Tuple<StatementNode> Header,
    Tuple<StatementNode> Body
) : StatementNode(Token, new LocationRange(Token.Location, Body.Location))
{
    public new static readonly ForNode NULL = new(Token.NULL, Tuple<StatementNode>.NULL, Tuple<StatementNode>.NULL) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IStatementVisitor<T> visitor) => visitor.Visit(this);
}
