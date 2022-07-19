public class StatementParser : Parser<StatementNode>
{
    public ExpressionParser ExpressionParser { get; protected set; }

    public new static readonly StatementNode ConstantDefault = StatementNode.NULL;

    public override StatementNode Default => ConstantDefault with { Location = Position };

    protected void Init() {
        ExpressionParser = new ExpressionParser(Tokenizer);
        _curr = ConstantDefault with { Location = Tokenizer.Position };
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

    public override ref readonly StatementNode Consume()
        => ref Consume(true);

    public ref readonly StatementNode Consume(bool checkSemicolon = true) {
        base.Consume();

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

        if (Grammar.TryGetStatementParslet(currToken, out var parslet)) {
            _curr = parslet.Parse(this, currToken);
        } else {
            Tokenizer.Reconsume();
            _curr = new StatementExpressionNode(ExpressionParser.Consume());
        }

        if (checkSemicolon && NeedsSemicolon(Current)) {
            CheckSemicolon();

            // consume trailing semicolons
            while (Tokenizer.Peek().Kind == TokenKind.semicolon) {
                Tokenizer.Consume();
            }
        }

        return ref _curr;
    }



    private void CheckSemicolon() {
        if (!Tokenizer.Consume(out var currToken) || currToken.Kind != TokenKind.semicolon) {
            var eMsg = Current.GetType() + "s must be terminated with semicolons ';'";
            var eLoc = Current.Location.GetLastLocation();
            if (currToken.Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    Message = eMsg,
                    Location = eLoc
                });
            } else {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Message = eMsg,
                    Value = currToken,
                    Location = eLoc
                });
            }

            _curr = Current with { IsValid = false };
            Tokenizer.Reconsume();
        }
    }

    public SimpleBlock ConsumeSimpleBlock(bool areOneLinersAllowed = true) {
        var isValid = true;

        // to consume a one-liner, you just consume a statement and return
        if (areOneLinersAllowed && Tokenizer.Peek() != "{") {
            if (!Consume(out var statement)) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    In = "a simple block",
                    Expected = "a statement",
                    Location = Tokenizer.Current.Location
                });

                isValid = false;
            }

            return new SimpleBlock(statement, statement.Token.Location, isValid);
        }

        var openingBracket = Tokenizer.Consume();

        // we don't have to check for EOF because that is (sorta) handled by "areOneLinersAllowed"
        if (openingBracket != "{") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = openingBracket,
                In = "at the start of simple block",
                Expected = "{",
                ExtraNotes = "This probably means there was an internal error, please report this!"
            });

            Tokenizer.Reconsume();
        }

        var location = openingBracket.Location;

        var statements = new List<StatementNode>();

        while (Tokenizer.Peek() != "}") {
            statements.Add(Consume());

            if (Tokenizer.Peek().Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    In = "a simple block",
                    Expected = "a statement",
                    Location = Tokenizer.Current.Location
                });

                isValid = false;

                break;
            }

            //if (Tokenizer.Peek() == ";") Tokenizer.Consume();
        }

        var closingBracket = Tokenizer.Peek();

        if (!(!isValid && closingBracket.Kind == TokenKind.EOF) && closingBracket != "}") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = closingBracket,
                In = "a simple block",
                Expected = "the character '}'"
            });

            if (closingBracket.Kind == TokenKind.EOF) {
                // if this node was already invalid, it probably means that we already encountered an EOF,
                // so no need to tell the user twice
                if (!isValid) Logger.errorStack.Pop();
            } else {
                Tokenizer.Reconsume();
            }

            isValid = false;
        }

        Tokenizer.Consume();

        return new SimpleBlock(statements.ToArray(), location, openingBracket, closingBracket, isValid);
    }

    // IMPORTANT: Update me if you add a new statement
    private bool NeedsSemicolon(StatementNode node)
        => node is not (
                   ElseNode
                or ForeachNode
                or ForNode
                or FunctionDeclarationNode
                or IfNode
                or WhileNode
            )
;

    public override StatementParser Clone() => new(this);
}