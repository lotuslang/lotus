internal sealed class StatementPrinter : IStatementVisitor<string>
{
    public string Default(StatementNode node)
        => ASTHelper.PrintToken(node.Token);

    public string Visit(DeclarationNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintToken(node.Name)
         + ASTHelper.PrintToken(node.EqualToken)
         + ASTHelper.PrintValue(node.Value);

    public string Visit(ElseNode node)
        => ASTHelper.PrintToken(node.Token)
         + node.BlockOrIfNode.Match(Print, Print);

    public string Visit(ForeachNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintToken(node.OpeningParen)
         + ASTHelper.PrintValue(node.ItemName)
         + ASTHelper.PrintToken(node.InToken)
         + ASTHelper.PrintValue(node.CollectionRef)
         + ASTHelper.PrintToken(node.ClosingParen)
         + Print(node.Body);

    public string Visit(ForNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintTuple(node.Header, ",", Print)
         + Print(node.Body);

    public string Visit(FunctionDeclarationNode node) {
        var output = ASTHelper.PrintToken(node.Token) + ASTHelper.PrintToken(node.FuncName) + ASTHelper.PrintToken(node.ParamList.OpeningToken);

        static string printParameter(FunctionParameter param) {
            var output = "";

            if (param.Type != ValueNode.NULL) output += ASTHelper.PrintValue(param.Type);

            output += ASTHelper.PrintValue(param.Name);

            if (param.HasDefaultValue) output += ASTHelper.PrintToken(param.EqualSign) + ASTHelper.PrintValue(param.DefaultValue);

            return output;
        }

        output += Utils.Join(",", printParameter, node.ParamList.Items) + ASTHelper.PrintToken(node.ParamList.ClosingToken);

        if (node.HasReturnType) output += ASTHelper.PrintToken(node.ColonToken) + ASTHelper.PrintValue(node.ReturnType);

        output += Print(node.Body);

        return output;
    }

    public string Visit(IfNode node)
        => ASTHelper.PrintToken(node.Token)
         + ASTHelper.PrintValue(node.Condition)
         + Print(node.Body)
         + (node.HasElse ? Print(node.ElseNode!) : "");

    public string Visit(PrintNode node)
        => ASTHelper.PrintToken(node.Token) + ASTHelper.PrintValue(node.Value);

    public string Visit(ReturnNode node)
        => ASTHelper.PrintToken(node.Token)
         + (node.IsReturningValue ? ASTHelper.PrintValue(node.Value) : "");

    public string Visit(StatementExpressionNode node)
        => ASTHelper.PrintValue(node.Value);

    public string Visit(WhileNode node)
        => node.IsDoLoop ?
        // if it's a do loop
            ASTHelper.PrintToken(node.DoToken!)
              + Print(node.Body)
              + ASTHelper.PrintToken(node.Token)
              + ASTHelper.PrintValue(node.Condition)
        // else if it's a normal while loop
        :   ASTHelper.PrintToken(node.Token)
              + ASTHelper.PrintValue(node.Condition)
              + Print(node.Body);

    public string Print(Tuple<StatementNode> tuple)
        => ASTHelper.PrintTuple(tuple, "", (stmt) => Print(stmt) + (Utils.NeedsSemicolon(stmt) ? ";" : ""));

    public string Print(StatementNode node) => node.Accept(this);
}