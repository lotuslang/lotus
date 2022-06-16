
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

            string? notes = null;

            if (token.Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                        Message = "Encountered an EOF where a value was expected",
                        Location = Position
                    }
                );
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

            return Default with { Token = token };
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

    public TupleNode ConsumeTuple(string start, string end, uint expectedItemCount = 0) {
        var startingToken = Tokenizer.Consume();

        var isValid = true;

        if (startingToken.Representation != start) {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) { // should we use InvalidCall instead ?
                Value = startingToken,
                In = "a tuple",
                Expected = start
            });

            isValid = false;
        }

        var items = new List<ValueNode>();

        while (Tokenizer.Consume(out var token) && token != end) {

            Tokenizer.Reconsume();

            items.Add(Consume());

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

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = Tokenizer.Current,
                    In = "a tuple",
                    Expected = "a ',' or '" + end + "'",
                    Message = "Did you forget '" + end + "' or a comma in this tuple ?"
                });


                // if it's an identifier, we should reconsume it so the error doesn't run over
                if (Tokenizer.Current.Kind == TokenKind.identifier) {
                    isValid = false;
                    Tokenizer.Reconsume();
                } else if (Tokenizer.Current.Kind == TokenKind.semicolon) {
                    isValid = false;
                    Tokenizer.Reconsume();

                    break;
                } else if (Tokenizer.Peek() == ",") {
                    isValid = false;
                    Tokenizer.Consume();
                }


                //Tokenizer.Reconsume();

                continue;
            }

            // since we know that there's a comma right before, if there's an ending delimiter right after it,
            // it's an error.
            // Example : (hello, there, friend,) // right here
            //           ----------------------^--------------
            //                      litterally right there
            if (Tokenizer.Peek() == end) {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = Tokenizer.Consume(),
                    In = "a tuple",
                    Expected = "value"
                });

                isValid = false;

                break;
            }
        }

        var endingToken = Tokenizer.Current;

        // we probably got out-of-scope (EOF or end of block)
        //
        // Or maybe we should just do that all the time ? Like if we got an unexpected
        // token we could show the first line/element of the tuple, and then show the end
        // or even where the error occurred (this also goes for earlier errors)
        if (isValid && endingToken != end) { // we probably either got an EOF or a bracket
            if (endingToken.Kind != TokenKind.EOF) {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = endingToken,
                    In = "a tuple",
                    Expected = "an ending delimiter '" + end + "'"
                });

                if (endingToken == "}") {
                    Tokenizer.Reconsume();
                }
            } else {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    In = "a tuple",
                    Expected = "an ending delimeter '" + end + "'",
                    Location = items.LastOrDefault()?.Location ?? startingToken.Location
                });
            }

            isValid = false;
        }

        // if expectedItemCount is 0, then it means there's no limit
        if (isValid && expectedItemCount != 0 && expectedItemCount != items.Count) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = items.LastOrDefault() ?? ValueNode.NULL,
                In = "a tuple",
                Location = items.LastOrDefault()?.Location ?? Position,
                Message =  (items.Count > expectedItemCount ? "There was too many" : "There wasn't enough")
                         + "values in this tuple.",
                Expected = expectedItemCount + $" values, but got " + items.Count
            });

            isValid = false;
        }

        return new TupleNode(items, startingToken, endingToken, isValid);
    }

    public override ExpressionParser Clone() => new(this);
}