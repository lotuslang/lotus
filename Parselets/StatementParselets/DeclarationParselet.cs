public sealed class DeclarationParselet : IStatementParselet<DeclarationNode>
{
    public DeclarationNode Parse(Parser parser, Token varToken) {

        // if the token isn't the keyword "var", throw an exception
        if (!(varToken is ComplexToken varKeyword && varKeyword == "var")) throw new UnexpectedTokenException(varToken, "in declaration", "var");

        // consume the name of the variable we're declaring
        var nameToken = parser.Tokenizer.Consume();

        // if the token isn't an identifier, throw an exception
        if (!(nameToken is ComplexToken name && name.Kind == TokenKind.ident)) throw new UnexpectedTokenException(nameToken, TokenKind.ident);

        // consume a token
        var equalSign = parser.Tokenizer.Consume();

        // if this token wasn't an equal sign, throw an exception
        if (equalSign != "=") throw new UnexpectedTokenException(equalSign, "=");

        // consume a ValueNode (which is the value of the variable we're declaring)
        var value = parser.ConsumeValue();

        // return that value
        return new DeclarationNode(value, name, varKeyword);
    }
}
