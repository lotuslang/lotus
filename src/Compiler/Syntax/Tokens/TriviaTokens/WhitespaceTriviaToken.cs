namespace Lotus.Syntax;

public record WhitespaceTriviaToken(char WhitespaceChar, int WhitespaceCount, LocationRange Location)
: TriviaToken(new string(WhitespaceChar, WhitespaceCount), TriviaKind.whitespace, Location)
{
    public new static readonly WhitespaceTriviaToken NULL = new('\0', 0, LocationRange.NULL) { IsValid = false };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}