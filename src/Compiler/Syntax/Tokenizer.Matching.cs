namespace Lotus.Syntax;

public partial class Tokenizer
{
    private Token ConsumeTokenCore() {
        if (!_input.TryConsumeChar(out char currChar))
            return ConsumeEOFToken(in currChar);

        if (Char.IsAsciiDigit(currChar))
            return ConsumeNumberToken();
        if (Char.IsAsciiLetter(currChar))
            return ConsumeIdentToken();

        switch (currChar) {
            case '.':
                if (Char.IsAsciiDigit(_input.PeekNextChar()))
                    return ConsumeNumberToken();
                else
                    return ConsumeOperatorToken();
            case '_':
            case '@':
                return ConsumeIdentToken();
            case '$' when _input.PeekNextChar() is '"' :
                return ConsumeComplexStringToken();
            case '"':
                return ConsumeStringToken();
            case '\'':
                return ConsumeCharToken();
            case '+':
            case '-':
            case '*':
            case '/':
            case '%':
            case '^':
            case '=':
            case '>':
            case '<':
            case '!':
            case '?':
                return ConsumeOperatorToken();
            case '&':
                if (_input.PeekNextChar() is '&')
                    return ConsumeOperatorToken();
                goto default;
            case '|':
                if (_input.PeekNextChar() is '|')
                    return ConsumeOperatorToken();
                goto default;
            case ';':
                return ConsumeSemicolonToken();
            case ':':
                if (_input.PeekNextChar() is ':')
                    return ConsumeDoubleColonToken();
                goto default;
            default:
                return ConsumeDelimToken(in currChar);
        }
    }
}