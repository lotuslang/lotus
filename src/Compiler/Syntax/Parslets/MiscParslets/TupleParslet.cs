namespace Lotus.Syntax;

public sealed class ValueTupleParslet<TValue> : TupleParslet<ExpressionParser, ValueNode, TValue> where TValue : ILocalized
{
    public ValueTupleParslet(Func<ExpressionParser, IEnumerable<TValue>> valParser) : base(valParser) {}

    public ValueTupleParslet(Func<ExpressionParser, TValue> valParser) : base(valParser) {}
}

/// <summary>
/// How the value delimiter should be handled at the end of a tuple
/// </summary>
public enum TrailingDelimiterBehaviour { // can't be moved into TupleParslet cause it'd require specifying type args
    /// <summary>
    /// Tuple should end without a value delimiter at the end.
    /// This is the default behavior.
    /// <br/>
    /// Example: <c>[a, b, c]</c>
    /// </summary>
    Forbidden,

    /// <summary>
    /// Tuple *can* have a trailing value delimiter at the end.
    /// This is how enums are parsed.
    /// <br/>
    /// Example: <c>{ a, b, c }</c> and <c>{ a, b, c, }</c> are both valid
    /// </summary>
    Accepted,

    /// <summary>
    /// Tuple *have to* end with a trailing value delimiter.
    /// This is how structs and statement blocks are parsed.
    /// <br/>
    /// Example: <c>{ a; b; c; }</c>
    /// </summary>
    Required
}

public class TupleParslet<TParser, TPNode, TValue> : IParslet<TParser, Tuple<TValue>>
    where TParser : Parser<TPNode>
    where TPNode : Node
    where TValue : ILocalized
{
    [MemberNotNullWhen(true, nameof(singleValueParser))]
    [MemberNotNullWhen(false, nameof(multiValueParser))]
    private bool IsSingleValueParser { get; }

    private readonly Func<TParser, IEnumerable<TValue>>? multiValueParser;
    private readonly Func<TParser, TValue>? singleValueParser;

    public string Start { get; init; } = "(";
    public string End { get; init; } = ")";
    public string Delim { get; init; } = ",";
    public TrailingDelimiterBehaviour EndingDelimBehaviour { get; init; } = TrailingDelimiterBehaviour.Forbidden;
    public string In { get; init; } = "a " + typeof(TValue).Name + " list";

    public TupleParslet(Func<TParser, IEnumerable<TValue>> valueParser) {
        IsSingleValueParser = false;
        multiValueParser = valueParser;
    }

    public TupleParslet(Func<TParser, TValue> valParser) {
        IsSingleValueParser = true;
        singleValueParser = valParser;
    }

    public Tuple<TValue> Parse(TParser parser) {
        var startingToken = parser.Tokenizer.Consume();

        var isValid = true;

        if (startingToken.Representation != Start) {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = startingToken,
                In = In,
                Expected = Start
            });

            isValid = false;
            parser.Tokenizer.Reconsume();
        }

        var items = ImmutableArray.CreateBuilder<TValue>();

        while (parser.Tokenizer.Consume(out var token) && token != End) {
            parser.Tokenizer.Reconsume();

            if (IsSingleValueParser)
                items.Add(singleValueParser(parser));
            else
                items.AddRange(multiValueParser(parser));

            if (parser.Tokenizer.Consume() != Delim) {
                if (parser.Tokenizer.Current == End) {
                    if (EndingDelimBehaviour is TrailingDelimiterBehaviour.Required) {
                        Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                            Value = parser.Tokenizer.Current,
                            In = In,
                            Expected = "a '" + Delim + "' before the closing '" + End + "'"
                        });

                        isValid = false;
                    }

                    break;
                }

                // fixme(parsing): this assumes the function used the main parser and/or set the
                // parser's Current property, which is not always the case
                if (!isValid || !parser.Current.IsValid) {
                    continue;
                }

                if (parser.Tokenizer.Current.Kind == TokenKind.keyword) {
                    parser.Tokenizer.Reconsume();

                    // If we set isValid here without emitting an error, execution will just continue normally
                    // since the errors at the end require that isValid is set to true
                    // fixme(logging): Although, it might be better to emit a custom error here :shrug:
                    //isValid = false;

                    break;
                }

                // no need to emit an error (or reconsume), it'll be handled later
                if (parser.Tokenizer.Current == "}")
                    break;

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
                    _ = parser.Tokenizer.Consume();
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
                if (EndingDelimBehaviour is TrailingDelimiterBehaviour.Accepted or TrailingDelimiterBehaviour.Required) {
                    _ = parser.Tokenizer.Consume();
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
                    Expected = "an ending delimiter '" + End + "'",
                    Location = (items.LastOrDefault() as ILocalized)?.Location ?? startingToken.Location
                });
            }

            isValid = false;
        }

        return new Tuple<TValue>(items.ToImmutable(), startingToken, endingToken) { IsValid = isValid };
    }
}