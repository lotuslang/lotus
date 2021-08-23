public record ComplexToken : Token
{
    public new static readonly ComplexToken NULL = new("", TokenKind.delimiter, LocationRange.NULL);

    public ComplexToken(string representation, TokenKind kind, LocationRange location, bool isValid = true)
        : base(representation, kind, location, isValid) { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}