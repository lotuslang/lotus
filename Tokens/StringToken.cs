public record StringToken(string Representation, LocationRange Location, bool IsValid = true)
: ComplexToken(Representation, TokenKind.@string, Location, IsValid)
{
    public new static readonly StringToken NULL = new("", LocationRange.NULL, false);

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}