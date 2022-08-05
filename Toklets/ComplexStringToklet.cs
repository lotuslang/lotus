public sealed class ComplexStringToklet : IToklet<ComplexStringToken>
{
    public static readonly ComplexStringToklet Instance = new();

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
	private static readonly Func<char, Func<IConsumer<char>>, bool> _condition =
        ((currChar, getInput) => {
                return currChar == '$' && getInput().Consume() is '\'' or '"';
            }
        );

    public ComplexStringToken Consume(IConsumer<char> input, Tokenizer tokenizer) {

        // consume the '$' in front
        input.Consume();

        var startPos = input.Position;

        var endingDelimiter = input.Consume();

        //consume a character
        var currChar = input.Consume();

        // the output token
        var output = new System.Text.StringBuilder();

        var isValid = true;

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

        return new ComplexStringToken(output.ToString(), sections, new LocationRange(startPos, input.Position)) { IsValid = isValid };
    }
}