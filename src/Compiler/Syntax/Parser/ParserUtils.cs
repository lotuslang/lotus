namespace Lotus.Syntax;

public static class ParserUtils
{
    public static ImmutableArray<Token> ConsumeModifiers<T>(Parser<T> parser) where T : Node {
        // don't allocate in case there's no modifier at all
        if (!LotusFacts.IsModifierKeyword(parser.Tokenizer.Peek()))
            return ImmutableArray<Token>.Empty;

        var currToken = parser.Tokenizer.Consume();

        // don't allocate a builder for only one token
        if (!LotusFacts.IsModifierKeyword(parser.Tokenizer.Peek()))
            return ImmutableArray.Create(currToken);

        // otherwise, create a builder and add the current token to it
        var modifierBuilder = ImmutableArray.CreateBuilder<Token>(4);
        modifierBuilder.Add(currToken);

        while (LotusFacts.IsModifierKeyword(parser.Tokenizer.Peek())) {
            modifierBuilder.Add(parser.Tokenizer.Consume());
        }

        return modifierBuilder.ToImmutable();
    }

    public static void CheckSemicolon<T>(Parser<T> parser) where T : Node {
        // if the next token is a semicolon
        if (parser.Tokenizer.Consume(out var currToken) && currToken.Kind == TokenKind.semicolon) {
            // consume trailing semicolons
            while (parser.Tokenizer.Peek().Kind == TokenKind.semicolon) {
                _ = parser.Tokenizer.Consume();
            }

            return;
        }

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
}