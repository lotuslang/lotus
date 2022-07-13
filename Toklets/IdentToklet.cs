public sealed class IdentToklet : IToklet<Token>
{
    public static readonly IdentToklet Instance = new();

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
	private static readonly Func<char, Func<IConsumer<char>>, bool> _condition =
        ((currChar, _) =>
            Char.IsLetter(currChar) || currChar == '_'
        );

    public Token Consume(IConsumer<char> input, Tokenizer tokenizer) {

        // consume a character
        var currChar = input.Consume();

        // the output token
        var output = new System.Text.StringBuilder();

        var startPos = input.Position;

        // if the character is not a letter or a low line
        if (currChar != '_' && !Char.IsLetter(currChar)) {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, new LocationRange(startPos, input.Position)));
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