namespace Lotus.Syntax.Visitors;

internal sealed class StatementPrinter : IStatementVisitor<string>
{
    public string Default(StatementNode node)
        => ASTUtils.PrintToken(node.Token);

    public string Visit(DeclarationNode node)
        => ASTUtils.PrintToken(node.Token)
         + ASTUtils.PrintToken(node.Name)
         + ASTUtils.PrintToken(node.EqualToken)
         + ASTUtils.PrintValue(node.Value);

    public string Visit(ElseNode node)
        => ASTUtils.PrintToken(node.Token)
         + node.BlockOrIfNode.Match(Print, Print);

    public string Visit(ForeachNode node)
        => ASTUtils.PrintToken(node.Token)
         + ASTUtils.PrintToken(node.OpeningParen)
         + ASTUtils.PrintValue(node.ItemName)
         + ASTUtils.PrintToken(node.InToken)
         + ASTUtils.PrintValue(node.CollectionRef)
         + ASTUtils.PrintToken(node.ClosingParen)
         + Print(node.Body);

    public string Visit(ForNode node)
        => ASTUtils.PrintToken(node.Token)
         + ASTUtils.PrintTuple(node.Header, ",", Print)
         + Print(node.Body);

    public string Visit(FunctionDeclarationNode node) {
        var output = ASTUtils.PrintToken(node.Token) + ASTUtils.PrintToken(node.FuncName) + ASTUtils.PrintToken(node.ParamList.OpeningToken);

        static string printParameter(FunctionParameter param) {
            var output = "";

            if (param.Type != ValueNode.NULL) output += ASTUtils.PrintValue(param.Type);

            output += ASTUtils.PrintValue(param.Name);

            if (param.HasDefaultValue) output += ASTUtils.PrintToken(param.EqualSign) + ASTUtils.PrintValue(param.DefaultValue);

            return output;
        }

        output += MiscUtils.Join(",", printParameter, node.ParamList.Items) + ASTUtils.PrintToken(node.ParamList.ClosingToken);

        if (node.HasReturnType) output += ASTUtils.PrintToken(node.ColonToken) + ASTUtils.PrintValue(node.ReturnType);

        output += Print(node.Body);

        return output;
    }

    public string Visit(IfNode node)
        => ASTUtils.PrintToken(node.Token)
         + ASTUtils.PrintValue(node.Condition)
         + Print(node.Body)
         + (node.HasElse ? Print(node.ElseNode!) : "");

    public string Visit(PrintNode node)
        => ASTUtils.PrintToken(node.Token) + ASTUtils.PrintValue(node.Value);

    public string Visit(ReturnNode node)
        => ASTUtils.PrintToken(node.Token)
         + (node.IsReturningValue ? ASTUtils.PrintValue(node.Value) : "");

    public string Visit(StatementExpressionNode node)
        => ASTUtils.PrintValue(node.Value);

    public string Visit(WhileNode node)
        => node.IsDoLoop ?
        // if it's a do loop
            ASTUtils.PrintToken(node.DoToken!)
              + Print(node.Body)
              + ASTUtils.PrintToken(node.Token)
              + ASTUtils.PrintValue(node.Condition)
        // else if it's a normal while loop
        :   ASTUtils.PrintToken(node.Token)
              + ASTUtils.PrintValue(node.Condition)
              + Print(node.Body);

    public string Print(Tuple<StatementNode> tuple)
        => ASTUtils.PrintTuple(tuple, "", (stmt) => Print(stmt) + (LotusFacts.NeedsSemicolon(stmt) ? ";" : ""));

    public string Print(StatementNode node) => node.Accept(this);
}