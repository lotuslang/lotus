namespace Lotus.Syntax;

public sealed class PrintParslet : IStatementParslet<PrintNode>
{
    public static readonly PrintParslet Instance = new();

    public PrintNode Parse(StatementParser parser, Token printToken) {
        Debug.Assert(printToken == "print");

        return new PrintNode(printToken, parser.ExpressionParser.Consume());
    }
}