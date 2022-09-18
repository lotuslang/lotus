namespace Lotus.Syntax;

internal static class NodeUtils
{
    public static bool IsOneLiner(this Tuple<StatementNode> block)
        => block.Count == 1
        && block.OpeningToken == Token.NULL
        && block.ClosingToken == Token.NULL;
}