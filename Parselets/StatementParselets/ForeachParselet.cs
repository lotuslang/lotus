public sealed class ForeachParselet : IStatementParselet<ForeachNode>
{
    public ForeachNode Parse(Parser parser, Token foreachToken)
    {
        if (!(foreachToken is ComplexToken foreachKeyword && foreachKeyword == "foreach"))
            throw Logger.Fatal(new InvalidCallException(foreachToken.Location));

        var isValid = true;

        if (parser.Tokenizer.Consume() != "(") {
            Logger.Error(new UnexpectedTokenException(
                token: parser.Tokenizer.Current,
                context: "in foreach header",
                expected: "an open parenthesis '('"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var itemNameToken = parser.Tokenizer.Consume();

        if (!(itemNameToken is IdentToken itemName && itemName != "in")) { // because in is a reserved keyword
            Logger.Error(new UnexpectedTokenException(
                token: itemNameToken,
                context: "in a foreach header",
                expected: TokenKind.ident
            ));

            isValid = false;

            if (itemNameToken == "in") parser.Tokenizer.Reconsume();

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

        var collectionName = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != ")") {
            Logger.Error(new UnexpectedTokenException(
                token: parser.Tokenizer.Current,
                context: "in a foreach header",
                expected: "a closing parenthesis ')'"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var body = parser.ConsumeSimpleBlock();

        return new ForeachNode(foreachKeyword, inKeyword, new IdentNode(itemNameToken.Representation, itemName), collectionName, body, isValid);
    }
}