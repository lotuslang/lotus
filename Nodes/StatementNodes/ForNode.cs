/// <summary>
/// Represents a for-loop statement
/// </summary>
public record ForNode(
    Token Token,
    IList<StatementNode> Header,
    SimpleBlock Body,
    Token OpeningParenthesis,
    Token ClosingParenthesis,
    bool IsValid = true
) : StatementNode(Token, new LocationRange(Token.Location, Body.Location), IsValid)
{
    public new static readonly ForNode NULL = new(Token.NULL, Array.Empty<StatementNode>(), SimpleBlock.NULL, Token.NULL, Token.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
