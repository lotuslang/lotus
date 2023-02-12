namespace Lotus.Syntax;

public record CharToken(char Value, LocationRange Location)
: Token(Value.ToString(), TokenKind.@char, Location)
{
    public new static readonly CharToken NULL = new('\0', LocationRange.NULL) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}