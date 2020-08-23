using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ImportNode : StatementNode
{
    public ReadOnlyCollection<ValueNode> ImportsName { get; protected set; }

    public FromNode FromStatement { get; protected set; }

    internal bool IsEverything { get => ImportsName.Count == 0; }

    public ImportNode(IList<ValueNode> imports, FromNode from, ComplexToken importToken, bool isValid = true) : base(importToken, isValid) {
        ImportsName = imports.AsReadOnly();
        FromStatement = from;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "import") {
            FromStatement.ToGraphNode()
        }.SetColor("fuchsia")
         .SetTooltip("import statement");

        var importsNode = new GraphNode(ImportsName.GetHashCode(), "import\\nnames")
            .SetColor("peru")
            .SetTooltip("import names");

        foreach (var import in ImportsName) {
            importsNode.Add(import.ToGraphNode());
        }

        root.Add(importsNode);

        return root;
    }
}
