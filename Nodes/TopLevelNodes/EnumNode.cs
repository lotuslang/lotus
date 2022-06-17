public record EnumNode(
    IdentNode Name,
    IList<ValueNode> Values,
    ValueNode Parent,
    Token EnumToken,
    Token OpenBracket,
    Token CloseBracket,
    bool IsValid
) : TopLevelNode(EnumToken, new LocationRange(EnumToken.Location, CloseBracket.Location), IsValid) {
    public new static readonly EnumNode NULL
        = new(
            IdentNode.NULL,
            Array.Empty<ValueNode>(),
            ValueNode.NULL,
            Token.NULL,
            Token.NULL,
            Token.NULL,
            false
        );

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}