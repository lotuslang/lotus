namespace Lotus.Syntax;

public record TriviaToken : Token
{
    public new TriviaKind Kind { get; init; }

    public TriviaToken(string rep, TriviaKind kind, LocationRange location)
        : base(rep, TokenKind.trivia, location) {
        Kind = kind;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}