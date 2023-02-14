using Lotus.Syntax.Visitors;

namespace Lotus.Extras.Graphs;

internal sealed partial class GraphMaker : ITopLevelVisitor<GraphNode>
{
    private static readonly (string tooltip, string color) From = ("from statement", "navy");
    private static readonly (string tooltip, string color) FromOrigin = ("origin name", "");

    private static readonly (string tooltip, string color) Import = ("import statement", "fuchsia");
    private static readonly (string tooltip, string color) ImportNames = ("import names", "peru");

    private static readonly (string tooltip, string color) Namespace = ("namespace declaration", "cornflowerblue");
    private static readonly (string tooltip, string color) NamespaceName = ("namespace name", "");

    private static readonly (string tooltip, string color) Using = (nameof(UsingNode), "");

    private static readonly (string tooltip, string color) Enum = (nameof(EnumNode), "");
    private static readonly (string tooltip, string color) EnumParent = ("this enum's parent value/name", "");
    private static readonly (string tooltip, string color) EnumValues = ("this enum's values", "");

    private static readonly (string tooltip, string color) Struct = (nameof(StructNode), "");
    private static readonly (string tooltip, string color) StructFields = ("this struct's fields", "");

    private static readonly (string tooltip, string color) TopLevel = (nameof(TopLevelNode), "black");

    private static readonly (string tooltip, string color) TypeDecName = ("type name", "");

    public GraphNode Default(TopLevelNode node)
        =>  new GraphNode(node.GetHashCode(), node.Token.Representation)
                .SetColor(TopLevel.color)
                .SetTooltip(TopLevel.tooltip);

    public GraphNode Visit(TopLevelStatementNode node)
        => ExtraUtils.ToGraphNode(node.Statement);

    public GraphNode Visit(ImportNode node) {
        var root = new GraphNode(node.GetHashCode(), "import")
            .SetColor(Import.color)
            .SetTooltip(Import.tooltip);

        var importsNode = new GraphNode(node.Names.GetHashCode(), @"import\nnames")
            .SetColor(ImportNames.color)
            .SetTooltip(ImportNames.tooltip);

        foreach (var import in node.Names) {
            importsNode.Add(ExtraUtils.ToGraphNode(import));
        }

        root.Add(importsNode);

        if (node.HasOrigin)
            root.Add(ToGraphNode(node.FromOrigin));

        return root;
    }

    public GraphNode Visit(EnumNode node) {
        var root = new GraphNode(node.GetHashCode(), "enum " + ASTUtils.PrintTypeName(node.Name, printTrivia: false))
            .SetColor(Enum.color)
            .SetTooltip(Enum.tooltip);

        if (node.Name.HasParent) {
            root.Add(
                new GraphNode(
                    DeterministicHashCode.Combine(node.Name.Parent, "parent"),
                    "parent"
                )
                .SetColor(EnumParent.color)
                .SetTooltip(EnumParent.tooltip)
                .Add(ExtraUtils.ToGraphNode(node.Name.Parent))
            );
        }

        var valuesNode = new GraphNode(node.Values.GetHashCode(), "Values")
            .SetColor(EnumValues.color)
            .SetTooltip(EnumValues.tooltip);

        foreach (var val in node.Values) {
            valuesNode.Add(ExtraUtils.ToGraphNode(val));
        }

        root.Add(valuesNode);

        return root;
    }

    public GraphNode Visit(NamespaceNode node)
        => new GraphNode(node.GetHashCode(), "namespace") {
                ExtraUtils.ToGraphNode(node.Name).SetTooltip(NamespaceName.tooltip)
            }.SetColor(Namespace.color)
             .SetTooltip(Namespace.tooltip);

    public GraphNode Visit(UsingNode node)
        => new GraphNode(node.GetHashCode(), "using") {
                ExtraUtils.UnionToGraphNode(node.Name)
            }.SetColor(Using.color)
             .SetTooltip(Using.tooltip);

    public GraphNode Visit(StructNode node) {
        var root = new GraphNode(node.GetHashCode(), "struct")
            .SetColor(Struct.color)
            .SetColor(Struct.tooltip);

        var fields = new GraphNode(node.Fields.GetHashCode(), "Fields")
            .SetColor(StructFields.color)
            .SetTooltip(StructFields.tooltip);

        foreach (var field in node.Fields) {
            var fieldNode = ExtraUtils.ToGraphNode(field.Name);

            fieldNode.Add(ExtraUtils.ToGraphNode(field.Type));

            if (field.HasDefaultValue) {
                fieldNode.Add(ExtraUtils.ToGraphNode(field.DefaultValue));
            }

            fields.Add(fieldNode);
        }

        root.Add(fields);

        return root;
    }

    public GraphNode ToGraphNode(FromOrigin node)
        => new GraphNode(node.GetHashCode(), "from") {
               ExtraUtils.UnionToGraphNode(node.OriginName)
                   .SetTooltip(FromOrigin.tooltip)
           }.SetColor(From.color)
            .SetTooltip(From.tooltip);

    public GraphNode ToGraphNode(TypeDecName typeDec) {
        var root = ExtraUtils.ToGraphNode(typeDec.TypeName)
                    .SetColor(TypeDecName.color)
                    .SetTooltip(TypeDecName.tooltip);

        if (typeDec.HasParent) {
            root.Add(
                new GraphNode(DeterministicHashCode.Combine(typeDec.Parent, "parent"), "parent") {
                    ExtraUtils.ToGraphNode(typeDec.Parent)
                }
            );
        }

        return root;
    }

    public GraphNode ToGraphNode(TopLevelNode node) => node.Accept(this);
}