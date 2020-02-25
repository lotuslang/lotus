using System;
using System.Linq;
using System.Collections.Generic;

public class Toklet
{
    protected virtual Predicate<IConsumer<char>> condition
        => new Predicate<IConsumer<char>>(consumer => true);

    public Predicate<IConsumer<char>> Condition => condition;

    public virtual Token Consume(IConsumer<char> consumer, Tokenizer tokenizer)
        => new Token(consumer.Consume(), TokenKind.delim, consumer.Position);
}

public class NumberToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (consumer => {
                var current = consumer.Consume();

                if (Char.IsDigit(current)) return true;

                if (current == '+' || current == '-' || current == '.') {
                    return Char.IsDigit(consumer.Consume());
                }

                return false;
            }
        );

    public override Token Consume(IConsumer<char> consumer, Tokenizer tokenizer) {
        // consume a character
        var currChar = consumer.Consume();

        // the output token
        var output = new NumberToken("", consumer.Position);

        if (currChar == '+' || currChar == '-') {
            output.Add(currChar);
            currChar = consumer.Consume();
        }

        // while the current character is a digit
        while (Char.IsDigit(currChar)) {

            // add it to the value of output
            output.Add(currChar);

            // consume a character
            currChar = consumer.Consume();
        }

        // if the character is '.'
        if (currChar == '.')
        {
            // add it to the value of output
            output.Add(currChar);

            // consume a character
            currChar = consumer.Consume();
        }

        // while the current character is a digit
        while (Char.IsDigit(currChar)) {

            // add it to the value of output
            output.Add(currChar);

            // consume a character
            currChar = consumer.Consume();
        }

        // if the character is an 'e' or an 'E'
        if (currChar == 'e' || currChar == 'E') {

            // add the e/E to the output
            output.Add(currChar);

            // consume a character
            currChar = consumer.Consume();


            // if the character is a '+' or a '-'
            if (currChar == '+' || currChar == '-') {

                // add it to the value of output
                output.Add(currChar);

                // consume a character
                currChar = consumer.Consume();
            }

            // while the current character is a digit
            while (Char.IsDigit(currChar)) {

                // add it to the value of output
                output.Add(currChar);

                // consume a character
                currChar = consumer.Consume();
            }
        }

        // if the character is '.'
        if (currChar == '.') {
            throw new Exception("Unexpected decimal separator at position " + consumer.Position);
        }

        // if the character is 'e' or 'E'
        if (currChar == 'e' || currChar == 'E') {
            throw new Exception("Unexpected power of ten separator at position " + consumer.Position);
        }

        consumer.Reconsume();

        return output;
    }
}

public class ComplexStringToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (consumer => {
                var current = consumer.Consume();

                if (current != '$') return false;

                current = consumer.Consume();

                if (current != '\'' && current != '"') return false;

                return true;
            }
        );

    public override Token Consume(IConsumer<char> consumer, Tokenizer tokenizer) {


        // consume the '$' in front
        consumer.Consume();

        var endingDelimiter = consumer.Consume();

        //consume a character
        var currChar = consumer.Consume();

        // the output token
        var output = new ComplexStringToken("", new List<Token[]>(), consumer.Position);

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            if (currChar == '{') {
                var tokenList = new List<Token>();
                Token currToken;

                while ((currToken = tokenizer.Consume()) != "}") {
                    if (currToken == ";") {
                        throw new Exception($"{consumer.Position} : Malformed formatted string. Unexpected ';'. Did you forget '}}' ?");
                    }

                    if (currToken == TokenKind.EOF) {
                        throw new Exception("Unexpected EOF in string");
                    }

                    tokenList.Add(currToken);
                }

                output.AddSection(tokenList.ToArray());

                output.Add("{" + (output.CodeSections.Count - 1) + "}");

                currChar = consumer.Consume();
                continue;
            }

            output.Add(currChar);

            if (!consumer.Consume(out currChar)) {
                throw new Exception("Unexpected EOF in string");
            }
        }

        return output;
    }
}

