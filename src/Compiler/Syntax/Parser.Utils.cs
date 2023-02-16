namespace Lotus.Syntax;

public sealed partial class Parser
{
    internal ImmutableArray<Token> ConsumeModifiers() {
        // don't allocate in case there's no modifier at all
        if (!LotusFacts.IsModifierKeyword(Tokenizer.Peek()))
            return ImmutableArray<Token>.Empty;

        var currToken = Tokenizer.Consume();

        // don't allocate a builder for only one token
        if (!LotusFacts.IsModifierKeyword(Tokenizer.Peek()))
            return ImmutableArray.Create(currToken);

        // otherwise, create a builder and add the current token to it
        var modifierBuilder = ImmutableArray.CreateBuilder<Token>(4);
        modifierBuilder.Add(currToken);

        while (LotusFacts.IsModifierKeyword(Tokenizer.Peek())) {
            modifierBuilder.Add(Tokenizer.Consume());
        }

        return modifierBuilder.ToImmutable();
    }

    internal static void ReportIfAnyModifiers(
        ImmutableArray<Token> modifiers,
        string nodeFriendlyName,
        out bool isValid
    ) {
        isValid = modifiers.IsDefaultOrEmpty;

        if (!isValid) {
            Logger.Error(new UnexpectedError<Token[]>(ErrorArea.Parser) {
                Value = modifiers.ToArray(), // why can't i use ImmutableArray directly :(
                As = "a modifier",
                Message = nodeFriendlyName + " cannot have any modifiers"
            });
        }
    }

    private void CheckSemicolon() {
        switch (_curr) {
            case ValueNode:
                break;
            case StatementNode stmt:
                if (!LotusFacts.NeedsSemicolon(stmt))
                    return;
                break;
            case TopLevelNode tl:
                if (!LotusFacts.NeedsSemicolon(tl))
                    return;
                break;
        }

        // if the next token is a semicolon
        if (Tokenizer.TryConsume(out var currToken) && currToken.Kind == TokenKind.semicolon) {
            // consume trailing semicolons
            while (Tokenizer.Peek().Kind == TokenKind.semicolon) {
                _ = Tokenizer.Consume();
            }

            return;
        }

        var eMsg = Current.GetType() + "s must be terminated with semicolons ';'";
        if (currToken.Kind == TokenKind.EOF) {
            Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                Message = eMsg,
                Location = Position.GetLastLocation()
            });
        } else {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Message = eMsg,
                Value = currToken,
                Location = currToken.Location
            });

            Tokenizer.Reconsume();
        }

        Current.IsValid = false;
    }
}