internal sealed class StatementExtractor : IStatementVisitor<IEnumerable<StatementNode>>
{
    private static readonly StatementNode[] _empty = Array.Empty<StatementNode>();

    public IEnumerable<StatementNode> Default(StatementNode _)
        => _empty;

    public IEnumerable<StatementNode> Visit(ElseNode node)
        => Visit(node.Body);

    public IEnumerable<StatementNode> Visit(ForeachNode node)
        => Visit(node.Body);

    public IEnumerable<StatementNode> Visit(FunctionDeclarationNode node)
        => ExtractStatement(node.Body);

    public IEnumerable<StatementNode> Visit(IfNode node)
        => node.HasElse ? Visit(node.Body).Concat(ExtractStatement(node.ElseNode!)) : Visit(node.Body);

    public IEnumerable<StatementNode> Visit(WhileNode node)
        => Visit(node.Body);

    // TODO: Find an easier/clearer/faster way to do this
    public IEnumerable<StatementNode> Visit(SimpleBlock block)
        => block.Content;

    public IEnumerable<StatementNode> ExtractStatement(SimpleBlock block) => Visit(block);
    public IEnumerable<StatementNode> ExtractStatement(StatementNode node) => node.Accept(this);
}