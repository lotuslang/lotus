namespace Lotus.Syntax;

public sealed class PrintParslet : IStatementParslet<PrintNode>
{
    public static readonly PrintParslet Instance = new();

    public PrintNode Parse(Parser parser, Token printToken) {
        Debug.Assert(printToken == "print");

        return new PrintNode(printToken, parser.ConsumeValue());
    }
}