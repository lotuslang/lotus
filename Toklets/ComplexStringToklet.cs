using System;
using System.Collections.Generic;

public sealed class ComplexStringToklet : IToklet<ComplexStringToken>
{
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

            if (currChar == '{') {
                var tokenList = new List<Token>();
                Token currToken;

                while (tokenizer.Peek() != "}") {

                    if (!tokenizer.Consume(out currToken!)) {
                        Logger.Error(new UnexpectedEOFException(
                            context: "in an interpolated string",
                            expected: $"`}}` followed by the string delimiter `{endingDelimiter}`",
                            range: new LocationRange(startPos, tokenizer.Position)
                        ));

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
                Logger.Error(new UnexpectedEOFException(
                    context: "in a string",
                    expected: $"the string delimitier `{endingDelimiter}`",
                    range: new LocationRange(startPos, tokenizer.Position)
                ));

                isValid = false;

                break;
            }
        }

        return new ComplexStringToken(output.ToString(), sections, new LocationRange(startPos, input.Position), isValid);
    }
}