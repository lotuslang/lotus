public sealed class ComplexStringToklet : IToklet<ComplexStringToken>
{
    public static readonly ComplexStringToklet Instance = new();

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
	private static readonly Func<char, Func<IConsumer<char>>, bool> _condition
        = (currChar, getInput) => currChar == '$' && getInput().Consume() is '\'' or '"';

    public ComplexStringToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        // consume the '$' in front
        _ = input.Consume();

        var startPos = input.Position;

        var endingDelimiter = input.Consume();

        //consume a character
        var currChar = input.Consume();

        // the output token
        var output = new System.Text.StringBuilder();

        var isValid = true;

        var sections = ImmutableArray.CreateBuilder<ImmutableArray<Token>>();

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
                var tokenList = ImmutableArray.CreateBuilder<Token>();

                var unmatchedBrackets = 1; // count the opening bracket as currently unmatched

                while (unmatchedBrackets != 0) { // until we match the opening bracket
                    if (!tokenizer.Consume(out var currToken)) {
                        Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                            In = "an interpolated string",
                            Expected = "} followed by a string delimiter like this: "
                                     + endingDelimiter
                                     + output.ToString()
                                     + endingDelimiter,
                            Location = new LocationRange(startPos, tokenizer.Position)
                        });

                        isValid = false;

                        break;
                    }

                    switch (currToken.Representation) {
                        case "{":
                            unmatchedBrackets++;
                            break;
                        case "}":
                            unmatchedBrackets--;
                            Debug.Assert(unmatchedBrackets >= 0);
                            break;
                    }

                    if (unmatchedBrackets != 0)
                        tokenList.Add(currToken);
                }

                tokenizer.Reconsume();

                sections.Add(tokenList.ToImmutable());

                output.Append('{').Append(sections.Count - 1).Append('}');

                if (tokenizer.Consume(preserveTrivia: true).TrailingTrivia != null) {
                    output.Append(tokenizer.Current.TrailingTrivia!.Representation);
                }
            } else {
                output.Append(currChar);
            }

            if (!input.Consume(out currChar)) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                    In = "an interpolated string",
                    Expected = "a string delimiter like this: " + endingDelimiter + output.ToString() + endingDelimiter,
                    Location = new LocationRange(startPos, tokenizer.Position)
                });

                isValid = false;

                break;
            }
        }

        return new ComplexStringToken(output.ToString(), sections.ToImmutable(), new LocationRange(startPos, input.Position)) { IsValid = isValid };
    }
}