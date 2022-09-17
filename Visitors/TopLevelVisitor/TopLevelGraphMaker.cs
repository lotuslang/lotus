internal class TopLevelGraphMaker : ITopLevelVisitor<GraphNode>
{
    protected readonly (string tooltip, string color) From = ("from statement", "navy");
    protected readonly (string tooltip, string color) FromOrigin = ("origin name", "");

    protected readonly (string tooltip, string color) Import = ("import statement", "fuchsia");
    protected readonly (string tooltip, string color) ImportNames = ("import names", "peru");

    protected readonly (string tooltip, string color) Namespace = ("namespace declaration", "cornflowerblue");
    protected readonly (string tooltip, string color) NamespaceName = ("namespace name", "");

    protected readonly (string tooltip, string color) Using = (nameof(UsingNode), "");

    protected readonly (string tooltip, string color) Enum = (nameof(EnumNode), "");
    protected readonly (string tooltip, string color) EnumParent = ("this enum's parent value/name", "");
    protected readonly (string tooltip, string color) EnumValues = ("this enum's values", "");

    protected readonly (string tooltip, string color) Struct = (nameof(StructNode), "");
    protected readonly (string tooltip, string color) StructFields = ("this struct's fields", "");

    protected readonly (string tooltip, string color) TopLevel = (nameof(TopLevelNode), "black");

    public GraphNode Default(TopLevelNode node)
        =>  new GraphNode(node.GetHashCode(), node.Token.Representation)
                .SetColor(TopLevel.color)
                .SetTooltip(TopLevel.tooltip);

    public GraphNode Visit(TopLevelStatementNode node)
        => ASTHelper.ToGraphNode(node.Statement);

    public GraphNode Visit(FromNode node)
        => new GraphNode(node.GetHashCode(), "from") {
               ASTHelper.UnionToGraphNode(node.OriginName)
                   .SetTooltip("origin name")
           }.SetColor(From.color)
            .SetTooltip(From.tooltip);

    public GraphNode Visit(ImportNode node) {
        var root = new GraphNode(node.GetHashCode(), "import") {
            ASTHelper.ToGraphNode(node.FromStatement)
        }.SetColor(Import.color)
         .SetTooltip(Import.tooltip);

        var importsNode = new GraphNode(node.Names.GetHashCode(), @"import\nnames")
            .SetColor(ImportNames.color)
            .SetTooltip(ImportNames.tooltip);

        foreach (var import in node.Names) {
            importsNode.Add(ASTHelper.ToGraphNode(import));
        }

        root.Add(importsNode);

        return root;
    }

    public GraphNode Visit(EnumNode node) {
        var root = new GraphNode(node.GetHashCode(), "enum " + ASTHelper.PrintTypeName(node.Name))
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
                .Add(ASTHelper.ToGraphNode(node.Name.Parent))
            );
        }

        var valuesNode = new GraphNode(node.Values.GetHashCode(), "Values")
            .SetColor(EnumValues.color)
            .SetTooltip(EnumValues.tooltip);

        foreach (var val in node.Values) {
            valuesNode.Add(ASTHelper.ToGraphNode(val));
        }

        root.Add(valuesNode);

        return root;
    }

    public GraphNode Visit(NamespaceNode node)
        => new GraphNode(node.GetHashCode(), "namespace") {
                ASTHelper.ToGraphNode(node.Name).SetTooltip("namespace name")
            }.SetColor(Namespace.color)
             .SetTooltip(Namespace.tooltip);

    public GraphNode Visit(UsingNode node)
        => new GraphNode(node.GetHashCode(), "using") {
                ASTHelper.UnionToGraphNode(node.Name)
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
            var fieldNode = ASTHelper.ToGraphNode(field.Name);

            fieldNode.Add(ASTHelper.ToGraphNode(field.Type));

            if (field.HasDefaultValue) {
                fieldNode.Add(ASTHelper.ToGraphNode(field.DefaultValue));
            }

            fields.Add(fieldNode);
        }

        root.Add(fields);

        return root;
    }

    public GraphNode Visit(TypeDecName typeDec) {
        var root = ASTHelper.ToGraphNode(typeDec.TypeName)
                    .SetColor("")
                    .SetTooltip("type name");

        if (typeDec.HasParent) {
            root.Add(
                new GraphNode(DeterministicHashCode.Combine(typeDec.Parent, "parent"), "parent") {
                    ASTHelper.ToGraphNode(typeDec.Parent as ValueNode)
                }
            );
        }

        return root;
    }

    public GraphNode ToGraphNode(TypeDecName typeDec) => typeDec.Accept(this);
    public GraphNode ToGraphNode(TopLevelNode node) => node.Accept(this);
}