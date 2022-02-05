
public sealed class IdentToklet : IToklet<Token>
{
    public Predicate<IConsumer<char>> Condition
        => (input => Char.IsLetter(input.Consume()) || input.Current == '_');

    public Token Consume(IConsumer<char> input, Tokenizer tokenizer) {

        // consume a character
        var currChar = input.Consume();

        // the output token
        var output = new System.Text.StringBuilder();

        var startPos = input.Position;

        // if the character is not a letter or a low line
        if (!(Char.IsLetter(currChar) || currChar == '_')) {
            throw Logger.Fatal(new InvalidCallException(new LocationRange(startPos, input.Position)));
        }

        // while the current character is a letter, a digit, or a low line
        while (Char.IsLetterOrDigit(currChar) || currChar == '_') {

            // add it to the value of output
            output.Append(currChar);

            // consume a character
            currChar = input.Consume();
        }

        var outputStr = output.ToString();

        // reconsume the last token (which is not a letter, a digit, or a low line,
        // since our while loop has exited) to make sure it is processed by the tokenizer afterwards
        input.Reconsume();

        if (outputStr is "true" or "false") {
            return new BoolToken(outputStr, new LocationRange(startPos, input.Position));
        }

        if (Utilities.keywords.Contains(outputStr)) {
            return new Token(outputStr, TokenKind.keyword, new LocationRange(startPos, input.Position));
        }

        // return the output token
        return new IdentToken(outputStr, new LocationRange(startPos, input.Position));
    }
}