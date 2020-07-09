public class IdentToken : ComplexToken
{
    public IdentToken(string representation, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.ident, location, leading, trailing) { }
}