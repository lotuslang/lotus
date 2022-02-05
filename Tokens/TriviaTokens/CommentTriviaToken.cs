
public record CommentTriviaToken : TriviaToken
{
    public new static readonly CommentTriviaToken NULL = new("", LocationRange.NULL, isValid: false);

    public List<CommentTriviaToken> InnerComments {
        get;
        init;
    }

    public CommentTriviaToken(string rep,
                              LocationRange location,
                              IList<CommentTriviaToken>? inner = null,
                              bool isValid = true)
        : base(rep, TriviaKind.comment, location, isValid)
    {
        InnerComments = inner?.ToList() ?? new List<CommentTriviaToken>();
    }

    public void AddComment(CommentTriviaToken comment)
        => InnerComments.Add(comment);

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}