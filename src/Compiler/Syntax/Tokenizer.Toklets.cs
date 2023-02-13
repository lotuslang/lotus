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
        Debug.Assert(!isComplex || _input.Current == '$');

        if (isComplex)
            _ = _input.Consume(); // consume the '$' prefix

        Debug.Assert(_input.Current is '"' or '\'');

        var startPos = _input.Position;

        // the output token
        var output = new StringBuilder();

        var sections
            = isComplex
            ? ImmutableArray.CreateBuilder<InterpolatedSection>()
            : null!;

        var isValid = true;

        // while the current character is not the ending delimiter
        while (_input.Consume(out var currChar) && currChar != '"') {
            if (currChar == '\n') {
                Logger.Error(new UnexpectedError<char>(ErrorArea.Tokenizer) {
                    In = "a string literal",
                    Value = currChar,
                    Location = _input.Position,
                    Expected = "a string delimiter like this: \"" + output.ToString() + '"'
                });

                isValid = false;

                break;
            }

            if (isComplex && currChar == '}') {
                output.Append('}');

                if (_input.Consume() != '}') {
                    Logger.Error(new UnexpectedError<char>(ErrorArea.Tokenizer) {
                        Value = _input.Current,
                        Message = "A raw '}' character must be escaped in interpolated strings",
                        In = "an interpolated string",
                        Location = _input.Position
                    });

                    _input.Reconsume();
                    isValid = false;
                }

                continue;
            }

            if (isComplex && currChar == '{') {
                output.Append('{');
                if (_input.Peek() == '{') {
                    _ = _input.Consume(); // consume the '{' to not consume it twice
                    continue;
                }

                var sectionStartPos = _input.Position;

                var tokenList = ImmutableArray.CreateBuilder<Token>();

                var unmatchedBrackets = 1; // count the opening bracket as currently unmatched

                while (unmatchedBrackets != 0) { // until we match the opening bracket
                    if (!this.Consume(out var currToken)) {
                        Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                            In = "an interpolated string",
                            Expected = "} followed by a string delimiter like this: \"" + output.ToString() + '"',
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
                    sections.Add(new InterpolatedSection(
                        output.Length - 1,
                        tokenList.ToImmutable(),
                        new LocationRange(sectionStartPos, _input.Position)
                    ));
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

                output.Append('}');

                // append whatever was after the closing '}'
                output.Append(this.Current.TrailingTrivia?.Representation);

                continue;
            }

            if (currChar == '\\') {
                if (!TryParseEscapeSequence(out currChar, out _))
                    isValid = false;

                output.Append(currChar);

                continue;
            }

            // add it to the value of output
            output.Append(currChar);
        }

        // if we encountered an EOF
        if (_input.Current == _input.Default) {
            Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                In = "a string",
                Expected = new[] {
                        "a character",
                        "a string delimiter ' or \"",
                    },
                Location = _input.Position
            });

            isValid = false;
        }

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

    private CharToken ConsumerCharToken() {
        Debug.Assert(_input.Current is '\'');

        var startPos = _input.Position;

        var currChar = _input.Consume();

        var isValid = true;
        if (currChar == '\n') {
            Logger.Error(new UnexpectedError<char>(ErrorArea.Tokenizer) {
                Value = currChar,
                Message = "Character literals cannot contain newlines",
                In = "a character literal",
                Expected = "the '\\n' escape sequence"
            });

            isValid = false;

            // return directly to not trigger further errors
            return new CharToken('\0', new LocationRange(startPos, _input.Position)) { IsValid = isValid };
        }

        string repr;
        if (currChar == '\\') {
            isValid &= TryParseEscapeSequence(out currChar, out repr);
        } else {
            repr = "\\" + currChar.ToString();
        }

        // if the next character isn't a quote
        if (_input.Consume() != '\'') {
            var sb = new StringBuilder();

            do
                sb.Append(_input.Current);
            while (_input.Consume(out currChar) && currChar is not ('\n' or '\''));

            Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                Value = sb.ToString(),
                Message = "Too many characters in character literal",
                In = "a character literal",
                Expected = "a single character, or an escape sequence"
            });

            isValid = false;
        }

        return new CharToken(currChar, repr, new LocationRange(startPos, _input.Position)) { IsValid = isValid };
    }

    private bool TryParseEscapeSequence(out char escapedChar, out string rawCharString) {
        Debug.Assert(_input.Current is '\\');

        var currChar = _input.Consume();

        var isValid = true;

        char throwInvalidEscapeAndGetChar(char rawChar) {
            Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                Value = "\\" + rawChar,
                In = "a string",
                Message = $@"Unrecognized escape sequence '\{rawChar}'",
                Location = _input.Position
            });

            isValid = false;

            return rawChar;
        }

        Span<char> rawChars = stackalloc char[4];

        (escapedChar, rawCharString) = currChar switch {
            '\\' => ('\\', @"\\"),
            '\'' => ('\'', @"\'"),
            '"' => ('"', "\""),
            '0' => ('\0', @"\0"),
            'a' => ('\a', @"\a"),
            'b' => ('\b', @"\b"),
            'f' => ('\f', @"\f"),
            'n' => ('\n', @"\n"),
            'r' => ('\r', @"\r"),
            't' => ('\t', @"\t"),
            'v' => ('\v', @"\v"),
            // todo(lexing): implement \U and \x
            'u' => (ParseUnicodeEscapeSequence(ref rawChars), @"\u" + rawChars.ToString()),
            _ => (throwInvalidEscapeAndGetChar(currChar), @"\" + currChar)
        };

        return isValid;
    }

    private char ParseUnicodeEscapeSequence(ref Span<char> rawChars) {
        Debug.Assert(_input.Current is 'u');

        for (int i = 0; i < 4; i++) {
            if (!_input.Consume(out rawChars[i])) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Tokenizer) {
                    In = "a string",
                    Expected = "a unicode escape sequence with 4 digits",
                    Location = _input.Position,
                });
            }

            if (!Char.IsAsciiHexDigit(rawChars[i])) {
                Logger.Error(new UnexpectedError<char>(ErrorArea.Tokenizer) {
                    Value = rawChars[i],
                    As = "an hex digit",
                    In = "a string",
                    Expected = "an hexadecimal digit, from 0-9, or A-F",
                    Location = _input.Position
                });

                // avoid filling rawChars with garbage, since it's used by CharToHexLookup
                rawChars[i] = '0';
            }
        }

        byte finalChar = 0;

        for (int i = 0; i < 4; i++) {
            finalChar *= 16;
            finalChar += MiscUtils.CharToHexLookup[rawChars[i]];
        }

        return (char)finalChar;
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
        var numberSB = new StringBuilder();

        var currChar = _input.Current;

        Debug.Assert(currChar is '.' || Char.IsAsciiDigit(currChar));

        var isValid = true;

        bool hasDecimalOrExponent = false;

        var originPos = _input.Position; // the position of the number's first character

        bool nextIsNumber() => Char.IsAsciiDigit(_input.Peek());

        /// <summary> consumes a span of consecutive digits </summary>
        void consumeAllNextDigits() {
            while (Char.IsAsciiDigit(currChar)) {
                numberSB.Append(currChar);

                currChar = _input.Consume();
            }
        }

        // consume the whole part
        consumeAllNextDigits();

        // decimal point (must be followed by a number, cause it might
        // be an access otherwise)
        if (currChar is '.' && nextIsNumber()) {
            hasDecimalOrExponent = true;
            numberSB.Append(currChar);

            currChar = _input.Consume();
            consumeAllNextDigits();
        }

        // exponent separator
        if (currChar is 'e' or 'E') {
            hasDecimalOrExponent = true;

            if (!nextIsNumber()) {
                var nextChar = _input.Consume();

                if (nextChar is '+' or '-') {
                    if (nextIsNumber()) {
                        _input.Reconsume(); // reconsumes the '+'/'-' to be parsed correctly
                        goto validExponent;
                    } else {
                        // display the non-digit in the error message and set the right position
                        currChar = _input.Consume();
                    }
                }

                Logger.Error(new UnexpectedError<char>(ErrorArea.Tokenizer) {
                    Value = currChar,
                    In = "a number literal",
                    As = "an exponent separator",
                    Message = "Exponent in numeric literal must be a number",
                    Location = _input.Position
                });

                isValid = false;
                goto dontTokenizeExponent;
            }

        validExponent:
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

            consumeAllNextDigits();
        }
    dontTokenizeExponent:

        // if we have another decimal separator...
        if (currChar == '.' && nextIsNumber()) {
            var errorCount = Logger.ErrorCount;

            _ = _input.Consume(); // we need to update _input.Current before lexing a new number token
            numberSB.Append(currChar).Append(ConsumeNumberToken().Representation);

            // The above .Consume(...) could generate extra errors, except we don't really
            // want to show them to the user since they don't really matter; we just wanna
            // be sure we consumed what we had to
            while (Logger.ErrorCount > errorCount) {
                _ = Logger.errorStack.Pop();
            }

            var str = numberSB.ToString();

            var loc = new LocationRange(originPos, _input.Position);

            if (str.Contains('e') || str.Contains('E')) {
                // ...we either stopped parsing an exponent number because of a
                // decimal (which is not valid syntax)
                Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                    Value = str,
                    As = "a number",
                    Message = "Exponents can't have any decimals",
                    Location = loc,
                });
            } else {
                // ...or there is a second decimal separator, which isn't valid either
                Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                    Value = str,
                    As = "a number",
                    Message = "This number literal has more than one decimal part",
                    Location = loc,
                });
            }

            isValid = false;
            currChar = _input.Consume();
        }

        // we already had an "exponent separator", so this is not valid.
        if (currChar is 'e' or 'E') {
            numberSB.Append(currChar);

            if (nextIsNumber() || _input.Peek() is '+' or '-') {
                // yes it's fine to consume a new one in any case, cause ConsumeNumberToken expects
                // _input.Current to already be set to a digit or '.'
                currChar = _input.Consume();

                if (currChar is '+' or '-') {
                    numberSB.Append(currChar);
                    _ = _input.Consume(); // ConsumeNumberToken expects a digit or '.' at the start
                }

                var errorCount = Logger.ErrorCount;

                numberSB.Append(ConsumeNumberToken().Representation);

                while (Logger.ErrorCount > errorCount) {
                    _ = Logger.errorStack.Pop();
                }
            }

            Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                Value = numberSB.ToString(),
                As = "a number. There already was an exponent separator earlier",
                Location = new LocationRange(originPos, _input.Position),
            });

            isValid = false;
            currChar = _input.Consume();
        }

        var numericStr = numberSB.ToString();

        var kindStr = "";

        var numberKind = NumberKind.Unknown;

        if (currChar is 'u' or 'U') {
            kindStr += currChar;
            numberKind |= NumberKind.Unsigned;
            currChar = _input.Consume();
        }

        if (currChar is 'f' or 'F') {
            kindStr += currChar;
            numberKind |= NumberKind.Float;
            currChar = _input.Consume();
        }

        if (currChar is 'd' or 'D') {
            kindStr += currChar;
            numberKind |= NumberKind.Double;
            currChar = _input.Consume();
        }

        if (currChar is 'l' or 'L') {
            kindStr += currChar;
            numberKind |= NumberKind.Long;
            _ = _input.Consume();
        }

        if (numberKind is NumberKind.Unknown or NumberKind.Unsigned) {
            if (hasDecimalOrExponent)
                numberKind |= NumberKind.Double;
            else
                numberKind |= NumberKind.Int;
        }

        _input.Reconsume();

        var range = new LocationRange(originPos, _input.Position);

        if (numberSB.Length == 0) //
            return new NumberToken("", 0, range, NumberKind.Int) { IsValid = false };

        var actualRepr = numericStr + kindStr;

        var isKindValid = ValidateNumberKindAndSanitize(ref numberKind, hasDecimalOrExponent, actualRepr, range);

        void outsideOfRange() {
            if (!isValid)
                return;

            Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                Value = actualRepr,
                Message = "Number " + numericStr + " is outside the range of type " + numberKind,
                As = "a number literal",
                Location = range
            });

            isValid = false;
        }

        object value = 0;

        // avoid a number too long for Int128.Parse
        if ((numberKind & (NumberKind.Int | NumberKind.Long)) != 0) {
            if (numericStr.Length >= 20)
                outsideOfRange();
            value = 0;

            // Double.Parse can handle anything :)
            numberKind = NumberKind.Double;
        }

        switch (numberKind) {
            case NumberKind.Unsigned:
            case NumberKind.Int: {
                var realValue = Int128.Parse(numericStr);
                if (realValue > Int32.MaxValue || realValue < Int32.MinValue)
                    outsideOfRange();
                value = (int)realValue;
                break;
            }
            case NumberKind.UInt: {
                var realValue = Int128.Parse(numericStr);
                if (realValue > UInt32.MaxValue || realValue < UInt32.MinValue)
                    outsideOfRange();
                value = (uint)realValue;
                break;
            }
            case NumberKind.Long: {
                var realValue = Int128.Parse(numericStr);
                if (realValue > Int64.MaxValue || realValue < Int64.MinValue)
                    outsideOfRange();
                value = (long)realValue;
                break;
            }
            case NumberKind.ULong: {
                var realValue = Int128.Parse(numericStr);
                if (realValue > UInt64.MaxValue || realValue < UInt64.MinValue)
                    outsideOfRange();
                value = (ulong)realValue;
                break;
            }
            case NumberKind.Float: {
                float realValue = 0;
                if (isValid && !Single.TryParse(numericStr, out realValue)) {
                    Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                        Value = actualRepr,
                        Message = "The number " + numericStr + " cannot be expressed as a float."
                    });
                }

                value = realValue;
                break;
            }
            case NumberKind.Double: {
                double realValue = 0;
                if (isValid && !Double.TryParse(numericStr, out realValue)) {
                    Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                        Value = actualRepr,
                        Message = "The number " + numericStr + " cannot be expressed as a double."
                    });
                }

                value = realValue;
                break;
            }
        }

        return new NumberToken(actualRepr, value, range, numberKind) { IsValid = isValid && isKindValid };
    }

    private bool ValidateNumberKindAndSanitize(ref NumberKind kind, bool hasDecimal, string numberStr, LocationRange loc) {
        // note: we don't need to test for NumberKind.Int cause it'll only be assigned
        // if there's no flag/marker and no decimal

        if (kind.HasFlag(NumberKind.Unsigned)
            && (hasDecimal || kind.HasFlag(NumberKind.Float) || kind.HasFlag(NumberKind.Double))
        ) {
            Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                Value = numberStr,
                Message = "Floating-point numbers can't be unsigned",
                As = "a number literal",
                Location = loc
            });

            kind = NumberKind.Double;
            return false;
        }

        // so we don't have to duplicate code in case of hasDecimal+Long
        if (hasDecimal && !kind.HasFlag(NumberKind.Float))
            kind |= NumberKind.Double;

        if (kind.HasFlag(NumberKind.Float)) {
            if (kind.HasFlag(NumberKind.Long)) {
                Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                    Value = numberStr,
                    Message = "A number can't be both a float and a long",
                    As = "a number literal",
                    Location = loc
                });

                kind = NumberKind.Float;
                return false;
            }

            if (kind.HasFlag(NumberKind.Double)) {
                Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                    Value = numberStr,
                    Message = "A number can't be both a float and a double",
                    As = "a number literal",
                    Location = loc
                });

                kind = NumberKind.Double;
                return false;
            }
        }

        if (kind.HasFlag(NumberKind.Double)) {
            if (kind.HasFlag(NumberKind.Long)) {
                Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                    Value = numberStr,
                    Message = "A number can't be both a double and a long",
                    As = "a number literal",
                    Location = loc
                });

                kind = NumberKind.Double;
                return false;
            }

            if (kind.HasFlag(NumberKind.Float)) {
                Logger.Error(new UnexpectedError<string>(ErrorArea.Tokenizer) {
                    Value = numberStr,
                    Message = "A number can't be both a float and a double",
                    As = "a number literal",
                    Location = loc
                });

                kind = NumberKind.Double;
                return false;
            }
        }

        return true;
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
