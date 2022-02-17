public sealed class DeclarationParslet : IStatementParslet<DeclarationNode>
{

    private static DeclarationParslet _instance = new();
    public static DeclarationParslet Instance => _instance;

	private DeclarationParslet() : base() { }

    public DeclarationNode Parse(StatementParser parser, Token varToken) {

        var isValid = true;

        // if the token isn't the keyword "var", throw an exception
        if (varToken is not Token varKeyword || varKeyword != "var")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, varToken.Location));

        // consume the name of the variable we're declaring
        var nameToken = parser.Tokenizer.Consume();

        // if the token isn't an identifier, throw an exception
        if (nameToken is not IdentToken name) {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = nameToken,
                Expected = "an identifier"
            });

            isValid = false;

            name = new IdentToken(nameToken.Representation, nameToken.Location, false);

            if (nameToken == "=") {
                Logger.errorStack.Pop(); // remove the last exception

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Message = "Did you forget to specify a variable name ?",
                    Value = nameToken
                });

                parser.Tokenizer.Reconsume();
            }
        }

        // consume a token
        var equalSign = parser.Tokenizer.Consume();

        // if this token wasn't an equal sign, throw an exception
        if (equalSign != "=") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = equalSign,
                Expected = "="
            });

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        // consume a ValueNode (which is the value of the variable we're declaring)
        var value = parser.ExpressionParser.Consume();

        // return that value
        return new DeclarationNode(value, name, varKeyword, equalSign, isValid);
    }
}
