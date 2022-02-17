
public sealed class StringToklet : IToklet<StringToken>
{

    private static StringToklet _instance = new();
    public static StringToklet Instance => _instance;

	private StringToklet() : base() { }


    public Predicate<IConsumer<char>> Condition
        => (input => input.Consume() == '\'' || input.Current == '"');

    public StringToken Consume(IConsumer<char> input, Tokenizer tokenizer) {

        var endingDelimiter = input.Consume();

        // consume a character
        var currChar = input.Consume();

        // the output token
        var outputStr = new System.Text.StringBuilder();

        var isValid = true;

        var startPos = input.Position;

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            // add it to the value of output
            outputStr.Append(currChar);

            if (!input.Consume(out currChar)) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                    In = "a string",
                    Expected = new[] {
                        "a character",
                        "a string delimiter ' or \"",
                    },
                    Location = input.Position
                });

                isValid = false;

                break;
            }
        }

        // return the output token
        return new StringToken(outputStr.ToString(), new LocationRange(startPos, input.Position), isValid);
    }
}