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

                while (tokenizer.Peek(preserveTrivia: true) != "}") {

                    currToken = tokenizer.Consume();

                    if (currToken == ";") { // FIXME: write a message to tell people that they can't use semicolons in interpolated strings
                        Logger.Error(new UnexpectedTokenException(
                            token: currToken,
                            context: "in an interpolated string",
                            expected: "`}`"
                        ));

                        output.IsValid = false;

                        break;
                    }

                    if (currToken.Kind == TokenKind.EOF) {
                        Logger.Error(new UnexpectedEOFException(
                            context: "in an interpolated string",
                            expected: $"`}}` followed by the string delimiter `{endingDelimiter}`",
                            tokenizer.Position
                        ));

                        output.IsValid = false;

                        break;
                    }

                    tokenList.Add(currToken);
                }

                output.AddSection(tokenList.ToArray());

                output.Add("{" + (output.CodeSections.Count - 1) + "}");

                tokenizer.Consume(preserveTrivia: true); // consume the '}'

                currChar = input.Consume();

                //Console.WriteLine(tokenizer.Current + " " + input.Current);
                continue;
            }

            output.Add(currChar);

            if (!input.Consume(out currChar)) {
                Logger.Error(new UnexpectedEOFException(
                    context: "in a string",
                    expected: $"the string delimitier `{endingDelimiter}`",
                    tokenizer.Position
                ));

                output.IsValid = false;

                break;
            }
        }

        return output;
    }
}