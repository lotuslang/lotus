namespace Lotus.Syntax.Visitors;

internal sealed partial class Printer : IStatementVisitor<string>
{
    public string Default(StatementNode node)
        => Print(node.Token);

    public string Visit(DeclarationNode node)
        => Print(node.Token)
         + Print(node.Name)
         + Print(node.EqualToken)
         + Print(node.Value);

    public string Visit(ElseNode node)
        => Print(node.Token)
         + node.BlockOrIfNode.Match(Print, Print);

    public string Visit(ForeachNode node)
        => Print(node.Token)
         + Print(node.OpeningParen)
         + Print(node.ItemName)
         + Print(node.InToken)
         + Print(node.CollectionRef)
         + Print(node.ClosingParen)
         + Print(node.Body);

    public string Visit(ForNode node)
        => Print(node.Token)
         + PrintTuple(node.Header, ",", stmt => stmt.Accept(this)) // we can't use Print(stmt) since it would insert semicolons
         + Print(node.Body);

    public string Visit(FunctionDeclarationNode node) {
        var output = Print(node.Token) + Print(node.FuncName) + Print(node.ParamList.OpeningToken);

        output += MiscUtils.Join(",", Print, node.ParamList.Items) + Print(node.ParamList.ClosingToken);

        if (node.HasReturnType) output += Print(node.ColonToken) + Print(node.ReturnType);

        output += Print(node.Body);

        return output;
    }

    public string Visit(IfNode node)
        => Print(node.Token)
         + Print(node.Condition)
         + Print(node.Body)
         + (node.HasElse ? Print(node.ElseNode!) : "");

    public string Visit(PrintNode node)
        => Print(node.Token) + Print(node.Value);

    public string Visit(ReturnNode node)
        => Print(node.Token)
         + (node.IsReturningValue ? Print(node.Value) : "");

    public string Visit(StatementExpressionNode node)
        => Print(node.Value);

    public string Visit(WhileNode node)
        => node.IsDoLoop ?
        // if it's a do loop
            Print(node.DoToken!)
              + Print(node.Body)
              + Print(node.Token)
              + Print(node.Condition)
        // else if it's a normal while loop
        :   Print(node.Token)
              + Print(node.Condition)
              + Print(node.Body);

    string Print(FunctionParameter param) {
        var output = "";

        if (param.Type != ValueNode.NULL) output += Print(param.Type);

        output += Print(param.Name);

        if (param.HasDefaultValue) output += Print(param.EqualSign) + Print(param.DefaultValue);

        return output;
    }

    public string Print(Tuple<StatementNode> tuple)
        => PrintTuple(tuple, "", Print);

    public string Print(StatementNode node) => node.Accept(this) + (LotusFacts.NeedsSemicolon(node) ? ";" : "");
}