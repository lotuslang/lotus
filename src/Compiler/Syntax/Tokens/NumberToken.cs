namespace Lotus.Syntax;

[Flags]
public enum NumberKind {
    Unknown = 0,
    Unsigned = 1 << 0,
    Int = 1 << 1,
    Long = 1 << 2,
    Float = 1 << 3,
    Double = 1 << 4,

    UInt = Unsigned | Int,
    ULong = Unsigned | Long
}

public sealed record NumberToken : Token
{
    public new static readonly NumberToken NULL = new("", Double.NaN, LocationRange.NULL, default) { IsValid = false };

    public NumberKind NumberKind { get; }

    public object Value { get; }

    public NumberToken(string repr, object value, LocationRange location, NumberKind numberKind)
        : base(repr, TokenKind.number, location) {
        _repr = repr;
        Value = value;
        NumberKind = numberKind;
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}