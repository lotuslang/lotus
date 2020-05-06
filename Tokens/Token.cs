using System;
using System.Text;

[System.Diagnostics.DebuggerDisplay("{Location} {Kind} : {rep.ToString()}")]
public class Token
{
    public static readonly Token NULL = new Token('\0', TokenKind.EOF, new Location());

    public TriviaToken? LeadingTrivia { get; protected set; }

    public TriviaToken? TrailingTrivia { get; protected set; }

    public TokenKind Kind { get; protected set; }

    protected StringBuilder rep;

    public string Representation {
        get => rep.ToString();
    }

    public Location Location { get; protected set; }

    public Token(char representation, TokenKind kind, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(representation.ToString(), kind, location, leading, trailing) { }

    public Token(string representation, TokenKind kind, Location location, TriviaToken? leading = null, TriviaToken? trailing = null) {
        rep = new StringBuilder();
        rep.Append(representation);
        Kind = kind;
        Location = location;
        LeadingTrivia = leading;
        TrailingTrivia = trailing;
    }

    public void AddLeadingTrivia(TriviaToken trivia) {
        if (trivia is null) {
            throw new ArgumentNullException(nameof(trivia));
        }

        if (LeadingTrivia is null)
            LeadingTrivia = trivia;
        else
            LeadingTrivia.AddLeadingTrivia(trivia);
    }

    public void AddTrailingTrivia(TriviaToken trivia) {
        if (trivia is null) {
            throw new ArgumentNullException(nameof(trivia));
        }

        if (TrailingTrivia is null)
            TrailingTrivia = trivia;
        else
            TrailingTrivia.AddTrailingTrivia(trivia);
    }

    public override string ToString() {
        return rep.ToString();
    }

    /*public virtual GraphNode ToGraphNode() {
        return new GraphNode(GetHashCode(), Representation);
    }*/

    public static implicit operator TokenKind(Token token) {
        return token.Kind;
    }

    public static implicit operator string(Token token) {
        return token.rep.ToString();
    }
}