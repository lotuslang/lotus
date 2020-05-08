using System;
using System.Collections.Generic;

public class ElseNode : StatementNode
{
    public SimpleBlock Body { get; protected set; }

    public IfNode? IfNode { get; protected set; }

    public bool HasIf { get => IfNode != null; }

    public ElseNode(SimpleBlock body, ComplexToken elseToken) : base(elseToken) {
        Body = body;
        IfNode = null;
    }

    public ElseNode(IfNode ifNode, ComplexToken elseToken) : base(elseToken) {
        IfNode = ifNode;
        Body = ifNode.Body; // works like a pointer so it's fine
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "else");

        // FIXME: Choose a color
        root.AddProperty("tooltip", "else branch");

        if (HasIf) {
            root.AddNode(IfNode!.ToGraphNode());
        } else {
            root.AddNode(Body.ToGraphNode());
        }

        return root;
    }
}