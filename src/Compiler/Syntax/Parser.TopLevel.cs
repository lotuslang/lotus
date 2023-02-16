namespace Lotus.Syntax;

public sealed partial class Parser
{
    public TopLevelNode ConsumeTopLevel() {
        // if the token is EOF, return TopLevelNode.NULL
        if (Tokenizer.Peek().Kind == TokenKind.EOF) {
            _curr = TopLevelNode.NULL with { Location = Tokenizer.Consume().Location };
            return (TopLevelNode)_curr;
        }

        var modifiers = ConsumeModifiers();

        var currToken = Tokenizer.Consume();

        // we don't need to update currToken here, since we know it's *not* a modifier

        if (LotusFacts.TryGetTopLevelParslet(currToken, out var parslet)) {
            _curr = parslet.Parse(this, currToken, modifiers);
            CheckSemicolon(); // ConsumeStatement already checks for semicolon, so we only do when it's not a stmt
        } else {
            Tokenizer.Reconsume();
            _curr = new TopLevelStatementNode(ConsumeStatement());
        }

        return (TopLevelNode)_curr;
    }

    public TypeDecName ConsumeTypeDeclarationName() {
        var typeName = ConsumeValue<NameNode>(IdentNode.NULL, @as: "the name of a type");

        var parent = NameNode.NULL;
        var colonToken = Token.NULL;

        if (Tokenizer.Peek() == "::") {
            colonToken = Tokenizer.Consume();
            parent = typeName;

            typeName = ConsumeValue<IdentNode>(IdentNode.NULL, @as: "the name of the new type");
        }

        bool isValid = typeName.IsValid;

        if (typeName.IsValid && typeName.Parts.Length != 1) {
            Logger.Error(new NotANameError(ErrorArea.Parser) {
                Value = (ValueNode)Current,
                Expected = "an identifier",
                As = "the name of a new type"
            });

            isValid = false;
        }

        if (typeName is not IdentNode typeIdent) {
            var token = typeName.Parts.FirstOrDefault(IdentToken.NULL);

            var location = typeName.Location;

            typeIdent = new IdentNode(
                new IdentToken(
                    token.Representation,
                    location
                ) { IsValid = false }
            );
        }

        return new TypeDecName(typeIdent, parent, colonToken) { IsValid = isValid };
    }
}