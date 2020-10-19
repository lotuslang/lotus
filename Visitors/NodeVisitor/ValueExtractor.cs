using System;
using System.Linq;
using System.Collections.Generic;

public sealed class ValueExtractor : NodeVisitor<IEnumerable<ValueNode>>
{

    private static readonly ValueNode[] emptyArray = new ValueNode[0];

    protected override IEnumerable<ValueNode> Default(StatementNode node)
        => emptyArray;

    protected override IEnumerable<ValueNode> Default(ValueNode node)
        => new[] { node };


    public override IEnumerable<ValueNode> Visit(DeclarationNode node)
        => new[] { node.Value };

    public override IEnumerable<ValueNode> Visit(ElseNode node)
        => (node.HasIf ? ExtractValue(node.IfNode!) : emptyArray)
                .Concat(Visit(node.Body));

    public override IEnumerable<ValueNode> Visit(ForeachNode node)
        => Visit(node.Collection)
                .Concat(Visit(node.Body));

    public override IEnumerable<ValueNode> Visit(ForNode node)
        => ExtractValue(node.Header[0])
                .Concat(ExtractValue(node.Header[1]))
                .Concat(ExtractValue(node.Header[2]))
                .Concat(Visit(node.Body));

    public override IEnumerable<ValueNode> Visit(FunctionDeclarationNode node) {
        var output = new List<ValueNode>();

        foreach (var param in node.Parameters) {
            if (param.HasDefaultValue) output.Concat(Visit(param.DefaultValue!));
        }

        return output.Concat(Visit(node.Body));
    }

    public override IEnumerable<ValueNode> Visit(IfNode node)
        => ExtractValue(node.Condition)
                .Concat(Visit(node.Body))
                .Concat(node.HasElse ? ExtractValue(node.ElseNode!) : emptyArray);

    public override IEnumerable<ValueNode> Visit(PrintNode node)
        => Visit(node.Value);

    public override IEnumerable<ValueNode> Visit(ReturnNode node)
        => node.IsReturningValue ? Visit(node.Value) : emptyArray;

    public override IEnumerable<ValueNode> Visit(WhileNode node)
        => node.IsDoLoop ?
                Visit(node.Body).Concat(Visit(node.Condition))
        :       Visit(node.Condition).Concat(Visit(node.Body));


    // TODO: Find an easier/clearer/faster way to do this
    public override IEnumerable<ValueNode> Visit(SimpleBlock block)
        // To extract values from a block, we extract every value from each statement and then return them.
        // However, it's not as straightforward as it sounds, since we have an array of statements, and each
        // statement will return an array of ValueNodes. Means we end up with a IEnumerable<ValueNode>[], which we need to flatten
        // into a IEnumerable<ValueNode> before returning. In code that gives :
        //
        // - Content.Select(ExtractValue) applies the ValueExtractor.ExtractValue method to each StatementNode in the block,
        //   which returns a IEnumerable<ValueNode> for each statement in Content.
        //   => returns a IEnumerable<ValueNode>[]
        //
        // - SelectMany(node => node) transforms the IEnumerable<ValueNode>[] returned by the previous operation into a lat IEnumerable<ValueNode>
        //   => returns a IEnumerable<ValueNode>
        => block.Content
                .Select(ExtractValue)
                .SelectMany(node => node);
    public IEnumerable<ValueNode> ExtractValue(StatementNode node) => node.Accept(this);
}