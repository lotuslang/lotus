public class TopLevelParser : Parser<TopLevelNode>
{
    public StatementParser StatementParser { get; private set; }

    public ExpressionParser ExpressionParser => StatementParser.ExpressionParser;

    public new static readonly TopLevelNode ConstantDefault = TopLevelNode.NULL;

    public override TopLevelNode Default => ConstantDefault with { Location = Position };

    protected void Init() {
        StatementParser = new StatementParser(Tokenizer);
        _curr = ConstantDefault;
    }

#nullable disable
    public TopLevelParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, LotusGrammar.Instance)
        => Init();

    public TopLevelParser(IConsumer<TopLevelNode> nodeConsumer) : base(nodeConsumer, LotusGrammar.Instance)
        => Init();

    public TopLevelParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public TopLevelParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public TopLevelParser(Uri file) : this(new LotusTokenizer(file)) { }

    public TopLevelParser(Parser<TopLevelNode> parser) : base(parser, LotusGrammar.Instance)
        => Init();
#nullable enable

    public override TopLevelNode Peek()
        => new TopLevelParser(this).Consume();

    public override TopLevelNode[] Peek(int n) {
        var parser = new TopLevelParser(this);

        var output = new List<TopLevelNode>();

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.ToArray();
    }

    public override ref readonly TopLevelNode Consume() {
        base.Consume();

        var currToken = Tokenizer.Consume();

        // if the token is EOF, return TopLevelNode.NULL
        if (currToken == Tokenizer.Default || currToken == "\u0003") {
            _curr = Default with { Location = currToken.Location };
            return ref _curr;
        }

        var accessKeyword = Token.NULL;

        switch (currToken) {
            case "public":
            case "protected":
            case "private":
                accessKeyword = (currToken as Token)!;
                break;
            case "internal":
                if (Tokenizer.Peek() != "protected") {
                    accessKeyword = (currToken as Token)!;
            } else {
                accessKeyword = new Token(
                    currToken + " " + Tokenizer.Consume(),
                    TokenKind.keyword,
                    new LocationRange(currToken.Location, Tokenizer.Current.Location)
                ) {
                    LeadingTrivia = currToken.LeadingTrivia,
                    // FIXME: We don't preserve the trivia between the two tokens :(
                    TrailingTrivia = Tokenizer.Current.TrailingTrivia
                };
            }

                break;
            default:
                Tokenizer.Reconsume();
                break;
        }

        // get the token after the accessor; if we didn't get an accessor, then
        // we'll just consume the same token again
        currToken = Tokenizer.Consume();

        if (Grammar.TryGetTopLevelParslet(currToken, out var parslet)) {
            _curr = parslet.Parse(this, currToken);

            // TODO: Throw an error when there's a modifier but the node isn't
            //       supposed to be modded
            if (accessKeyword != Token.NULL && Current is IAccessible accessibleCurrent)
                accessibleCurrent.AccessKeyword = accessKeyword;
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

            typeName = ExpressionParser.Consume<IdentNode>(IdentNode.NULL, @as: "the name of a type");
        }

        bool isValid = typeName.IsValid;

        if (typeName.IsValid && typeName.Parts.Count != 1) {
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
                    location,
                    false
                ),
                false
            );
        }

        return new TypeDecName(typeIdent, parent, colonToken, isValid);
    }

    public override TopLevelParser Clone() => new(this);
}