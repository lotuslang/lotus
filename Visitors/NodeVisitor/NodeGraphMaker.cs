using System;

public class NodeGraphMaker : NodeVisitor<GraphNode>
{

    protected override GraphNode Default(StatementNode node) => Visit(node);
    protected override GraphNode Default(ValueNode node) => Visit(node);

    public override GraphNode Visit(StatementNode node)
        => new GraphNode(node.GetHashCode(), node.Representation)
            .SetColor("black")
            .SetTooltip(GetType().Name);


    public override GraphNode Visit(BreakNode node)
        => Default(node).SetTooltip("break keyword");

    public override GraphNode Visit(ContinueNode node)
        => Default(node).SetTooltip("continue keyword");

    public override GraphNode Visit(DeclarationNode node)
        => new GraphNode(node.GetHashCode(), "var") {
               ASTHelper.ToGraphNode(node.Name).SetTooltip("name"),
               ToGraphNode(node.Value)
           }.SetColor("palegreen")
            .SetTooltip(nameof(DeclarationNode));

    public override GraphNode Visit(ElseNode node)
        => new GraphNode(node.GetHashCode(), "else") {
               node.HasIf ? ToGraphNode(node.IfNode!) : ToGraphNode(node.Body)
           }.SetTooltip("else branch"); // FIXME: Choose a color

    public override GraphNode Visit(ForeachNode node)
        => new GraphNode(node.GetHashCode(), "foreach") {
               new GraphNode(node.InToken.GetHashCode(), "in") {
                   ToGraphNode(node.ItemName),
                   ToGraphNode(node.Collection)
               }.SetTooltip("in iterator"),
               ToGraphNode(node.Body),
           }.SetColor("pink")
            .SetTooltip("foreach loop");

    public override GraphNode Visit(ForNode node) {
        var root = new GraphNode(node.GetHashCode(), "for loop"); // FIXME: choose color and tooltip

        if (node.Header.Count != 0) {
            var headerNode = new GraphNode(node.Header.GetHashCode(), "header")
                .SetColor("deepskyblue")
                .SetTooltip("for-loop header");

            foreach (var statement in node.Header) headerNode.Add(ToGraphNode(statement));

            root.Add(headerNode);

        } else {
            root.Add(new GraphNode(node.Header.GetHashCode(), "(empty header)"));
        }

        root.Add(ToGraphNode(node.Body));

        return root;
    }

    public override GraphNode Visit(FromNode node)
        => new GraphNode(node.GetHashCode(), "from") {
               ToGraphNode(node.OriginName)
                   .SetTooltip("origin name")
           }.SetColor("navy")
            .SetTooltip("from statement");

    public override GraphNode Visit(FunctionDeclarationNode node) {
        var root = new GraphNode(node.GetHashCode(), "func " + node.Name.Representation)
            .SetColor("indianred")
            .SetTooltip(nameof(FunctionDeclarationNode));

        if (node.Parameters.Count == 0) {
            root.Add(new GraphNode(node.Parameters.GetHashCode(), "(no params)"));
        } else {
            var parametersNode = new GraphNode(node.Parameters.GetHashCode(), "param")
                .SetTooltip("parameters");

            foreach (var parameter in node.Parameters) { // FIXME: Write tooltips

                var paramNameNode = ToGraphNode(parameter.Name);

                if (parameter.Type == ValueNode.NULL) {
                    paramNameNode.Add(new GraphNode(HashCode.Combine(ValueNode.NULL, parameter), "any"));
                } else {
                    paramNameNode.Add(ToGraphNode(parameter.Type));
                }

                if (parameter.HasDefaultValue) {
                    paramNameNode.Add(ToGraphNode(parameter.DefaultValue));
                }

                parametersNode.Add(paramNameNode);
            }

            root.Add(parametersNode);
        }

        if (node.HasReturnType) {
            root.Add(new GraphNode(HashCode.Combine(node.ReturnType, node), "return type") { // FIXME: Color & Tooltip
                ToGraphNode(node.ReturnType)
            });
        }

        root.Add(ToGraphNode(node.Body).SetTooltip("body"));

        return root;
    }

    public override GraphNode Visit(IfNode node) {
        var root = new GraphNode(node.GetHashCode(), "if") {
            new GraphNode(HashCode.Combine(node, "condition"), "condition") {
                ToGraphNode(node.Condition)
            }.SetTooltip("if condition"),
            ToGraphNode(node.Body)
        }.SetTooltip("if statement"); // FIXME: Choose color

        if (node.HasElse) {
            root.Add(ToGraphNode(node.ElseNode!));
        }

        return root;
    }

    public override GraphNode Visit(ImportNode node){
        var root = new GraphNode(node.GetHashCode(), "import") {
            ToGraphNode(node.FromStatement)
        }.SetColor("fuchsia")
         .SetTooltip("import statement");

        var importsNode = new GraphNode(node.ImportsName.GetHashCode(), "import\\nnames")
            .SetColor("peru")
            .SetTooltip("import names");

        foreach (var import in node.ImportsName) {
            importsNode.Add(ToGraphNode(import));
        }

        root.Add(importsNode);

        return root;
    }

    public override GraphNode Visit(NamespaceNode node)
        => new GraphNode(node.GetHashCode(), "namespace") {
                ToGraphNode(node.NamespaceName).SetTooltip("namespace name")
            }.SetColor("cornflowerblue")
             .SetTooltip("namespace declaration");

