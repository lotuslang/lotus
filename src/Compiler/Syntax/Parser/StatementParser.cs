namespace Lotus.Syntax;

public sealed class StatementParser : Parser<StatementNode>
{
    public ExpressionParser ExpressionParser { get; private set; }

    public new static readonly StatementNode ConstantDefault = StatementNode.NULL;

    public override StatementNode Default => ConstantDefault with { Location = Position };

    private void Init() {
        ExpressionParser = new ExpressionParser(Tokenizer);
        _curr = ConstantDefault with { Location = Tokenizer.Position };
    }

#nullable disable
    public StatementParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer)
        => Init();

    public StatementParser(IConsumer<StatementNode> nodeConsumer) : base(nodeConsumer)
        => Init();

    public StatementParser(StringConsumer consumer) : this(new Tokenizer(consumer)) { }

    public StatementParser(IEnumerable<char> collection) : this(new Tokenizer(collection)) { }

    public StatementParser(Uri file) : this(new Tokenizer(file)) { }

    public StatementParser(Parser<StatementNode> parser) : base(parser)
        => Init();
#nullable enable

    public override StatementNode Peek()
        => new StatementParser(this).Consume();

    public override ImmutableArray<StatementNode> Peek(int n) {
        var parser = new StatementParser(this);

        var output = ImmutableArray.CreateBuilder<StatementNode>(n);

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.MoveToImmutable();
    }

    public override ref readonly StatementNode Consume()
        => ref Consume(true);

    public ref readonly StatementNode Consume(bool checkSemicolon = true) {
        _ = base.Consume();

        // Consume a token
        var currToken = Tokenizer.Consume();

        // consume leading semicolons
        while (currToken.Kind == TokenKind.semicolon && Tokenizer.Consume(out currToken))
        { }

        // if the token is EOF, return StatementNode.NULL
        if (currToken.Kind == TokenKind.EOF) {
            _curr = Default with { Location = currToken.Location };
            return ref _curr;
        }

        if (LotusFacts.TryGetStatementParslet(currToken, out var parslet)) {
            _curr = parslet.Parse(this, currToken);
        } else {
            Tokenizer.Reconsume();
            _curr = new StatementExpressionNode(ExpressionParser.Consume());
        }

        if (checkSemicolon && LotusFacts.NeedsSemicolon(Current)) {
            CheckSemicolon();
        }

        return ref _curr;
    }

    private void CheckSemicolon() {
        if (!Tokenizer.Consume(out var currToken) || currToken.Kind != TokenKind.semicolon) {
            var eMsg = Current.GetType() + "s must be terminated with semicolons ';'";
            if (currToken.Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    Message = eMsg,
                    Location = Current.Location.GetLastLocation()
                });
            } else {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Message = eMsg,
                    Value = currToken,
                    Location = currToken.Location
                });

                Tokenizer.Reconsume();
            }

            _curr.IsValid = false;
        }

        // consume trailing semicolons
        while (Tokenizer.Peek().Kind == TokenKind.semicolon) {
            _ = Tokenizer.Consume();
        }
    }

    public Tuple<StatementNode> ConsumeStatementBlock(bool areOneLinersAllowed = true)
        => (areOneLinersAllowed
            ? StatementBlockParslet.Default
            : StatementBlockParslet.NoOneLiner).Parse(this);

    public override StatementParser Clone() => new(this);
}