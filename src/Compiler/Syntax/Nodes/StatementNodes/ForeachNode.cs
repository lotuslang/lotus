namespace Lotus.Syntax;

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
    Token ClosingParen
) : StatementNode(Token, new LocationRange(Token.Location, Body.Location))
{
    public new static readonly ForeachNode NULL
        = new(
            Token.NULL,
            Token.NULL,
            IdentNode.NULL,
            ValueNode.NULL,
            Tuple<StatementNode>.NULL,
            Token.NULL,
            Token.NULL
        ) { IsValid = false };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IStatementVisitor<T> visitor) => visitor.Visit(this);
}
