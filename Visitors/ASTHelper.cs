internal static class ASTHelper
{
    public static readonly NameChecker NameChecker = new();

    public static readonly TopLevelPrinter TopLevelPrinter = new();

    public static readonly StatementPrinter StatementPrinter = new();

    public static readonly ValuePrinter ValuePrinter = new();

    public static readonly TokenPrinter TokenPrinter = new();

    public static readonly TokenGraphMaker TokenGraphMaker = new();

    public static readonly StatementGraphMaker StatementGraphMaker = new();

    public static readonly TopLevelGraphMaker TopLevelGraphMaker = new();

    public static readonly ConstantChecker ConstantChecker = new();

    public static readonly ValueExtractor ValueExtractor = new();

    public static readonly StatementExtractor StatementExtractor = new();

    public static readonly Flattener Flattener = new();

    public static readonly ConstantVisualizer ConstantVisualizer = new();


    public static bool IsName(ValueNode node) => NameChecker.IsName(node);

    public static string PrintNode(Node node)
        => node switch {
            ValueNode value         => PrintValue(value),
            StatementNode statement => PrintStatement(statement),
            TopLevelNode tl         => PrintTopLevel(tl),
            _                       => throw new NotImplementedException(
                                           "There's no ToGraphNode() method for type " + node.GetType() + " or any of its base types"
                                       )
        };

    public static string PrintTopLevel(TopLevelNode node) => TopLevelPrinter.Print(node);
    public static string PrintStatement(StatementNode node) => StatementPrinter.Print(node);
    public static string PrintValue(ValueNode node) => ValuePrinter.Print(node);
    public static string PrintTypeName(TypeDecName typeDec) => TopLevelPrinter.Print(typeDec);
    public static string PrintToken(Token token) => TokenPrinter.Print(token);

    public static GraphNode ToGraphNode(Token token) => TokenGraphMaker.ToGraphNode(token);
    public static GraphNode ToGraphNode(ValueNode node) => StatementGraphMaker.ToGraphNode(node);
    public static GraphNode ToGraphNode(StatementNode node) => StatementGraphMaker.ToGraphNode(node);
    public static GraphNode ToGraphNode(TopLevelNode node) => TopLevelGraphMaker.ToGraphNode(node);
    public static GraphNode ToGraphNode(TypeDecName typeDec) => TopLevelGraphMaker.ToGraphNode(typeDec);


    public static bool IsContant(ValueNode node) => ConstantChecker.IsContant(node);

    public static IEnumerable<ValueNode> ExtractValue(StatementNode node) => ValueExtractor.ExtractValue(node);
    public static IEnumerable<StatementNode> ExtractStatement(StatementNode node) => StatementExtractor.ExtractStatement(node);

    public static IEnumerable<StatementNode> Flatten(StatementNode node) => Flattener.Flatten(node);

    public static GraphNode ShowConstants(StatementNode node) => ConstantVisualizer.ShowConstants(node);
    public static GraphNode ShowConstants(ValueNode node) => ConstantVisualizer.ShowConstants(node);

    public static GraphNode ShowConstants(StatementNode node, string constantColor, string nonConstantColor)
        => new ConstantVisualizer(constantColor, nonConstantColor).ShowConstants(node);
    public static GraphNode ShowConstants(ValueNode node, string constantColor, string nonConstantColor)
        => new ConstantVisualizer(constantColor, nonConstantColor).ShowConstants(node);
}