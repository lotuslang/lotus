internal sealed class TopLevelPrinter : ITopLevelVisitor<string>
{
    public string Default(TopLevelNode node)
        => ASTHelper.PrintToken(node.Token);

    public string Visit(FromNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintUnion(node.OriginName);

    public string Visit(TopLevelStatementNode node)
        => ASTHelper.PrintStatement(node.Statement);

    public string Visit(EnumNode node)
        => ASTHelper.PrintToken(node.AccessToken)
         + ASTHelper.PrintToken(node.EnumToken)
         + Print(node.Name)
         + ASTHelper.PrintTuple(node.Values, ",", ASTHelper.PrintValue);

    public string Visit(ImportNode node)
        => Visit(node.FromStatement)
         + ASTHelper.PrintToken(node.Token)
         + Utilities.Join(",", ASTHelper.PrintValue, node.Names);

    public string Visit(NamespaceNode node)
        => ASTHelper.PrintToken(node.AccessToken) + ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.Name);

    public string Visit(UsingNode node)
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintUnion(node.Name);

    public string Visit(StructNode node)
        => ASTHelper.PrintToken(node.AccessToken)
         + ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintTypeName(node.Name)
         + ASTHelper.PrintToken(node.Fields.OpeningToken)
         + Utilities.Join("; ", coll: node.Fields.Items, convert:
                (field) => ASTHelper.PrintValue(field.Name)
                         + ": "
                         + ASTHelper.PrintValue(field.Type)
                         + ASTHelper.PrintToken(field.EqualSign)
                         + ASTHelper.PrintValue(field.DefaultValue)
           )
         + (node.Fields.Count != 0 ? ";" : "")
         + ASTHelper.PrintToken(node.Fields.ClosingToken);

    public string Visit(TypeDecName typeDec)
        => !typeDec.HasParent
                ? ASTHelper.PrintValue(typeDec.TypeName)
                : ASTHelper.PrintValue(typeDec.Parent)
                    + ASTHelper.PrintToken(typeDec.ColonToken)
                    + ASTHelper.PrintValue(typeDec.TypeName);

    public string Print(TypeDecName typeDec) => Visit(typeDec);
    public string Print(TopLevelNode node) => node.Accept(this);
}