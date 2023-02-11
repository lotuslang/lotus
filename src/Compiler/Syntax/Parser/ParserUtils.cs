namespace Lotus.Syntax;

public static class ParserUtils
{
    public static void CheckSemicolon<T>(Parser<T> parser) where T : Node {
        if (!parser.Tokenizer.Consume(out var currToken) || currToken.Kind != TokenKind.semicolon) {
            var eMsg = parser.Current.GetType() + "s must be terminated with semicolons ';'";
            if (currToken.Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    Message = eMsg,
                    Location = parser.Current.Location.GetLastLocation()
                });
            } else {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Message = eMsg,
                    Value = currToken,
                    Location = currToken.Location
                });

                parser.Tokenizer.Reconsume();
            }

            parser.Current.IsValid = false;
        }

        // consume trailing semicolons
        while (parser.Tokenizer.Peek().Kind == TokenKind.semicolon) {
            _ = parser.Tokenizer.Consume();
        }
    }
}