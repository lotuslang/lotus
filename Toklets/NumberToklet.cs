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

            var errorCount = Logger.ErrorCount;

            Consume(input, _);

            while (Logger.ErrorCount > errorCount) {
                Logger.Exceptions.Pop();
            }

            var str = numberStr.ToString() + currChar;
            if (str.Contains('e') || str.Contains('E')) {
                // ...we either stopped parsing a power-of-ten number because of a decimal (which is not valid syntax)
                Logger.Error(new InvalidInputException(
                    input: numberStr.ToString() + currChar,
                    context: "as a number",
                    reason: "because you cannot use a decimal separator after a power-of-ten separator",
                    range: new LocationRange(originPos, input.Position)
                ));
            } else {
                // ...or there is a second decimal separator, which isn't valid either
                Logger.Error(new InvalidInputException(
                    input: numberStr.ToString() + currChar,
                    context: "as a number",
                    reason: "because there already was a decimal separator earier",
                    range: new LocationRange(originPos, input.Position)
                ));
            }

            isValid = false;

            // in both cases, if the next character is a digit, we consume it for error-recovery's sake
            if (Char.IsDigit(input.Consume())) {
                while (Char.IsDigit(input.Consume())); // consume until it's not a digit anymore (yes this is an empty loop, I know)
            }
        }

        // we already had a "power-of-ten separator", so this is not valid.
        if (currChar is 'e' or 'E') {
            Logger.Error(new InvalidInputException(
                input: numberStr.ToString() + currChar,
                context: "as a number",
                reason: "because there already was a power-of-ten separator earlier",
                range: new LocationRange(originPos, input.Position)
            ));

            isValid = false;

            if (Condition(input)) { // FIXME: this is lazy
                while (Char.IsDigit(input.Consume())); // consume until it's not a digit anymore (yes this is an empty loop, I know)
            }
        }

        input.Reconsume();

        return new NumberToken(numberStr.ToString(), new LocationRange(originPos, input.Position), isValid);
    }
}