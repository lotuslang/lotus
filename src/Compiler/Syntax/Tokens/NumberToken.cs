namespace Lotus.Syntax;

public sealed record NumberToken : Token
{
    public new static readonly NumberToken NULL = new("", Double.NaN, LocationRange.NULL, default) { IsValid = false };

    public NumberKind NumberKind { get; }

    public object Value { get; }

    public NumberToken(string repr, object value, LocationRange location, NumberKind numberKind)
        : base(repr, TokenKind.number, location) {
        Value = value;
        NumberKind = numberKind;
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}