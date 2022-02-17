
public sealed class ComplexStringToklet : IToklet<ComplexStringToken>
{

    private static ComplexStringToklet _instance = new();
    public static ComplexStringToklet Instance => _instance;

	private ComplexStringToklet() : base() { }


    public Predicate<IConsumer<char>> Condition
        => (input => {
                var current = input.Consume();

                if (current != '$') return false;

                current = input.Consume();

                return current is '\'' or '"';
            }
        );

    public ComplexStringToken Consume(IConsumer<char> input, Tokenizer tokenizer) {

        // consume the '$' in front
        input.Consume();

        var endingDelimiter = input.Consume();

        //consume a character
        var currChar = input.Consume();

        // the output token
        var output = new System.Text.StringBuilder();

        var isValid = true;

        var startPos = input.Position;

        var sections = new List<Token[]>();

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            if (currChar == '\n') {
                Logger.Error(new UnexpectedError<char>(ErrorArea.Tokenizer) {
                    In = "an interpolated string",
                    Value = currChar,
                    Location = input.Position,
                    Expected = "a string delimiter like this : " + endingDelimiter + output.ToString() + endingDelimiter
                });

                isValid = false;

                break;
            }

            if (currChar == '{') {
                var tokenList = new List<Token>();

                while (tokenizer.Peek() != "}") {


                    if (!tokenizer.Consume(out var currToken)) {
                        Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                            In = "an interpolated string",
                            Expected = "} followed by a string delimiter like this :\n\t"
                                     + endingDelimiter
                                     + output.ToString()
                                     + endingDelimiter,
                            Location = new LocationRange(startPos, tokenizer.Position)
                        });

                        isValid = false;

                        break;
                    }

                    tokenList.Add(currToken);
                }

                sections.Add(tokenList.ToArray());

                output.Append("{" + (sections.Count - 1) + "}");

                if (tokenizer.Consume(preserveTrivia: true).TrailingTrivia != null) {
                    output.Append(tokenizer.Current.TrailingTrivia!.Representation);
                }
            } else {
                output.Append(currChar);
            }

            if (!input.Consume(out currChar)) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                    In = "an interpolated string",
                    Expected = "a string delimiter like this : " + endingDelimiter + output.ToString() + endingDelimiter,
                    Location = new LocationRange(startPos, tokenizer.Position)
                });

                isValid = false;

                break;
            }
        }

        return new ComplexStringToken(output.ToString(), sections, new LocationRange(startPos, input.Position), isValid);
    }
}