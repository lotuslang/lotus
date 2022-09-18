public sealed record NewlineTriviaToken : WhitespaceTriviaToken
{
    public new static readonly NewlineTriviaToken NULL = new(0, LocationRange.NULL) { IsValid = false };
    public NewlineTriviaToken(int count, LocationRange location)
        : base('\n', count, location)
    {
        Kind = TriviaKind.newline;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}