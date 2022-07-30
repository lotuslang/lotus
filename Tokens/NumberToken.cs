[DebuggerDisplay("{Location} {Kind} : {val}")]
public sealed record NumberToken : Token
{
    public new static readonly NumberToken NULL = new("", Double.NaN, LocationRange.NULL, false);

    private double _val;

    public ref readonly double Value => ref _val;

    public NumberToken(string s, double d, LocationRange location, bool isValid = true)
        : base(s, TokenKind.number, location, isValid) {
        _repr = s;
        _val = d;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}