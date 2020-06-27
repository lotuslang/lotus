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
        var root = new GraphNode(GetHashCode(), "import") {
            FromStatement.ToGraphNode()
        }.SetColor("fuchsia")
         .SetTooltip("import statement");

        var importsNode = new GraphNode(ImportsName.GetHashCode(), "imports\\nname")
            .SetColor("peru")
            .SetTooltip("imports name");

        foreach (var import in ImportsName) {
            importsNode.Add(import.ToGraphNode());
        }

        root.Add(importsNode);

        return root;
    }
}
