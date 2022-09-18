namespace Lotus.Syntax;

public sealed class IdentToklet : IToklet<Token>
{
    public static readonly IdentToklet Instance = new();

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
	private static readonly Func<char, Func<IConsumer<char>>, bool> _condition =
        ((currChar, getInput) =>
               currChar == '_'
            || Char.IsLetter(currChar)
            || (currChar == '@' && Char.IsLetter(getInput().Consume()))
        );

        public Token Consume(IConsumer<char> input, Tokenizer tokenizer) {
        // consume a character
        var currChar = input.Consume();

        Debug.Assert(currChar is '_' or '@' || Char.IsLetter(currChar));

        // the output token
        var output = new System.Text.StringBuilder().Append(currChar);

        var startPos = input.Position;

        // while the current character is a letter, a digit, or an underscore
        while (input.Consume(out currChar) && (Char.IsLetterOrDigit(currChar) || currChar is '_')) {
            // add it to the value of output
            output.Append(currChar);
        }

        var outputStr = output.ToString();

        // reconsume the last token (which is not a letter, a digit, or an underscore,
        // since our while loop has exited) to make sure it is processed by the tokenizer afterwards
        input.Reconsume();

        if (outputStr is "true" or "false") {
            return new BoolToken(outputStr, outputStr == "true", new LocationRange(startPos, input.Position));
        }

        if (Resources.keywords.Contains(outputStr)) {
            return new Token(outputStr, TokenKind.keyword, new LocationRange(startPos, input.Position));
        }

        // return the output token
        return new IdentToken(outputStr, new LocationRange(startPos, input.Position));
    }
}