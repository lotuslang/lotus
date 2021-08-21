using System;
using System.Linq;
using System.Collections.Generic;

internal sealed class ValueExtractor : IValueVisitor<IEnumerable<ValueNode>>, IStatementVisitor<IEnumerable<ValueNode>>
{
    private static readonly ValueNode[] emptyArray = Array.Empty<ValueNode>();

    public IEnumerable<ValueNode> Default(StatementNode node)
        => emptyArray;

    public IEnumerable<ValueNode> Default(ValueNode node)
        => emptyArray;


    public IEnumerable<ValueNode> Visit(DeclarationNode node)
        => new[] { node.Value };

    public IEnumerable<ValueNode> Visit(ElseNode node)
        => node.HasIf ? ExtractValue(node.IfNode!) : emptyArray;

    public IEnumerable<ValueNode> Visit(ForeachNode node)
        => new[] { node.ItemName, node.Collection };

    public IEnumerable<ValueNode> Visit(IfNode node)
        => node.HasElse ? ExtractValue(node.ElseNode!).Concat(new[] { node.Condition }) : new[] { node.Condition };

    public IEnumerable<ValueNode> Visit(PrintNode node)
        => node.Value.Accept(this);

    public IEnumerable<ValueNode> Visit(ReturnNode node)
        => node.IsReturningValue ? new[] { node.Value } : emptyArray;

    public IEnumerable<ValueNode> Visit(StatementExpressionNode node) => ExtractValue(node.Value);

    public IEnumerable<ValueNode> Visit(WhileNode node)
        => new[] { node.Condition };


    public IEnumerable<ValueNode> Visit(ComplexStringNode node)
        => node.CodeSections;

    public IEnumerable<ValueNode> Visit(FunctionCallNode node)
        => ExtractValue(node.ArgList).Append(node.FunctionName);

    public IEnumerable<ValueNode> Visit(ObjectCreationNode node)
        => ExtractValue(node.InvocationNode).Append(node.TypeName);

    public IEnumerable<ValueNode> Visit(OperationNode node)
        => node.Operands;

    public IEnumerable<ValueNode> Visit(ParenthesizedValueNode node)
        => new[] { node.Values };

    public IEnumerable<ValueNode> Visit(TupleNode node)
        => node.Values;


    public IEnumerable<ValueNode> Visit(SimpleBlock block)
        => emptyArray;

    public IEnumerable<ValueNode> ExtractValue(StatementNode node) => node.Accept(this);

    public IEnumerable<ValueNode> ExtractValue(ValueNode node) => node.Accept(this);
}