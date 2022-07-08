public record WhileNode(
    ParenthesizedValueNode Condition,
    SimpleBlock Body,
    Token Token,
    Token DoToken,
    bool IsValid = true
) : StatementNode(
    Token,
    DoToken != Token.NULL
        ? new LocationRange(DoToken.Location, Token.Location)
        : new LocationRange(Token.Location, Body.Location),
    IsValid
)
{
    public new static readonly WhileNode NULL = new(ParenthesizedValueNode.NULL, SimpleBlock.NULL, Token.NULL, Token.NULL, false);

    public bool IsDoLoop => DoToken != Token.NULL;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}