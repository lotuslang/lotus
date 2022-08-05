public sealed record ImportNode(ImmutableArray<NameNode> Names, FromNode FromStatement, Token Token)
: TopLevelNode(
    Token,
    new LocationRange(
        FromStatement.Location,
        Names.LastOrDefault()?.Location ?? Token.Location // this shouldn't happen anyway
    )
)
{
    public new static readonly ImportNode NULL = new(ImmutableArray<NameNode>.Empty, FromNode.NULL, Token.NULL) { IsValid = false };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}
