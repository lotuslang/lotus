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
        =>  new GraphNode(node.Token.Representation)
                .SetColor(TopLevel.color)
                .SetTooltip(TopLevel.tooltip);

    public GraphNode Visit(TopLevelNode node)
        => node.Token.Kind == TokenKind.EOF
                ? new GraphNode("<EOF>").SetProperty("style", "invis")
                : Default(node);

    public GraphNode Visit(ImportNode node) {
        var root = new GraphNode("import")
            .SetColor(Import.color)
            .SetTooltip(Import.tooltip);

        var importsNode = new Graph("imports")
            .SetGraphProp("color", ImportNames.color);

        foreach (var import in node.Names)
            importsNode.Add(ExtraUtils.ToGraphNode(import));

        root.Add(importsNode);

        if (node.HasOrigin)
            root.Add(ToGraphNode(node.FromOrigin));

        return root;
    }

    public GraphNode Visit(EnumNode node) {
        var root = new GraphNode("enum " + ASTUtils.PrintTypeName(node.Name, printTrivia: false))
            .SetColor(Enum.color)
            .SetTooltip(Enum.tooltip);

        if (node.Name.HasParent) {
            root.Add(
                new GraphNode("parent")
                .SetColor(EnumParent.color)
                .SetTooltip(EnumParent.tooltip)
                .Add(ExtraUtils.ToGraphNode(node.Name.Parent))
            );
        }

        var valuesNode = new Graph("Values") // todo(graphviz): use record shape for enum values (+struct fields?)
            .SetGraphProp("color", EnumValues.color);

        foreach (var val in node.Values) {
            valuesNode.Add(ExtraUtils.ToGraphNode(val));
        }

        root.Add(valuesNode);

        return root;
    }

    public GraphNode Visit(FunctionHeaderNode node) {
        var root = new GraphNode("func " + node.Name.Representation)
                        .SetColor(FuncDec.color)
                        .SetTooltip(FuncDec.tooltip);

        var parametersNode = new Graph("parameters")
            .SetGraphProp("color", FuncDecParameters.color);

        foreach (var parameter in node.Parameters.Items) {
            var paramNameNode = ToGraphNode(parameter.Name);

            paramNameNode.Add(ToGraphNode(parameter.Type));

            if (parameter.HasDefaultValue) {
                paramNameNode.Add(ToGraphNode(parameter.DefaultValue));
            }

            parametersNode.Add(paramNameNode);
        }

        root.Add(parametersNode);

        if (node.HasReturnType)
            root.Add(new GraphNode("return type") { ToGraphNode(node.ReturnType) });

        return root;
    }

    public GraphNode Visit(FunctionDefinitionNode node)
        => ToGraphNode(node.Header)
            .Add(ToCluster(node.Body)
                .SetName(node.Name + "()'s body")
            );

    public GraphNode Visit(NamespaceNode node)
        => new GraphNode("namespace") {
                ExtraUtils.ToGraphNode(node.Name).SetTooltip(NamespaceName.tooltip)
            }.SetColor(Namespace.color)
             .SetTooltip(Namespace.tooltip);

    public GraphNode Visit(UsingNode node)
        => new GraphNode("using") {
                ExtraUtils.UnionToGraphNode(node.Name)
            }.SetColor(Using.color)
             .SetTooltip(Using.tooltip);

    public GraphNode Visit(TraitNode node) {
        var root = new GraphNode("trait " + node.Name.Value);

        foreach (var func in node.Functions)
            root.Add(ToGraphNode(func));

        return root;
    }

    public GraphNode Visit(StructNode node) {
        var root = new GraphNode("struct " + node.Name.Value)
            .SetColor(Struct.color)
            .SetColor(Struct.tooltip);

        var fields = new Graph("Fields")
            .SetGraphProp("color", StructFields.color);

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

    public GraphNode ToGraphNode(FunctionParameter parameter) {
        var paramNameNode = ToGraphNode(parameter.Name);

        paramNameNode.Add(ToGraphNode(parameter.Type));

        if (parameter.HasDefaultValue) {
            paramNameNode.Add(ToGraphNode(parameter.DefaultValue));
        }

        return paramNameNode;
    }

    public GraphNode ToGraphNode(FromOrigin node)
        => new GraphNode("from") {
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
                new GraphNode("parent") { // todo(graphviz): put custom label on edge and remove useless 'parent' node
                    ExtraUtils.ToGraphNode(typeDec.Parent)
                }
            );
        }

        return root;
    }

    public GraphNode ToGraphNode(TopLevelNode node) => node.Accept(this);
}