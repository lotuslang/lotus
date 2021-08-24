/// <summary>
/// Represents a foreach loop statement (foreach (item in collection) { })
/// </summary>
public record ForeachNode(
    Token Token,
    Token InToken,
    IdentNode ItemName,
    ValueNode CollectionRef,
    SimpleBlock Body,
    Token OpenParenthesis,
    Token CloseParenthesis,
    bool IsValid = true
) : StatementNode(Token, new LocationRange(Token.Location, Body.Location), IsValid)
{
    public new static readonly ForeachNode NULL
        = new(
            Token.NULL,
            Token.NULL,
            IdentNode.NULL,
            ValueNode.NULL,
            SimpleBlock.NULL,
            Token.NULL,
            Token.NULL,
            false
        );

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
