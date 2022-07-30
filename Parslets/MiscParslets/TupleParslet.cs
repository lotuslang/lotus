using System.Diagnostics.CodeAnalysis;

public sealed class ValueTupleParslet<TValue> : TupleParslet<ExpressionParser, ValueNode, TValue>
{
    public ValueTupleParslet(Func<ExpressionParser, IEnumerable<TValue>> valParser) : base(valParser) {}

    public ValueTupleParslet(Func<ExpressionParser, TValue> valParser) : base(valParser) {}
}

public class TupleParslet<TParser, TPNode, TValue> : IParslet<TParser, Tuple<TValue>>
    where TPNode : Node
    where TParser : Parser<TPNode>
{
    [MemberNotNullWhen(true, nameof(simpleValueParser))]
    [MemberNotNullWhen(false, nameof(valueParser))]
    private bool isSimpleParser { get; }

    private readonly Func<TParser, IEnumerable<TValue>>? valueParser;
    private readonly Func<TParser, TValue>? simpleValueParser;

    public string Start { get; init; } = "(";
    public string End { get; init; } = ")";
    public string Delim { get; init; } = ",";
    public bool AcceptEndingComma { get; init; } = false;
    public string In { get; init; } = new Lazy<string>(static () => "a " + typeof(TValue).Name + " list", isThreadSafe: false).Value;

    public TupleParslet(Func<TParser, IEnumerable<TValue>> valParser) {
        isSimpleParser = false;
        valueParser = valParser;
    }

    public TupleParslet(Func<TParser, TValue> valParser) {
        isSimpleParser = true;
        simpleValueParser = valParser;
    }

    public Tuple<TValue> Parse(TParser parser) {
        var startingToken = parser.Tokenizer.Consume();

        var isValid = true;

        if (startingToken.Representation != Start) {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) { // should we use InvalidCall instead ?
                Value = startingToken,
                In = In,
                Expected = Start
            });

            isValid = false;
        }

        var items = new List<TValue>();

        while (parser.Tokenizer.Consume(out var token) && token != End) {

            parser.Tokenizer.Reconsume();

            if (isSimpleParser)
                items.Add(simpleValueParser(parser));
            else
                items.AddRange(valueParser(parser));

            if (parser.Tokenizer.Consume() != Delim) {

                if (parser.Tokenizer.Current == End) {
                    break;
                }

                var lastItem = items.Last();

                if (!isValid || !parser.Current.IsValid) {
                    continue;
                }

                if (parser.Tokenizer.Current.Kind == TokenKind.keyword) {
                    parser.Tokenizer.Reconsume();

                    // If we set isValid here without emitting an error, execution will just continue normally
                    // since the errors at the end require that isValid is set to true
                    // FIXME: Although, it might be better to emit a custom error here :shrug:
                    //isValid = false;

                    break;
                }

                if (parser.Tokenizer.Current == "}") {
                    parser.Tokenizer.Reconsume();

                    break;
                }

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = parser.Tokenizer.Current,
                    In = In,
                    Expected = "a ',' or '" + End + "'",
                    Message = "Did you forget '" + End + "' or '" + Delim + "' in this " + typeof(TValue).Name + " list ?"
                });


                // if it's an identifier, we should reconsume it so the error doesn't run over
                if (parser.Tokenizer.Current.Kind == TokenKind.identifier) {
                    isValid = false;
                    parser.Tokenizer.Reconsume();
                } else if (parser.Tokenizer.Current.Kind == TokenKind.semicolon) {
                    isValid = false;
                    parser.Tokenizer.Reconsume();

                    break;
                } else if (parser.Tokenizer.Peek() == Delim) {
                    isValid = false;
                    parser.Tokenizer.Consume();
                }


                //parser.Tokenizer.Reconsume();

                continue;
            }

            // since we know that there's a comma right before, if there's an ending delimiter right after it,
            // it might be an error.
            // Example : (hello, there, friend,) // right here
            //           ----------------------^--------------
            //                      literally right there
            if (parser.Tokenizer.Peek() == End) {
                if (AcceptEndingComma) {
                    parser.Tokenizer.Consume();
                    break;
                }

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = parser.Tokenizer.Consume(),
                    In = In,
                    Expected = "a " + typeof(TValue).Name
                });

                isValid = false;

                break;
            }
        }

        var endingToken = parser.Tokenizer.Current;

        // we probably got out-of-scope (EOF or end of block)
        //
        // Or maybe we should just do that all the time ? Like if we got an unexpected
        // token we could show the first line/element of the tuple, and then show the end
        // or even where the error occurred (this also goes for earlier errors)
        if (isValid && endingToken != End) { // we probably either got an EOF or a bracket
            if (endingToken.Kind != TokenKind.EOF) {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = endingToken,
                    In = In,
                    Expected = "an ending delimiter '" + End + "'"
                });

                if (endingToken == "}") {
                    parser.Tokenizer.Reconsume();
                }
            } else {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    In = In,
                    Expected = "an ending delimeter '" + End + "'",
                    Location = (items.LastOrDefault() as ILocalized)?.Location ?? startingToken.Location
                });
            }

            isValid = false;
        }

        return new Tuple<TValue>(items, startingToken, endingToken, isValid);
    }
}