namespace Lotus.Syntax;

[DebuggerDisplay("{DbgStr(),nq}")]
public record Token : ILocalized
{
    public static readonly Token NULL = new("", TokenKind.EOF, LocationRange.NULL) { IsValid = false };

    protected TokenKind _kind;
    public ref readonly TokenKind Kind => ref _kind;

    protected string _repr;
    public ref readonly string Representation => ref _repr;

    public LocationRange Location { get; init; }

    public bool IsValid { get; set; } = true;

    public Token(string repr, TokenKind kind, LocationRange location) {
        _repr = repr;
        _kind = kind;
        Location = location;
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
        Debug.Assert(trivia is not null);

        if (_leading is null)
            _leading = trivia;
        else
            _leading.AddLeadingTrivia(trivia);
    }

    public void AddTrailingTrivia(TriviaToken? trivia) {
        Debug.Assert(trivia is not null);

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

    public static implicit operator ReadOnlySpan<char>(Token token)
        => token._repr;

    internal Token ShallowClone() => new(this);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);

    protected virtual string DbgStr() => $"<{_kind}> {_repr} @ {Location}";
}