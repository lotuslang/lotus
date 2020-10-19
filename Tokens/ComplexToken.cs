public class ComplexToken : Token
{
    public ComplexToken(string representation, TokenKind kind, Location location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, kind, location, isValid, leading, trailing) { }

    public virtual void Add(char ch)
        => rep += ch;

    public virtual void Add(string str)
        => rep += str;

    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}