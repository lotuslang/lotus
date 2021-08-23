public record IdentToken(string Representation, LocationRange Location, bool IsValid = true)
: ComplexToken(Representation, TokenKind.identifier, Location, IsValid)
{
    public new static readonly IdentToken NULL = new("", LocationRange.NULL, false);

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}