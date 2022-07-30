public sealed class ForeachParslet : IStatementParslet<ForeachNode>
{
    public static readonly ForeachParslet Instance = new();

    public ForeachNode Parse(StatementParser parser, Token foreachToken) {
        Debug.Assert(foreachToken == "foreach");

        var isValid = true;

        var openingParen = parser.Tokenizer.Consume();

        if (openingParen != "(") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = openingParen,
                In = "a foreach header",
                Expected = "an open parenthesis '('"
            });

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var itemNameToken = parser.Tokenizer.Consume();

        if (itemNameToken is not IdentToken itemName) { // because `in` is a reserved keyword
            if (itemNameToken == "in") {
                if (parser.Tokenizer.Peek() == "in") { // if the next token is 'in' again, it probably in was meant as a variable name
                    Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                        Message = "You cannot use 'in' as a variable name because it is a reserved keyword",
                        Value = itemNameToken
                    });
                } else { // otherwise, it probably means that the user forgot the item name
                    Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                        Message = "Did you forget to specify a name for each item in the collection ? Or did you mean to ignore the item ?",
                        ExtraNotes = "If you just want to iterate over a collection without using an item each time, you can either discard the variable with "
                                + "the special name '_', or use a for loop instead",
                        Value = itemNameToken
                    });

                    // no we DON'T reconsume because the tokenizer already has the result of Peek() in its reconsumeQueue and reconsuming the 'in' token would
                    // put it at the back of the queue, so the result of Peek() would come BEFORE the 'in' token
                    // also it doesn't really have any negative side-effect, contrary to trying to reconsume the 'in' token
                }
            } else {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = itemNameToken,
                    In = "a foreach header",
                    Expected = "an identifier"
                });
            }

            isValid = false;

            itemName = new IdentToken(itemNameToken.Representation, itemNameToken.Location, false);
        }

        var inToken = parser.Tokenizer.Consume();

        if (inToken is not Token inKeyword || inKeyword != "in") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = inToken,
                In = "a foreach header",
                Expected = "the 'in' keyword"
            });

            isValid = false;

            parser.Tokenizer.Reconsume();

            inKeyword = new Token(inToken.Representation, inToken.Kind, inToken.Location, false);
        }

        var collectionName = parser.ExpressionParser.Consume();

        var closingParen = parser.Tokenizer.Consume();

        if (closingParen != ")") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = closingParen,
                In = "a foreach header",
                Expected = "a closing parenthesis ')'"
            });

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var body = parser.ConsumeStatementBlock();

        return new ForeachNode(
            foreachToken,
            inKeyword,
            new IdentNode(itemName),
            collectionName,
            body,
            openingParen,
            closingParen,
            isValid
        );
    }
}