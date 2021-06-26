public class TriviaToken : Token
{
    public static readonly new TriviaToken NULL = new TriviaToken("", TriviaKind.EOF, LocationRange.NULL);

    public new TriviaKind Kind { get; protected set; }

    public TriviaToken(string rep, TriviaKind kind, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(rep, TokenKind.trivia, location, isValid, leading, trailing) {
        Kind = kind;
    }

    public TriviaToken(char rep, TriviaKind kind, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(rep, TokenKind.trivia, location, isValid, leading, trailing) {
        Kind = kind;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}