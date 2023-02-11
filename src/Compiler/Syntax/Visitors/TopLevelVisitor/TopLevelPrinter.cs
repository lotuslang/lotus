namespace Lotus.Syntax.Visitors;

internal sealed class TopLevelPrinter : ITopLevelVisitor<string>
{
    public string Default(TopLevelNode node)
        => ASTUtils.PrintToken(node.Token);

    public string Visit(TopLevelStatementNode node)
        => ASTUtils.PrintStatement(node.Statement);

    public string Visit(EnumNode node)
        => MiscUtils.Join(" ", ASTUtils.PrintToken, node.Modifiers)
         + ASTUtils.PrintToken(node.EnumToken)
         + Print(node.Name)
         + ASTUtils.PrintTuple(node.Values, ",", ASTUtils.PrintValue);

    public string Visit(ImportNode node)
        => ASTUtils.PrintToken(node.Token)
         + ASTUtils.PrintToken(node.Names.OpeningToken)
         + MiscUtils.Join(",", ASTUtils.PrintValue, node.Names)
         + ASTUtils.PrintToken(node.Names.ClosingToken)
         + (node.FromOrigin is null ? "" : Visit(node.FromOrigin));

    public string Visit(NamespaceNode node)
        => MiscUtils.Join(" ", ASTUtils.PrintToken, node.Modifiers) + ASTUtils.PrintToken(node.Token) + ASTUtils.PrintValue(node.Name);

    public string Visit(UsingNode node)
        => ASTUtils.PrintToken(node.Token) + ASTUtils.PrintUnion(node.Name);

    public string Visit(StructNode node)
        => MiscUtils.Join(" ", ASTUtils.PrintToken, node.Modifiers)
         + ASTUtils.PrintToken(node.Token)
         + ASTUtils.PrintNode(node.Name)
         + ASTUtils.PrintToken(node.Fields.OpeningToken)
         + MiscUtils.Join(
                "; ",
                (field) => ASTUtils.PrintValue(field.Name)
                         + ": "
                         + ASTUtils.PrintValue(field.Type)
                         + ASTUtils.PrintToken(field.EqualSign ?? Token.NULL)
                         + ASTUtils.PrintValue(field.DefaultValue ?? ValueNode.NULL),
                node.Fields.Items
            )
         + (node.Fields.Count != 0 ? ";" : "")
         + ASTUtils.PrintToken(node.Fields.ClosingToken);

    public string Visit(FromOrigin node)
        => ASTUtils.PrintToken(node.FromToken)
         + ASTUtils.PrintUnion(node.OriginName);

    public string Print(TypeDecName typeDec)
        => !typeDec.HasParent
                ? ASTUtils.PrintValue(typeDec.TypeName)
                : ASTUtils.PrintValue(typeDec.Parent)
                    + ASTUtils.PrintToken(typeDec.ColonToken)
                    + ASTUtils.PrintValue(typeDec.TypeName);

    public string Print(TopLevelNode node) => node.Accept(this);
}