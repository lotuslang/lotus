public class StringToken : ComplexToken
{
    public StringToken(string representation, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.@string, location, isValid, leading, trailing) { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}