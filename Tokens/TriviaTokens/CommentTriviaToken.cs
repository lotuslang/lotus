public sealed record CommentTriviaToken : TriviaToken
{
    public new static readonly CommentTriviaToken NULL = new("", LocationRange.NULL, isValid: false);

    public ImmutableArray<CommentTriviaToken> InnerComments { get; init; }

    public CommentTriviaToken(string rep,
                              LocationRange location,
                              IList<CommentTriviaToken>? inner = null,
                              bool isValid = true)
        : base(rep, TriviaKind.comment, location, isValid)
    {
        InnerComments = inner?.ToImmutableArray() ?? ImmutableArray<CommentTriviaToken>.Empty;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}