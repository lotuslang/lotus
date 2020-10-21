using System;
using System.Collections.Generic;

public sealed class ComplexStringToklet : IToklet<ComplexStringToken>
{
    public Predicate<IConsumer<char>> Condition
        => (input => {
                var current = input.Consume();

                if (current != '$') return false;

                current = input.Consume();

                if (current != '\'' && current != '"') return false;

                return true;
            }
        );

    public ComplexStringToken Consume(IConsumer<char> input, Tokenizer tokenizer) {

        // consume the '$' in front
        input.Consume();

        var endingDelimiter = input.Consume();

        //consume a character
        var currChar = input.Consume();

        // the output token
        var output = new ComplexStringToken("", new List<Token[]>(), input.Position);

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            if (currChar == '{') {
                var tokenList = new List<Token>();
                Token currToken;

                while (tokenizer.Peek() != "}") {

                    currToken = tokenizer.Consume();

                    if (currToken.Kind == TokenKind.EOF) {
                        Logger.Error(new UnexpectedEOFException(
                            context: "in an interpolated string",
                            expected: $"`}}` followed by the string delimiter `{endingDelimiter}`",
                            range: new LocationRange(output.Location, tokenizer.Position)
                        ));

                        output.IsValid = false;

                        break;
                    }

                    tokenList.Add(currToken);
                }

                output.AddSection(tokenList.ToArray());

                output.Add("{" + (output.CodeSections.Count - 1) + "}");

                if (tokenizer.Consume(preserveTrivia: true).TrailingTrivia != null) {
                    output.Add(tokenizer.Current.TrailingTrivia!.Representation);
                }
            } else {
                output.Add(currChar);
            }

            if (!input.Consume(out currChar)) {
                Logger.Error(new UnexpectedEOFException(
                    context: "in a string",
                    expected: $"the string delimitier `{endingDelimiter}`",
                    range: new LocationRange(output.Location, tokenizer.Position)
                ));

                output.IsValid = false;

                break;
            }
        }

        output.Location = new LocationRange(output.Location, tokenizer.Position);

        return output;
    }
}