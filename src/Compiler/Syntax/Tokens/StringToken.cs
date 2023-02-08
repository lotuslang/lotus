namespace Lotus.Syntax;

public record StringToken(string Representation, LocationRange Location)
: Token(Representation, TokenKind.@string, Location)
{
    public new static readonly StringToken NULL = new("", LocationRange.NULL) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}