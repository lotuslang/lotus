using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;


public class Tokenizer : IConsumer<Token>
{

    private readonly List<Toklet> toklets;

    private Token current;

    public Token Current {
        get => current;
    }

    private Queue<Token> reconsumeQueue;

    public Location Position {
        get => input.Position;
    }

    private StringConsumer input;

    private Tokenizer() {
        reconsumeQueue = new Queue<Token>();

        current = new Token('\0', TokenKind.delim, default(Location));

        toklets = new List<Toklet> {
            new NumberToklet(),
            new ComplexStringToklet(),
            new StringToklet(),
            new IdentToklet(),
            new CommentToklet(),
            new OperatorToklet(),
            new Toklet(),
        };
    }

    public Tokenizer(StringConsumer stringConsumer) : this () {
        input = new StringConsumer(stringConsumer);
    }

    public Tokenizer(IConsumer<char> consumer) : this () {
        input = new StringConsumer(consumer);
    }

    public Tokenizer(IConsumer<Token> tokenConsumer) : this(new StringConsumer("")) {
        while (tokenConsumer.Consume(out _)) {
            reconsumeQueue.Enqueue(tokenConsumer.Current);
        }
    }

    public Tokenizer(Tokenizer tokenizer) : this(tokenizer.input) {
        reconsumeQueue = new Queue<Token>(tokenizer.reconsumeQueue);

        current = tokenizer.current;
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
        // although, you'll also need to modify the parser and parselets because they might not work correctly

        // If the character is U+0003 END OF TRANSMISSION, it means there is nothing left to consume. Return an EOF token
        if (currChar == '\u0003' || currChar == '\0') {
            current = new Token(currChar, TokenKind.EOF, input.Position);

            return current;
        }

        input.Reconsume();

        current = toklets.Find(toklet => toklet.Condition(new StringConsumer(input))).Consume(input, this);

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
        if (Utilities.keywords.Contains(name)) {

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

    protected ComplexStringToken ConsumeComplexStringToken(char endingDelimiter) {

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