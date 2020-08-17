public sealed class DeclarationParselet : IStatementParselet<DeclarationNode>
{
    public DeclarationNode Parse(Parser parser, Token varToken) {

        var isValid = true;

        // if the token isn't the keyword "var", throw an exception
        if (!(varToken is ComplexToken varKeyword && varKeyword == "var"))
            throw Logger.Fatal(new InvalidCallException(varToken.Location));

        // consume the name of the variable we're declaring
        var nameToken = parser.Tokenizer.Consume();

        // if the token isn't an identifier, throw an exception
        if (!(nameToken is IdentToken name)) {
            Logger.Error(new UnexpectedTokenException(
                token: nameToken,
                expected: TokenKind.ident
            ));

            isValid = false;

            name = new IdentToken(nameToken.Representation, nameToken.Location, false);

            if (nameToken == "=") parser.Tokenizer.Reconsume(); // FIXME: Specific error message
        }

        // consume a token
        var equalSign = parser.Tokenizer.Consume();

        // if this token wasn't an equal sign, throw an exception
        if (equalSign != "=") {
            Logger.Error(new UnexpectedTokenException(equalSign, "="));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        // consume a ValueNode (which is the value of the variable we're declaring)
        var value = parser.ConsumeValue();

        // return that value
        return new DeclarationNode(value, name, varKeyword, isValid);
    }
}
