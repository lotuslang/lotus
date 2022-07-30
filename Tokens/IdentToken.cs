public sealed record IdentToken(string Representation, LocationRange Location, bool IsValid = true)
: Token(Representation, TokenKind.identifier, Location, IsValid)
{
    public new static readonly IdentToken NULL = new("", LocationRange.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}