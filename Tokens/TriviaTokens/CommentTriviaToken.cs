using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class CommentTriviaToken : TriviaToken
{
    public new static readonly CommentTriviaToken NULL = new CommentTriviaToken("", LocationRange.NULL, isValid: false);
    protected IList<CommentTriviaToken> innerComments;

    public ReadOnlyCollection<CommentTriviaToken> InnerComments {
        get => innerComments.AsReadOnly();
    }

    public CommentTriviaToken(string rep,
                              LocationRange location,
                              IList<CommentTriviaToken>? inner = null,
                              bool isValid = true,
                              TriviaToken? leading = null,
                              TriviaToken? trailing = null)
        : base(rep, TriviaKind.comment, location, isValid, leading, trailing)
    {
        innerComments = inner ?? new List<CommentTriviaToken>();
    }

    public void AddComment(CommentTriviaToken comment) {
        innerComments.Add(comment);
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}