public class NewlineTriviaToken : WhitespaceTriviaToken
{
    public NewlineTriviaToken(int count,
                              Location location,
                              bool isValid = true,
                              TriviaToken? leading = null,
                              TriviaToken? trailing = null)
        : base('\n', count, location, isValid, leading, trailing)
    {
        Kind = TriviaKind.newline;
    }
}