public class StringToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (consumer => consumer.Consume() == '\'' || consumer.Current == '"');

    public override Token Consume(IConsumer<char> consumer, Tokenizer tokenizer) {

        var endingDelimiter = consumer.Consume();

        // consume a character
        var currChar = consumer.Consume();

        // the output token
        var output = new ComplexToken("", TokenKind.@string, consumer.Position);

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            // add it to the value of output
            output.Add(currChar);

            if (!consumer.Consume(out currChar)) {
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
        => (consumer => Char.IsLetter(consumer.Consume()) || consumer.Current == '_');

    public override Token Consume(IConsumer<char> consumer, Tokenizer tokenizer) {

        // consume a character
        var currChar = consumer.Consume();

        // the output token
        var output = new ComplexToken("", TokenKind.ident, consumer.Position);

        // if the character is not a letter or a low line
        if (!(Char.IsLetter(currChar) || currChar == '_')) {
            throw new Exception("An identifier cannot start with the character " + currChar);
        }

        // while the current character is a letter, a digit, or a low line
        while (Char.IsLetterOrDigit(currChar) || currChar == '_') {

            // add it to the value of output
            output.Add(currChar);

            // consume a character
            currChar = consumer.Consume();
        }

        // reconsume the last token (which is not a letter, a digit, or a low line,
        // since our while loop has exited) to make sure it is processed by the tokenizer afterwards
        consumer.Reconsume();

        // return the output token
        return output;
    }
}

public class CommentToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (consumer => {
                if (consumer.Consume() != '/') {
                    return false;
                }

                if (consumer.Consume() != '/' && consumer.Current != '*') {
                    return false;
                }

                return true;
            }
        );

    public override Token Consume(IConsumer<char> consumer, Tokenizer tokenizer) {

        var currChar = consumer.Consume();

        // if the character isn't '/' throw an exception
        if (currChar != '/') throw new Exception("Comments start with a '/' character");

        // and the next character is '*', then it is a limited comment
        if (consumer.Peek() == '*') {

            consumer.Consume();

            // consume characters until the two next characters form "*/"
            while (consumer.Consume(out currChar) && !(currChar == '*' && consumer.Peek() == '/')) {
                if (currChar == '/' && consumer.Peek() == '*') {
                    consumer.Reconsume();

                    Consume(consumer, tokenizer);
                }
            }

            // consume the "*/" characters
            consumer.Consume(); // should be '*'
            consumer.Consume(); // should be '/'

            // consume a token and return it
            return tokenizer.Consume();
        }

        if (consumer.Peek() == '/') {

            // consume charcaters until the two next characters form "//"
            while (consumer.Peek() != '\n') { consumer.Consume(); }

            consumer.Consume(); // should be '\n'

            // consume a token and return it
            return tokenizer.Consume();
        }

        // return a new operator token with precedence Division
        return new OperatorToken(currChar, Precedence.Division, "left", consumer.Position);
    }
}

public class OperatorToklet : Toklet
{
    protected override Predicate<IConsumer<char>> condition
        => (consumer => {
                var c = consumer.Consume();

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
                    if (consumer.Consume() == '&') return true;
                } else if (c == '|') {
                    if (consumer.Consume() == '|') return true;
                }

                return false;
            }
        );

    public override Token Consume(IConsumer<char> consumer, Tokenizer tokenizer) {

        var currChar = consumer.Consume();

        if (currChar == '+' || currChar == '-') {

            if (consumer.Peek() == currChar) {
                return new OperatorToken(currChar +""+ consumer.Consume(), Precedence.Unary, "right", consumer.Position);
            }

            return new OperatorToken(currChar, Precedence.Addition, "left", consumer.Position);
        }

        if (currChar == '*') return new OperatorToken(currChar, Precedence.Multiplication, "left", consumer.Position);

        // if the character is '/'
        if (currChar == '/') return new OperatorToken(currChar, Precedence.Division, "left", consumer.Position);

        // if the character is '^' return a new operator token with precedence 4
        if (currChar == '^') return new OperatorToken(currChar, Precedence.Power, "right", consumer.Position);

        if (currChar == '.') return new OperatorToken(currChar, Precedence.Access, "left", consumer.Position);

        // if the current and next character for "=="
        if (currChar == '=') {

            if (consumer.Peek() == '=') {
                return new OperatorToken(currChar +""+ consumer.Consume(), Precedence.Equal, "left", consumer.Position);
            }

            return new OperatorToken(currChar, Precedence.Assignment, "left", consumer.Position);
        }

        if (currChar == '>') {

            if (consumer.Peek() == '=') {
                return new OperatorToken(currChar +""+ consumer.Consume(), Precedence.GreaterThanOrEqual, "left", consumer.Position);
            }

            return new OperatorToken(currChar, Precedence.GreaterThan, "left", consumer.Position);
        }

        if (currChar == '<') {

            if (consumer.Peek() == '=') {
                return new OperatorToken(currChar +""+ consumer.Consume(), Precedence.LessThanOrEqual, "left", consumer.Position);
            }

            return new OperatorToken(currChar, Precedence.LessThan, "left", consumer.Position);
        }

        // if the character is '!'
        if (currChar == '!') {

            if (consumer.Peek() == '=') {
                return new OperatorToken(currChar +""+ consumer.Consume(), Precedence.NotEqual, "left", consumer.Position);
            }

            // return a new operator token with precedence 5
            return new OperatorToken(currChar, Precedence.Unary, "right", consumer.Position);
        }

        // if the current and next character form "&&"
        if (currChar == '&' && consumer.Peek() == '&') {

            // return a new operator token with precedence 5
            return new OperatorToken(currChar +""+ consumer.Consume(), Precedence.And, "left", consumer.Position);
        }

        // if the current and next character form "||"
        if (currChar == '|' && consumer.Peek() == '|') {

            // return a new operator token with precedence 5
            return new OperatorToken(currChar +""+ consumer.Consume(), Precedence.Or, "left", consumer.Position);
        }

        throw new Exception();
    }
}