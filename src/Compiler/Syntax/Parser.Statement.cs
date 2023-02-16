namespace Lotus.Syntax;

public sealed partial class Parser
{
    public StatementNode ConsumeStatement(bool shouldCheckSemicolon = true) {
        // Consume a token
        var currToken = Tokenizer.Consume();

        // consume leading semicolons
        while (currToken.Kind == TokenKind.semicolon)
            currToken = Tokenizer.Consume();

        // if the token is EOF, return StatementNode.NULL
        if (currToken.Kind == TokenKind.EOF) {
            _curr = CreateFakeStatement(currToken);
            return (StatementNode)_curr;
        }

        if (LotusFacts.TryGetStatementParslet(currToken, out var parslet)) {
            _curr = parslet.Parse(this, currToken);
        } else {
            Tokenizer.Reconsume();
            _curr = new StatementExpressionNode(ConsumeValue());
        }

        if (shouldCheckSemicolon)
            CheckSemicolon();

        return (StatementNode)_curr;
    }

    public Tuple<StatementNode> ConsumeStatementBlock(bool areOneLinersAllowed = true)
        => (areOneLinersAllowed
            ? StatementBlockParslet.Default
            : StatementBlockParslet.NoOneLiner).Parse(this);
}