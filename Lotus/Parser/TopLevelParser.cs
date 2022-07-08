public class TopLevelParser : Parser<TopLevelNode>
{
    public StatementParser StatementParser { get; private set; }

    public ExpressionParser ExpressionParser => StatementParser.ExpressionParser;

    public override TopLevelNode Current {
        get;
        protected set;
    } = ConstantDefault;

    public new static readonly TopLevelNode ConstantDefault = TopLevelNode.NULL;

    public override TopLevelNode Default => ConstantDefault with { Location = Position };

    protected void Init() {
        StatementParser = new StatementParser(Tokenizer);
        Current = ConstantDefault;
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

    public override TopLevelNode Consume() {
        base.Consume();

        var currToken = Tokenizer.Consume();

        // if the token is EOF, return TopLevelNode.NULL
        if (currToken == Tokenizer.Default || currToken == "\u0003") {
            return (Current = Default with { Location = currToken.Location });
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
            Current = parslet.Parse(this, currToken);

            // TODO: Throw an error when there's a modifier but the node isn't
            //       supposed to be modded
            if (accessKeyword != Token.NULL && Current is IAccessible accessibleCurrent)
                accessibleCurrent.AccessKeyword = accessKeyword;
        } else {
            Tokenizer.Reconsume();
            Current = new TopLevelStatementNode(StatementParser.Consume());
        }

        return Current;
    }

    public TypeDecName ConsumeTypeDeclarationName() {
        bool isValid = true;

        var nameOrParent = ExpressionParser.Consume();

        var nameValue = nameOrParent;
        var parent = ValueNode.NULL;
        var colonToken = Token.NULL;

        if (Tokenizer.Peek() == "::") {
            colonToken = Tokenizer.Consume();
            parent = nameOrParent;

            if (!Utilities.IsName(parent)) {
                // TODO: make a custom error for things that are supposed to be names
                // + that would allow them to be clearer about why the value cannot
                //   be a name, because rn it would spit out "OperationNodes cannot
                //   be used to specify an enum's parent" but to the user that doesn't
                //   mean anything
                Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                    Value = parent,
                    Message = parent.GetType().Name + "s cannot be used to specify the name of a base type",
                    As = "a type's parent name"
                });

                isValid = false;
            }

            nameValue = ExpressionParser.Consume();
        }

        if (nameValue is not IdentNode name) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = nameValue,
                Expected = "an identifier",
                As = "a new type name"
            });

            isValid = false;
            name = new IdentNode(
                new IdentToken(
                    nameValue.Token.Representation,
                    nameValue.Token.Location,
                    false
                ),
                false
            );
        }

        return new TypeDecName(name, parent, colonToken, isValid);
    }

    public override TopLevelParser Clone() => new(this);
}