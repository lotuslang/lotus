namespace Lotus.Syntax;

public sealed record OperatorToken(string Representation, Precedence Precedence, bool IsLeftAssociative, LocationRange Location)
: Token(Representation, TokenKind.@operator, Location)
{
    public new static readonly OperatorToken NULL = new("", Precedence.Comma, false, LocationRange.NULL) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);

    protected override string DbgStr() => $"<operator({Precedence})> {_repr} @ {Location}";
}