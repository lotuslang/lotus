public class TriviaToken : Token
{
    public new TriviaKind Kind { get; protected set; }

    public static readonly new TriviaToken NULL = new TriviaToken("", TriviaKind.EOF, new Location(-1, -1));

    public TriviaToken(string rep, TriviaKind kind, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(rep, TokenKind.trivia, location, isValid, leading, trailing) {
        Kind = kind;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}