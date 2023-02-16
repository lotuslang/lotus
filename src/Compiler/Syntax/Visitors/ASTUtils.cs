using Lotus.Syntax.Visitors;

namespace Lotus.Syntax;

internal static class ASTUtils
{
    internal static readonly Printer Printer = new();
    internal static readonly Printer NoTriviaPrinter = new() { PrintTrivia = false };

    public static string PrintNode(Node node) => PrintNode(node, true);
    public static string PrintNode(Node node, bool printTrivia)
        => node switch {
            ValueNode value => PrintValue(value, printTrivia),
            StatementNode statement => PrintStatement(statement, printTrivia),
            TopLevelNode tl => PrintTopLevel(tl, printTrivia),
            _ => throw new NotImplementedException(
                    $"There's no ToGraphNode() method for type {node.GetType().GetDisplayName()} or any of its base types"
                )
        };

    public static string PrintTopLevel(TopLevelNode node) => PrintTopLevel(node, true);
    public static string PrintTopLevel(TopLevelNode node, bool printTrivia)
        => printTrivia
            ? Printer.Print(node)
            : NoTriviaPrinter.Print(node);

    public static string PrintStatement(StatementNode node) => PrintStatement(node, true);
    public static string PrintStatement(StatementNode node, bool printTrivia)
        => printTrivia
            ? Printer.Print(node)
            : NoTriviaPrinter.Print(node);

    public static string PrintValue(ValueNode node) => PrintValue(node, true);
    public static string PrintValue(ValueNode node, bool printTrivia)
        => printTrivia
            ? Printer.Print(node)
            : NoTriviaPrinter.Print(node);

    public static string PrintTypeName(TypeDecName typeDec) => PrintTypeName(typeDec, true);
    public static string PrintTypeName(TypeDecName typeDec, bool printTrivia)
        => printTrivia
            ? Printer.Print(typeDec)
            : NoTriviaPrinter.Print(typeDec);

    public static string PrintToken(Token token) => PrintToken(token, true);
    public static string PrintToken(Token token, bool printTrivia)
        => printTrivia
            ? Printer.Print(token)
            : NoTriviaPrinter.Print(token);

    public static string PrintUnion<T, U>(Union<T, U> u, bool printTrivia = true) where T : Node where U : Node
        => u.Match(t => PrintNode(t, printTrivia), u => PrintNode(u, printTrivia));

    public static string PrintTuple<T>(Tuple<T> tuple, string sep, Func<T, string> transform) where T : ILocalized
        => PrintTuple(tuple, sep, (t, _) => transform(t), true);
    public static string PrintTuple<T>(Tuple<T> tuple, string sep, Func<T, bool, string> transform, bool printTrivia = true) where T : ILocalized
        => PrintToken(tuple.OpeningToken, printTrivia)
        +  MiscUtils.Join(sep, n => transform(n, printTrivia), tuple.Items)
        +  PrintToken(tuple.ClosingToken, printTrivia);
}