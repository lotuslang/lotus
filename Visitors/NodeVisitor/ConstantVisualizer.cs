public sealed class ConstantVisualizer : NodeGraphMaker
{

    private string ConstantColor { get; }

    private string NonConstantColor { get; }

    public ConstantVisualizer(string constantColor = "green", string nonConstantColor = "red") {
        ConstantColor = constantColor;
        NonConstantColor = nonConstantColor;
    }

    protected override GraphNode Default(ValueNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(ValueNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(ArrayLiteralNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(BoolNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(ComplexStringNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(FunctionCallNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(IdentNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(NumberNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(ObjectCreationNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(OperationNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(ParenthesizedValueNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(StringNode node) {
        return base.Visit(node).SetColor(ASTHelper.IsContant(node) ? "green" : "red");
    }

    public override GraphNode Visit(SimpleBlock block) {
        return base.Visit(block);
    }

    public GraphNode ShowConstants(StatementNode node) => node.Accept(this);
}