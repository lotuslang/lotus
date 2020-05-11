using System;

public sealed class OperatorToklet : IToklet<OperatorToken>
{
    public Predicate<IConsumer<char>> Condition
        => (input => {
                var c = input.Consume();

                if (c == '+'
                ||  c == '-'
                ||  c == '*'
                ||  c == '/'
                ||  c == '%'
                ||  c == '^'
                ||  c == '.'
                ||  c == '='
                ||  c == '>'
                ||  c == '<'
                ||  c == '!') return true;

                if (c == '&') {
                    return input.Consume() == '&';
                } else if (c == '|') {
                    return input.Consume() == '|';
                }

                return false;
            }
        );

    public OperatorToken Consume(IConsumer<char> input, Tokenizer tokenizer) {

        var currChar = input.Consume();

        // easy and clear switches
        switch (currChar) {
            case '*':
                // Multiplication operator a * b
                return new OperatorToken(currChar, Precedence.Multiplication, "left", input.Position);
            case '/':
                // Division operator a / b
                return new OperatorToken(currChar, Precedence.Division, "left", input.Position);
            case '%':
                // Modulo operator a % b
                return new OperatorToken(currChar, Precedence.Modulo, "left", input.Position);
            case '.':
                // Member access "operator" a.b
                return new OperatorToken(currChar, Precedence.Access, "left", input.Position);
            case '&' when input.Peek() == '&':
                // Logical AND operator a && b
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.And, "left", input.Position);
            case '|' when input.Peek() == '|':
                // Logical OR operator a || b
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.Or, "left", input.Position);
        }

        // this part is for cases that aren't simple and/or wouldn't look good in a switch expression

        if (currChar == '+' || currChar == '-') {

            if (input.Peek() == currChar) {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.Unary, "right", input.Position);
            }

            return new OperatorToken(currChar, Precedence.Addition, "left", input.Position);
        }

        if (currChar == '^') {
            if (input.Peek() == '^') {
                input.Consume(); // consume the '^' we just peeked at

                return new OperatorToken("^^", Precedence.Xor, "left", input.Position);
            }

            // Power/Exponent operator a ^ b
                return new OperatorToken(currChar, Precedence.Power, "right", input.Position);
        }


        if (currChar == '=') {

            // Equality comparison operator a == b
            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.Equal, "left", input.Position);
            }

            // Assignment operator a = b
            return new OperatorToken(currChar, Precedence.Assignment, "left", input.Position);
        }

        if (currChar == '>') {

            // Greater-than-or-equal comparison operator a >= b
            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.GreaterThanOrEqual, "left", input.Position);
            }

            // Greater-than comparison operator a > b
            return new OperatorToken(currChar, Precedence.GreaterThan, "left", input.Position);
        }

        if (currChar == '<') {

            // Less-than-or-equal comparison operator a <= b
            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.LessThanOrEqual, "left", input.Position);
            }

            // Less-than comparison operator a < b
            return new OperatorToken(currChar, Precedence.LessThan, "left", input.Position);
        }

        if (currChar == '!') {

            // Not-equal comparison operator a != b
            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.NotEqual, "left", input.Position);
            }

            // Unary logical NOT operator !a
            return new OperatorToken(currChar, Precedence.Unary, "right", input.Position);
        }

        throw new InvalidInputException(currChar.ToString(), "as an operator", tokenizer.Position);
    }
}