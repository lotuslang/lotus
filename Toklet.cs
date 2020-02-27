using System;
using System.Linq;
using System.Collections.Generic;

public class Toklet
{
    protected virtual Predicate<IConsumer<char>> condition
        => (_ => true);

    public Predicate<IConsumer<char>> Condition => condition;

    public virtual Token Consume(IConsumer<char> input, Tokenizer tokenizer)
        => new Token(input.Consume(), TokenKind.delim, input.Position);
}

public class NumberToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (input => {
                var current = input.Consume();

                if (Char.IsDigit(current)) return true;

                if (current == '+' || current == '-' || current == '.') {
                    return Char.IsDigit(input.Consume());
                }

                return false;
            }
        );

    public override Token Consume(IConsumer<char> input, Tokenizer tokenizer) {
        // consume a character
        var currChar = input.Consume();

        // the output token
        var output = new NumberToken("", input.Position);

        if (currChar == '+' || currChar == '-') {
            output.Add(currChar);
            currChar = input.Consume();
        }

        // while the current character is a digit
        while (Char.IsDigit(currChar)) {

            // add it to the value of output
            output.Add(currChar);

            // consume a character
            currChar = input.Consume();
        }

        // if the character is '.'
        if (currChar == '.')
        {
            // add it to the value of output
            output.Add(currChar);

            // consume a character
            currChar = input.Consume();
        }

        // while the current character is a digit
        while (Char.IsDigit(currChar)) {

            // add it to the value of output
            output.Add(currChar);

            // consume a character
            currChar = input.Consume();
        }

        // if the character is an 'e' or an 'E'
        if (currChar == 'e' || currChar == 'E') {

            // add the e/E to the output
            output.Add(currChar);

            // consume a character
            currChar = input.Consume();


            // if the character is a '+' or a '-'
            if (currChar == '+' || currChar == '-') {

                // add it to the value of output
                output.Add(currChar);

                // consume a character
                currChar = input.Consume();
            }

            // while the current character is a digit
            while (Char.IsDigit(currChar)) {

                // add it to the value of output
                output.Add(currChar);

                // consume a character
                currChar = input.Consume();
            }
        }

        // if the character is '.'
        if (currChar == '.') {
            throw new Exception("Unexpected decimal separator at position " + input.Position);
        }

        // if the character is 'e' or 'E'
        if (currChar == 'e' || currChar == 'E') {
            throw new Exception("Unexpected power of ten separator at position " + input.Position);
        }

        input.Reconsume();

        return output;
    }
}

public class ComplexStringToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (input => {
                var current = input.Consume();

                if (current != '$') return false;

                current = input.Consume();

                if (current != '\'' && current != '"') return false;

                return true;
            }
        );

    public override Token Consume(IConsumer<char> input, Tokenizer tokenizer) {


        // consume the '$' in front
        input.Consume();

        var endingDelimiter = input.Consume();

        //consume a character
        var currChar = input.Consume();

        // the output token
        var output = new ComplexStringToken("", new List<Token[]>(), input.Position);

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            if (currChar == '{') {
                var tokenList = new List<Token>();
                Token currToken;

                while ((currToken = tokenizer.Consume()) != "}") {
                    if (currToken == ";") {
                        throw new Exception($"{input.Position} : Malformed formatted string. Unexpected ';'. Did you forget '}}' ?");
                    }

                    if (currToken == TokenKind.EOF) {
                        throw new Exception("Unexpected EOF in string");
                    }

                    tokenList.Add(currToken);
                }

                output.AddSection(tokenList.ToArray());

                output.Add("{" + (output.CodeSections.Count - 1) + "}");

                currChar = input.Consume();
                continue;
            }

            output.Add(currChar);

            if (!input.Consume(out currChar)) {
                throw new Exception("Unexpected EOF in string");
            }
        }

        return output;
    }
}

