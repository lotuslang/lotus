namespace Lotus.Syntax.Visitors;

internal sealed class TopLevelPrinter : ITopLevelVisitor<string>
{
    public string Default(TopLevelNode node)
        => ASTUtils.PrintToken(node.Token);

    public string Visit(FromNode node)
        => ASTUtils.PrintToken(node.Token)
         + ASTUtils.PrintUnion(node.OriginName);

    public string Visit(TopLevelStatementNode node)
        => ASTUtils.PrintStatement(node.Statement);

    public string Visit(EnumNode node)
        => MiscUtils.Join(" ", ASTUtils.PrintToken, node.Modifiers)
         + ASTUtils.PrintToken(node.EnumToken)
         + Print(node.Name)
         + ASTUtils.PrintTuple(node.Values, ",", ASTUtils.PrintValue);

    public string Visit(ImportNode node)
        => Visit(node.FromStatement)
         + ASTUtils.PrintToken(node.Token)
         + MiscUtils.Join(",", ASTUtils.PrintValue, node.Names);

    public string Visit(NamespaceNode node)
        => MiscUtils.Join(" ", ASTUtils.PrintToken, node.Modifiers) + ASTUtils.PrintToken(node.Token) + ASTUtils.PrintValue(node.Name);

    public string Visit(UsingNode node)
        => ASTUtils.PrintToken(node.Token) + ASTUtils.PrintUnion(node.Name);

    public string Visit(StructNode node)
        => MiscUtils.Join(" ", ASTUtils.PrintToken, node.Modifiers)
         + ASTUtils.PrintToken(node.Token)
         + ASTUtils.PrintTypeName(node.Name)
         + ASTUtils.PrintToken(node.Fields.OpeningToken)
         + MiscUtils.Join(
                "; ",
                (field) => ASTUtils.PrintValue(field.Name)
                         + ": "
                         + ASTUtils.PrintValue(field.Type)
                         + ASTUtils.PrintToken(field.EqualSign)
                         + ASTUtils.PrintValue(field.DefaultValue),
                node.Fields.Items
            )
         + (node.Fields.Count != 0 ? ";" : "")
         + ASTUtils.PrintToken(node.Fields.ClosingToken);

    public string Visit(TypeDecName typeDec)
        => !typeDec.HasParent
                ? ASTUtils.PrintValue(typeDec.TypeName)
                : ASTUtils.PrintValue(typeDec.Parent)
                    + ASTUtils.PrintToken(typeDec.ColonToken)
                    + ASTUtils.PrintValue(typeDec.TypeName);

    public string Print(TypeDecName typeDec) => Visit(typeDec);
    public string Print(TopLevelNode node) => node.Accept(this);
}