namespace Lotus.Syntax;

public sealed record ImportNode(Tuple<NameNode> Names, FromOrigin? FromOrigin, Token Token)
: TopLevelNode(
    Token,
    FromOrigin is null
        ? Names.LastOrDefault()?.Location ?? Token.Location  // this shouldn't happen anyway
        : new LocationRange(
                Names.LastOrDefault()?.Location ?? Token.Location,
                FromOrigin.Location
          )
)
{
    public new static readonly ImportNode NULL = new(Tuple<NameNode>.NULL, null, Token.NULL) { IsValid = false };

    [MemberNotNullWhen(true, nameof(FromOrigin))]
    public bool HasOrigin => FromOrigin is not null;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}

public class FromOrigin : ILocalized
{
    public static readonly FromOrigin NULL = new(StringNode.NULL, Token.NULL);

    public Union<StringNode, NameNode> OriginName { get; }
    public Token FromToken { get; }
    public LocationRange Location { get; init; }
    public bool IsValid { get; init; }

    public FromOrigin(Union<StringNode, NameNode> originName, Token token) {
        OriginName = originName;
        FromToken = token;
        Location = new(
            token.Location,
            originName.Match(
                str => str.Location,
                name => name.Location
            )
        );
    }
}