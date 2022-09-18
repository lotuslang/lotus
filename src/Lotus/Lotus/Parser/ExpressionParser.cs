namespace Lotus.Syntax;

public sealed class ExpressionParser : Parser<ValueNode>
{
    public new static readonly ValueNode ConstantDefault = ValueNode.NULL;

    public override ValueNode Default => ConstantDefault with { Location = Position };

    public void SetCurrentToDefault()
        => _curr = ConstantDefault with { Location = Tokenizer.Position };

    public ExpressionParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, LotusGrammar.Instance)
        => SetCurrentToDefault();

    public ExpressionParser(IConsumer<ValueNode> nodeConsumer) : base(nodeConsumer, LotusGrammar.Instance)
        => SetCurrentToDefault();

    public ExpressionParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public ExpressionParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public ExpressionParser(Uri file) : this(new LotusTokenizer(file)) { }

    public ExpressionParser(Parser<ValueNode> parser) : base(parser)
        => SetCurrentToDefault();

    public override ValueNode Peek()
        => new ExpressionParser(this).Consume();

    public override ImmutableArray<ValueNode> Peek(int n) {
        var parser = new ExpressionParser(this);

        var output = ImmutableArray.CreateBuilder<ValueNode>(n);

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.MoveToImmutable();
    }

    public override ref readonly ValueNode Consume()
        => ref Consume(Precedence.Comma);

    public ref readonly ValueNode Consume(Precedence precedence = 0) {
        _ = base.Consume();

        _curr = ConsumeValue(precedence);

        return ref _curr;
    }

    private ValueNode ConsumeValue(Precedence precedence) {
        var token = Tokenizer.Consume();

        if (!Grammar.IsPrefix(Grammar.GetExpressionKind(token))) {
            string? notes = null;

            if (token.Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    Message = "Encountered an EOF where a value was expected",
                    Location = Position
                });
            } else {
                if (token.Kind == TokenKind.keyword)
                    notes = "You can't use " + token.Representation + " as an identifier/name, because it's a reserved keyword";
                else if (token.Kind == TokenKind.@operator)
                    notes = "The '" + token.Representation + "' operator cannot be used as a prefix (i.e. in front of an expression)";

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = token,
                    In = "an expression",
                    As = "a prefix or value literal",
                    Expected =
                          "a bool, "
                        + (token.Kind != TokenKind.@operator ? "prefix operator, " : "")
                        + "string, variable name, or number",
                    ExtraNotes = notes
                });

                if (token.Kind == TokenKind.semicolon)
                    Tokenizer.Reconsume();
            }

            return ConstantDefault with { Token = token, Location = token.Location };
        }

        var left = Grammar.GetPrefixParslet(token).Parse(this, token);

        if (!Tokenizer.Consume(out token)) {
            return left;
        }

        if (Grammar.IsPostfix(Grammar.GetExpressionKind(token))) {
            left = Grammar.GetPostfixParslet(token).Parse(this, token, left);

            token = Tokenizer.Consume();
        }

        while (precedence < Grammar.GetPrecedence(token)) {
            left = Grammar.GetOperatorParslet(token).Parse(this, token, left);

            token = Tokenizer.Consume();
        }

        Tokenizer.Reconsume();

        //left.IsValid = isValid;

        return left;
    }

    private static readonly ValueTupleParslet<ValueNode> _defaultTupleParslet
        = new(static (parser) => parser.Consume());

    public Tuple<ValueNode> ConsumeTuple(uint expectedItemCount = 0) {
        var baseTuple = _defaultTupleParslet.Parse(this);

        var items = baseTuple.Items;

        // if expectedItemCount is 0, then it means there's no limit
        if (baseTuple.IsValid && expectedItemCount != 0 && expectedItemCount != items.Length) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = items.LastOrDefault() ?? ValueNode.NULL,
                In = "a tuple",
                Location = items.LastOrDefault()?.Location ?? Position,
                Message = (items.Length > expectedItemCount ? "There were too many" : "There weren't enough")
                         + "values in this tuple.",
                Expected = expectedItemCount + " values, but got " + items.Length
            });

            baseTuple.IsValid = false;
        }

        return baseTuple;
    }

    public override ExpressionParser Clone() => new(this);
}