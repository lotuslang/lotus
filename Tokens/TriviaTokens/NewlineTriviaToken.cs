public class NewlineTriviaToken : WhitespaceTriviaToken
{
    public NewlineTriviaToken(int count,
                              LocationRange location,
                              bool isValid = true,
                              TriviaToken? leading = null,
                              TriviaToken? trailing = null)
        : base('\n', count, location, isValid, leading, trailing)
    {
        Kind = TriviaKind.newline;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}