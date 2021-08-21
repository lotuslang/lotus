using System;
using System.Linq;
using System.Collections.Generic;

internal sealed class StatementExtractor : IStatementVisitor<IEnumerable<StatementNode>>
{
    private static readonly StatementNode[] emptyArray = Array.Empty<StatementNode>();

    public IEnumerable<StatementNode> Default(StatementNode _)
        => emptyArray;

    public IEnumerable<StatementNode> Visit(ElseNode node)
        => node.HasIf ? Visit(node.IfNode!.Body) : Visit(node.Body);

    public IEnumerable<StatementNode> Visit(ForeachNode node)
        => Visit(node.Body);

    public IEnumerable<StatementNode> Visit(IfNode node)
        => node.HasElse ? Visit(node.Body).Concat(ExtractStatement(node.ElseNode!)) : Visit(node.Body);

    public IEnumerable<StatementNode> Visit(WhileNode node)
        => Visit(node.Body);

    // TODO: Find an easier/clearer/faster way to do this
    public IEnumerable<StatementNode> Visit(SimpleBlock block)
        => block.Content;

    public IEnumerable<StatementNode> ExtractStatement(StatementNode node) => node.Accept(this);
}