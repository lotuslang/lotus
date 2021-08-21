using System;

public sealed class OperatorToklet : IToklet<OperatorToken>
{
    public Predicate<IConsumer<char>> Condition
        => (input => {
                switch (input.Consume()) {
                    case   '+' or '-'
                        or '*' or '/'
                        or '%' or '^'
                        or '.' or '='
                        or '>' or '<'
                        or '!' or '?':
                        return true;
                    case '&':
                    return input.Consume() == '&';
                    case '|':
                    return input.Consume() == '|';
                    default:
                return false;
            }
            }
        );

    public OperatorToken Consume(IConsumer<char> input, Tokenizer tokenizer) {

        var currChar = input.Consume();

        var currPos = input.Position;

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
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.And, "left", new LocationRange(currPos, input.Position));
            case '|' when input.Peek() == '|':
                // Logical OR operator a || b
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.Or, "left", new LocationRange(currPos, input.Position));
            case '?':
                // Ternary comparison operator a ? b : c
                return new OperatorToken(currChar, Precedence.Ternary, "left", input.Position);
        }

        // this part is for cases that aren't simple and/or wouldn't look good in a switch expression

        if (currChar is '+' or '-') {

            if (input.Peek() == currChar) {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.Unary, "right", new LocationRange(currPos, input.Position));
            }

            return new OperatorToken(currChar, Precedence.Addition, "left", input.Position);
        }

        if (currChar == '^') {
            if (input.Peek() == '^') {
                input.Consume(); // consume the '^' we just peeked at

                return new OperatorToken("^^", Precedence.Xor, "left", new LocationRange(currPos, input.Position));
            }

            // Power/Exponent operator a ^ b
                return new OperatorToken(currChar, Precedence.Power, "right", input.Position);
        }


        if (currChar == '=') {

            // Equality comparison operator a == b
            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.Equal, "left", new LocationRange(currPos, input.Position));
            }

            // Assignment operator a = b
            return new OperatorToken(currChar, Precedence.Assignment, "left", input.Position);
        }

        if (currChar == '>') {

            // Greater-than-or-equal comparison operator a >= b
            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.GreaterThanOrEqual, "left", new LocationRange(currPos, input.Position));
            }

            // Greater-than comparison operator a > b
            return new OperatorToken(currChar, Precedence.GreaterThan, "left", input.Position);
        }

        if (currChar == '<') {

            // Less-than-or-equal comparison operator a <= b
            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.LessThanOrEqual, "left", new LocationRange(currPos, input.Position));
            }

            // Less-than comparison operator a < b
            return new OperatorToken(currChar, Precedence.LessThan, "left", input.Position);
        }

        if (currChar == '!') {

            // Not-equal comparison operator a != b
            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.NotEqual, "left", new LocationRange(currPos, input.Position));
            }

            // Unary logical NOT operator !a
            return new OperatorToken(currChar, Precedence.Unary, "right", input.Position);
        }

        throw Logger.Fatal(new InvalidCallException(new LocationRange(currPos, input.Position)));
    }
}