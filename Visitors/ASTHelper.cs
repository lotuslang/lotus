using System.Collections.Generic;

public static class ASTHelper
{
    public static readonly NameChecker NameChecker = new NameChecker();

    public static readonly StatementPrinter StatementPrinter = new StatementPrinter();

    public static readonly ValuePrinter ValuePrinter = new ValuePrinter();

    public static readonly TokenPrinter TokenPrinter = new TokenPrinter();

    public static readonly TokenGraphMaker TokenGraphMaker = new TokenGraphMaker();

    public static readonly NodeGraphMaker NodeGraphMaker = new NodeGraphMaker();

    public static readonly ConstantChecker ConstantChecker = new ConstantChecker();

    public static readonly ValueExtractor ValueExtractor = new ValueExtractor();

    public static readonly StatementExtractor StatementExtractor = new StatementExtractor();

    public static readonly Flattener Flattener = new Flattener();

    public static readonly ConstantVisualizer ConstantVisualizer = new ConstantVisualizer();


    public static bool IsName(StatementNode node) => NameChecker.IsName(node);

    public static string PrintStatement(StatementNode node) => StatementPrinter.Print(node);

    public static string PrintValue(ValueNode node) => ValuePrinter.Print(node);

    public static string PrintToken(Token token) => TokenPrinter.Print(token);

    public static GraphNode ToGraphNode(Token token) => TokenGraphMaker.ToGraphNode(token);

    public static GraphNode ToGraphNode(StatementNode node) => NodeGraphMaker.ToGraphNode(node);

    public static bool IsContant(ValueNode node) => ConstantChecker.IsContant(node);

    public static IEnumerable<ValueNode> ExtractValue(StatementNode node) => ValueExtractor.ExtractValue(node);

    public static IEnumerable<StatementNode> ExtractStatement(StatementNode node) => StatementExtractor.ExtractStatement(node);

    public static IEnumerable<StatementNode> Flatten(StatementNode node) => Flattener.Flatten(node);

    public static GraphNode ShowConstants(StatementNode node) => ConstantVisualizer.ShowConstants(node);

    public static GraphNode ShowConstants(StatementNode node, string constantColor, string nonConstantColor)
        => new ConstantVisualizer(constantColor, nonConstantColor).ShowConstants(node);
}