    public override GraphNode Visit(PrintNode node)
        => new GraphNode(node.GetHashCode(), "print") {
                ToGraphNode(node.Value)
            }.SetColor("")
             .SetTooltip(""); // FIXME: find color

    public override GraphNode Visit(ReturnNode node) {
        var root = new GraphNode(node.GetHashCode(), "return")
            .SetColor("brown")
            .SetTooltip("return");

        if (node.IsReturningValue) {
            root.Add(ToGraphNode(node.Value));
        }

        return root;
    }

    public override GraphNode Visit(UsingNode node)
        => new GraphNode(node.GetHashCode(), "using") {
                ToGraphNode(node.ImportName)
            }.SetColor("")
             .SetTooltip(""); // FIXME: find color

    public override GraphNode Visit(WhileNode node)
        => new GraphNode(node.GetHashCode(), node.IsDoLoop ? "do-while" : "while") {
                new GraphNode(HashCode.Combine(node, "condition"), "condition") {
                    ToGraphNode(node.Condition)
                }.SetTooltip("loop condition"), // FIXME: Choose color
                ToGraphNode(node.Body)
            }.SetTooltip(node.IsDoLoop ? "do-while loop" : "while loop"); // FIXME: Choose color



    public override GraphNode Visit(ValueNode node)
        => new GraphNode(node.GetHashCode(), node.Representation)
            .SetColor("lightgrey")
            .SetTooltip(GetType().Name);


    public override GraphNode Visit(ArrayLiteralNode node) {
        var root = new GraphNode(node.GetHashCode(), "array literal\\n(" + node.Content.Count + " item(s))")
            .SetColor("teal")
            .SetTooltip("array literal");

        var itemCount = 0;

        foreach (var item in node.Content) root.Add(ToGraphNode(item).SetTooltip("item " + ++itemCount));

        return root;
    }

    public override GraphNode Visit(BoolNode node)
        => Default(node).SetTooltip("bool literal");

    public override GraphNode Visit(ComplexStringNode node) {
        var root = new GraphNode(node.GetHashCode(), node.Representation);

        if (node.CodeSections.Count != 0) {
            var sectionNode = new GraphNode(HashCode.Combine(node, "sections"), "code sections");

            foreach (var section in node.CodeSections) {
                sectionNode.Add(ToGraphNode(section));
            }

            root.Add(sectionNode);
        }

        return root;
    }

    public override GraphNode Visit(FunctionCallNode node) {
        GraphNode root;

        if (node.FunctionName is IdentNode name) {
            root = new GraphNode(node.GetHashCode(), name.Representation + "(...)");
        } else {
            root = new GraphNode(node.GetHashCode(), "call") {
                new GraphNode("function") {
                    ToGraphNode(node.FunctionName)
                },
            };
        }

        root.SetColor("tomato")
            .SetTooltip("call to a function");

        if (node.CallingParameters.Count == 0) {
            root.Add(new GraphNode(node.CallingParameters.GetHashCode(), "(no args)"));

            return root;
        }

        var argsNode = new GraphNode("args");

        foreach (var parameter in node.CallingParameters) argsNode.Add(ToGraphNode(parameter).SetTooltip("argument"));

        root.Add(argsNode);

        return root;
    }

    public override GraphNode Visit(IdentNode node)
        => Default(node).SetTooltip("ident");

    public override GraphNode Visit(NumberNode node)
        => Default(node).SetTooltip("number");

    public override GraphNode Visit(ObjectCreationNode node) {
        var root = new GraphNode(node.GetHashCode(), "obj creation") {
            ToGraphNode(node.TypeName)
                .SetColor("")
                .SetTooltip("class name"),
        };

        root.SetColor("indigo")
            .SetTooltip("ctor class/object creation");

        if (node.InvocationNode.CallingParameters.Count == 0) {
            root.Add(new GraphNode(node.InvocationNode.CallingParameters.GetHashCode(), "(no args)"));

            return root;
        }

        var argsNode = new GraphNode("args");

        foreach (var parameter in node.InvocationNode.CallingParameters) {
            argsNode.Add(ToGraphNode(parameter).SetTooltip("argument"));
        }

        root.Add(argsNode);

        return root;
    }

    public override GraphNode Visit(OperationNode node) {
        GraphNode root;

        if (node.Representation == "++" || node.Representation == "--") {
            root = new GraphNode(node.GetHashCode(), (node.OperationType.ToString().StartsWith("Postfix") ? "(postfix)" : "(prefix)") + node.Representation);
        } else {
            root = new GraphNode(node.GetHashCode(), node.Representation);
        }

        root.SetColor("dodgerblue")
            .SetTooltip(GetType().Name);

        foreach (var child in node.Operands) {
            root.Add(ToGraphNode(child));
        }

        return root;
    }

    public override GraphNode Visit(ParenthesizedValueNode node)
        => ToGraphNode(node.Value);

    public override GraphNode Visit(StringNode node)
        => new GraphNode(node.GetHashCode(), "'" + node.Representation.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\"") + "'")
            .SetColor("orange")
            .SetTooltip(nameof(StringNode));

    public override GraphNode Visit(SimpleBlock block) {
        var root = new GraphNode(block.GetHashCode(), "block")
            .SetColor("darkviolet")
            .SetTooltip(nameof(SimpleBlock));

        foreach (var statement in block.Content) root.Add(ToGraphNode(statement));

        return root;
    }

    public GraphNode ToGraphNode(StatementNode node) => node.Accept(this);

    public GraphNode ToGraphNode(SimpleBlock block) => Visit(block);
}