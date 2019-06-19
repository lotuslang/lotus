using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;


public class Tokenizer : IConsumer<Token>
{
    private bool reconsumeLastToken;

    protected Token current;

    public Token Current {
        get => current;
    }

    protected int position;

    public int Position {
        get => position;
    }

    protected StringConsumer consumer;

    public Tokenizer(StringConsumer consumer)
    {
        this.consumer = consumer;
    }

    public Tokenizer(IEnumerable<char> collection) {
        consumer = new StringConsumer(collection);
    }

    public Tokenizer(IEnumerable<string> collection) {
        consumer = new StringConsumer(collection);
    }

    public void Reconsume() => reconsumeLastToken = true;

    public Token Consume(out bool success) {
        var token = Consume();

        success = token != TokenKind.EOF;

        return token;
    }

    public Token Consume() {

        // If we are instructed to reconsume the last token, then return the last token consumed
        if (reconsumeLastToken) {
            reconsumeLastToken = false;
            return current;
        }

        // Updates the position
        position++;

        // Consume a character from the LinesConsumer object
        var currChar = consumer.Consume();

        // While there is whitespace, consume it
        while (Char.IsWhiteSpace(currChar)) {
            currChar = consumer.Consume();
        }

        // if you want to preserve whitespace, you could do an if before the while loop and then return a whitespace token

        // If the character is U+0003 END OF TRANSMISSION, it means there is nothing left to consume. Return an EOF token
        if (currChar == '\u0003' || currChar == '\0') return new Token(currChar, TokenKind.EOF, consumer.Position);

        // If the chacracter is a digit
        if (Char.IsDigit(currChar))
        {
            // Reconsume the current character
            consumer.Reconsume();

            // Consume a number token
            current = ConsumeNumberToken();

            // And return it
            return current;
        }

        // If the character is '+' or '-' and the last token consumed was a number,
        // then it's probably an operation without whitespace (which is annoying).
        if ((currChar == '+' || currChar == '-') && current is NumberToken) {

            // return a new operator token with precedence 2
            current = new OperatorToken(currChar, 2, true, consumer.Position);

            return current;
        }

        // If the character is '+', '-', or '.', followed by a digit
        if ((currChar == '+'
        ||  currChar == '-'
        ||  currChar == '.') && Char.IsDigit(consumer.Peek()))
        {
            // Reconsume the current character
            consumer.Reconsume();

            // Consume a number token
            current = ConsumeNumberToken();

            return current;
        }

        // If the character is a single or double quote
        if (currChar == '\'' || currChar == '"') {
            // Consume a string token and return it
            current = ConsumeStringToken(currChar);

            return current;
        }

        // If the character is a letter or a low line
        if (Char.IsLetter(currChar) || currChar == '_') {

            // Reconsume the current character
            consumer.Reconsume();

            // Consume an identifier token and return it
            current = ConsumeIdentToken();

            return current;
        }

        // If the character is '+' or '-'
        if (currChar == '+' || currChar == '-') {

            // return a new operator token with precedence 2
            current = new OperatorToken(currChar, 2, true, consumer.Position);

            return current;
        }

        // If the character is '*' or '/'
        if (currChar == '*' || currChar == '/') {

            // return a new operator token with precedence 3
            current = new OperatorToken(currChar, 3, true, consumer.Position);

            return current;
        }

        // if the character is '^'
        if (currChar == '^') {

            // return a new operator token with precedence 4
            current = new OperatorToken(currChar, 4, false, consumer.Position);

            return current;
        }

        // return a new delim token
        current = new Token(currChar, TokenKind.delim, consumer.Position);

        return current;
    }

    protected ComplexToken ConsumeIdentToken() {

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
        // since our while loop has exited) to make sure it is processed by the tokenizer
        consumer.Reconsume();

        // return the output token
        return output;
    }

    protected ComplexToken ConsumeStringToken(char endingDelimiter) {

        // consume a character
        var currChar = consumer.Consume();

        // the output token
        var output = new ComplexToken("", TokenKind.@string, consumer.Position);

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            // add it to the value of output
            output.Add(currChar);

            // consume a character
            currChar = consumer.Consume();
        }

        // return the output token
        return output;
    }

    protected NumberToken ConsumeNumberToken() {

        // consume a character
        var currChar = consumer.Consume();

        // the output token
        var output = new NumberToken("", consumer.Position);

        // if the character is a '+' or '-'
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