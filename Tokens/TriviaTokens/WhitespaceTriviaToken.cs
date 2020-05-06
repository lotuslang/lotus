public class WhitespaceTriviaToken : TriviaToken
{
    public int WhitespaceCount { get; protected set; }

    public WhitespaceTriviaToken(char whitespaceChar,
                                 int count,
                                 Location location,
                                 TriviaToken? leading = null,
                                 TriviaToken? trailing = null)
        : base(new string(whitespaceChar, count), TriviaKind.whitespace, location,leading, trailing)
    {
        WhitespaceCount = count;
    }
}