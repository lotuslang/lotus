namespace Lotus.Syntax;

public sealed class ContinueParslet : IStatementParslet<ContinueNode>
{
    public static readonly ContinueParslet Instance = new();

    public ContinueNode Parse(Parser parser, Token continueToken) {
        Debug.Assert(continueToken == "continue");

        return new ContinueNode(continueToken);
    }
}