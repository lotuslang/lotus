using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ImportNode : StatementNode
{
    public new static readonly ImportNode NULL = new ImportNode(new ValueNode[0], FromNode.NULL, ComplexToken.NULL, false);

    public ReadOnlyCollection<ValueNode> ImportsName { get; protected set; }

    public FromNode FromStatement { get; protected set; }

    public ImportNode(IList<ValueNode> imports, FromNode from, ComplexToken importToken, bool isValid = true)
        : base(importToken, new LocationRange(from.Location, imports[0].Location), isValid)
    {
        ImportsName = imports.AsReadOnly();
        FromStatement = from;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
