namespace Lotus.Syntax;

public sealed class OperatorToklet : IToklet<OperatorToken>
{
    public static readonly OperatorToklet Instance = new();

    public ref readonly Func<char, Func<IConsumer<char>>, bool> Condition => ref _condition;
	private static readonly Func<char, Func<IConsumer<char>>, bool> _condition =
        ((currChar, getInput) => {
                switch (currChar) {
                    case   '+' or '-'
                        or '*' or '/'
                        or '%' or '^'
                        or '.' or '='
                        or '>' or '<'
                        or '!' or '?':
                        return true;
                    case '&':
                        return getInput().Consume() == '&';
                    case '|':
                        return getInput().Consume() == '|';
                    default:
                        return false;
                }
            }
        );

    public OperatorToken Consume(IConsumer<char> input, Tokenizer tokenizer) {
        var currChar = input.Consume();
        var currCharStr = currChar.ToString();

        var currPos = input.Position;

        // easy and clear switches
        switch (currCharStr) {
            case "*":
                // Multiplication operator a * b
                return new OperatorToken(currCharStr, Precedence.Multiplication, true, input.Position);
            case "/":
                // Division operator a / b
                return new OperatorToken(currCharStr, Precedence.Division, true, input.Position);
            case "%":
                // Modulo operator a % b
                return new OperatorToken(currCharStr, Precedence.Modulo, true, input.Position);
            case ".":
                // Member access "operator" a.b
                return new OperatorToken(currCharStr, Precedence.Access, true, input.Position);
            case "&" when input.Peek() == '&':
                // Logical AND operator a && b
                return new OperatorToken(currCharStr +""+ input.Consume(), Precedence.And, true, new LocationRange(currPos, input.Position));
            case "|" when input.Peek() == '|':
                // Logical OR operator a || b
                return new OperatorToken(currCharStr +""+ input.Consume(), Precedence.Or, true, new LocationRange(currPos, input.Position));
            case "?":
                // Ternary comparison operator a ? b : c
                return new OperatorToken(currCharStr, Precedence.Ternary, true, input.Position);
        }

        // this part is for cases that aren't simple and/or wouldn't look good in a switch expression

        if (currChar is '+' or '-') {
            if (input.Peek() == currChar) {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.Unary, false, new LocationRange(currPos, input.Position));
            }

            return new OperatorToken(currCharStr, Precedence.Addition, true, input.Position);
        }

        if (currChar == '^') {
            if (input.Peek() == '^') {
                _ = input.Consume(); // consume the '^' we just peeked at

                return new OperatorToken("^^", Precedence.Xor, true, new LocationRange(currPos, input.Position));
            }

            // Power/Exponent operator a ^ b
            return new OperatorToken(currCharStr, Precedence.Power, false, input.Position);
        }

        if (currChar == '=') {
            // Equality comparison operator a == b
            if (input.Peek() == '=') {
                return new OperatorToken(currCharStr + input.Consume(), Precedence.Equal, true, new LocationRange(currPos, input.Position));
            }

            // Assignment operator a = b
            return new OperatorToken(currCharStr, Precedence.Assignment, true, input.Position);
        }

        if (currChar == '>') {
            // Greater-than-or-equal comparison operator a >= b
            if (input.Peek() == '=') {
                return new OperatorToken(currCharStr + input.Consume(), Precedence.GreaterThanOrEqual, true, new LocationRange(currPos, input.Position));
            }

            // Greater-than comparison operator a > b
            return new OperatorToken(currCharStr, Precedence.GreaterThan, true, input.Position);
        }

        if (currChar == '<') {
            // Less-than-or-equal comparison operator a <= b
            if (input.Peek() == '=') {
                return new OperatorToken(currCharStr + input.Consume(), Precedence.LessThanOrEqual, true, new LocationRange(currPos, input.Position));
            }

            // Less-than comparison operator a < b
            return new OperatorToken(currCharStr, Precedence.LessThan, true, input.Position);
        }

        if (currChar == '!') {
            // Not-equal comparison operator a != b
            if (input.Peek() == '=') {
                return new OperatorToken(currCharStr + input.Consume(), Precedence.NotEqual, true, new LocationRange(currPos, input.Position));
            }

            // Unary logical NOT operator !a
            return new OperatorToken(currCharStr, Precedence.Unary, false, input.Position);
        }

        Debug.Fail("OperatorToklet can't be called on '" + currCharStr + "'");
        throw null;
    }
}