/// <summary>
/// Represents a for-loop statement
/// </summary>
public record ForNode(
    Token Token,
    Tuple<StatementNode> Header,
    Tuple<StatementNode> Body,
    bool IsValid = true
) : StatementNode(Token, new LocationRange(Token.Location, Body.Location), IsValid)
{
    public new static readonly ForNode NULL = new(Token.NULL, Tuple<StatementNode>.NULL, Tuple<StatementNode>.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
