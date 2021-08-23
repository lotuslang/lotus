[System.Diagnostics.DebuggerDisplay("<{Kind}> {Representation} @ {Location}")]
public record Token(string Representation, TokenKind Kind, LocationRange Location, bool IsValid = true)
//public class Token
{
    public static readonly Token NULL = new("", TokenKind.EOF, LocationRange.NULL, false);

    protected TriviaToken? leading, trailing;

    public TriviaToken? LeadingTrivia {
        get => leading;
        init {
            if (value is null) return;
            AddLeadingTrivia(value);
        }
    }
    public TriviaToken? TrailingTrivia {
        get => trailing;
        init {
            if (value is null) return;
            AddTrailingTrivia(value);
        }
    }

    public void AddLeadingTrivia(TriviaToken trivia) {
        if (trivia is null) {
            Logger.Warning(new InvalidCallException(
                message : "Something tried to add a null (leading) TriviaToken to this token, but that's not allowed",
                range: Location
            ));

            return;
        }

        if (leading is null)
            leading = trivia;
        else
            leading.AddLeadingTrivia(trivia);
    }

    public void AddTrailingTrivia(TriviaToken trivia) {
        if (trivia is null) {
            Logger.Warning(new InvalidCallException(
                message : "Something tried to add a null (trailing) TriviaToken to this token, but that's not allowed",
                range: Location
            ));

            return;
        }

        if (trailing is null)
            trailing = trivia;
        else
            trailing.AddTrailingTrivia(trivia);
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
        if (leading is not null) {
            if (leading == rep) {
                trivia = leading;
                return true;
            }

            return leading.HasTrivia(rep, out trivia);
        }

        if (trailing is not null) {
            if (trailing == rep) {
                trivia = trailing;
                return true;
            }

            return trailing.HasTrivia(rep, out trivia);
        }

        trivia = TriviaToken.NULL;

        return false;
    }

    public override string ToString()
        => Representation;

    public static implicit operator string(Token token)
        => token.Representation;

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}