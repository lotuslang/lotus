using System;
using System.Linq;
using System.Collections.Generic;

public abstract class Parser : IConsumer<StatementNode>
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

    /// <summary>
    /// Contrary to <see cref="Parser.Default"/>, this variable is constant, and just returns <see cref="StatementNode.NULL"/>
    /// </summary>
    public static readonly StatementNode ConstantDefault = StatementNode.NULL;

    /// <summary>
    /// Returns the value of <see cref="Parser.ConstantDefault"/> BUT adjusted for the current position. <br/>
    /// Most of the time, this is the variable you want, because when comparing nodes, position is important,
    /// and the parser will always return a node with relevant position, even if it is EOF and other things that
    /// prompt for the use of <see cref="StatementNode.NULL"/>
    /// </summary>
    /// <value></value>
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
                message: "Something tried to create a new Parser with a null grammar."
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

    public Parser(Parser parser) : this(parser.Grammar) {
        reconsumeQueue = parser.reconsumeQueue;
        Tokenizer = parser.Tokenizer;
        Current = parser.Current;
    }

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

    public abstract StatementNode Peek();

    public abstract StatementNode[] Peek(int n);

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
    public virtual StatementNode Consume() {

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

        return Default;
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
}