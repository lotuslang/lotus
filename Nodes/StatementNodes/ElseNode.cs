using System;
using System.Collections.Generic;

public class ElseNode : StatementNode
{
    public SimpleBlock Body { get; protected set; }

    public IfNode? IfNode { get; protected set; }

    public bool HasIf { get => IfNode != null; }

    public ElseNode(SimpleBlock body, ComplexToken elseToken, bool isValid = true) : base(elseToken, isValid) {
        Body = body;
        IfNode = null; // FIXME: we shouldn't have pure nulls here. another reason to write nulls for every node
    }

    public ElseNode(IfNode ifNode, ComplexToken elseToken, bool isValid = true) : base(elseToken, isValid) {
        IfNode = ifNode;
        Body = ifNode.Body; // works like a pointer so it's fine
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}