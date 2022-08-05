public sealed record WhileNode(
    ParenthesizedValueNode Condition,
    Tuple<StatementNode> Body,
    Token Token,
    Token DoToken
) : StatementNode(
    Token,
    DoToken != Token.NULL
        ? new LocationRange(DoToken.Location, Token.Location)
        : new LocationRange(Token.Location, Body.Location)
)
{
    public new static readonly WhileNode NULL = new(ParenthesizedValueNode.NULL, Tuple<StatementNode>.NULL, Token.NULL, Token.NULL) { IsValid = false };

    public bool IsDoLoop => DoToken != Token.NULL;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}