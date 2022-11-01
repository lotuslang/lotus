namespace Lotus.Extras.Graphs;

internal sealed class ConstantVisualizer : StatementGraphMaker
{
    private string ConstantColor { get; }

    private string NonConstantColor { get; }

    public ConstantVisualizer(string constantColor = "green", string nonConstantColor = "red") {
        ConstantColor = constantColor;
        NonConstantColor = nonConstantColor;
    }

    public override GraphNode Default(ValueNode node)
        => BaseDefault(node).SetColor(ASTUtils.IsContant(node) ? ConstantColor : NonConstantColor);

    public override GraphNode ToGraphNode(ValueNode node)
        => node.Accept(this).SetColor(ASTUtils.IsContant(node) ? ConstantColor : NonConstantColor);

    public GraphNode ShowConstants(StatementNode node) => node.Accept(this);
    public GraphNode ShowConstants(ValueNode node) => node.Accept(this);
}