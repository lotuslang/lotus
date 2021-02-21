using System;
using System.Linq;
using System.Collections.Generic;

public sealed class StatementExtractor : StatementVisitor<IEnumerable<StatementNode>>
{
    private static readonly StatementNode[] emptyArray = new StatementNode[0];

    protected override IEnumerable<StatementNode> Default(StatementNode node)
        => emptyArray;

    protected override IEnumerable<StatementNode> Default(ValueNode node)
        => emptyArray;

    public override IEnumerable<StatementNode> Visit(ElseNode node)
        => node.HasIf ? Visit(node.IfNode!.Body) : Visit(node.Body);

    public override IEnumerable<StatementNode> Visit(ForeachNode node)
        => Visit(node.Body);

    public override IEnumerable<StatementNode> Visit(IfNode node)
        => node.HasElse ? Visit(node.Body).Concat(ExtractStatement(node.ElseNode!)) : Visit(node.Body);

    public override IEnumerable<StatementNode> Visit(WhileNode node)
        => Visit(node.Body);

    // TODO: Find an easier/clearer/faster way to do this
    public override IEnumerable<StatementNode> Visit(SimpleBlock block)
        => block.Content;

    public IEnumerable<StatementNode> ExtractStatement(StatementNode node) => node.Accept(this);
}