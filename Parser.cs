using System;
using System.Linq;
using System.Collections.Generic;

public class Parser : IConsumer<StatementNode>
{
    private Queue<StatementNode> reconsumeQueue;

    private IConsumer<Token> tokenizer;

    public IConsumer<Token> Tokenizer {
        get => tokenizer;
    }

    public Location Position {
        get => tokenizer != null ? tokenizer.Position : default;
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
        tokenizer = tokenConsumer;
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
        reconsumeQueue = new Queue<StatementNode>(parser.reconsumeQueue);
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

        if (tokenizer == null) {
            Console.WriteLine("wtf the tokenizer is null ?????");
            return null;
        }

        // Consume a token
        var currToken = tokenizer.Consume();

        // if the token is EOF, return ValueNode.NULL
        if (currToken == "\u0003" || currToken == "\0") return null;

        // if the token is ';', return the next statement node
        if (currToken == ";") return Consume();

        // if the token is "var"
        switch (currToken)
        {
            case "var":
                current = new DeclarationParselet().Parse(this, currToken);
                break;
            case "new":
                current = new ObjectCreationParselet().Parse(this, currToken);
                break;
            case "def":
                current = new FunctionDeclarationParselet().Parse(this, currToken);
                break;
            case "return":
                current = new ReturnParselet().Parse(this, currToken);
                break;
            case "from":
                current = new ImportParselet().Parse(this, currToken);
                break;
            default:
                tokenizer.Reconsume();
                current = ConsumeValue();
                break;
        }

        return current;
    }

    public ValueNode ConsumeValue(Precedence precedence = 0) {
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

        var left = Utilities.GetPrefixParselet(token).Parse(this, token);

        if (tokenizer.Peek() == null) return left as ValueNode;

        while (precedence < GetPrecedence(tokenizer.Peek())) {
            token = tokenizer.Consume();

            left = Utilities.GetOperatorParselet(token).Parse(this, token, left);
        }

        return left as ValueNode;
    }

    public SimpleBlock ConsumeSimpleBlock() {

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

    public ValueNode[] ConsumeCommaSeparatedList(string start, string end) {
        var startingDelimiter = tokenizer.Consume();

        var items = new List<ValueNode>();

        if (startingDelimiter.Representation != start) {
            throw new UnexpectedTokenException(startingDelimiter, "in comma-separated list", start);
        }

        while (tokenizer.Peek() != end) {
            items.Add(ConsumeValue());

            if (!tokenizer.Consume(out Token currToken) || currToken == end) {
                break;
            }

            if (currToken != ",") {
                throw new UnexpectedTokenException(currToken, "in comma-separated list", ",");
            }
        }

        return items.ToArray();
    }

    private Precedence GetPrecedence(ExpressionKind kind) {
        if (kind.IsOperatorParselet()) {
            return Utilities.GetOperatorParselet(kind).Precedence;
        }

        return 0;
    }

    private Precedence GetPrecedence(Token token)
        => GetPrecedence(token.GetExpressionKind());
}