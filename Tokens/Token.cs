using System;
using System.Text;

[System.Diagnostics.DebuggerDisplay("{Location} {Kind} : {rep.ToString()}")]
public class Token
{
    public static readonly Token NULL = new Token('\0', TokenKind.EOF, new Location(-1, -1), false);

    public bool IsValid { get; set; } // yes, we want it to be public for error-recovery stuff

    public TriviaToken? LeadingTrivia { get; protected set; }

    public TriviaToken? TrailingTrivia { get; protected set; }

    public TokenKind Kind { get; protected set; }

    protected string rep;

    public string Representation {
        get => rep;
    }

    public Location Location { get; protected set; }

    public Token(char representation, TokenKind kind, Location location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(representation.ToString(), kind, location, isValid, leading, trailing) { }

    public Token(string representation, TokenKind kind, Location location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null) {
        rep = representation;
        Kind = kind;
        Location = location;
        LeadingTrivia = leading;
        TrailingTrivia = trailing;
        IsValid = isValid;
    }

    public void AddLeadingTrivia(TriviaToken trivia) {
        if (trivia is null) {
            Logger.Warning(new InvalidCallException(
                message : "Something tried to add a null (leading) TriviaToken to this token, but that's not allowed",
                location: Location
            ));

            return;
        }

        if (LeadingTrivia is null)
            LeadingTrivia = trivia;
        else
            LeadingTrivia.AddLeadingTrivia(trivia);
    }

    public void AddTrailingTrivia(TriviaToken trivia) {
        if (trivia is null) {
            Logger.Warning(new InvalidCallException(
                message : "Something tried to add a null (trailing) TriviaToken to this token, but that's not allowed",
                location: Location
            ));

            return;
        }

        if (TrailingTrivia is null)
            TrailingTrivia = trivia;
        else
            TrailingTrivia.AddTrailingTrivia(trivia);
    }

    public override string ToString() {
        return rep;
    }

    public static implicit operator string(Token token) {
        return token.rep;
    }
}