public sealed class StringToklet : IToklet<StringToken>
{
    public static readonly StringToklet Instance = new();

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
	private static readonly Func<char, Func<IConsumer<char>>, bool> _condition =
        ((currChar, _) =>
            currChar is '"' or '\"'
        );

    public StringToken Consume(IConsumer<char> input, Tokenizer tokenizer) {

        var endingDelimiter = input.Consume();

        var startPos = input.Position;

        // consume a character
        var currChar = input.Consume();

        // the output token
        var output = new System.Text.StringBuilder();

        var isValid = true;

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            if (currChar == '\n') {
                Logger.Error(new UnexpectedError<char>(ErrorArea.Tokenizer) {
                    In = "a string",
                    Value = currChar,
                    Location = input.Position,
                    Expected = "a string delimiter like this : " + endingDelimiter + output.ToString() + endingDelimiter
                });

                isValid = false;

                break;
            }

            // add it to the value of output
            output.Append(currChar);

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
        return new StringToken(output.ToString(), new LocationRange(startPos, input.Position)) { IsValid = isValid };
    }
}