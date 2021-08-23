public record NewlineTriviaToken : WhitespaceTriviaToken
{
    public new static readonly NewlineTriviaToken NULL = new(0, LocationRange.NULL, false);
    public NewlineTriviaToken(int count,
                              LocationRange location,
                              bool isValid = true)
        : base('\n', count, location, isValid)
    {
        Kind = TriviaKind.newline;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}