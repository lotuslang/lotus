using System;
using System.Text;

public sealed class NumberToklet : IToklet<NumberToken>
{
    public Predicate<IConsumer<char>> Condition
        => (input => {
                var current = input.Consume();

                if (Char.IsDigit(current)) return true;

                if (current == '+' || current == '-' || current == '.') {
                    return Char.IsDigit(input.Consume());
                }

                return false;
            }
        );

    public NumberToken Consume(IConsumer<char> input, Tokenizer _) {
        // the output token
        var numberStr = new StringBuilder();

        var currChar = input.Consume();

        var isValid = true;

        var origin = input.Position; // the position of the number's first character

        if (currChar == '+' || currChar == '-') {
            numberStr.Append(currChar);
            currChar = input.Consume();
        }

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
        if (currChar == 'e' || currChar == 'E') {

            // add the e/E to the output
            numberStr.Append(currChar);

            // consume a character
            currChar = input.Consume();


            // if the character is a '+' or a '-'
            if (currChar == '+' || currChar == '-') {

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
            var str = numberStr.ToString() + currChar;
            if (str.Contains('e') || str.Contains('E')) {
                // ...we either stopped parsing a power-of-ten number because of a decimal (which is not valid syntax)
                Logger.Error(new InvalidInputException(numberStr.ToString() + currChar, "as a number", "because you cannot use a decimal separator after a power-of-ten separator", input.Position));
            } else {
                // ...or there is a second decimal separator, which isn't valid either
                Logger.Error(new InvalidInputException(numberStr.ToString() + currChar, "as a number", "because there already was a decimal separator earier", input.Position));
            }

            isValid = false;

            // in both cases, if the next character is a digit, we consume it for error-recovery's sake
            if (Char.IsDigit(input.Consume())) {
                while (Char.IsDigit(input.Consume())); // consume until it's not a digit anymore (yes this is an empty loop, I know)
            }
        }

        // we already had a "power-of-ten separator", so this is not valid.
        if (currChar == 'e' || currChar == 'E') {
            Logger.Error(new InvalidInputException(numberStr.ToString() + currChar, "as a number", "because there already was a power-of-ten separator earlier", input.Position));

            isValid = false;

            if (Condition(input)) { // FIXME: this is lazy
                while (Char.IsDigit(input.Consume())); // consume until it's not a digit anymore (yes this is an empty loop, I know)
            }
        }

        input.Reconsume();

        return new NumberToken(numberStr.ToString(), origin, isValid);
    }
}