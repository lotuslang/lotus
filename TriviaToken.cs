using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TriviaToken : Token
{
    public new TriviaKind Kind { get; protected set; }

    public TriviaToken(string rep, TriviaKind kind, Location? location, TriviaToken leading = null, TriviaToken trailing = null)
        : base(rep, TokenKind.trivia, location, leading, trailing) {
        Kind = kind;
    }
}

public class CommentTriviaToken : TriviaToken
{
    protected List<CommentTriviaToken> innerComments;

    public ReadOnlyCollection<CommentTriviaToken> InnerComments {
        get => innerComments.AsReadOnly();
    }

    public CommentTriviaToken(string rep,
                              Location? location,
                              IEnumerable<CommentTriviaToken> inner = null,
                              TriviaToken leading = null,
                              TriviaToken trailing = null)
        : base(rep, TriviaKind.comment, location, leading, trailing)
    {
        if (inner is null) {
            innerComments = new List<CommentTriviaToken>();
        } else {
            innerComments = new List<CommentTriviaToken>(inner);
        }
    }

    public void AddComment(CommentTriviaToken comment) {
        innerComments.Add(comment);
    }
}

public class WhitespaceTriviaToken : TriviaToken
{
    public int WhitespaceCounter { get; protected set; }

    public WhitespaceTriviaToken(char whitespaceChar,
                                 int count,
                                 Location? location,
                                 TriviaToken leading = null,
                                 TriviaToken trailing = null)
        : base(new string(whitespaceChar, count), TriviaKind.whitespace, location,leading, trailing)
    {
        WhitespaceCounter = count;
    }
}

public class NewlineTriviaToken : WhitespaceTriviaToken
{
    public NewlineTriviaToken(int count,
                              Location? location,
                              TriviaToken leading = null,
                              TriviaToken trailing = null)
        : base('\n', count, location, leading, trailing)
    {
        Kind = TriviaKind.newline;
    }
}

public enum TriviaKind {
    whitespace, newline, comment, EOF
}