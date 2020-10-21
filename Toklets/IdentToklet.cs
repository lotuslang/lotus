using System;
using System.Linq;

public sealed class IdentToklet : IToklet<ComplexToken>
{
    public Predicate<IConsumer<char>> Condition
        => (input => Char.IsLetter(input.Consume()) || input.Current == '_');

    public ComplexToken Consume(IConsumer<char> input, Tokenizer tokenizer) {

        // consume a character
        var currChar = input.Consume();

        // the output token
        var output = new IdentToken("", input.Position);

        // if the character is not a letter or a low line
        if (!(Char.IsLetter(currChar) || currChar == '_')) {
            throw Logger.Fatal(new InvalidCallException(new LocationRange(output.Location, input.Position)));
        }

        // while the current character is a letter, a digit, or a low line
        while (Char.IsLetterOrDigit(currChar) || currChar == '_') {

            // add it to the value of output
            output.Add(currChar);

            // consume a character
            currChar = input.Consume();
        }

        // reconsume the last token (which is not a letter, a digit, or a low line,
        // since our while loop has exited) to make sure it is processed by the tokenizer afterwards
        input.Reconsume();

        if (output == "true" || output == "false") {
            return new BoolToken(output, new LocationRange(output.Location, input.Position));
        }

        if (Utilities.keywords.Contains(output)) {
            return new ComplexToken(output, TokenKind.keyword, new LocationRange(output.Location, input.Position));
        }

        output.Location = new LocationRange(output.Location, input.Position);

        // return the output token
        return output;
    }
}