using System;
using System.Collections.Generic;

public class StatementParser : Parser<StatementNode>
{
    public ExpressionParser ExpressionParser { get; protected set; }

    public override StatementNode Current {
        get;
        protected set;
    }

    public new static readonly StatementNode ConstantDefault = StatementNode.NULL;

    public StatementNode Default {
        get {
            var output = ConstantDefault;

            output.Location = Position;

            return output;
        }
    }

    protected void Init() {
        ExpressionParser = new ExpressionParser(Tokenizer);
        Current = ConstantDefault;
    }

#nullable disable
    public StatementParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, LotusGrammar.Instance)
        => Init();

    public StatementParser(IConsumer<StatementNode> nodeConsumer) : base(nodeConsumer, LotusGrammar.Instance)
        => Init();

    public StatementParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public StatementParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public StatementParser(Uri file) : this(new LotusTokenizer(file)) { }

    public StatementParser(Parser<StatementNode> parser) : base(parser)
        => Init();
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

    public override StatementNode Consume() {
        base.Consume();

        // Consume a token
        var currToken = Tokenizer.Consume();

        // if the token is EOF, return StatementNode.NULL
        if (currToken == Tokenizer.Default || currToken == "\u0003") {
            return (Current = Default);
        }

        if (Grammar.TryGetStatementParslet(currToken, out var parslet)) {
            Current = parslet.Parse(this, currToken);
        } else {
            Tokenizer.Reconsume();
            Current = new StatementExpressionNode(ExpressionParser.Consume());
        }

        return Current;
    }

    public SimpleBlock ConsumeSimpleBlock(bool areOneLinersAllowed = true) {
        var isValid = true;

        // to consume a one-liner, you just consume a statement and return
        if (areOneLinersAllowed && Tokenizer.Peek() != "{") {
            if (!Consume(out var statement)) {
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

        if (closingBracket != "}") {
            Logger.Error(new UnexpectedTokenException(
                token: closingBracket,
                context: "in simple block",
                expected: "the character '}'"
            ));

            if (closingBracket.Kind == TokenKind.EOF) {
                // if this node was already invalid, it probably means that we already encountered an EOF,
                // so no need to tell the user twice
                if (!isValid) Logger.Exceptions.Pop();
            } else {
                Tokenizer.Reconsume();
            }

            isValid = false;
        }

        Tokenizer.Consume();

        return new SimpleBlock(statements.ToArray(), location, openingBracket, closingBracket, isValid);
    }

    public override StatementParser Clone() => new(this);
}