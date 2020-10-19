public sealed class StatementPrinter : NodeVisitor<string>
{
    protected override string Default(StatementNode node) => ASTHelper.PrintToken(node.Token);

    protected override string Default(ValueNode node) => ASTHelper.PrintValue(node);

    public override string Visit(DeclarationNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintToken(node.Name)
         + ASTHelper.PrintToken(node.EqualToken)
         + ASTHelper.PrintValue(node.Value);

    public override string Visit(ElseNode node)
        => ASTHelper.PrintToken(node.Token)
         + (node.HasIf ? Print(node.IfNode!) : Visit(node.Body));

    public override string Visit(ForeachNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintToken(node.OpeningParenthesis)
         + ASTHelper.PrintValue(node.ItemName)
         + ASTHelper.PrintToken(node.InToken)
         + ASTHelper.PrintValue(node.Collection)
         + ASTHelper.PrintToken(node.ClosingParenthesis)
         + Visit(node.Body);

    public override string Visit(ForNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintToken(node.OpeningParenthesis)
         + Utilities.Join(",", Print, node.Header)
         + ASTHelper.PrintToken(node.ClosingParenthesis)
         + Visit(node.Body);

    public override string Visit(FromNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintValue(node.OriginName);

    public override string Visit(FunctionDeclarationNode node) {
        var output = ASTHelper.PrintToken(node.Token) + ASTHelper.PrintToken(node.Name) + ASTHelper.PrintToken(node.OpeningParenthesis);

        static string printParameter(FunctionParameter param) {
            var output = "";

            if (param.Type != ValueNode.NULL) output += ASTHelper.PrintValue(param.Type);

            output += ASTHelper.PrintValue(param.Name);

            if (param.HasDefaultValue) output += ASTHelper.PrintToken(param.EqualSign) + ASTHelper.PrintValue(param.DefaultValue);

            return output;
        }

        output += Utilities.Join(",", printParameter, node.Parameters) + ASTHelper.PrintToken(node.ClosingParenthesis);

        if (node.HasReturnType) output += ASTHelper.PrintToken(node.ColonToken) + ASTHelper.PrintValue(node.ReturnType);

        output += Visit(node.Body);

        return output;
    }

    public override string Visit(IfNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintValue(node.Condition)
         + Visit(node.Body)
         + (node.HasElse ? Print(node.ElseNode!) : "");

    public override string Visit(ImportNode node)
        => Visit(node.FromStatement)
         + ASTHelper.PrintToken(node.Token)
         + Utilities.Join(",", ASTHelper.PrintValue, node.ImportsName);

    public override string Visit(NamespaceNode node)
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.NamespaceName);

    public override string Visit(PrintNode node)
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.Value);

    public override string Visit(ReturnNode node)
        => ASTHelper.PrintToken(node.Token)
         + (node.IsReturningValue ? ASTHelper.PrintValue(node.Value) : "");

    public override string Visit(UsingNode node)
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.ImportName);

    public override string Visit(WhileNode node)
        => node.IsDoLoop ?
        // if it's a do loop
            ASTHelper.PrintToken(node.DoToken!)
              + Visit(node.Body)
              + ASTHelper.PrintToken(node.Token)
              + ASTHelper.PrintValue(node.Condition)
        // else if it's a normal while loop
        :   ASTHelper.PrintToken(node.Token)
              + ASTHelper.PrintValue(node.Condition)
              + Visit(node.Body);


    public override string Visit(SimpleBlock block)
        => (!block.IsOneLiner ? ASTHelper.PrintToken(block.OpeningToken) : "")
         + Utilities.Join("", Print, block.Content)
         + (!block.IsOneLiner ? ASTHelper.PrintToken(block.ClosingToken) : "");

    public string Print(StatementNode node) => node.Accept(this);
}