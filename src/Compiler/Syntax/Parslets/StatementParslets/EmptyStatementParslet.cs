namespace Lotus.Syntax;

public sealed class EmptyStatementParslet : IStatementParslet<EmptyStatementNode>
{
    public static readonly EmptyStatementParslet Instance = new();

    public EmptyStatementNode Parse(Parser parser, Token semicolon) {
        Debug.Assert(semicolon == ";");

        return new EmptyStatementNode(semicolon.Location, semicolon.IsValid) { Token = semicolon };
    }
}