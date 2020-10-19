public class StringToken : ComplexToken
{
    public StringToken(string representation, Location location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.@string, location, isValid, leading, trailing) { }

    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}