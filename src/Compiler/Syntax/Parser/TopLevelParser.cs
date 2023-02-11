namespace Lotus.Syntax;

public sealed class TopLevelParser : Parser<TopLevelNode>
{
    public StatementParser StatementParser { get; private set; }

    public ExpressionParser ExpressionParser => StatementParser.ExpressionParser;

    public new static readonly TopLevelNode ConstantDefault = TopLevelNode.NULL;

    public override TopLevelNode Default => ConstantDefault with { Location = Position };

    private void Init() {
        StatementParser = new StatementParser(Tokenizer);
        _curr = ConstantDefault with { Location = Tokenizer.Position };
    }

#nullable disable
    public TopLevelParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer)
        => Init();

    public TopLevelParser(IConsumer<TopLevelNode> nodeConsumer) : base(nodeConsumer)
        => Init();

    public TopLevelParser(StringConsumer consumer) : this(new Tokenizer(consumer)) { }

    public TopLevelParser(IEnumerable<char> collection) : this(new Tokenizer(collection)) { }

    public TopLevelParser(Uri file) : this(new Tokenizer(file)) { }

    public TopLevelParser(Parser<TopLevelNode> parser) : base(parser)
        => Init();
#nullable enable

    public override TopLevelNode Peek()
        => new TopLevelParser(this).Consume();

    public override ref readonly TopLevelNode Consume() {
        _ = base.Consume();

        var currToken = Tokenizer.Consume();

        // if the token is EOF, return TopLevelNode.NULL
        if (currToken.Kind == TokenKind.EOF) {
            _curr = Default with { Location = currToken.Location };
            return ref _curr;
        }

        var modifiers = ImmutableArray.CreateBuilder<Token>();

        while (LotusFacts.IsModifierKeyword(currToken)) {
            modifiers.Add(currToken);
            currToken = Tokenizer.Consume();
        }

        // we don't need to update currToken here, since we know it's *not* a modifier

        if (LotusFacts.TryGetTopLevelParslet(currToken, out var parslet)) {
            _curr = parslet.Parse(this, currToken, modifiers.ToImmutable());

            // todo(logging): Throw an error when there's a modifier but the node isn't
            //       supposed to be modded
        } else {
            Tokenizer.Reconsume();
            _curr = new TopLevelStatementNode(StatementParser.Consume());
        }

        if (LotusFacts.NeedsSemicolon(Current))
            ParserUtils.CheckSemicolon(this);

        return ref _curr;
    }

    public TypeDecName ConsumeTypeDeclarationName() {
        var typeName = ExpressionParser.Consume<NameNode>(IdentNode.NULL, @as: "the name of a type");

        var parent = NameNode.NULL;
        var colonToken = Token.NULL;

        if (Tokenizer.Peek() == "::") {
            colonToken = Tokenizer.Consume();
            parent = typeName;

            typeName = ExpressionParser.Consume<IdentNode>(IdentNode.NULL, @as: "the name of a type");
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

    public override TopLevelParser Clone() => new(this);
}