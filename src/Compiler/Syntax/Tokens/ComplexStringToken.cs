namespace Lotus.Syntax;

public sealed record ComplexStringToken(string Representation, ImmutableArray<ImmutableArray<Token>> CodeSections, LocationRange Location)
: StringToken(Representation, Location)
{
    public new static readonly ComplexStringToken NULL = new("", ImmutableArray<ImmutableArray<Token>>.Empty, LocationRange.NULL) { IsValid = false };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}