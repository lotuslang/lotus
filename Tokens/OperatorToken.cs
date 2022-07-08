[DebuggerDisplay("{Location} {Precedence}({(int)Precedence}) : {Representation}")]
public record OperatorToken(string Representation, Precedence Precedence, bool IsLeftAssociative, LocationRange Location, bool IsValid = true)
: Token(Representation, TokenKind.@operator, Location, IsValid)
{
    public new static readonly OperatorToken NULL = new("", Precedence.Comma, false, LocationRange.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}