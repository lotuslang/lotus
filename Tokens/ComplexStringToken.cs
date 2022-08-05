public sealed record ComplexStringToken(string Representation, List<Token[]> CodeSections, LocationRange Location)
: StringToken(Representation, Location)
{
    public new static readonly ComplexStringToken NULL = new("", new List<Token[]>(), LocationRange.NULL) { IsValid = false };

    public void AddSection(Token[] section)
        => CodeSections.Add(section);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}