public sealed class TopLevelPrinter : TopLevelVisitor<string>
{
    protected override string Default(TopLevelNode node) => ASTHelper.PrintToken(node.Token);


    public override string Visit(FromNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintValue(node.OriginName);

    public override string Visit(TopLevelStatementNode node)
        => ASTHelper.PrintStatement(node.Statement);

    public override string Visit(ImportNode node)
        => Visit(node.FromStatement)
         + ASTHelper.PrintToken(node.Token)
         + Utilities.Join(",", ASTHelper.PrintValue, node.ImportsName);

    public override string Visit(NamespaceNode node)
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.NamespaceName);

    public override string Visit(UsingNode node)
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.ImportName);


    public string Print(TopLevelNode node) => node.Accept(this);
}