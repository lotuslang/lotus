namespace Lotus.Syntax;

public sealed class FunctionDefinitionParslet : ITopLevelParslet<FunctionDefinitionNode>
{
    public static readonly FunctionDefinitionParslet Instance = new();

    private FunctionHeaderParslet _funcHeaderParslet = FunctionHeaderParslet.Instance;

    public FunctionDefinitionNode Parse(Parser parser, Token funcToken, ImmutableArray<Token> modifiers) {
        var header = _funcHeaderParslet.Parse(parser, funcToken, modifiers);

        // consume a simple block
        var block = parser.ConsumeStatementBlock(areOneLinersAllowed: false);

        // return a new FunctionDeclarationNode with 'block' as the value, 'parameters' as the list of params, and funcName as the name
        return new FunctionDefinitionNode(
            header,
            block
        ) { IsValid = header.IsValid && block.IsValid };
    }
}