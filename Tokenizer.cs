using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;


public class Tokenizer : IConsumer<Token>
{
    private Token current;

    public Token Current {
        get => current;
    }

    private Queue<Token> reconsumeQueue;

    public Location Position {
        get => consumer.Position;
    }

    private StringConsumer consumer;

    public Tokenizer(StringConsumer stringConsumer)
    {
        this.consumer = new StringConsumer(stringConsumer);

        reconsumeQueue = new Queue<Token>();

        current = new Token('\0', TokenKind.delim, stringConsumer.Position);
    }

    public Tokenizer(IConsumer<Token> tokenConsumer) : this(new StringConsumer("")) {
        reconsumeQueue = new Queue<Token>();

        while (tokenConsumer.Consume(out _)) {
            reconsumeQueue.Enqueue(tokenConsumer.Current);
        }
    }

    public Tokenizer(Tokenizer tokenizer) : this(tokenizer.consumer) {
        current = tokenizer.current;
        reconsumeQueue = new Queue<Token>(tokenizer.reconsumeQueue);
    }

    public Tokenizer(System.IO.FileInfo fileInfo) : this(new StringConsumer(fileInfo))
    { }

    public Tokenizer(IEnumerable<char> collection) : this(new StringConsumer(collection))
    { }

    public Tokenizer(IEnumerable<string> collection) : this(new StringConsumer(collection))
    { }

    public void Reconsume() {
        if (reconsumeQueue.TryPeek(out Token token) && Object.ReferenceEquals(token, current)) return;

        reconsumeQueue.Enqueue(current);
    }

    public Token Peek() => new Tokenizer(this).Consume();

    public Token[] Peek(int n) {
        // create a new (dee-copy of) tokenizer from this one
        var tokenizer = new Tokenizer(this);

        // the output list
        var output = new Token[n];

        // consume `n` tokens and add them to the output
        for (int i = 0; i < n; i++) {
            output[i] = tokenizer.Consume();
        }

        return output;
    }

    public bool Consume(out Token result) {
        result = Consume();

        return result != TokenKind.EOF;
    }

