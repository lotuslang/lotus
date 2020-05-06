public class TriviaToken : Token
{
    public new TriviaKind Kind { get; protected set; }

    public static readonly new TriviaToken NULL = new TriviaToken("", TriviaKind.EOF, new Location());

    public TriviaToken(string rep, TriviaKind kind, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(rep, TokenKind.trivia, location, leading, trailing) {
        Kind = kind;
    }
}