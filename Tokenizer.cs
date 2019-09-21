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
        get => input.Position;
    }

    private StringConsumer input;

    public Tokenizer(StringConsumer stringConsumer)
    {
        this.input = new StringConsumer(stringConsumer);

        reconsumeQueue = new Queue<Token>();

        current = new Token('\0', TokenKind.delim, stringConsumer.Position);
    }

    public Tokenizer(IConsumer<Token> tokenConsumer) : this(new StringConsumer("")) {
        reconsumeQueue = new Queue<Token>();

        while (tokenConsumer.Consume(out _)) {
            reconsumeQueue.Enqueue(tokenConsumer.Current);
        }
    }

    public Tokenizer(Tokenizer tokenizer) : this(tokenizer.input) {
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

    public Token Peek()
        => new Tokenizer(this).Consume();

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

        // If we are instructed to reconsume the last token, hen dequeue a token from the reconsumeQueue and return it
        if (reconsumeQueue.Count != 0) {
            return reconsumeQueue.Dequeue();
        }

        // Consume a character from the LinesConsumer object
        var currChar = input.Consume();

        // While there is whitespace, consume it
        while (Char.IsWhiteSpace(currChar)) {
            currChar = input.Consume();
        }

        // if you want to preserve whitespace, you could do an if before the while loop and then return a whitespace token
        // although, you'll also need to modify the parser (in particular Parser.ConsumeValue or Parser.ToPostFixNotation)
        // because it might not correctly detect function calls and a lot of other thing

        // If the character is U+0003 END OF TRANSMISSION, it means there is nothing left to consume. Return an EOF token
        if (currChar == '\u0003' || currChar == '\0') return new Token(currChar, TokenKind.EOF, input.Position);

        // If the character is a digit
        if (Char.IsDigit(currChar)) {

            // Reconsume the current character
            input.Reconsume();

            // Consume a number token
            current = ConsumeNumberToken();

            // And return it
            return current;
        }

        // If the character is '+', '-', or '.', followed by a digit
        if (currChar == '.') {

            if (Char.IsDigit(input.Peek())) {
                // Reconsume the current character
                input.Reconsume();

                // Consume a number token
                current = ConsumeNumberToken();

                return current;
            }

            return new OperatorToken(currChar, Precedence.Access, "left", input.Position);
        }

        if (currChar == '$') {
            if (input.Peek() != '\'' && input.Peek() != '"') return new Token(currChar, TokenKind.delim, input.Position);

            current = ConsumeSpecialStringToken(input.Consume());

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
            input.Reconsume();

            // Consume an identifier token and return it
            current = ConsumeIdentLike();

            return current;
        }

        // If the character is '+' or '-'
        if (currChar == '+' || currChar == '-') {

            if (Char.IsDigit(input.Peek())) {
                input.Reconsume();

                current = ConsumeNumberToken();

                return current;
            }

            // if the next character is the same as now ('+' and '+' for example), then it is either an increment or a decrement ("++" and "--")
            if (input.Peek() == currChar) {

                // return a new operator token with precedence 7
                current = new OperatorToken(currChar +""+ input.Consume(), Precedence.Unary, "right", input.Position);

                return current;
            }

            // return a new operator token with precedence 2
            current = new OperatorToken(currChar, Precedence.Addition, "left", input.Position);

            return current;
        }

        // If the character is '*'
        if (currChar == '*') {

            // return a new operator token with precedence Multiplication
            current = new OperatorToken(currChar, Precedence.Multiplication, "left", input.Position);

            return current;
        }

        // if the character is '/'
        if (currChar == '/') {

            // and the next character is '*', then it is a limited comment
            if (input.Peek() == '*') {

                // consume characters until the two next characters form "*/"
                while (String.Join("", input.Peek(2)) != "*/") { input.Consume(); }

                // consume the "*/" characters
                input.Consume(); // should be '*'
                input.Consume(); // should be '/'

                // consume a token and return it
                return Consume();
            }

            // return a new operator token with precedence Division
            current = new OperatorToken(currChar, Precedence.Division, "left", input.Position);

            return current;
        }

        // if the character is '^'
        if (currChar == '^') {

            // return a new operator token with precedence 4
            current = new OperatorToken(currChar, Precedence.Power, "right", input.Position);

            return current;
        }

        // if the character is '!'
        if (currChar == '!') {

            if (input.Peek() == '=') {
                current = new OperatorToken(currChar +""+ input.Consume(), Precedence.NotEqual, "left", input.Position);

                return current;
            }

            // return a new operator token with precedence 5
            current = new OperatorToken(currChar, Precedence.Unary, "right", input.Position);

            return current;
        }

        // if the current and next character form "&&"
        if (currChar == '&' && input.Peek() == '&') {

            // return a new operator token with precedence 5
            current = new OperatorToken(currChar +""+ input.Consume(), Precedence.LogicalAND, "left", input.Position);

            return current;
        }

        // if the current and next character form "||"
        if (currChar == '|' && input.Peek() == '|') {

            // return a new operator token with precedence 5
            current = new OperatorToken(currChar +""+ input.Consume(), Precedence.LogicalOR, "left", input.Position);

            return current;
        }

        // if the current and next character for "=="
        if (currChar == '=') {

            if (input.Peek() == '=') {
                current = new OperatorToken(currChar +""+ input.Consume(), Precedence.Equal, "left", input.Position);

                return current;
            }

            current = new OperatorToken(currChar, Precedence.Assignment, "left", input.Position);

            return current;
        }

        if (currChar == '>') {

            if (input.Peek() == '=') {
                current = new OperatorToken(currChar +""+ input.Consume(), Precedence.GreaterThanOrEqual, "left", input.Position);

                return current;
            }

            current = new OperatorToken(currChar, Precedence.GreaterThan, "left", input.Position);

            return current;
        }

        if (currChar == '<') {

            if (input.Peek() == '=') {
                current = new OperatorToken(currChar +""+ input.Consume(), Precedence.LessThanOrEqual, "left", input.Position);

                return current;
            }

            current = new OperatorToken(currChar, Precedence.LessThan, "left", input.Position);

            return current;
        }

        // return a new delim token
        current = new Token(currChar, TokenKind.delim, input.Position);

        return current;
    }

    protected Token ConsumeIdentLike() {

        // consume a name
        var name = ConsumeName();

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
            return new ComplexToken(name, TokenKind.keyword, name.Location);
        }

        return name;
    }

    protected ComplexToken ConsumeName() {

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
        // since our while loop has exited) to make sure it is processed by the tokenizer
        input.Reconsume();

        // return the output token
        return output;
    }

    protected ComplexToken ConsumeStringToken(char endingDelimiter) {

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

    protected ComplexStringToken ConsumeSpecialStringToken(char endingDelimiter) {

        //consume a character
        var currChar = input.Consume();

        // the output token
        var output = new ComplexStringToken("", new List<Token[]>(), input.Position);

        // while the current character is not the ending delimiter
        while (currChar != endingDelimiter) {

            if (currChar == '{') {
                var tokenList = new List<Token>();
                Token currToken;
                while ((currToken = Consume())!= "}") {
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

    protected NumberToken ConsumeNumberToken() {

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