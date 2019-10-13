using System;

internal static class Convert
{
    internal static NumberNode ToNumber(ValueNode node) {
        if (node is NumberNode) return node as NumberNode;

        if (node is BoolNode boolNode) return new NumberNode(boolNode.Value ? 1 : 0, node.Token);

        throw new Exception("cannot convert to number");
    }

    internal static StringNode ToString(ValueNode node) {
        if (node is StringNode) return node as StringNode;

        if (node is NumberNode number) return new StringNode(number.Value.ToString(), node.Token);

        if (node is BoolNode boolNode) return new StringNode(boolNode.Value ? "True" : "False", node.Token);

        throw new Exception("cannot convert to string");
    }
}