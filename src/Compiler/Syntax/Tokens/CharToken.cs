namespace Lotus.Syntax;

public record CharToken(char Value, string Representation, LocationRange Location)
: Token(Representation, TokenKind.@char, Location)
{
    public new static readonly CharToken NULL = new('\0', LocationRange.NULL) { IsValid = false };

    public CharToken(char value, LocationRange location) : this(value, value.ToString(), location) {}

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}