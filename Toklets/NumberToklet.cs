using System.Text;

public sealed class NumberToklet : IToklet<NumberToken>
{

    private static NumberToklet _instance = new();
    public static NumberToklet Instance => _instance;

	private NumberToklet() : base() { }


    public Predicate<IConsumer<char>> Condition
        => (input => {
                var current = input.Consume();

                return  Char.IsDigit(current)
                    || (current == '.' && Char.IsDigit(input.Consume()));
            }
        );

    public NumberToken Consume(IConsumer<char> input, Tokenizer _) {
        // the output token
        var numberStr = new StringBuilder();

        var currChar = input.Consume();

        var isValid = true;

        var originPos = input.Position; // the position of the number's first character

        // while the current character is a digit
        while (Char.IsDigit(currChar)) {

            // add it to the value of output
            numberStr.Append(currChar);

            // consume a character
            currChar = input.Consume();
        }

        // if the character is '.'
        if (currChar == '.')
        {
            // add it to the value of output
            numberStr.Append(currChar);

            // consume a character
            currChar = input.Consume();
        }

        // while the current character is a digit
        while (Char.IsDigit(currChar)) {

            // add it to the value of output
            numberStr.Append(currChar);

            // consume a character
            currChar = input.Consume();
        }

        // if the character is an 'e' or an 'E'
        if (currChar is 'e' or 'E') {

            // add the e/E to the output
            numberStr.Append(currChar);

            // consume a character
            currChar = input.Consume();


            // if the character is a '+' or a '-'
            if (currChar is '+' or '-') {

                // add it to the value of output
                numberStr.Append(currChar);

                // consume a character
                currChar = input.Consume();
            }

            // while the current character is a digit
            while (Char.IsDigit(currChar)) {

                // add it to the value of output
                numberStr.Append(currChar);

                // consume a character
                currChar = input.Consume();
            }
        }

        // if we have a decimal separator...
        if (currChar == '.') {
            if (Condition(input.Clone())) {
                var errorCount = Logger.ErrorCount;

                numberStr.Append(currChar).Append(Consume(input, _).Representation);

                // The above .Consume(...) could generate extra errors, except we don't really
                // want to show them to the user since they don't really matter ; we just wanna
                // be sure we consumed what we had to
                while (Logger.ErrorCount > errorCount) {
                    Logger.errorStack.Pop();
                }

                var str = numberStr.ToString();

                if (str.Contains('e') || str.Contains('E')) {
                    // ...we either stopped parsing a power-of-ten number because of a decimal (which is not valid syntax)
                    Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                        Value = str,
                        As = "a number. You cannot use a decimal separator after a power-of-ten separator",
                        Location = new LocationRange(originPos, input.Position),
                        //ExtraNotes = notes
                    });
                } else {
                    // ...or there is a second decimal separator, which isn't valid either
                    Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                        Value = str,
                        As = "a number. There already was a decimal separator earlier",
                        Location = new LocationRange(originPos, input.Position),
                        //ExtraNotes = notes
                    });
                }

                isValid = false;

                input.Consume(); // hack to prevent the reconsume from fucking up the input
            }
        }

        // we already had a "power-of-ten separator", so this is not valid.
        if (currChar is 'e' or 'E') {
            if (Condition(input.Clone())) {
                var errorCount = Logger.ErrorCount;

                numberStr.Append(currChar).Append(Consume(input, _).Representation);

                while (Logger.ErrorCount > errorCount) {
                    Logger.errorStack.Pop();
                }
            }

            Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                Value = numberStr.ToString(),
                As = "a number. There already was a power-of-ten separator earlier",
                Location = new LocationRange(originPos, input.Position),
            });

            isValid = false;

            input.Consume(); // hack to prevent the reconsume from fucking up the input
        }

        input.Reconsume();

        return new NumberToken(numberStr.ToString(), new LocationRange(originPos, input.Position), isValid);
    }
}