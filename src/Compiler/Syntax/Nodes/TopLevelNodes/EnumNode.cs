namespace Lotus.Syntax;

public sealed record EnumNode(
    TypeDecName Name,
    Tuple<ValueNode> Values,
    Token EnumToken,
    ImmutableArray<Token> Modifiers
) : TopLevelNode(EnumToken, new LocationRange(EnumToken.Location, Values.ClosingToken.Location))
{
    public new static readonly EnumNode NULL
        = new(
            TypeDecName.NULL,
            Tuple<ValueNode>.NULL,
            Token.NULL,
            default
        ) { IsValid = false };

    public Token OpeningBracket => Values.OpeningToken;
    public Token ClosingBracket => Values.ClosingToken;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}