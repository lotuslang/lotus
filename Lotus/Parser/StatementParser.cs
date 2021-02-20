using System.IO;
using System.Linq;
using System.Collections.Generic;

public class StatementParser : Parser
{
    public ExpressionParser ExpressionParser { get; protected set; }

    public new StatementNode Current { get; protected set; }

    public StatementNode Default {
        get {
            var output = StatementNode.NULL;

            output.Location = Position;

            return output;
        }
    }

    protected void Init() {
        ExpressionParser = new ExpressionParser(this);
    }

#nullable disable
    public StatementParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, LotusGrammar.Instance) {
        Init();
    }

    public StatementParser(IConsumer<StatementNode> nodeConsumer) : base(LotusGrammar.Instance) {
        while (nodeConsumer.Consume(out StatementNode node)) {
            reconsumeQueue.Enqueue(node);
        }
        Init();
    }

    public StatementParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public StatementParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public StatementParser(FileInfo file) : this(new LotusTokenizer(file)) { }

    public StatementParser(Parser parser) : base(parser) {
        Init();
    }
#nullable enable

    public override StatementNode Peek()
        => new StatementParser(this).Consume();

    public override StatementNode[] Peek(int n) {
        var parser = new StatementParser(this);

        var output = new List<StatementNode>();

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.ToArray();
    }

    public bool Consume(out StatementNode result) {
        result = Consume(); // consume a StatementNode

        return result != Default;
    }

    public override StatementNode Consume() {
        base.Consume();

        // Consume a token
        var currToken = Tokenizer.Consume();

        // if the token is EOF, return StatementNode.NULL
        if (currToken == Tokenizer.Default || currToken == "\u0003") {
            return ((Current = Default) as StatementNode)!;
        }

        if (Grammar.TryGetStatementParslet(currToken, out var parslet)) {
            Current = parslet.Parse(this, currToken);
        } else {
            Tokenizer.Reconsume();
            Current = ExpressionParser.Consume();
        }

        return (Current as StatementNode)!;
    }

    public SimpleBlock ConsumeSimpleBlock(bool areOneLinersAllowed = true)
    {
        var isValid = true;

        // to consume a one-liner, you just consume a statement and return
        if (areOneLinersAllowed && Tokenizer.Peek() != "{")
        {
            if (!Consume(out StatementNode statement))
            {
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
        if (openingBracket != "{")
        {
            Logger.Error(new UnexpectedTokenException(
                token: openingBracket,
                context: "at the start of simple block (this probably means there was an internal error, please report this!)",
                expected: "{"
            ));

            Tokenizer.Reconsume();
        }

        var location = openingBracket.Location;

        var statements = new List<StatementNode>();

        while (Tokenizer.Peek() != "}")
        {
            statements.Add(Consume());

            if (Tokenizer.Peek().Kind == TokenKind.EOF)
            {
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

        if (closingBracket.Kind == TokenKind.EOF)
        {
            Logger.Error(new UnexpectedEOFException(
                context: "in simple block",
                expected: "the character '}'",
                range: Tokenizer.Position
            ));

            isValid = false;
        }
        else if (closingBracket != "}")
        {
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