/// <summary>
/// Represents a foreach loop statement (foreach (item in collection) { })
/// </summary>
public sealed record ForeachNode(
    Token Token,
    Token InToken,
    IdentNode ItemName,
    ValueNode CollectionRef,
    Tuple<StatementNode> Body,
    Token OpeningParen,
    Token ClosingParen,
    bool IsValid = true
) : StatementNode(Token, new LocationRange(Token.Location, Body.Location), IsValid)
{
    public new static readonly ForeachNode NULL
        = new(
            Token.NULL,
            Token.NULL,
            IdentNode.NULL,
            ValueNode.NULL,
            Tuple<StatementNode>.NULL,
            Token.NULL,
            Token.NULL,
            false
        );

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
