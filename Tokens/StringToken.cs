public class StringToken : ComplexToken
{
    public StringToken(string representation, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.@string, location, leading, trailing) { }
}