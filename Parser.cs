using System;
using System.Linq;
using System.Collections.Generic;

public class Parser : IConsumer<StatementNode>
{
    private readonly Queue<StatementNode> reconsumeQueue;

    public IConsumer<Token> Tokenizer { get; }

    public LocationRange Position {
        get => Current.Token.Location;
    }

    /// <summary>
    /// Gets the last StatementNode object consumed by this instance.
    /// </summary>
    /// <value>The last StatementNode consumed.</value>
    public StatementNode Current { get; protected set; }

    public StatementNode Default {
        get {
            var output = StatementNode.NULL;

            output.Location = Position;

            return output;
        }
    }

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
            Logger.Warning(new InvalidCallException(
                message : "Something tried to create a new Parser with a null grammar."
                        + "That's not allowed, and might throw in future versions, but for now the grammar will just be empty...",
                range: Position
            ));

            grammar = new ReadOnlyGrammar();
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
    public bool Consume(out StatementNode result) {
        result = Consume(); // consume a StatementNode

        return result != Default;
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
            throw Logger.Fatal(new InternalErrorException(
                message: "The parser's tokenizer was null. Something went seriously wrong",
                range: Position
            ));
        }

        // Consume a token
        var currToken = Tokenizer.Consume();

        // if the token is EOF, return ValueNode.NULL
        if (currToken == Tokenizer.Default || currToken == "\u0003") {
            return (Current = Default);
        }

        var statementKind = Grammar.GetStatementKind(currToken);

        if (statementKind != StatementKind.NotAStatement) {
            Current = Grammar.GetStatementParselet(statementKind).Parse(this, currToken);
        } else {
            Tokenizer.Reconsume();
            Current = ConsumeValue();
        }

        //while (Tokenizer.Peek() == ";") Tokenizer.Consume(); // advance tokenizer if we have a semicolon (we don't care about them)

