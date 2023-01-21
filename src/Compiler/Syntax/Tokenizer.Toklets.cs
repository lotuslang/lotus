using System.Text;

namespace Lotus.Syntax;

public partial class Tokenizer : IConsumer<Token>
{
    private Token ConsumeSemicolonToken()
        => new(";", TokenKind.semicolon, _input.Position);

    private Token ConsumeDoubleColonToken() {
        _ = _input.Consume();
        return new Token("::", TokenKind.delimiter, _input.Position);
    }

    private Token ConsumeDelimToken(in char c)
        => new(c.ToString(), TokenKind.delimiter, _input.Position);

    private Token ConsumeEOFToken(in char c)
        => new(c.ToString(), TokenKind.EOF, _input.Position) { IsValid = false };

#pragma warning disable IDE0003 // Name can be simplified => avoid confusion between text consumer and tokenizer
    private StringToken ConsumeStringToken(bool isComplex = false) {
        if (isComplex)
            _ = _input.Consume(); // consume the '$' prefix

        var endingDelimiter = _input.Current;

        var startPos = _input.Position;

        // the output token
        var output = new StringBuilder();

        var sections
            = isComplex
            ? ImmutableArray.CreateBuilder<ImmutableArray<Token>>()
            : null!;

        var isValid = true;

        // while the current character is not the ending delimiter
        char currChar = '\0';
        while (currChar != endingDelimiter) {
            if (!_input.Consume(out currChar)) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                    In = "a string",
                    Expected = new[] {
                        "a character",
                        "a string delimiter ' or \"",
                    },
                    Location = _input.Position
                });

                isValid = false;

                break;
            }

            if (currChar == '\n') {
                Logger.Error(new UnexpectedError<char>(ErrorArea.Tokenizer) {
                    In = "a string",
                    Value = currChar,
                    Location = _input.Position,
                    Expected = "a string delimiter like this: " + endingDelimiter + output.ToString() + endingDelimiter
                });

                isValid = false;

                break;
            }

            if (isComplex && currChar == '{') {
                var tokenList = ImmutableArray.CreateBuilder<Token>();

                var unmatchedBrackets = 1; // count the opening bracket as currently unmatched

                while (unmatchedBrackets != 0) { // until we match the opening bracket
                    if (!this.Consume(out var currToken)) {
                        Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                            In = "an interpolated string",
                            Expected = "} followed by a string delimiter like this: "
                                     + endingDelimiter
                                     + output.ToString()
                                     + endingDelimiter,
                            Location = new LocationRange(startPos, this.Position)
                        });

                        isValid = false;

                        break;
                    }

                    switch (currToken.Representation) {
                        case "{":
                            unmatchedBrackets++;
                            break;
                        case "}":
                            unmatchedBrackets--;
                            Debug.Assert(unmatchedBrackets >= 0);
                            break;
                    }

                    if (unmatchedBrackets != 0)
                        tokenList.Add(currToken);
                }

                if (tokenList.Count > 0) {
                    sections.Add(tokenList.ToImmutable());

                    output.Append('{').Append(sections.Count - 1).Append('}');

                    // append whatever was after the closing '}'
                    output.Append(this.Current.TrailingTrivia?.Representation);
                } else {
                    Logger.Error(new UnexpectedError<char>(ErrorArea.Tokenizer) {
                        In = "an interpolated string",
                        Value = '}',
                        Expected = "a value",
                        Message = "An interpolated section can't be empty.",
                        Location = this.Position
                    });

                    isValid = false;
                }

                continue;
            }

            if (currChar == '\\') {
                if (!_input.Consume(out currChar))
                    break; // by breaking, we go back to the start of the loop, which also checks for EOFs

                char throwInvalidEscapeAndGetChar(char escape) {
                    Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                        Value = "\\" + escape,
                        In = "a string",
                        Message = $@"Unrecognized escape sequence '\{escape}'",
                        Location = _input.Position
                    });

                    isValid = false;

                    return '\0';
                }

                output.Append(
                    currChar switch {
                        '\\' or '\'' or '"' => currChar,
                        '0' => '\0',
                        'a' => '\a',
                        'b' => '\b',
                        'f' => '\f',
                        'n' => '\n',
                        'r' => '\r',
                        't' => '\t',
                        'v' => '\v',
                        'u' => ParseUnicodeEscapeSequence(), // todo(lexer): implement \U and \x
                        _   => throwInvalidEscapeAndGetChar(currChar)
                    }
                );

                break;
            }

            // add it to the value of output
            output.Append(currChar);
        }

        if (output.Length > 0) // have to check because the string might be empty and stopped because of EOF
            output.Length--; // remove the ending delimiter

        if (isComplex) {
            return
                new ComplexStringToken(
                    output.ToString(),
                    sections.ToImmutable(),
                    new LocationRange(startPos, _input.Position)
                ) { IsValid = isValid };
        } else {
            return
                new StringToken(
                    output.ToString(),
                    new LocationRange(startPos, _input.Position)
                ) { IsValid = isValid };
        }
    }
