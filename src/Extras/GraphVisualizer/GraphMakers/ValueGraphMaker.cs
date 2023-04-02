using Lotus.Syntax.Visitors;

namespace Lotus.Extras.Graphs;

internal sealed partial class GraphMaker : IValueVisitor<GraphNode>
{
    private static readonly (string tooltip, string color) Bool = ("bool literal", "teal");

    private static readonly (string tooltip, string color) Char = ("char literal", "lightorange");

    private static readonly (string tooltip, string color) ComplexString = ("Complex string literal", "darkorange");

    private static readonly (string tooltip, string color) FuncCall = ("call to a function", "tomato");

    private static readonly (string tooltip, string color) Ident = ("identifier", "grey");

    private static readonly (string tooltip, string color) Number = ("number", "");

    private static readonly (string tooltip, string color) ObjCreation = ("object creation/ctor call", "indigo");
    private static readonly (string tooltip, string color) ObjTypeName = ("class name", "");

    private static readonly (string tooltip, string color) Operation = ("OperationNode", "dodgerblue");

    private static readonly (string tooltip, string color) String = ("StringNode", "orange");

    private static readonly (string tooltip, string color) Value = ("ValueNode", "lightgrey");

    private static readonly (string tooltip, string color) ValueTuple = ("List of values", "");

    public GraphNode Default(ValueNode node)
        => new GraphNode(node.Token.Representation)
            .SetColor(Value.color)
            .SetTooltip(Value.tooltip);

    public GraphNode Visit(ValueNode node)
        => node.Token.Kind == TokenKind.EOF
                ? new GraphNode("<EOF>").SetProperty("style", "invis")
                : Default(node);

    public GraphNode Visit(BoolNode node)
        => Default(node)
            .SetColor(Bool.color)
            .SetTooltip(Bool.tooltip);

    public GraphNode Visit(CharNode node)
        => Default(node)
            .SetColor(Char.color)
            .SetTooltip(Char.tooltip);

    public GraphNode Visit(ComplexStringNode node) {
        var root = new GraphNode(node.Value)
                        .SetColor(ComplexString.color)
                        .SetTooltip(ComplexString.tooltip);

        if (node.CodeSections.Length != 0) {
            var sectionNode = new GraphNode("code sections");

            foreach (var section in node.CodeSections) {
                sectionNode.Add(ToGraphNode(section));
            }

            root.Add(sectionNode);
        }

        return root;
    }

    public GraphNode Visit(FunctionCallNode node) {
        GraphNode root;

        if (node.Name is IdentNode name) {
            root = new GraphNode(name.Value + "(...)");
        } else {
            root = new GraphNode("call") {
                new GraphNode("function") {
                    ToGraphNode(node.Name)
                },
            };
        }

        root.SetColor(FuncCall.color)
            .SetTooltip(FuncCall.tooltip);

        if (node.ArgList.Count == 0) {
            root.Add(new GraphNode("(no args)"));

            return root;
        }

        var argsNode = ToGraphNode(node.ArgList).SetTooltip("arguments");

        argsNode.Name = "args";

        root.Add(argsNode);

        return root;
    }

    public GraphNode Visit(NameNode node)
        => new GraphNode(ASTUtils.PrintValue(node, printTrivia: false))
            .SetColor(Ident.color)
            .SetTooltip("name");

    public GraphNode Visit(NumberNode node)
        => Default(node)
            .SetColor(Number.color)
            .SetTooltip(node.Kind.ToString());

    public GraphNode Visit(ObjectCreationNode node) {
        var root = new GraphNode("obj creation") {
            ToGraphNode(node.TypeName)
                .SetColor(ObjTypeName.color)
                .SetTooltip(ObjTypeName.tooltip),
        };

        root.SetColor(ObjCreation.color)
            .SetTooltip(ObjCreation.tooltip);

        if (node.Invocation.ArgList.Count == 0) {
            root.Add(new GraphNode("(no args)"));

            return root;
        }

        var argsNode = ToGraphNode(node.Invocation.ArgList).SetTooltip("argument");

        argsNode.Name = "args";

        root.Add(argsNode);

        return root;
    }

    public GraphNode Visit(OperationNode node) {
        GraphNode root;

        if (node.Token.Representation is "++" or "--") {
            root = new GraphNode((node.OperationType.ToString().StartsWith("Postfix") ? "(postfix)" : "(prefix)") + node.Token.Representation);
        } else {
            root = new GraphNode(node.Token.Representation);
        }

        root.SetColor(Operation.color)
            .SetTooltip(Operation.tooltip);

        foreach (var child in node.Operands) {
            root.Add(ToGraphNode(child));
        }

        return root;
    }

    public GraphNode Visit(ParenthesizedValueNode node)
        => ToGraphNode(node.Value);

    // todo(graph): handle escape sequences (same for Visit(CharNode))
    public GraphNode Visit(StringNode node)
        => new GraphNode("'" + node.Value.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\"") + "'")
            .SetColor(String.color)
            .SetTooltip(String.tooltip);

    public GraphNode Visit(TupleNode node) {
        var root = new GraphNode(node.Count == 0 ? "Empty tuple" : "Tuple with\\n" + node.Count + " elements")
                        .SetColor(ValueTuple.color)
                        .SetTooltip(ValueTuple.tooltip);

        foreach (var value in node.Items) {
            root.Add(ToGraphNode(value));
        }

        return root;
    }

    public GraphNode ToGraphNode(ValueNode node) => node.Accept(this);
}