namespace Lotus.Syntax;

public sealed class TopLevelParser : Parser<TopLevelNode>
{
    public StatementParser StatementParser { get; private set; }

    public ExpressionParser ExpressionParser => StatementParser.ExpressionParser;

    public new static readonly TopLevelNode ConstantDefault = TopLevelNode.NULL;

    public override TopLevelNode Default => ConstantDefault with { Location = Position };

    [MemberNotNull(nameof(StatementParser))]
    private void Init() {
        StatementParser = new StatementParser(Tokenizer);
        _curr = ConstantDefault with { Location = Tokenizer.Position };
    }

    public TopLevelParser(Tokenizer tokenizer) : base(tokenizer)
        => Init();

    public TopLevelParser(TextStream stream) : base(stream)
        => Init();

    public TopLevelParser(Parser<TopLevelNode> parser) : base(parser)
        => Init();

    public override ref readonly TopLevelNode Consume() {
        _ = base.Consume();

        // if the token is EOF, return TopLevelNode.NULL
        if (Tokenizer.Peek().Kind == TokenKind.EOF) {
            _curr = Default with { Location = Tokenizer.Consume().Location };
            return ref _curr;
        }

        var modifiers = ParserUtils.ConsumeModifiers(this);

        var currToken = Tokenizer.Consume();

        // we don't need to update currToken here, since we know it's *not* a modifier

        if (LotusFacts.TryGetTopLevelParslet(currToken, out var parslet)) {
            _curr = parslet.Parse(this, currToken, modifiers);

            if (LotusFacts.NeedsSemicolon(Current))
                ParserUtils.CheckSemicolon(this);
        } else {
            Tokenizer.Reconsume();
            _curr = new TopLevelStatementNode(StatementParser.Consume());
        }

        return ref _curr;
    }

    public TypeDecName ConsumeTypeDeclarationName() {
        var typeName = ExpressionParser.Consume<NameNode>(IdentNode.NULL, @as: "the name of a type");

        var parent = NameNode.NULL;
        var colonToken = Token.NULL;

        if (Tokenizer.Peek() == "::") {
            colonToken = Tokenizer.Consume();
            parent = typeName;

            typeName = ExpressionParser.Consume<IdentNode>(IdentNode.NULL, @as: "the name of the new type");
        }

        bool isValid = typeName.IsValid;

        if (typeName.IsValid && typeName.Parts.Length != 1) {
            Logger.Error(new NotANameError(ErrorArea.Parser) {
                Value = ExpressionParser.Current,
                Expected = "an identifier",
                As = "the name of a new type"
            });

            isValid = false;
        }

        if (typeName is not IdentNode typeIdent) {
            var token = typeName.Parts.FirstOrDefault(IdentToken.NULL);

            var location = typeName.Location;

            typeIdent = new IdentNode(
                new IdentToken(
                    token.Representation,
                    location
                ) { IsValid = false }
            );
        }

        return new TypeDecName(typeIdent, parent, colonToken) { IsValid = isValid };
    }

    internal static void ReportIfAnyModifiers(
        ImmutableArray<Token> modifiers,
        string nodeFriendlyName,
        out bool isValid
    ) {
        isValid = modifiers.IsDefaultOrEmpty;

        if (!isValid) {
            Logger.Error(new UnexpectedError<Token[]>(ErrorArea.Parser) {
                Value = modifiers.ToArray(), // why can't i use ImmutableArray directly :(
                As = "a modifier",
                Message = nodeFriendlyName + " cannot have any modifiers"
            });
        }
    }

    internal override TopLevelParser Clone() => new(this);
}