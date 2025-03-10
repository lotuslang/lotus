namespace Lotus.Syntax;

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

public class TupleParslet<TValue> : IParslet<Tuple<TValue>>
    where TValue : ILocalized
{
    [MemberNotNullWhen(true, nameof(singleValueParser))]
    [MemberNotNullWhen(false, nameof(multiValueParser))]
    private bool IsSingleValueParser { get; }

    private readonly Func<Parser, IEnumerable<TValue>>? multiValueParser;
    private readonly Func<Parser, TValue>? singleValueParser;

    public string Start { get; init; } = "(";
    public string End { get; init; } = ")";
    public string Delim { get; init; } = ",";
    public TrailingDelimiterBehaviour EndingDelimBehaviour { get; init; } = TrailingDelimiterBehaviour.Forbidden;
    public string In { get; init; } = "a " + typeof(TValue).Name + " list";

    public TupleParslet(Func<Parser, IEnumerable<TValue>> valueParser) {
        IsSingleValueParser = false;
        multiValueParser = valueParser;
    }

    public TupleParslet(Func<Parser, TValue> valParser) {
        IsSingleValueParser = true;
        singleValueParser = valParser;
    }

    public Tuple<TValue> Parse(Parser parser) {
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

        // in case this tuple is empty, don't alloc a builder
        if (parser.Tokenizer.TryConsume(out var token) && token == End) {
            var items = ImmutableArray<TValue>.Empty;

            return new Tuple<TValue>(items, startingToken, token) { IsValid = isValid };
        }

        var itemsBuilder = ImmutableArray.CreateBuilder<TValue>();

        do {
            parser.Tokenizer.Reconsume();

            // fixme: check that the parsed value is valid
            if (IsSingleValueParser)
                itemsBuilder.Add(singleValueParser(parser));
            else
                itemsBuilder.AddRange(multiValueParser(parser));

            if (!parser.Tokenizer.TryConsume(out token) || token != Delim) {
                if (token == End) {
                    if (EndingDelimBehaviour is TrailingDelimiterBehaviour.Required) {
                        Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                            Value = token,
                            In = In,
                            Expected = "a '" + Delim + "' before the closing '" + End + "'"
                        });

                        isValid = false;
                    }

                    break;
                }

                if (!isValid || !parser.Current.IsValid)
                    continue;

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
                if (parser.Tokenizer.Current.Kind is var kind && kind is TokenKind.identifier or TokenKind.semicolon) {
                    isValid = false;
                    parser.Tokenizer.Reconsume();

                    if (kind is TokenKind.semicolon)
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
                if (EndingDelimBehaviour is not TrailingDelimiterBehaviour.Forbidden) {
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
        while (parser.Tokenizer.TryConsume(out token) && token != End);

        var endingToken = parser.Tokenizer.Current;

        // we probably got out-of-scope (EOF or end of block)
        //
        // Or maybe we should just do that all the time ? Like if we got an unexpected
        // token we could show the first line/element of the tuple, and then show the end
        // or even where the error occurred (this also goes for earlier errors)
        if (isValid && endingToken != End) { // we probably either got an EOF or a bracket
            if (endingToken.Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    In = In,
                    Expected = "an ending delimiter '" + End + "'",
                    Location = (itemsBuilder.LastOrDefault() as ILocalized)?.Location ?? startingToken.Location
                });
            } else {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = endingToken,
                    In = In,
                    Expected = "an ending delimiter '" + End + "'"
                });

                if (endingToken == "}") {
                    parser.Tokenizer.Reconsume();
                }
            }
            isValid = false;
        }

        return new Tuple<TValue>(itemsBuilder.ToImmutable(), startingToken, endingToken) { IsValid = isValid };
    }
}