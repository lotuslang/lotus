namespace Lotus.Syntax;

public sealed record ComplexStringToken(
    ImmutableArray<string> TextSections,
    ImmutableArray<InterpolatedSection> CodeSections,
    LocationRange Location
)
    : Token(String.Join("{}", TextSections), TokenKind.complexString, Location)
{
    public new static readonly ComplexStringToken NULL = new([], [], LocationRange.NULL) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}

public readonly struct InterpolatedSection(
    int offset,
    ImmutableArray<Token> tokens,
    LocationRange location
) : ILocalized {
    public readonly int StringOffset = offset;
    public readonly ImmutableArray<Token> Tokens = tokens;
    public readonly LocationRange Location => location;

    public int TokenCount => Tokens.Length;

    public Tokenizer CreateTokenizer()
        => Tokenizer.FromExtractedTokens(
            Tokens,
            Location.filename
        );
}