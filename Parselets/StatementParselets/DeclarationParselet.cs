public class DeclarationParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token varKeyword) {

        // if the token isn't the keyword "var", throw an exception
        if (varKeyword != "var") throw new UnexpectedTokenException(varKeyword, "in declaration", "var");

        // consume the name of the variable we're declaring
        var name = parser.Tokenizer.Consume();

        // if the token isn't an identifier, throw an exception
        if (name != TokenKind.ident) throw new UnexpectedTokenException(name, TokenKind.ident);

        // consume a token
        var equalSign = parser.Tokenizer.Consume();

        // if this token wasn't an equal sign, throw an exception
        if (equalSign != "=") throw new UnexpectedTokenException(equalSign, "=");

        // consume a ValueNode (which is the value of the variable we're declaring)
        var value = parser.ConsumeValue();

        // return that value
        return new DeclarationNode(value, name as ComplexToken, varKeyword as ComplexToken);
    }
}
