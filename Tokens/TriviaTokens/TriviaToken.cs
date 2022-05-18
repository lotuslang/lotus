public record TriviaToken : Token
{
    public new TriviaKind Kind { get; init; }

    public TriviaToken(string rep, TriviaKind kind, LocationRange location, bool isValid = true)
        : base(rep, TokenKind.trivia, location, isValid) {
        Kind = kind;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}