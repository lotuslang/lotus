public sealed class ForeachParslet : IStatementParslet<ForeachNode>
{
    public ForeachNode Parse(StatementParser parser, Token foreachToken) {
        if (!(foreachToken is ComplexToken foreachKeyword && foreachKeyword == "foreach"))
            throw Logger.Fatal(new InvalidCallException(foreachToken.Location));

        var isValid = true;

        var openingParen = parser.Tokenizer.Consume();

        if (openingParen != "(") {
            Logger.Error(new UnexpectedTokenException(
                token: openingParen,
                context: "in foreach header",
                expected: "an open parenthesis '('"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var itemNameToken = parser.Tokenizer.Consume();

        if (!(itemNameToken is IdentToken itemName)) { // because `in` is a reserved keyword
            if (itemNameToken == "in") {
                if (parser.Tokenizer.Peek() == "in") { // if the next token is 'in' again, it probably in was meant as a variable name
                    Logger.Error(new UnexpectedTokenException(
                        message: "You cannot use 'in' as a variable name because it is a reserved keyword",
                        token: itemNameToken
                    ));
                } else { // otherwise, it probably means that the user forgot the item name
                    Logger.Error(new UnexpectedTokenException(
                        message: "Did you forget to specify a name for each item in the collection ? Or did you mean to ignore the item ? "
                                + "If you just want to iterate over a collection without using an item each time, you can either discard the variable with "
                                + "the special name '_', or use a for loop instead",
                        token: itemNameToken
                    ));

                    // no we DON'T reconsume because the tokenizer already has the result of Peek() in its reconsumeQueue and reconsuming the 'in' token would
                    // put it at the back of the queue, so the result of Peek() would come BEFORE the 'in' token
                    // also it doesn't really have any negative side-effect, contrary to trying to reconsume the 'in' token
                }
            } else {
                Logger.Error(new UnexpectedTokenException(
                    token: itemNameToken,
                    context: "in a foreach header",
                    expected: TokenKind.identifier
                ));
            }

            isValid = false;

            itemName = new IdentToken(itemNameToken.Representation, itemNameToken.Location, false);
        }

        var inToken = parser.Tokenizer.Consume();

        if (!(inToken is ComplexToken inKeyword && inKeyword == "in")) {
            Logger.Error(new UnexpectedTokenException(
                token: inToken,
                context: "in a foreach header",
                expected: "the 'in' keyword"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();

            inKeyword = new ComplexToken(inToken.Representation, inToken.Kind, inToken.Location, false);
        }

        var collectionName = parser.ExpressionParser.ConsumeValue();

        var closingParen = parser.Tokenizer.Consume();

        if (closingParen != ")") {
            Logger.Error(new UnexpectedTokenException(
                token: closingParen,
                context: "in a foreach header",
                expected: "a closing parenthesis ')'"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var body = parser.ConsumeSimpleBlock();

        return new ForeachNode(
            foreachKeyword,
            inKeyword,
            new IdentNode(itemNameToken.Representation, itemName),
            collectionName,
            body,
            openingParen,
            closingParen,
            isValid
        );
    }
}