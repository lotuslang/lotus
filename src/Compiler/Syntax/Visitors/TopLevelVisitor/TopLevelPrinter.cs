namespace Lotus.Syntax.Visitors;

internal sealed partial class Printer : ITopLevelVisitor<string>
{
    public string Default(TopLevelNode node)
        => Print(node.Token);

    public string Visit(TopLevelStatementNode node)
        => Print(node.Statement);

    public string Visit(EnumNode node)
        => MiscUtils.Join(" ", Print, node.Modifiers)
         + Print(node.EnumToken)
         + Print(node.Name)
         + PrintTuple(node.Values, ",", Print);

    public string Visit(ImportNode node)
        => Print(node.Token)
         + Print(node.Names.OpeningToken)
         + MiscUtils.Join(",", Print, node.Names)
         + Print(node.Names.ClosingToken)
         + (node.FromOrigin is null ? "" : Print(node.FromOrigin));

    public string Visit(NamespaceNode node)
        => MiscUtils.Join(" ", Print, node.Modifiers) + Print(node.Token) + Print(node.Name);

    public string Visit(UsingNode node)
        => Print(node.Token) + Print(node.Name);

    public string Visit(StructNode node)
        => MiscUtils.Join(" ", Print, node.Modifiers)
         + Print(node.Token)
         + Print(node.Name)
         + Print(node.Fields.OpeningToken)
         + MiscUtils.Join(
                "; ",
                (field) => Print(field.Name)
                         + ": "
                         + Print(field.Type)
                         + Print(field.EqualSign ?? Token.NULL)
                         + Print(field.DefaultValue ?? ValueNode.NULL),
                node.Fields.Items
            )
         + (node.Fields.Count != 0 ? ";" : "")
         + Print(node.Fields.ClosingToken);

    public string Print(FromOrigin node)
        => Print(node.FromToken)
         + Print(node.OriginName);

    public string Print(TypeDecName typeDec)
        => !typeDec.HasParent
                ? Print(typeDec.TypeName)
                : Print(typeDec.Parent)
                    + Print(typeDec.ColonToken)
                    + Print(typeDec.TypeName);

    public string Print(Tuple<TopLevelNode> tuple)
        => PrintTuple(tuple, "", Print);

    public string Print(TopLevelNode node)
        => node.Accept(this) + (LotusFacts.NeedsSemicolon(node) ? ";" : "");
}