#pragma warning restore IDE0003

    private char ParseUnicodeEscapeSequence() {
        Debug.Assert(_input.Current is 'u');

        Span<char> chars = stackalloc char[4];

        for (int i = 0; i < 4; i++) {
            if (!_input.Consume(out chars[i])) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                    In = "a string",
                    Expected = "a unicode escape sequence with 4 digits",
                    Location = _input.Position,
                });
            }

            if (!Char.IsAsciiHexDigit(chars[i])) {
                Logger.Error(new UnexpectedError<char>(ErrorArea.Tokenizer) {
                    Value = chars[i],
                    As = "a hex digit",
                    In = "a string",
                    Expected = "an hexadecimal digit, from 0-9, or A-F",
                    Location = _input.Position
                });
            }
        }

        char finalChar = '\0';

        for (int i = 0; i < 4; i++) {
            finalChar <<= 1;
            finalChar += (char)MiscUtils.CharToHexLookup[chars[i]];
        }

        return finalChar;
    }

    private Token ConsumeIdentToken() {
        // consume a character
        var currChar = _input.Current;

        Debug.Assert(currChar is '_' or '@' || Char.IsLetter(currChar));

        // the output token
        var output = new StringBuilder().Append(currChar);

        var startPos = _input.Position;

        // while the current character is a letter, a digit, or an underscore
        while (_input.Consume(out currChar) && (Char.IsLetterOrDigit(currChar) || currChar is '_')) {
            // add it to the value of output
            output.Append(currChar);
        }

        var outputStr = output.ToString();

        // reconsume the last token (which is not a letter, a digit, or an underscore,
        // since our while loop has exited) to make sure it is processed by the tokenizer afterwards
        _input.Reconsume();

        if (outputStr is "true" or "false") {
            return new BoolToken(outputStr, outputStr == "true", new LocationRange(startPos, _input.Position));
        }

        if (LotusFacts.IsKeyword(outputStr)) {
            return new Token(outputStr, TokenKind.keyword, new LocationRange(startPos, _input.Position));
        }

        // return the output token
        return new IdentToken(outputStr, new LocationRange(startPos, _input.Position));
    }

    private NumberToken ConsumeNumberToken() {
        // the output token
        var numberSB = new StringBuilder();

        var currChar = _input.Current;

        var isValid = true;

        var originPos = _input.Position; // the position of the number's first character

        // while the current character is a digit
        while (Char.IsDigit(currChar)) {
            // add it to the value of output
            numberSB.Append(currChar);

            // consume a character
            currChar = _input.Consume();
        }

        // if the character is '.'
        if (currChar == '.') {
            // add it to the value of output
            numberSB.Append(currChar);

            // consume a character
            currChar = _input.Consume();
        }

        // while the current character is a digit
        while (Char.IsDigit(currChar)) {
            // add it to the value of output
            numberSB.Append(currChar);

            // consume a character
            currChar = _input.Consume();
        }

        // if the character is an 'e' or an 'E'
        if (currChar is 'e' or 'E') {
            // add the e/E to the output
            numberSB.Append(currChar);

            // consume a character
            currChar = _input.Consume();

            // if the character is a '+' or a '-'
            if (currChar is '+' or '-') {
                // add it to the value of output
                numberSB.Append(currChar);

                // consume a character
                currChar = _input.Consume();
            }

            // while the current character is a digit
            while (Char.IsDigit(currChar)) {
                // add it to the value of output
                numberSB.Append(currChar);

                // consume a character
                currChar = _input.Consume();
            }
        }

        bool nextIsNumber() => LotusFacts.IsStartOfNumber(_input.Consume(), _input.Peek());

        // if we have a decimal separator...
        if (currChar == '.') {
            if (nextIsNumber()) {
                var errorCount = Logger.ErrorCount;

                numberSB.Append(currChar).Append(ConsumeNumberToken().Representation);

                // The above .Consume(...) could generate extra errors, except we don't really
                // want to show them to the user since they don't really matter; we just wanna
                // be sure we consumed what we had to
                while (Logger.ErrorCount > errorCount) {
                    _ = Logger.errorStack.Pop();
                }

                var str = numberSB.ToString();

                if (str.Contains('e') || str.Contains('E')) {
                    // ...we either stopped parsing a power-of-ten number because of a decimal (which is not valid syntax)
                    Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                        Value = str,
                        As = "a number. You cannot use a decimal separator after a power-of-ten separator",
                        Location = new LocationRange(originPos, _input.Position),
                        //ExtraNotes = notes
                    });
                } else {
                    // ...or there is a second decimal separator, which isn't valid either
                    Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                        Value = str,
                        As = "a number. There already was a decimal separator earlier",
                        Location = new LocationRange(originPos, _input.Position),
                        //ExtraNotes = notes
                    });
                }

                isValid = false;

                _ = _input.Consume(); // hack to prevent the reconsume from fucking up the input
            }
        }

        // we already had a "power-of-ten separator", so this is not valid.
        if (currChar is 'e' or 'E') {
            if (nextIsNumber()) {
                var errorCount = Logger.ErrorCount;

                numberSB.Append(currChar).Append(ConsumeNumberToken().Representation);

                while (Logger.ErrorCount > errorCount) {
                    _ = Logger.errorStack.Pop();
                }
            }

            Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                Value = numberSB.ToString(),
                As = "a number. There already was a power-of-ten separator earlier",
                Location = new LocationRange(originPos, _input.Position),
            });

            isValid = false;

            _ = _input.Consume(); // hack to prevent the reconsume from fucking up the input
        }

        _input.Reconsume();

        double val = 0;
        string numberStr = numberSB.ToString();

        if (isValid && numberStr.Length != 0 && !Double.TryParse(numberStr.AsSpan(), out val)) {
            Logger.Error(new UnexpectedError<string>(ErrorArea.Parser) {
                Value = numberStr,
                Message = "The number " + numberSB + " cannot be expressed in lotus"
            });

            isValid = false;
        }

        return new NumberToken(numberStr, val, new LocationRange(originPos, _input.Position)) { IsValid = isValid };
    }

    private OperatorToken ConsumeOperatorToken() {
        var currChar = _input.Current;
        var currCharStr = currChar.ToString();

        var currPos = _input.Position;

        // easy and clear switches
        switch (currCharStr) {
            case "*":
                // Multiplication operator a * b
                return new OperatorToken(currCharStr, Precedence.Multiplication, true, _input.Position);
            case "/":
                // Division operator a / b
                return new OperatorToken(currCharStr, Precedence.Division, true, _input.Position);
            case "%":
                // Modulo operator a % b
                return new OperatorToken(currCharStr, Precedence.Modulo, true, _input.Position);
            case ".":
                // Member access "operator" a.b
                return new OperatorToken(currCharStr, Precedence.Access, true, _input.Position);
            case "&" when _input.Peek() == '&':
                // Logical AND operator a && b
                return new OperatorToken(currCharStr + "" + _input.Consume(), Precedence.And, true, new LocationRange(currPos, _input.Position));
            case "|" when _input.Peek() == '|':
                // Logical OR operator a || b
                return new OperatorToken(currCharStr + "" + _input.Consume(), Precedence.Or, true, new LocationRange(currPos, _input.Position));
            case "?":
                // Ternary comparison operator a ? b : c
                return new OperatorToken(currCharStr, Precedence.Ternary, true, _input.Position);
        }

        // this part is for cases that aren't simple and/or wouldn't look good in a switch expression

        if (currChar is '+' or '-') {
            if (_input.Peek() == currChar) {
                return new OperatorToken(currChar + "" + _input.Consume(), Precedence.Unary, false, new LocationRange(currPos, _input.Position));
            }

            return new OperatorToken(currCharStr, Precedence.Addition, true, _input.Position);
        }

        if (currChar == '^') {
            if (_input.Peek() == '^') {
                _ = _input.Consume(); // consume the '^' we just peeked at

                return new OperatorToken("^^", Precedence.Xor, true, new LocationRange(currPos, _input.Position));
            }

            // Power/Exponent operator a ^ b
            return new OperatorToken(currCharStr, Precedence.Power, false, _input.Position);
        }

        if (currChar == '=') {
            // Equality comparison operator a == b
            if (_input.Peek() == '=') {
                return new OperatorToken(currCharStr + _input.Consume(), Precedence.Equal, true, new LocationRange(currPos, _input.Position));
            }

            // Assignment operator a = b
            return new OperatorToken(currCharStr, Precedence.Assignment, true, _input.Position);
        }

        if (currChar == '>') {
            // Greater-than-or-equal comparison operator a >= b
            if (_input.Peek() == '=') {
                return new OperatorToken(currCharStr + _input.Consume(), Precedence.GreaterThanOrEqual, true, new LocationRange(currPos, _input.Position));
            }

            // Greater-than comparison operator a > b
            return new OperatorToken(currCharStr, Precedence.GreaterThan, true, _input.Position);
        }

        if (currChar == '<') {
            // Less-than-or-equal comparison operator a <= b
            if (_input.Peek() == '=') {
                return new OperatorToken(currCharStr + _input.Consume(), Precedence.LessThanOrEqual, true, new LocationRange(currPos, _input.Position));
            }

            // Less-than comparison operator a < b
            return new OperatorToken(currCharStr, Precedence.LessThan, true, _input.Position);
        }

        if (currChar == '!') {
            // Not-equal comparison operator a != b
            if (_input.Peek() == '=') {
                return new OperatorToken(currCharStr + _input.Consume(), Precedence.NotEqual, true, new LocationRange(currPos, _input.Position));
            }

            // Unary logical NOT operator !a
            return new OperatorToken(currCharStr, Precedence.Unary, false, _input.Position);
        }

        Debug.Fail("Couldn't make an operator token from '" + currCharStr + "'");
        throw null;
    }
}