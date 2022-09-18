using Lotus.Semantic;

namespace Lotus.Syntax;

public sealed record EnumNode(
    TypeDecName Name,
    Tuple<ValueNode> Values,
    Token EnumToken
) : TopLevelNode(EnumToken, new LocationRange(EnumToken.Location, Values.ClosingToken.Location)), IAccessible
{
    public new static readonly EnumNode NULL
        = new(
            TypeDecName.NULL,
            Tuple<ValueNode>.NULL,
            Token.NULL
        ) { IsValid = false };

    public Token OpeningBracket => Values.OpeningToken;
    public Token ClosingBracket => Values.ClosingToken;

    public Token AccessToken { get; set; } = Token.NULL;

    public AccessLevel AccessLevel { get; set; } = AccessLevel.Public;

    AccessLevel IAccessible.DefaultAccessLevel => AccessLevel.Public;
    AccessLevel IAccessible.ValidLevels => AccessLevel.Public | AccessLevel.Internal;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}