        return Current;
    }

    public ValueNode ConsumeValue(Precedence precedence = 0) {
        var token = Tokenizer.Consume();

        if (!Grammar.IsPrefix(Grammar.GetExpressionKind(token))) {

            if (token.Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFException(
                    message: "Encountered an EOF where a value was expected",
                    range: Position
                ));
            } else if (token != ";") {
                Logger.Error(new UnexpectedTokenException(
                    token: token,
                    expected: new[] {
                        TokenKind.@bool,
                        TokenKind.@operator,
                        TokenKind.@string,
                        TokenKind.complexString,
                        TokenKind.ident,
                        TokenKind.number
                    }
                ));
            }

            return new ValueNode(token, Position, false);
        }

        var left = Grammar.GetPrefixParselet(token).Parse(this, token);

        token = Tokenizer.Consume();

        // it might be null because the complex-string parsing algorithm uses a Consumer<T>,
        // not a tokenizer, and it returns null when there's no more to consume
        // FIXME: This
        if (token == null) {
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

        //left.IsValid = isValid;

        return left;
    }

    public SimpleBlock ConsumeSimpleBlock(bool areOneLinersAllowed = true) {
        var isValid = true;

        // to consume a one-liner, you just consume a statement and return
        if (areOneLinersAllowed && Tokenizer.Peek() != "{") {
            if (!Consume(out StatementNode statement)) {
                Logger.Error(new UnexpectedEOFException(
                    context: "in simple block",
                    expected: "a statement",
                    range: Tokenizer.Current.Location
                ));

                isValid = false;
            }

            return new SimpleBlock(statement, statement.Token.Location, isValid);
        }

        var openingBracket = Tokenizer.Consume();

        // we don't have to check for EOF because that is (sorta) handled by "areOneLinersAllowed"
        if (openingBracket != "{") {
            Logger.Error(new UnexpectedTokenException(
                token: openingBracket,
                context: "at the start of simple block (this probably means there was an internal error, please report this!)",
                expected: "{"
            ));

            Tokenizer.Reconsume();
        }

        var location = openingBracket.Location;

        var statements = new List<StatementNode>();

        while (Tokenizer.Peek() != "}") {
            statements.Add(Consume());

            if (Tokenizer.Peek().Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFException(
                    context: "in simple block",
                    expected: "a statement",
                    range: Tokenizer.Current.Location
                ));

                isValid = false;

                break;
            }

            //if (Tokenizer.Peek() == ";") Tokenizer.Consume();
        }

        var closingBracket = Tokenizer.Peek();

        if (closingBracket.Kind == TokenKind.EOF) {
            Logger.Error(new UnexpectedEOFException(
                context: "in simple block",
                expected: "the character '}'",
                range: Tokenizer.Position
            ));

            isValid = false;
        } else if (closingBracket != "}") {
            Logger.Error(new UnexpectedTokenException(
                token: closingBracket,
                context: "in simple block",
                expected: "the character '}'"
            ));

            isValid = false;

            Tokenizer.Reconsume();
        }

        Tokenizer.Consume();

        return new SimpleBlock(statements.ToArray(), location, openingBracket, closingBracket, isValid);
    }

    public TupleNode ConsumeCommaSeparatedValueList(string start, string end, int expectedItemCount = -1) {
        var startingDelimiter = Tokenizer.Consume();

        var isValid = true;

        if (startingDelimiter.Representation != start) {
            Logger.Warning(new UnexpectedTokenException( // should we use InvalidCallException ?
                token: startingDelimiter,
                context: "in comma-separated list",
                expected: start
            ));
        }

        var items = new List<ValueNode>();

        var itemCount = 0;

        while (Tokenizer.Peek() != end) {

            items.Add(ConsumeValue());

            ++itemCount;

            if (Tokenizer.Consume() != ",") {

                if (Tokenizer.Current == end) {
                    Tokenizer.Reconsume();

                    break;
                }

                var lastItem = items.Last();

                if (Tokenizer.Current.Kind == TokenKind.keyword) {
                    Tokenizer.Reconsume();

                    isValid = false;

                    break;
                }

                if (!isValid || !lastItem.IsValid) {
                    continue;
                }

                Logger.Error(new UnexpectedTokenException(
                    message: "Did you forget a parenthesis or a comma in this tuple ? Expected '(' or ','",
                    token: Tokenizer.Current
                ));

                isValid = false;

                Tokenizer.Reconsume();

                continue;
            }

            // since we know that there's a comma right before, if there's an ending delimiter right after it,
            // it's an error.
            // Example : (hello, there, friend,) // right here
            //           ----------------------^--------------
            //                      litterally right there
            if (Tokenizer.Peek() == end) {
                Logger.Error(new UnexpectedTokenException(
                    token: Tokenizer.Consume(),
                    context: "in comma-separated list",
                    expected: "value"
                ));

                isValid = false;

                break;
            }
        }

        var endingToken = Tokenizer.Consume();

        if (isValid && endingToken != end) { // we probably got an EOF
            Logger.Error(new UnexpectedEOFException(
                context: "in a comma-separated value list",
                expected: "a closing parenthesis ')'",
                range: Tokenizer.Position
            ));

            isValid = false;
        }

        if (isValid && expectedItemCount != -1 && itemCount != expectedItemCount) {

            if (itemCount > expectedItemCount) {
                Logger.Error(new LotusException(
                    message: "There was too many values in a comma-separated value list. Expected "
                           + expectedItemCount + ", but got " + itemCount,
                    range: items.Last()?.Token.Location ?? Tokenizer.Position
                ));
            } else {
                Logger.Error(new LotusException(
                    message: "There wasn't enough values in a comma-separated value list. Expected "
                           + expectedItemCount + ", but got " + itemCount,
                    range: items.Last()?.Token.Location ?? Tokenizer.Position
                ));
            }

            isValid = false;
        }

        return new TupleNode(items, startingDelimiter, endingToken, isValid);
    }
}