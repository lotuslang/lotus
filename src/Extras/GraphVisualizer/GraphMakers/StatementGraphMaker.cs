using Lotus.Syntax.Visitors;

namespace Lotus.Extras.Graphs;

internal sealed partial class GraphMaker : IStatementVisitor<GraphNode>
{
    private static readonly (string tooltip, string color) Break = ("break keyword", "");

    private static readonly (string tooltip, string color) Continue = ("continue keyword", "");

    private static readonly (string tooltip, string color) Declaration = ("DeclarationNode", "palegreen");
    private static readonly (string tooltip, string color) DeclarationName = ("variable name", "");

    private static readonly (string tooltip, string color) Else = ("else branch", "");

    private static readonly (string tooltip, string color) EmptyStatement = ("empty statement", "");

    private static readonly (string tooltip, string color) Foreach = ("foreach loop", "pink");

    private static readonly (string tooltip, string color) For = ("for loop", "");
    private static readonly (string tooltip, string color) ForHeader = ("for-loop header", "deepskyblue");

    private static readonly (string tooltip, string color) FuncDec = ("FunctionDeclarationNode", "indianred");
    private static readonly (string tooltip, string color) FuncDecParameters = ("parameters", "");

    private static readonly (string tooltip, string color) If = ("if statement", "");
    private static readonly (string tooltip, string color) IfCondition = ("if condition", "");

    private static readonly (string tooltip, string color) Print = ("PrintNode", "");

    private static readonly (string tooltip, string color) Return = ("return statement", "brown");

    private static readonly (string tooltip, string color) Statement = ("StatementNode", "black");

    private static readonly (string tooltip, string color) While = ("(do-)while loop", "pink");
    private static readonly (string tooltip, string color) WhileCondition = ("loop condition", "");

    private static readonly (string tooltip, string color) SimpleBlock = ("block", "darkviolet");

    public GraphNode Default(StatementNode node)
        => new GraphNode(node.Token.Representation)
            .SetColor(Statement.color)
            .SetTooltip(Statement.tooltip);

    public GraphNode Visit(StatementNode node)
        =>  node.Token.Kind == TokenKind.EOF
                ? new GraphNode("<EOF>").SetProperty("style", "invis")
                : Default(node);

    public GraphNode Visit(BreakNode node)
        => Default(node)
            .SetColor(Break.color)
            .SetTooltip(Break.tooltip);

    public GraphNode Visit(ContinueNode node)
        => Default(node)
            .SetColor(Continue.color)
            .SetTooltip(Continue.tooltip);

    public GraphNode Visit(DeclarationNode node)
        => new GraphNode("var") {
               ExtraUtils.ToGraphNode(node.Name)
                    .SetColor(DeclarationName.color)
                    .SetTooltip(DeclarationName.tooltip),
               ToGraphNode(node.Value)
           }.SetColor(Declaration.color)
            .SetTooltip(Declaration.tooltip);

    public GraphNode Visit(ElseNode node) {
        var root = new GraphNode("else")
            .SetColor(Else.color)
            .SetTooltip(Else.tooltip);

        if (node.BlockOrIfNode.Is<IfNode>(out var ifNode))
            root.Add(ToGraphNode(ifNode));
        else
            root.Add(ToCluster((Syntax.Tuple<StatementNode>)node.BlockOrIfNode));

        return root;
    }

    public GraphNode Visit(EmptyStatementNode node)
        => Default(node)
                .SetTooltip(EmptyStatement.tooltip)
                .SetColor(EmptyStatement.color);

    public GraphNode Visit(ForeachNode node)
        => new GraphNode("foreach") {
               new GraphNode("in") {
                    ToGraphNode(node.ItemName),
                    ToGraphNode(node.CollectionRef)
               }.SetTooltip("in iterator"),
               ToCluster(node.Body),
           }.SetColor(Foreach.color)
            .SetTooltip(Foreach.tooltip);

    public GraphNode Visit(ForNode node) {
        var root = new GraphNode("for loop")
                        .SetColor(For.color)
                        .SetTooltip(For.tooltip);

        var headerNode = new Graph("header")
            .SetGraphProp("color", ForHeader.color);

        foreach (var statement in node.Header) {
            if (statement.Token.Kind == TokenKind.EOF)
                headerNode.Add(new GraphNode("(empty)"));
            else
                headerNode.Add(ToGraphNode(statement));
        }

        root.Add(headerNode);

        root.Add(ToCluster(node.Body));

        return root;
    }

    public GraphNode Visit(IfNode node) {
        var root = new GraphNode("if") {
            new GraphNode("condition") {
                ToGraphNode(node.Condition)
            }.SetColor(IfCondition.color)
             .SetTooltip(IfCondition.tooltip),
            ToCluster(node.Body)
        }.SetColor(If.color)
         .SetTooltip(If.tooltip); // fixme(graph): Choose color

        if (node.HasElse) {
            root.Add(ToGraphNode(node.ElseNode));
        }

        return root;
    }

    public GraphNode Visit(PrintNode node)
        => new GraphNode("print") {
                ToGraphNode(node.Value)
            }.SetColor(Print.color)
             .SetTooltip(Print.tooltip); // fixme(graph): find color

    public GraphNode Visit(ReturnNode node) {
        var root = new GraphNode("return")
            .SetColor(Return.color)
            .SetTooltip(Return.tooltip);

        if (node.IsReturningValue) {
            root.Add(ToGraphNode(node.Value));
        }

        return root;
    }

    public GraphNode Visit(StatementExpressionNode node)
        => ToGraphNode(node.Value);

    public GraphNode Visit(WhileNode node)
        => new GraphNode(node.IsDoLoop ? "do-while" : "while") {
                new GraphNode("condition") {
                    ToGraphNode(node.Condition)
                }.SetColor(WhileCondition.color)
                 .SetTooltip(WhileCondition.tooltip),
                ToCluster(node.Body)
            }.SetColor(While.color)
             .SetTooltip(node.IsDoLoop ? "do-while loop" : "while loop");

    public Graph ToCluster(Syntax.Tuple<StatementNode> block) {
        // it would just call itself forever otherwise, and since it's purely based
        // on generics, there's no way to specify that we want the "simpler" version
        Graph ToClusterAsTuple<TVal>(Syntax.Tuple<TVal> tuple) where TVal : Node
            => ToCluster(tuple);

        return ToClusterAsTuple(block)
            .SetName(SimpleBlock.tooltip)
            .SetGraphProp("color", SimpleBlock.color);
    }

    public GraphNode ToGraphNode(StatementNode node) => node.Accept(this);
}