public class WhitespaceTriviaToken : TriviaToken
{
    public new static readonly WhitespaceTriviaToken NULL = new WhitespaceTriviaToken('\0', 0, LocationRange.NULL, false);
    public int WhitespaceCount { get; protected set; }

    public WhitespaceTriviaToken(char whitespaceChar,
                                 int count,
                                 LocationRange location,
                                 bool isValid = true,
                                 TriviaToken? leading = null,
                                 TriviaToken? trailing = null)
        : base(new string(whitespaceChar, count), TriviaKind.whitespace, location, isValid, leading, trailing)
    {
        WhitespaceCount = count;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}