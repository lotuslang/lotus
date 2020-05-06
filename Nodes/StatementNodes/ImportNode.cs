using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ImportNode : StatementNode
{
    public ReadOnlyCollection<ValueNode> ImportsName { get; protected set; }

    public FromNode FromStatement { get; protected set; }

    internal bool IsEverything { get => ImportsName.Count == 0; }

    public ImportNode(IList<ValueNode> imports, FromNode from, ComplexToken importToken) : base(importToken) {
        ImportsName = imports.AsReadOnly();
        FromStatement = from;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "import");

        root.AddProperty("color", "fuchsia");
        root.AddProperty("tooltip", "import statement");

        var fromNode = FromStatement.ToGraphNode();

        root.AddNode(fromNode);

        var importsNode = new GraphNode(ImportsName.GetHashCode(), "imports\\nname");

        importsNode.AddProperty("color", "peru");
        importsNode.AddProperty("tooltip", "imports name");

        foreach (var import in ImportsName) {
            importsNode.AddNode(import.ToGraphNode());
        }

        root.AddNode(importsNode);

        return root;
    }
}
