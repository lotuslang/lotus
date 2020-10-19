using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ImportNode : StatementNode
{
    public ReadOnlyCollection<ValueNode> ImportsName { get; protected set; }

    public FromNode FromStatement { get; protected set; }

    public ImportNode(IList<ValueNode> imports, FromNode from, ComplexToken importToken, bool isValid = true) : base(importToken, isValid) {
        ImportsName = imports.AsReadOnly();
        FromStatement = from;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
