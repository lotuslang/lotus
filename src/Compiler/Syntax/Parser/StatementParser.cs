namespace Lotus.Syntax;

public sealed class StatementParser : Parser<StatementNode>
{
    public ExpressionParser ExpressionParser { get; private set; }

    public new static readonly StatementNode ConstantDefault = StatementNode.NULL;

    public override StatementNode Default => ConstantDefault with { Location = Position };

    [MemberNotNull(nameof(ExpressionParser))]
    private void Init() {
        ExpressionParser = new ExpressionParser(Tokenizer);
        _curr = ConstantDefault with { Location = Tokenizer.Position };
    }

    public StatementParser(Tokenizer tokenizer) : base(tokenizer)
        => Init();

    public StatementParser(TextStream stream) : base(stream)
        => Init();

    public StatementParser(Parser<StatementNode> parser) : base(parser)
        => Init();

    public override StatementNode Peek()
        => new StatementParser(this).Consume();

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

        if (checkSemicolon && LotusFacts.NeedsSemicolon(Current))
            ParserUtils.CheckSemicolon(this);

        return ref _curr;
    }

    public Tuple<StatementNode> ConsumeStatementBlock(bool areOneLinersAllowed = true)
        => (areOneLinersAllowed
            ? StatementBlockParslet.Default
            : StatementBlockParslet.NoOneLiner).Parse(this);

    public override StatementParser Clone() => new(this);
}