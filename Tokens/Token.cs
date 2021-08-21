[System.Diagnostics.DebuggerDisplay("<{Kind}> {rep.ToString()} @ {Location}")]
public class Token
{
    public static readonly Token NULL = new('\0', TokenKind.EOF, LocationRange.NULL, false);

    public bool IsValid { get; set; } // yes, we want it to be public for error-recovery stuff

    public TriviaToken? LeadingTrivia { get; protected set; }

    public TriviaToken? TrailingTrivia { get; protected set; }

    public TokenKind Kind { get; protected set; }

    protected string rep;

    public string Representation {
        get => rep;
    }

    public LocationRange Location { get; set; }

    public Token(char representation, TokenKind kind, Location location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(representation.ToString(), kind, location, isValid, leading, trailing) { }

    public Token(char representation, TokenKind kind, LocationRange range, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : this(representation.ToString(), kind, range, isValid, leading, trailing) { }

    public Token(string representation, TokenKind kind, LocationRange range, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null) {
        rep = representation;
        Kind = kind;
        Location = range;
        LeadingTrivia = leading;
        TrailingTrivia = trailing;
        IsValid = isValid;
    }

    public void AddLeadingTrivia(TriviaToken trivia) {
        if (trivia is null) {
            Logger.Warning(new InvalidCallException(
                message : "Something tried to add a null (leading) TriviaToken to this token, but that's not allowed",
                range: Location
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
                range: Location
            ));

            return;
        }

        if (TrailingTrivia is null)
            TrailingTrivia = trivia;
        else
            TrailingTrivia.AddTrailingTrivia(trivia);
    }

    public bool HasTrivia(TriviaKind kind, out TriviaToken trivia) {

        if (LeadingTrivia is not null) {
            if (LeadingTrivia.Kind == kind) {
                trivia = LeadingTrivia;
                return true;
            }

            return LeadingTrivia.HasTrivia(kind, out trivia);
        }

        if (TrailingTrivia is not null) {
            if (TrailingTrivia.Kind == kind) {
                trivia = TrailingTrivia;
                return true;
            }

            return TrailingTrivia.HasTrivia(kind, out trivia);
        }

        trivia = TriviaToken.NULL;

        return false;
    }

    public bool HasTrivia(string rep, out TriviaToken trivia) {
        if (LeadingTrivia is not null) {
            if (LeadingTrivia == rep) {
                trivia = LeadingTrivia;
                return true;
            }

            return LeadingTrivia.HasTrivia(rep, out trivia);
        }

        if (TrailingTrivia is not null) {
            if (TrailingTrivia == rep) {
                trivia = TrailingTrivia;
                return true;
            }

            return TrailingTrivia.HasTrivia(rep, out trivia);
        }

        trivia = TriviaToken.NULL;

        return false;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);

    public override string ToString()
        => rep;

    public static implicit operator string(Token token)
        => token.rep;
}