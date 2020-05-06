public class NewlineTriviaToken : WhitespaceTriviaToken
{
    public NewlineTriviaToken(int count,
                              Location location,
                              TriviaToken? leading = null,
                              TriviaToken? trailing = null)
        : base('\n', count, location, leading, trailing)
    {
        Kind = TriviaKind.newline;
    }
}