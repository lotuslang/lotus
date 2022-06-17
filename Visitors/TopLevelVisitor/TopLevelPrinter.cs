internal sealed class TopLevelPrinter : ITopLevelVisitor<string>
{
    public string Default(TopLevelNode node) => ASTHelper.PrintToken(node.Token);


    public string Visit(FromNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintValue(node.OriginName);

    public string Visit(TopLevelStatementNode node)
        => ASTHelper.PrintStatement(node.Statement);

    public string Visit(EnumNode node) {
        var s = "";

        if (node.AccessKeyword != Token.NULL) {
            s = ASTHelper.PrintToken(node.AccessKeyword);
        }

        s += ASTHelper.PrintToken(node.EnumToken);

        return s + Print(node.Name)
                 + ASTHelper.PrintToken(node.OpenBracket)
                 + String.Join(",", node.Values.Select(ASTHelper.PrintNode))
                 + ASTHelper.PrintToken(node.CloseBracket);
    }


    public string Visit(ImportNode node)
        => Visit(node.FromStatement)
         + ASTHelper.PrintToken(node.Token)
         + Utilities.Join(",", ASTHelper.PrintValue, node.Names);

    public string Visit(NamespaceNode node)
        // FIXME: Add AccMods as well
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.Name);

    public string Visit(UsingNode node)
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.Name);

    public string Visit(TypeDecName typeDec)
            => typeDec.Parent == ValueNode.NULL
                    ? ASTHelper.PrintNode(typeDec.TypeName)
                    : ASTHelper.PrintNode(typeDec.Parent)
                        + ASTHelper.PrintToken(typeDec.ColonToken)
                        + ASTHelper.PrintNode(typeDec.TypeName);


    public string Print(TypeDecName typeDec) => Visit(typeDec);

    public string Print(TopLevelNode node) => node.Accept(this);
}