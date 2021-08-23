public record WhitespaceTriviaToken(char WhitespaceChar, int WhitespaceCount, LocationRange Location, bool IsValid = true)
: TriviaToken(new string(WhitespaceChar, WhitespaceCount), TriviaKind.whitespace, Location, IsValid)
{
    public new static readonly WhitespaceTriviaToken NULL = new('\0', 0, LocationRange.NULL, false);

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}