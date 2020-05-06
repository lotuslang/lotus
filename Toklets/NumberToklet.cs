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

        // if the character is '.'
        if (currChar == '.') {
            var str = numberStr.Append(currChar).ToString();
            if (str.Contains('e') || str.Contains('E')) {
                throw new InvalidInputException(numberStr.Append(currChar).ToString(), "as a number", "because you cannot use a decimal separator after a power-of-ten separator", input.Position);
            }
            throw new InvalidInputException(numberStr.Append(currChar).ToString(), "as a number", "because there already was a decimal separator earier", input.Position);
        }

        // if the character is 'e' or 'E'
        if (currChar == 'e' || currChar == 'E') {
            throw new InvalidInputException(numberStr.Append(currChar).ToString(), "as a number", "because there already was a power-of-ten separator earlier", input.Position);
        }

        input.Reconsume();

        return new NumberToken(numberStr.ToString(), origin);
    }
}