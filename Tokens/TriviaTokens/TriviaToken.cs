public class TriviaToken : Token
{
    public new TriviaKind Kind { get; protected set; }

    public static readonly new TriviaToken NULL = new TriviaToken("", TriviaKind.EOF, new Location());

    public TriviaToken(string rep, TriviaKind kind, Location location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(rep, TokenKind.trivia, location, isValid, leading, trailing) {
        Kind = kind;
    }
}