    public Token Consume() {

        // If we are instructed to reconsume the last token, then return the last token consumed
        if (reconsumeQueue.Count != 0) {
            return reconsumeQueue.Dequeue();
        }

        // Consume a character from the LinesConsumer object
        var currChar = consumer.Consume();

        // While there is whitespace, consume it
        while (Char.IsWhiteSpace(currChar)) {
            currChar = consumer.Consume();
        }

        // if you want to preserve whitespace, you could do an if before the while loop and then return a whitespace token
        // although, you'll also need to modify the parser (in particular Parser.ConsumeValue or Parser.ToPostFixNotation)
        // because it might not correctly detect function calls and a lot of other thing

        // If the character is U+0003 END OF TRANSMISSION, it means there is nothing left to consume. Return an EOF token
        if (currChar == '\u0003' || currChar == '\0') return new Token(currChar, TokenKind.EOF, consumer.Position);

        // If the character is a digit
        if (Char.IsDigit(currChar)) {
            // Reconsume the current character
            consumer.Reconsume();

            // Consume a number token
            current = ConsumeNumberToken();

            // And return it
            return current;
        }

        // If the character is '+', '-', or '.', followed by a digit
        if (currChar == '.' && Char.IsDigit(consumer.Peek())) {
            // Reconsume the current character
            consumer.Reconsume();

            // Consume a number token
            current = ConsumeNumberToken();

            return current;
        }

        if (currChar == '$') {
            if (consumer.Peek() != '\'' || consumer.Peek() != '"') return new Token(currChar, TokenKind.delim, consumer.Position);

            current = ConsumeSpecialStringToken(consumer.Consume());

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
            current = ConsumeIdentLike();

            return current;
        }

        // If the character is '+' or '-'
        if (currChar == '+' || currChar == '-') {

            // if the next character is the same as now ('+' and '+' for example), then it is either an increment or a decrement ("++" and "--")
            if (consumer.Peek() == currChar) {

                // return a new operator token with precedence 7
                current = new OperatorToken(currChar +""+ consumer.Consume(), Precedence.Unary, "right", consumer.Position);

                return current;
            }

            // return a new operator token with precedence 2
            current = new OperatorToken(currChar, Precedence.Addition, "left", consumer.Position);

            return current;
        }

        // If the character is '*'
        if (currChar == '*') {

            // return a new operator token with precedence Multiplication
            current = new OperatorToken(currChar, Precedence.Multiplication, "left", consumer.Position);

            return current;
        }

        // if the character is '/'
        if (currChar == '/') {

            // and the next character is '*', then it is a limited comment
            if (consumer.Peek() == '*') {

                // consume characters until the two next characters form "*/"
                while (String.Join("", consumer.Peek(2)) != "*/") { consumer.Consume(); }

                // consume the "*/" characters
                consumer.Consume(); // should be '*'
                consumer.Consume(); // should be '/'

                // consume a token and return it
                return Consume();
            }

            // return a new operator token with precedence Division
            current = new OperatorToken(currChar, Precedence.Division, "left", consumer.Position);

            return current;
        }

        // if the character is '^'
        if (currChar == '^') {

            // return a new operator token with precedence 4
            current = new OperatorToken(currChar, Precedence.Power, "right", consumer.Position);

            return current;
        }

        // if the character is '!'
        if (currChar == '!') {

            if (consumer.Peek() == '=') {
                current = new OperatorToken(currChar +""+ consumer.Consume(), Precedence.NotEqual, "right", consumer.Position);

                return current;
            }

            // return a new operator token with precedence 5
            current = new OperatorToken(currChar, Precedence.Unary, "right", consumer.Position);

            return current;
        }

        // if the current and next character form "&&"
        if (currChar == '&' && consumer.Peek() == '&') {

            // return a new operator token with precedence 5
            current = new OperatorToken(currChar +""+ consumer.Consume(), Precedence.LogicalAND, "left", consumer.Position);

            return current;
        }

        // if the current and next character form "||"
        if (currChar == '|' && consumer.Peek() == '|') {

            // return a new operator token with precedence 5
            current = new OperatorToken(currChar +""+ consumer.Consume(), Precedence.LogicalOR, "left", consumer.Position);

            return current;
        }

        // if the current and next character for "=="
        if (currChar == '=') {

            if (consumer.Peek() == '=') {
                current = new OperatorToken(currChar +""+ consumer.Consume(), Precedence.Equal, "left", consumer.Position);

                return current;
            }

            current = new OperatorToken(currChar, Precedence.Assignment, "left", consumer.Position);

            return current;
        }

        if (currChar == '>') {

            if (consumer.Peek() == '=') {
                current = new OperatorToken(currChar +""+ consumer.Consume(), Precedence.GreaterThanOrEqual, "left", consumer.Position);

                return current;
            }

            current = new OperatorToken(currChar, Precedence.GreaterThan, "left", consumer.Position);

            return current;
        }

        if (currChar == '<') {

            if (consumer.Peek() == '=') {
                current = new OperatorToken(currChar +""+ consumer.Consume(), Precedence.LessThanOrEqual, "left", consumer.Position);

                return current;
            }

            current = new OperatorToken(currChar, Precedence.LessThan, "left", consumer.Position);

            return current;
        }

        // return a new delim token
        current = new Token(currChar, TokenKind.delim, consumer.Position);

        return current;
    }

    protected Token ConsumeIdentLike() {

        // consume a name
        var name = ConsumeName();

        // if the identifier is the keyword "new"
        if (name == "new") {

            // return a right-associative operator token with the precedence of a function call, since instantiation is the same as a call to .ctor()
            return new OperatorToken(name, Precedence.FuncCall, "right", name.Location);
        }

        // if the identifier is the keyword "def"
        if (name == "def") {

            // return a right-associative token with the precedence of a declaration
            return new OperatorToken(name, Precedence.Declaration, "right", name.Location);
        }

        // if the identifier is the keyword "var"
        if (name == "var") {

            //return a right-associative token with the precedence of a declaration
            return new OperatorToken(name, Precedence.Declaration, "right", name.Location);
        }

        // if the identifier is the keyword "true" or "false"
        if (name == "true" || name == "false") {

            // return a BoolToken with its value set to "true" if name was "true", and "false" if name was "false"
            return new BoolToken(name == "true", name.Location);
        }

        // if the identifier is a keyword
        if (Constants.keywords.Contains(name)) {

            // return the same token but with a kind of TokenKind.keyword
            return new ComplexToken(name, TokenKind.@string, name.Location);
        }

        return name;
    }

    protected ComplexToken ConsumeName() {

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

    protected ComplexToken ConsumeSpecialStringToken(char endingDelimiter) {

        //consume a character
        var currChar = consumer.Consume();

        // the output token
        var output = new ComplexToken("", TokenKind.complexString, consumer.Position);

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            if (currChar == '{') {
                var tokenList = new List<Token>();
                while (Consume() != "}") {

                }
            }
        }

        return null;
    }

    protected NumberToken ConsumeNumberToken() {

        // consume a character
        var currChar = consumer.Consume();

        // the output token
        var output = new NumberToken("", consumer.Position);

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