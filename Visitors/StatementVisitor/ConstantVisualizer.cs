public sealed class ConstantVisualizer : StatementGraphMaker
{

    private string ConstantColor { get; }

    private string NonConstantColor { get; }

    public ConstantVisualizer(string constantColor = "green", string nonConstantColor = "red") {
        ConstantColor = constantColor;
        NonConstantColor = nonConstantColor;
    }

    protected override GraphNode Default(ValueNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? ConstantColor : NonConstantColor);
    }

    public GraphNode ShowConstants(StatementNode node) => node.Accept(this);
    public GraphNode ShowConstants(ValueNode node) => node.Accept(this);
}