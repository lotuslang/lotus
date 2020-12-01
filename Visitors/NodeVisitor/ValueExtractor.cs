using System;
using System.Linq;
using System.Collections.Generic;

public sealed class ValueExtractor : NodeVisitor<IEnumerable<ValueNode>>
{
    private static readonly ValueNode[] emptyArray = new ValueNode[0];

    protected override IEnumerable<ValueNode> Default(StatementNode node)
        => emptyArray;

    protected override IEnumerable<ValueNode> Default(ValueNode node)
        => emptyArray;


    public override IEnumerable<ValueNode> Visit(DeclarationNode node)
        => new[] { node.Value };

    public override IEnumerable<ValueNode> Visit(ElseNode node)
        => node.HasIf ? ExtractValue(node.IfNode!) : emptyArray;

    public override IEnumerable<ValueNode> Visit(ForeachNode node)
        => new[] { node.ItemName, node.Collection };

    public override IEnumerable<ValueNode> Visit(IfNode node)
        => node.HasElse ? ExtractValue(node.ElseNode!).Concat(new[] { node.Condition }) : new[] { node.Condition };

    public override IEnumerable<ValueNode> Visit(PrintNode node)
        => Visit(node.Value);

    public override IEnumerable<ValueNode> Visit(ReturnNode node)
        => node.IsReturningValue ? new[] { node.Value } : emptyArray;

    public override IEnumerable<ValueNode> Visit(WhileNode node)
        => new[] { node.Condition };


    public override IEnumerable<ValueNode> Visit(ComplexStringNode node)
        => node.CodeSections;

    public override IEnumerable<ValueNode> Visit(FunctionCallNode node)
        => ExtractValue(node.ArgList).Append(node.FunctionName);

    public override IEnumerable<ValueNode> Visit(ObjectCreationNode node)
        => ExtractValue(node.InvocationNode).Append(node.TypeName);

    public override IEnumerable<ValueNode> Visit(OperationNode node)
        => node.Operands;

    public override IEnumerable<ValueNode> Visit(ParenthesizedValueNode node)
        => new[] { node.Value };

    public override IEnumerable<ValueNode> Visit(TupleNode node)
        => node.Values;


    public override IEnumerable<ValueNode> Visit(SimpleBlock block)
        => emptyArray;

    public IEnumerable<ValueNode> ExtractValue(StatementNode node) => node.Accept(this);
}