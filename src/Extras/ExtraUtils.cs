using Lotus.Extras.Graphs;
using Lotus.Syntax.Visitors;

namespace Lotus.Extras;

public static class ExtraUtils
{
    internal static readonly TokenGraphMaker TokenGraphMaker = new();

    internal static readonly StatementGraphMaker StatementGraphMaker = new();

    internal static readonly TopLevelGraphMaker TopLevelGraphMaker = new();

    internal static readonly ConstantVisualizer ConstantVisualizer = new();

    public static string PrintNode(Node node) => ASTUtils.PrintNode(node);

    public static string PrintTuple<T>(Syntax.Tuple<T> tuple, string sep, Func<T, string> transform)
        => ASTUtils.PrintTuple<T>(tuple, sep, transform);

    public static string PrintTopLevel(TopLevelNode node) => ASTUtils.PrintNode(node);
    public static string PrintStatement(StatementNode node) => ASTUtils.PrintNode(node);
    public static string PrintValue(ValueNode node) => ASTUtils.PrintNode(node);
    public static string PrintTypeName(TypeDecName typeDec) => ASTUtils.PrintTypeName(typeDec);
    public static string PrintToken(Token token) => ASTUtils.PrintToken(token);
    public static string PrintUnion<T, U>(Union<T, U> u) where T : Node
                                                         where U : Node
        => ASTUtils.PrintUnion(u);

    public static GraphNode ToGraphNode(Token token) => TokenGraphMaker.ToGraphNode(token);
    public static GraphNode ToGraphNode(ValueNode node) => StatementGraphMaker.ToGraphNode(node);
    public static GraphNode ToGraphNode(StatementNode node) => StatementGraphMaker.ToGraphNode(node);
    public static GraphNode ToGraphNode(TopLevelNode node) => TopLevelGraphMaker.ToGraphNode(node);
    public static GraphNode ToGraphNode(TypeDecName typeDec) => TopLevelGraphMaker.ToGraphNode(typeDec);
    public static GraphNode UnionToGraphNode<T, U>(Union<T, U> u) where T : Node
                                                                  where U : Node
        => u.Match(ToGraphNode, ToGraphNode);
    public static GraphNode ToGraphNode(Node node)
        => node switch {
            ValueNode vn     => ToGraphNode(vn),
            StatementNode sn => ToGraphNode(sn),
            TopLevelNode tn  => ToGraphNode(tn),
            _                => throw new NotImplementedException(
                                    "There's no ToGraphNode() method for type " + node.GetType() + " or any of its base types"
                                )
        };

    public static GraphNode ShowConstants(StatementNode node) => ConstantVisualizer.ShowConstants(node);
    public static GraphNode ShowConstants(ValueNode node) => ConstantVisualizer.ShowConstants(node);

    public static GraphNode ShowConstants(StatementNode node, string constantColor, string nonConstantColor)
        => new ConstantVisualizer(constantColor, nonConstantColor).ShowConstants(node);
    public static GraphNode ShowConstants(ValueNode node, string constantColor, string nonConstantColor)
        => new ConstantVisualizer(constantColor, nonConstantColor).ShowConstants(node);
}