using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class CommentTriviaToken : TriviaToken
{
    protected IList<CommentTriviaToken> innerComments;

    public ReadOnlyCollection<CommentTriviaToken> InnerComments {
        get => innerComments.AsReadOnly();
    }

    public CommentTriviaToken(string rep,
                              Location location,
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
}