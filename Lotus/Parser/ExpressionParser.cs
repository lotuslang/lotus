
public class ExpressionParser : Parser<ValueNode>
{
    public override ValueNode Current {
        get;
        protected set;
    } = ConstantDefault;

    public new static readonly ValueNode ConstantDefault = ValueNode.NULL;

    public override ValueNode Default => ConstantDefault with { Location = Position };

    public void Init() {
        Current = ConstantDefault with { Location = Tokenizer.Position };
    }

    public ExpressionParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, LotusGrammar.Instance)
        => Init();

    public ExpressionParser(IConsumer<ValueNode> nodeConsumer) : base(nodeConsumer, LotusGrammar.Instance)
        => Init();

    public ExpressionParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public ExpressionParser(IEnumerable<Token> tokens) : base(tokens, LotusGrammar.Instance)
        => Init();

    public ExpressionParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public ExpressionParser(Uri file) : this(new LotusTokenizer(file)) { }

    public ExpressionParser(Parser<ValueNode> parser) : base(parser)
        => Init();


    public override ValueNode Peek()
        => new ExpressionParser(this).Consume();

    public override ValueNode[] Peek(int n) {
        var parser = new ExpressionParser(this);

        var output = new List<ValueNode>();

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.ToArray();
    }

    public override ValueNode Consume()
        => Consume(Precedence.Comma);

    public ValueNode Consume(Precedence precedence = 0) {
        base.Consume();

        Current = ConsumeValue(precedence);

        return Current;
    }

    private ValueNode ConsumeValue(Precedence precedence) {
        var token = Tokenizer.Consume();

        if (!Grammar.IsPrefix(Grammar.GetExpressionKind(token))) {

            if (token.Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFException(
                    message: "Encountered an EOF where a value was expected",
                    range: Position
                ));
            } else if (token != ";") {
                Logger.Error(new UnexpectedTokenException(
                    token: token,
                    expected: new[] {
                        TokenKind.@bool,
                        TokenKind.@operator,
                        TokenKind.@string,
                        TokenKind.identifier,
                        TokenKind.number
                    }
                ));
            }

            return ValueNode.NULL with { Token = token, Location = Position };
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

    public TupleNode ConsumeTuple(string start, string end, int expectedItemCount = -1) {
        var startingDelimiter = Tokenizer.Consume();

        var isValid = true;

        if (startingDelimiter.Representation != start) {
            Logger.Error(new UnexpectedTokenException( // should we use InvalidCallException ?
                token: startingDelimiter,
                context: "in a tuple",
                expected: start
            ));

            isValid = false;
        }

        var items = new List<ValueNode>();

        var itemCount = 0;

        while (Tokenizer.Consume(out var token) && token != end) {

            Tokenizer.Reconsume();

            items.Add(Consume());

            ++itemCount;

            if (Tokenizer.Consume() != ",") {

                if (Tokenizer.Current == end) {
                    break;
                }

                var lastItem = items.Last();

                if (!isValid || !lastItem.IsValid) {
                    continue;
                }

                if (Tokenizer.Current.Kind == TokenKind.keyword || lastItem.Token.Kind == TokenKind.keyword) {
                    Tokenizer.Reconsume();

                    // If we set isValid here without emitting an error, execution will just continue normally
                    // since the errors at the end require that isValid is set to true
                    // FIXME: Although, it might be better to emit a custom error here :shrug:
                    //isValid = false;

                    break;
                }

                if (Tokenizer.Current == "}") {
                    Tokenizer.Reconsume();

                    break;
                }

                Logger.Error(new UnexpectedTokenException(
                    message: "Did you forget a parenthesis or a comma in this tuple ? Expected " + end + " or ','",
                    token: Tokenizer.Current
                ));

                // if it's an identifier, we should reconsume it so the error doesn't run over
                if (Tokenizer.Current.Kind == TokenKind.identifier) {
                    Tokenizer.Reconsume();
                } else if (Tokenizer.Peek() == ",") {
                    Tokenizer.Consume();
                }

                isValid = false;

                //Tokenizer.Reconsume();

                continue;
            }

            // since we know that there's a comma right before, if there's an ending delimiter right after it,
            // it's an error.
            // Example : (hello, there, friend,) // right here
            //           ----------------------^--------------
            //                      litterally right there
            if (Tokenizer.Peek() == end) {
                Logger.Error(new UnexpectedTokenException(
                    token: Tokenizer.Consume(),
                    context: "in a tuple",
                    expected: "value"
                ));

                isValid = false;

                break;
            }
        }

        var endingToken = Tokenizer.Current;

        // we probably got out-of-scope (EOF or end of block)
        //
        // TODO: Handle EOF differently (like show where the start of the tuple
        // was instead of the bottom of the file)
        //
        // Or maybe we should just do that all the time ? Like if we got an unexpected
        // token we could show the first line/element of the tuple, and then show the end
        // or even where the error occurred (this also goes for earlier errors)
        if (isValid && endingToken != end) {
            Logger.Error(new UnexpectedTokenException(
                token: endingToken,
                context: "in a tuple",
                expected: "an ending delimiter (here, it would be '" + end + "')"
            ));

            isValid = false;
        }

        if (isValid && expectedItemCount != -1 && itemCount != expectedItemCount) {

            if (itemCount > expectedItemCount) {
                Logger.Error(new LotusException(
                    message: "There was too many values in this tuple. Expected "
                           + expectedItemCount + ", but got " + itemCount,
                    range: items.Last()?.Token.Location ?? Tokenizer.Position
                ));
            } else {
                Logger.Error(new LotusException(
                    message: "There wasn't enough values in this tuple. Expected "
                           + expectedItemCount + ", but got " + itemCount,
                    range: items.Last()?.Token.Location ?? Tokenizer.Position
                ));
            }

            isValid = false;
        }

        return new TupleNode(items, startingDelimiter, endingToken, isValid);
    }

    public override ExpressionParser Clone() => new(this);
}