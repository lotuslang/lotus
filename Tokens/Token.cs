[DebuggerDisplay("<{Kind}> {Representation} @ {Location}")]
public record Token : ILocalized
{
    public static readonly Token NULL = new("", TokenKind.EOF, LocationRange.NULL, false);

    public TokenKind Kind { get; }

    public string Representation { get; init; }

    public LocationRange Location { get; init; }

    public bool IsValid { get; set; }

    public Token(string repr, TokenKind kind, LocationRange location, bool isValid = true) {
        Representation = repr;
        Kind = kind;
        Location = location;
        IsValid = isValid;
    }

    protected TriviaToken? _leading, _trailing;

    public TriviaToken? LeadingTrivia {
        get => _leading;
        init {
            if (value is null) return;
            AddLeadingTrivia(value);
        }
    }
    public TriviaToken? TrailingTrivia {
        get => _trailing;
        init {
            if (value is null) return;
            AddTrailingTrivia(value);
        }
    }

    public void AddLeadingTrivia(TriviaToken? trivia) {
        if (trivia is null) {
            Logger.Warning(new InvalidCallError(ErrorArea.Tokenizer, Location) {
                Message = "Something tried to add a null (leading) TriviaToken to this token, but that's not allowed"
            });

            return;
        }

        if (_leading is null)
            _leading = trivia;
        else
            _leading.AddLeadingTrivia(trivia);
    }

    public void AddTrailingTrivia(TriviaToken? trivia) {
        if (trivia is null) {
            Logger.Warning(new InvalidCallError(ErrorArea.Tokenizer, Location) {
                Message = "Something tried to add a null (trailing) TriviaToken to this token, but that's not allowed"
            });

            return;
        }

        if (_trailing is null)
            _trailing = trivia;
        else
            _trailing.AddTrailingTrivia(trivia);
    }

    public bool HasTrivia(TriviaKind kind, out TriviaToken? trivia) {

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

        trivia = null;

        return false;
    }

    public bool HasTrivia(string rep, out TriviaToken? trivia) {
        if (_leading is not null) {
            if (_leading == rep) {
                trivia = _leading;
                return true;
            }

            return _leading.HasTrivia(rep, out trivia);
        }

        if (_trailing is not null) {
            if (_trailing == rep) {
                trivia = _trailing;
                return true;
            }

            return _trailing.HasTrivia(rep, out trivia);
        }

        trivia = null;

        return false;
    }

    public override string ToString()
        => Representation;

    public static implicit operator string(Token token)
        => token.Representation;

    internal Token ShallowClone() => new Token(this);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}