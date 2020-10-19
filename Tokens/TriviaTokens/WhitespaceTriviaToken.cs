public class WhitespaceTriviaToken : TriviaToken
{
    public int WhitespaceCount { get; protected set; }

    public WhitespaceTriviaToken(char whitespaceChar,
                                 int count,
                                 Location location,
                                 bool isValid = true,
                                 TriviaToken? leading = null,
                                 TriviaToken? trailing = null)
        : base(new string(whitespaceChar, count), TriviaKind.whitespace, location, isValid, leading, trailing)
    {
        WhitespaceCount = count;
    }

    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}