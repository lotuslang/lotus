internal sealed class ConstantVisualizer : StatementGraphMaker
{

    private string ConstantColor { get; }

    private string NonConstantColor { get; }

    public ConstantVisualizer(string constantColor = "green", string nonConstantColor = "red") {
        ConstantColor = constantColor;
        NonConstantColor = nonConstantColor;
    }

    public override GraphNode Default(ValueNode node) {
        return BaseDefault(node).SetColor(ASTHelper.IsContant(node) ? ConstantColor : NonConstantColor);
    }

    public override GraphNode ToGraphNode(ValueNode node) {
        return node.Accept(this).SetColor(ASTHelper.IsContant(node) ? ConstantColor : NonConstantColor);
    }

    public GraphNode ShowConstants(StatementNode node) => node.Accept(this);
    public GraphNode ShowConstants(ValueNode node) => node.Accept(this);
}