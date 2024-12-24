namespace Lotus.Syntax;

public sealed record TraitNode(
    Token Token,
    IdentNode Name,
    Token OpeningBracket,
    ImmutableArray<FunctionHeaderNode> Functions,
    Token ClosingBracket
) : TopLevelNode(Token, new LocationRange(Token, ClosingBracket)) {
    public new static readonly TraitNode NULL
        = new(Token.NULL, IdentNode.NULL, Token.NULL, [], Token.NULL);

    public ImmutableArray<Token> Modifiers { get; init; } = [];

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}