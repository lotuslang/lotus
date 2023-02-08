namespace Lotus.Syntax;

public sealed record IdentToken(string Representation, LocationRange Location)
: Token(Representation, TokenKind.identifier, Location)
{
    public new static readonly IdentToken NULL = new("", LocationRange.NULL) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}