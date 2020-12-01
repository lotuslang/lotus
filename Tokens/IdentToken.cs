public class IdentToken : ComplexToken
{
    public new static readonly IdentToken NULL = new IdentToken("", LocationRange.NULL, false);

    public IdentToken(string representation, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.ident, location, isValid, leading, trailing) { }

    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}