using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class Parser : IConsumer<StatementNode>
{
    private readonly Queue<StatementNode> reconsumeQueue;

    public IConsumer<Token> Tokenizer { get; }

    public Location Position {
        get => Current.Token.Location;
    }

    /// <summary>
    /// Gets the last StatementNode object consumed by this instance.
    /// </summary>
    /// <value>The last StatementNode consumed.</value>
    public StatementNode Current { get; protected set; }

    public ReadOnlyGrammar Grammar { get; protected set; }

#pragma warning disable CS8618
    protected Parser() {
        reconsumeQueue = new Queue<StatementNode>();

        Current = StatementNode.NULL;

        Grammar = new ReadOnlyGrammar();
    }
#pragma warning restore

    protected Parser(ReadOnlyGrammar grammar) : this() {
        if (grammar is null) {
            throw new ArgumentNullException(nameof(grammar));
        }

        Grammar = grammar;
    }

    //public Parser(Tokenizer tokenizer) : this(tokenizer as IConsumer<Token>) { }

    public Parser(IConsumer<Token> tokenConsumer, ReadOnlyGrammar grammar) : this(grammar) {
        Tokenizer = tokenConsumer;
        reconsumeQueue = new Queue<StatementNode>();
    }

    public Parser(IConsumer<StatementNode> nodeConsumer, ReadOnlyGrammar grammar) : this(grammar) {
        while (nodeConsumer.Consume(out StatementNode? node)) {
            reconsumeQueue.Enqueue(node);
        }
    }

    public Parser(StringConsumer consumer, ReadOnlyGrammar grammar) : this(new Tokenizer(consumer, grammar), grammar) { }

    public Parser(IEnumerable<char> collection, ReadOnlyGrammar grammar) : this(new Tokenizer(collection, grammar), grammar) { }

    public Parser(System.IO.FileInfo file, ReadOnlyGrammar grammar) : this(new Tokenizer(file, grammar), grammar) { }

    public Parser(Parser parser) : this(parser, parser.Grammar)
    { }

    public Parser(Parser parser, ReadOnlyGrammar grammar) : this(parser.Tokenizer, grammar) {
        reconsumeQueue = new Queue<StatementNode>(parser.reconsumeQueue);

        Current = parser.Current;
    }

    /// <summary>
    /// Reconsumes the last StatementNode object.
    /// </summary>
    public void Reconsume() {
        if (reconsumeQueue.TryPeek(out StatementNode? node) && Object.ReferenceEquals(node, Current)) return;
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
    public bool Consume([MaybeNullWhen(false)] out StatementNode result) {
        result = Consume(); // consume a StatementNode

        return result != StatementNode.NULL;
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

        if (Tokenizer == null) {
            throw new InternalErrorException(message: "The parser's tokenizer was null. Something went seriously wrong");
        }

        // Consume a token
        var currToken = Tokenizer.Consume();

        // if the token is EOF, return ValueNode.NULL
        if (currToken == "\u0003" || currToken == "\0") return StatementNode.NULL;

        var statementKind = Grammar.GetStatementKind(currToken);

        if (statementKind != StatementKind.NotAStatement) {
            Current = Grammar.GetStatementParselet(statementKind).Parse(this, currToken);
        } else {
            Tokenizer.Reconsume();
            Current = ConsumeValue();
        }

        return Current;
    }

    public ValueNode ConsumeValue(Precedence precedence = 0) {
        var token = Tokenizer.Consume();

        if (!Grammar.IsPrefix(Grammar.GetExpressionKind(token))) {
            throw new UnexpectedTokenException(token,
                TokenKind.@bool,
                TokenKind.@operator,
                TokenKind.@string,
                TokenKind.complexString,
                TokenKind.ident,
                TokenKind.number
            );
        }

        var left = Grammar.GetPrefixParselet(token).Parse(this, token);

        token = Tokenizer.Consume();

        // it might be null because the complex-string parsing algorithm uses a Consumer<T>,
        // not a tokenizer, and it returns null when there's no more to consume
        // FIXME: This
        if (token == null || token == ";") {
            return left;
        }

        if (Grammar.IsPostfix(Grammar.GetExpressionKind(token))) {
            left = Grammar.GetPostfixParselet(token).Parse(this, token, left);

            token = Tokenizer.Consume();
        }

        while (precedence < Grammar.GetPrecedence(token)) {
            left = Grammar.GetOperatorParselet(token).Parse(this, token, left);

            token = Tokenizer.Consume();
        }

        Tokenizer.Reconsume();

        return left;
    }

    public SimpleBlock ConsumeSimpleBlock(bool areOneLinersAllowed = true) {

        // to consume a one-liner, you just consume a statement and return
        if (areOneLinersAllowed && Tokenizer.Peek() != "{") {
            return new SimpleBlock(new[] { Consume() });
        }

        var bracket = Tokenizer.Consume();

        if (bracket != "{") throw new UnexpectedTokenException(bracket, "at the start of simple block", "{");

        var statements = new List<StatementNode>();

        while (Tokenizer.Peek() != "}") {
            statements.Add(Consume());

            if (Tokenizer.Peek() == TokenKind.EOF) {
                throw new UnexpectedTokenException(
                    Tokenizer.Consume(),
                    "in simple block",
                    "a statement"
                    // Extendable version with TokenKind :
                    // Enum.GetValues(typeof(TokenKind)).Cast<TokenKind>().Where(value => value != TokenKind.EOF).ToArray()
                );
            }

            if (Tokenizer.Peek() == ";") Tokenizer.Consume();
        }

        bracket = Tokenizer.Consume();

        if (bracket == ";") bracket = Tokenizer.Consume();

        if (bracket != "}") {
            throw new UnexpectedTokenException(bracket, "in simple block", "}");
        }

        return new SimpleBlock(statements.ToArray());
    }

    public ValueNode[] ConsumeCommaSeparatedValueList(string start, string end, int maxItemCount = -1) {
        var startingDelimiter = Tokenizer.Consume();

        if (startingDelimiter.Representation != start) {
            throw new UnexpectedTokenException(startingDelimiter, "in comma-separated list", start);
        }

        var items = new List<ValueNode>();

        var itemCount = 0;

        while (itemCount++ != maxItemCount && Tokenizer.Peek() != end) {

            items.Add(ConsumeValue());

            if (Tokenizer.Consume() != ",") {

                if (Tokenizer.Current == end) {
                    Tokenizer.Reconsume();
                    break;
                }

                throw new UnexpectedTokenException(Tokenizer.Current, "in comma-separated list", ",");
            }

            if (Tokenizer.Peek() == end) {
                throw new UnexpectedTokenException(Tokenizer.Consume(), "in comma-separated list", "value");
            }
        }

        if (Tokenizer.Consume() != end) {
            throw new Exception();
        }

        if (maxItemCount != -1 && itemCount != maxItemCount) {
            throw new InternalErrorException(
                "while parsing a comma-separated value list delimited by `" + start + "` and `" + end + "`",
                "because there was too many values. Expected " + maxItemCount + ", but got only " + itemCount,
                Position
            );
        }

        return items.ToArray();
    }
}