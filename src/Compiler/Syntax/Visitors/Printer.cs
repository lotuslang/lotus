namespace Lotus.Syntax.Visitors;

internal sealed partial class Printer
{
    public bool PrintTrivia { get; set; } = true;

    public string PrintModifiers(ImmutableArray<Token> modifiers)
        => String.Concat(modifiers.Select(Print));

    public string Print<T, U>(Union<T, U> u) where T : Node where U : Node
        => u.Match(Print, Print);

    public string Print<TVal>(Tuple<TVal> tuple) where TVal : ValueNode
        => Print(tuple.OpeningToken)
         + MiscUtils.Join(",", Print, tuple.Items)
         + Print(tuple.ClosingToken);

    public string PrintTuple<T>(Tuple<T> tuple, string sep, Func<T, string> transform) where T : ILocalized
        => Print(tuple.OpeningToken)
        +  MiscUtils.Join(sep, transform, tuple.Items)
        +  Print(tuple.ClosingToken);

    public string Print(Node node)
        => node switch {
            ValueNode value => Print(value),
            StatementNode statement => Print(statement),
            TopLevelNode tl => Print(tl),
            _ => throw new NotImplementedException(
                    $"There's no ToGraphNode() method for type {node.GetType().GetDisplayName()} or any of its base types"
                )
        };
}