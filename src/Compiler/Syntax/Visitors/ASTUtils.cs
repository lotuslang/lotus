using Lotus.Syntax.Visitors;

namespace Lotus.Syntax;

internal static class ASTUtils
{
    internal static readonly TopLevelPrinter TopLevelPrinter = new();

    internal static readonly StatementPrinter StatementPrinter = new();

    internal static readonly ValuePrinter ValuePrinter = new();

    internal static readonly TokenPrinter TokenPrinter = new();

    internal static readonly ConstantChecker ConstantChecker = new();

    // todo(utils): add a way to print nodes/tokens without trivia

    public static string PrintNode(Node node)
        => node switch {
            ValueNode value => PrintValue(value),
            StatementNode statement => PrintStatement(statement),
            TopLevelNode tl => PrintTopLevel(tl),
            _ => throw new NotImplementedException(
                    $"There's no ToGraphNode() method for type {node.GetType()} or any of its base types"
                )
        };

    public static string PrintTuple<T>(Tuple<T> tuple, string sep, Func<T, string> transform) where T : ILocalized
        => PrintToken(tuple.OpeningToken) + MiscUtils.Join(sep, transform, tuple.Items) + PrintToken(tuple.ClosingToken);

    public static string PrintTopLevel(TopLevelNode node) => TopLevelPrinter.Print(node);
    public static string PrintStatement(StatementNode node) => StatementPrinter.Print(node);
    public static string PrintValue(ValueNode node) => ValuePrinter.Print(node);
    public static string PrintTypeName(TypeDecName typeDec) => TopLevelPrinter.Print(typeDec);
    public static string PrintToken(Token token) => TokenPrinter.Print(token);
    public static string PrintUnion<T, U>(Union<T, U> u) where T : Node where U : Node
        => u.Match(PrintNode, PrintNode);

    public static bool IsContant(ValueNode node) => ConstantChecker.IsContant(node);
}