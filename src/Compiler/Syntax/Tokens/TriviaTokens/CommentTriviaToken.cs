namespace Lotus.Syntax;

public sealed record CommentTriviaToken : TriviaToken
{
    public new static readonly CommentTriviaToken NULL = new("", LocationRange.NULL) { IsValid = false };

    public ImmutableArray<CommentTriviaToken> InnerComments { get; init; }

    public CommentTriviaToken(string rep, LocationRange location, ImmutableArray<CommentTriviaToken> inner = default)
        : base(rep, TriviaKind.comment, location)
    {
        InnerComments = inner;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}