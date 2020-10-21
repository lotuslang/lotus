public class NewlineTriviaToken : WhitespaceTriviaToken
{
    public NewlineTriviaToken(int count,
                              LocationRange location,
                              bool isValid = true,
                              TriviaToken? leading = null,
                              TriviaToken? trailing = null)
        : base('\n', count, location, isValid, leading, trailing)
    {
        Kind = TriviaKind.newline;
    }

    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}