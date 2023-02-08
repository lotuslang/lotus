namespace Lotus.Syntax;

public sealed record ComplexStringToken(
    string Representation,
    ImmutableArray<InterpolatedSection> CodeSections,
    LocationRange Location
)
: StringToken(Representation, Location)
{
    public new static readonly ComplexStringToken NULL = new("", ImmutableArray<InterpolatedSection>.Empty, LocationRange.NULL) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}

public readonly struct InterpolatedSection : ILocalized {
    public readonly int StringOffset;
    public readonly ImmutableArray<Token> Tokens;
    public readonly LocationRange Location { get; }

    public InterpolatedSection(int offset, ImmutableArray<Token> tokens, LocationRange location) {
        StringOffset = offset;
        Tokens = tokens;
        Location = location;
    }

    public int TokenCount => Tokens.Length;

    public Consumer<Token> ToConsumer()
        => new(
            Tokens,
            Token.NULL with { Location = Location.GetLastLocation() },
            Location.filename
        );
}