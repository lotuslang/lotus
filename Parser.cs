using System;
using System.Linq;
using System.Collections.Generic;

public class Parser : IConsumer<StatementNode>
{
    private Queue<StatementNode> reconsumeQueue;

    private IConsumer<Token> tokenizer;

    internal IConsumer<Token> Tokenizer {
        get => tokenizer;
    }

    public Location Position {
        get => tokenizer != null ? tokenizer.Position : default(Location);
    }

    protected StatementNode current;

    /// <summary>
    /// Gets the last StatementNode object consumed by this instance.
    /// </summary>
    /// <value>The last StatementNode consumed.</value>
    public StatementNode Current {
        get => current;
    }

    public Parser(Tokenizer tokenizer) {
        this.tokenizer = new Tokenizer(tokenizer);
        reconsumeQueue = new Queue<StatementNode>();
    }

    public Parser(IConsumer<Token> tokenConsumer) {
        this.tokenizer = tokenConsumer;
        reconsumeQueue = new Queue<StatementNode>();
    }

    public Parser(IConsumer<StatementNode> nodeConsumer) {
        reconsumeQueue = new Queue<StatementNode>();

        while (nodeConsumer.Consume(out StatementNode node)) {
            reconsumeQueue.Enqueue(node);
        }
    }

    public Parser(StringConsumer consumer) : this(new Tokenizer(consumer)) { }

    public Parser(IEnumerable<char> collection) : this(new Tokenizer(collection)) { }

    public Parser(System.IO.FileInfo file) : this(new Tokenizer(file)) { }

    public Parser(Parser parser) : this(parser.Tokenizer) {
        this.reconsumeQueue = new Queue<StatementNode>(parser.reconsumeQueue);
    }

    /// <summary>
    /// Reconsumes the last StatementNode object.
    /// </summary>
    public void Reconsume() {
        if (reconsumeQueue.TryPeek(out StatementNode node) && Object.ReferenceEquals(node, current)) return;
    }

    public StatementNode Peek()
        => new Parser(this).Consume();

    public StatementNode[] Peek(int n) {
        var parser = new Parser(this);

        var output = new List<StatementNode>();

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.ToArray();
    }

    /// <summary>
    /// Consumes a StatementNode object and returns it.
    /// </summary>
    /// <param name="success">True if the operation succeeded, false otherwise.</param>
    /// <returns>The StatementNode object consumed.</returns>
    public bool Consume(out StatementNode result) {
        result = Consume(); // consume a StatementNode

        return result != null;
    }

    /// <summary>
    /// Consume a StatementNode object and returns it.
    /// </summary>
    /// <returns>The StatementNode object consumed.</returns>
    public StatementNode Consume() {

        // If we are instructed to reconsume the last node, then dequeue a node from the reconsumeQueue and return it
        if (reconsumeQueue.Count != 0) {
            return reconsumeQueue.Dequeue();
        }

        if (tokenizer == null) return null;

        // Consume a token
        var currToken = tokenizer.Consume();

        // if the token is EOF, return ValueNode.NULL
        if (currToken == TokenKind.EOF) return null;

        // if the token is ';', return the next statement node
        if (currToken == ";") return Consume();

        // if the token is "var"
        if (currToken == "var") {

            // reconsume it
            tokenizer.Reconsume();

            // consume a declaration and return it
            current = ConsumeDeclaration();

            return current;
        }

        // if the token is "new"
        if (currToken == "new") {

            // reconsume it
            tokenizer.Reconsume();

            // consume a value and return it (since an object instantiation is just a fancy function call, and function calls can be parsed as values)
            current = ConsumeValue();

            return current;
        }

        // if the token is "def"
        if (currToken == "def") {

            // reconsume it
            tokenizer.Reconsume();

            // consume a function declaration and return it
            current = ConsumeFunctionDeclaration();

            return current;
        }

        if (currToken == "return") {

            // reconsume it
            tokenizer.Reconsume();

            current = ConsumeReturn();

            return current;
        }

        tokenizer.Reconsume();

        current = ConsumeValue();

        return current;
    }

    protected DeclarationNode ConsumeDeclaration() {

        // consume the keyword "var" token
        var varKeyword = tokenizer.Consume();

        // if the token isn't the keyword "var", throw an exception
        if (varKeyword != "var") throw new UnexpectedTokenException(varKeyword, "in declaration", "var");

        // consume the name of the variable we're declaring
        var name = tokenizer.Consume();

        // if the token isn't an identifier, throw an exception
        if (name != TokenKind.ident) throw new UnexpectedTokenException(name, TokenKind.ident);

        // consume a token
        var equalSign = tokenizer.Consume();

        // if this token wasn't an equal sign, throw an exception
        if (equalSign != "=") throw new UnexpectedTokenException(equalSign, "=");

        // consume a ValueNode (which is the value of the variable we're declaring)
        var value = ConsumeValue();

        // return that value
        return new DeclarationNode(value, name as ComplexToken);
    }