public class StringToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (input => input.Consume() == '\'' || input.Current == '"');

    public override Token Consume(IConsumer<char> input, Tokenizer tokenizer) {

        var endingDelimiter = input.Consume();

        // consume a character
        var currChar = input.Consume();

        // the output token
        var output = new ComplexToken("", TokenKind.@string, input.Position);

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            // add it to the value of output
            output.Add(currChar);

            if (!input.Consume(out currChar)) {
                throw new Exception("Unexpected EOF in string");
            }
        }

        // return the output token
        return output;
    }
}

public class IdentToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (input => Char.IsLetter(input.Consume()) || input.Current == '_');

    public override Token Consume(IConsumer<char> input, Tokenizer tokenizer) {

        // consume a character
        var currChar = input.Consume();

        // the output token
        var output = new ComplexToken("", TokenKind.ident, input.Position);

        // if the character is not a letter or a low line
        if (!(Char.IsLetter(currChar) || currChar == '_')) {
            throw new Exception("An identifier cannot start with the character " + currChar);
        }

        // while the current character is a letter, a digit, or a low line
        while (Char.IsLetterOrDigit(currChar) || currChar == '_') {

            // add it to the value of output
            output.Add(currChar);

            // consume a character
            currChar = input.Consume();
        }

        // reconsume the last token (which is not a letter, a digit, or a low line,
        // since our while loop has exited) to make sure it is processed by the tokenizer afterwards
        input.Reconsume();

        // return the output token
        return output;
    }
}

public class OperatorToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (input => {
                var c = input.Consume();

                if (c == '+'
                ||  c == '-'
                ||  c == '*'
                ||  c == '/'
                ||  c == '^'
                ||  c == '.'
                ||  c == '='
                ||  c == '>'
                ||  c == '<'
                ||  c == '!') return true;

                if (c == '&') {
                    if (input.Consume() == '&') return true;
                } else if (c == '|') {
                    if (input.Consume() == '|') return true;
                }

                return false;
            }
        );

    public override Token Consume(IConsumer<char> input, Tokenizer tokenizer) {

        var currChar = input.Consume();

        if (currChar == '+' || currChar == '-') {

            if (input.Peek() == currChar) {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.Unary, "right", input.Position);
            }

            return new OperatorToken(currChar, Precedence.Addition, "left", input.Position);
        }

        if (currChar == '*') return new OperatorToken(currChar, Precedence.Multiplication, "left", input.Position);

        // if the character is '/'
        if (currChar == '/') return new OperatorToken(currChar, Precedence.Division, "left", input.Position);

        // if the character is '^' return a new operator token with precedence 4
        if (currChar == '^') return new OperatorToken(currChar, Precedence.Power, "right", input.Position);

        if (currChar == '.') return new OperatorToken(currChar, Precedence.Access, "left", input.Position);

        // if the current and next character for "=="
        if (currChar == '=') {

            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.Equal, "left", input.Position);
            }

            return new OperatorToken(currChar, Precedence.Assignment, "left", input.Position);
        }

        if (currChar == '>') {

            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.GreaterThanOrEqual, "left", input.Position);
            }

            return new OperatorToken(currChar, Precedence.GreaterThan, "left", input.Position);
        }

        if (currChar == '<') {

            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.LessThanOrEqual, "left", input.Position);
            }

            return new OperatorToken(currChar, Precedence.LessThan, "left", input.Position);
        }

        // if the character is '!'
        if (currChar == '!') {

            if (input.Peek() == '=') {
                return new OperatorToken(currChar +""+ input.Consume(), Precedence.NotEqual, "left", input.Position);
            }

            // return a new operator token with precedence 5
            return new OperatorToken(currChar, Precedence.Unary, "right", input.Position);
        }

        // if the current and next character form "&&"
        if (currChar == '&' && input.Peek() == '&') {

            // return a new operator token with precedence 5
            return new OperatorToken(currChar +""+ input.Consume(), Precedence.And, "left", input.Position);
        }

        // if the current and next character form "||"
        if (currChar == '|' && input.Peek() == '|') {

            // return a new operator token with precedence 5
            return new OperatorToken(currChar +""+ input.Consume(), Precedence.Or, "left", input.Position);
        }

        throw new Exception();
    }
}