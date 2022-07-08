public record ImportNode(IList<NameNode> Names, FromNode FromStatement, Token Token, bool IsValid = true)
: TopLevelNode(
    Token,
    new LocationRange(
        FromStatement.Location,
        Names.LastOrDefault()?.Location ?? Token.Location // this shouldn't happen anyway
    ),
    IsValid
)
{
    public new static readonly ImportNode NULL = new(Array.Empty<NameNode>(), FromNode.NULL, Token.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}