    protected FunctionDeclarationNode ConsumeFunctionDeclaration() {

        // consume the keyword "def" token
        var defKeyword = tokenizer.Consume();

        // if the token consumed was not "def", then throw an exception
        if (defKeyword != "def") throw new UnexpectedTokenException("A function definition must start with the keyword 'def'", defKeyword);

        // consume the name of the function
        var funcName = tokenizer.Consume();

        // if the token consumed was not an identifier, then throw an exception
        if (funcName != TokenKind.ident) throw new UnexpectedTokenException(funcName, "in function declaration", TokenKind.ident);

        // the list of the parameters the function has
        var parameters = new List<ComplexToken>();

        // consume a token
        var parenthesis = tokenizer.Consume();

        // if it wasn't an open parenthesis, throw an exception
        if (parenthesis != "(") throw new UnexpectedTokenException(parenthesis, "in function declaration", "(");

        // consume a token
        var paramName = tokenizer.Consume();

        // while this token is not a parenthesis
        while (paramName != ")") {

            // if the token is not an identifier, throw an exception
            if (paramName != TokenKind.ident) throw new UnexpectedTokenException(paramName, "in function argument declaration", TokenKind.ident);

            // add the parameter (as a ComplexToken) to the list of parameters
            parameters.Add(paramName as ComplexToken);

            // consume a token
            var comma = tokenizer.Consume();

            // if the token was a closing parenthesis, break out
            if (comma == ")") break;

            // if the token was not a comma, throw an exception
            if (comma != ",") throw new UnexpectedTokenException(comma, "in function argument declaration", ",");

            // assign paramName to a new token
            paramName = tokenizer.Consume();
        }

        // consume a simple block
        var block = ConsumeSimpleBlock();

        // return a new FunctionDeclarationNode with 'block' as the value, 'parameters' as the list of params, and funcName as the name
        return new FunctionDeclarationNode(block, parameters.ToArray(), funcName as ComplexToken);
    }

    protected SimpleBlock ConsumeSimpleBlock() {

        var bracket = tokenizer.Consume();

        if (bracket != "{") throw new UnexpectedTokenException(bracket, "at the start of simple block", "{");

        var statements = new List<StatementNode>();

        while (tokenizer.Peek() != "}" && tokenizer.Peek() != TokenKind.EOF) {
            statements.Add(Consume());
            if (tokenizer.Peek() == ";") tokenizer.Consume();
        }

        bracket = tokenizer.Consume();

        if (bracket == ";") bracket = tokenizer.Consume();

        if (bracket != "}") throw new Exception("what² (" + bracket.Representation + ")");

        return new SimpleBlock(statements.ToArray());
    }

    protected ReturnNode ConsumeReturn() {
        var returnToken = tokenizer.Consume();

        if (returnToken != "return") throw new UnexpectedTokenException(returnToken, "in return statement", "return");

        if (tokenizer.Peek() == ";") return new ReturnNode(ValueNode.NULL, returnToken as ComplexToken);

        var value = ConsumeValue();

        return new ReturnNode(value, returnToken as ComplexToken);
    }

    internal ValueNode ConsumeValue(Precedence precedence = 0) {
        var token = tokenizer.Consume();

        if (!token.GetExpressionKind().IsPrefixParselet()) {
            throw new UnexpectedTokenException(token,
                TokenKind.@bool,
                TokenKind.@operator,
                TokenKind.@string,
                TokenKind.complexString,
                TokenKind.ident,
                TokenKind.number
            );
        }

        var left = Constants.GetPrefixParselet(token).Parse(this, token);

        if (tokenizer.Peek() == null) return left as ValueNode;

        while (precedence < GetPrecedence(tokenizer.Peek())) {
            token = tokenizer.Consume();

            left = Constants.GetOperatorParselet(token).Parse(this, token, left);
        }

        if (tokenizer.Peek() != null && tokenizer.Peek() == ",") tokenizer.Consume();

        return left as ValueNode;
    }

    private Precedence GetPrecedence(ExpressionKind kind) {
        if (kind.IsOperatorParselet()) {
            return Constants.GetOperatorParselet(kind).Precedence;
        }

        return 0;
    }

    private Precedence GetPrecedence(Token token)
        => GetPrecedence(token.GetExpressionKind());
}