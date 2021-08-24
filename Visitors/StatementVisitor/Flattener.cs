using System;
using System.Linq;
using System.Collections.Generic;

internal sealed class Flattener : IStatementVisitor<IEnumerable<StatementNode>>, IValueVisitor<IEnumerable<StatementNode>>
{
    private static readonly StatementNode[] emptyArray = Array.Empty<StatementNode>();

    public IEnumerable<StatementNode> Default(StatementNode node)
        => new[] { node };

    public IEnumerable<StatementNode> Default(ValueNode node)
        => new[] { (StatementExpressionNode)node };

    public IEnumerable<StatementNode> Visit(DeclarationNode node)
        => new StatementNode[] { (StatementExpressionNode)node.Value, node };

    public IEnumerable<StatementNode> Visit(ElseNode node)
        => (node.BlockOrIfNode.Match(
                body => Visit(body),
                node => Flatten(node)
        )).Append(node);

    public IEnumerable<StatementNode> Visit(ForeachNode node)
        => Flatten(node.CollectionRef)
                .Concat(Visit(node.Body))
                .Append(node);

    public IEnumerable<StatementNode> Visit(ForNode node)
        => Flatten(node.Header[0])
                .Concat(Flatten(node.Header[1]))
                .Concat(Flatten(node.Header[2]))
                .Concat(Visit(node.Body))
                .Append(node);

    public IEnumerable<StatementNode> Visit(FunctionDeclarationNode node) {
        IEnumerable<StatementNode> output = new List<StatementNode>();

        foreach (var param in node.Parameters) {
            if (param.HasDefaultValue) output = output.Concat(Flatten(param.DefaultValue!));
        }

        return output.Concat(Visit(node.Body)).Append(node);
    }

    public IEnumerable<StatementNode> Visit(IfNode node)
        =>  Flatten(node.Condition)
                .Concat(Visit(node.Body))
                .Concat(node.HasElse ? Flatten(node.ElseNode!) : emptyArray)
                .Append(node);

    public IEnumerable<StatementNode> Visit(PrintNode node)
        => Flatten(node.Value)
                .Append(node);

    public IEnumerable<StatementNode> Visit(ReturnNode node)
        => (node.IsReturningValue ? Flatten(node.Value) : emptyArray)
                .Append(node);

    public IEnumerable<StatementNode> Visit(StatementExpressionNode node) => Flatten(node.Value);

    public IEnumerable<StatementNode> Visit(WhileNode node)
        => (node.IsDoLoop ? Visit(node.Body).Concat(Flatten(node.Condition)) : Flatten(node.Condition).Concat(Visit(node.Body)))
                .Append(node);

    public IEnumerable<StatementNode> Visit(OperationNode node)
        => node.Operands.SelectMany(Flatten)
                .Append((StatementExpressionNode)node);

    public IEnumerable<StatementNode> Visit(FunctionCallNode node)
        => node.ArgList.Values.SelectMany(Flatten)
                .Append((StatementExpressionNode)node);

    public IEnumerable<StatementNode> Visit(ObjectCreationNode node)
        => Flatten(node.Invocation)
                .Append((StatementExpressionNode)node);

    public IEnumerable<StatementNode> Visit(ParenthesizedValueNode node)
        => Flatten(node.Values)
                .Append((StatementExpressionNode)node);

    public IEnumerable<StatementNode> Visit(TupleNode node)
        =>  node.Values.SelectMany(Flatten)
                .Append((StatementExpressionNode)node);

    // TODO: Find an easier/clearer/faster way to do this
    public IEnumerable<StatementNode> Visit(SimpleBlock block)
        // To extract values from a block, we extract every value from each statement and then return them.
        // However, it's not as straightforward as it sounds, since we have an array of statements, and each
        // statement will return an array of ValueNodes. Means we end up with a IEnumerable<StatementNode>[], which we need to flatten
        // into a IEnumerable<StatementNode> before returning. In code that gives :
        //
        // - Content.Select(ExtractValue) applies the ValueExtractor.ExtractValue method to each StatementNode in the block,
        //   which returns a IEnumerable<StatementNode> for each statement in Content.
        //   => returns a IEnumerable<StatementNode>[]
        //
        // - SelectMany(node => node) transforms the IEnumerable<StatementNode>[] returned by the previous operation into a lat IEnumerable<StatementNode>
        //   => returns a IEnumerable<StatementNode>
        => block.Content
                .Select(Flatten)
                .SelectMany(node => node);
    public IEnumerable<StatementNode> Flatten(StatementNode node) => node.Accept(this);

    public IEnumerable<StatementNode> Flatten(ValueNode node) => node.Accept(this);
}