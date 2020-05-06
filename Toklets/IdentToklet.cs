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
        var output = new ComplexToken("", TokenKind.ident, input.Position);

        // if the character is not a letter or a low line
        if (!(Char.IsLetter(currChar) || currChar == '_')) {
            throw new InvalidInputException(
                currChar.ToString(),
                "as the beginning of an identifier",
                "because identifiers can only start with a letter or an underscore",
                tokenizer.Position
            );
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

        if (Utilities.keywords.Contains(output)) {
            return new ComplexToken(output, TokenKind.keyword, output.Location);
        }

        // return the output token
        return output;
    }
}