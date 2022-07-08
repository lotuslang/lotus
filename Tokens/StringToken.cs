public record StringToken(string Representation, LocationRange Location, bool IsValid = true)
: Token(Representation, TokenKind.@string, Location, IsValid)
{
    public new static readonly StringToken NULL = new("", LocationRange.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}