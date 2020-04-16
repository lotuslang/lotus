using System;
using System.IO;

class Program
{
    static void Main(string[] _) {

        // Initializes the tokenizer with the content of the "sample.txt" file
        var tokenizer = new Tokenizer(new FileInfo(Directory.GetCurrentDirectory() + "/test.txt"));

        //tokenizer = new Tokenizer("for (a,b,) { print(i) }");

        var parser = new Parser(tokenizer);

        var g = new Graph("AST");

        g.AddNodeProp("fontname", "Consolas, monospace");
        g.AddGraphProp("fontname", "Consolas, monospace");
        g.AddGraphProp("label", $"Abstract Syntax Tree of {tokenizer.Position.filename}\\n\\n");
        g.AddGraphProp("labelloc", "top");

        while (parser.Consume(out StatementNode val)) {
            g.AddNode(ToGraphNode((dynamic)val));
        }

        Console.WriteLine(g.ToText());

        /*tokenizer = new Tokenizer(new FileInfo(Directory.GetCurrentDirectory() + "/sample.txt"));

        parser = new Parser(tokenizer);

        var interpreter = new Interpreter(parser);

        interpreter.RunAll();*/
    }

    public static GraphNode ToGraphNode(ValueNode node) {
        var output = new GraphNode(node.GetHashCode(), node.Representation);

        output.AddProperty("color", "lightgrey");
        output.AddProperty("tooltip", nameof(ValueNode));

        return output;
    }

    public static GraphNode ToGraphNode(StringNode node) {
        var output = new GraphNode(node.GetHashCode(), "'" + node.Representation.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\"") + "'");

        output.AddProperty("color", "orange");
        output.AddProperty("tooltip", nameof(StringNode));

        return output;
    }

    public static GraphNode ToGraphNode(StatementNode node) {
        var output = new GraphNode(node.GetHashCode(), node.Representation);

        output.AddProperty("color", "black");
        output.AddProperty("tooltip", nameof(StatementNode));

        return output;
    }

    public static GraphNode ToGraphNode(OperationNode node) {

        var root = new GraphNode(node.GetHashCode(), node.Representation);

        root.AddProperty("color", "dodgerblue");
        root.AddProperty("tooltip", nameof(OperationNode));

        if (node.Representation == "++" || node.Representation == "--") {
            root = new GraphNode(node.GetHashCode(), (node.OperationType.StartsWith("post") ? "(postfix)" : "(prefix)") + node.Representation);
        }

        foreach (var child in node.Operands)
        {
            root.AddNode(ToGraphNode((dynamic)child));
        }

        return root;
    }

    public static GraphNode ToGraphNode(AssignmentNode node) {
        var root = new GraphNode(node.GetHashCode(), node.Name);

        root.AddProperty("color", "darkgreen");
        root.AddProperty("tooltip", nameof(AssignmentNode));

        root.AddNode(ToGraphNode((dynamic)node.Value));

        return root;
    }

    public static GraphNode ToGraphNode(DeclarationNode node) {
        var root = new GraphNode(node.GetHashCode(), "var");

        root.AddProperty("color", "palegreen");
        root.AddProperty("tooltip", nameof(DeclarationNode));

        root.AddNode(ToGraphNode(node.Name, "name"));

        root.AddNode(ToGraphNode((dynamic)node.Value));

        return root;
    }

    public static GraphNode ToGraphNode(FunctionCallNode node) {

        GraphNode root;

        if (node.FunctionName is IdentNode name) {
            root = new GraphNode(node.GetHashCode(), name.Representation + "(...)");

        } else {
            root = new GraphNode(node.GetHashCode(), "call");

            var called = new GraphNode("function");

            called.AddNode(ToGraphNode((dynamic)node.FunctionName));

            root.AddNode(called);
        }

        root.AddProperty("color", "tomato");
        root.AddProperty("tooltip", "call to " + nameof(FunctionCallNode));

        if (node.CallingParameters.Count == 0) {
            root.AddNode(new GraphNode(node.CallingParameters.GetHashCode(), "(no args)"));

            return root;
        }

        var argsNode = new GraphNode("args");

        foreach (var parameter in node.CallingParameters)
        {
            var paramNode = ToGraphNode((dynamic)parameter);

            paramNode.AddProperty("tooltip", "argument");

            argsNode.AddNode(paramNode);
        }

        root.AddNode(argsNode);

        return root;
    }

    public static GraphNode ToGraphNode(FunctionDeclarationNode node) {
        var root = new GraphNode(node.GetHashCode(), "def " + node.Name.Representation);

        root.AddProperty("color", "indianred");
        root.AddProperty("tooltip", nameof(FunctionDeclarationNode));

        if (node.Parameters.Count == 0) {

            root.AddNode(new GraphNode(node.Parameters.GetHashCode(), "(no params)"));

        } else {

            var parameters = new GraphNode(node.Parameters.GetHashCode(), "param");

            parameters.AddProperty("tooltip", "parameters");

            foreach (var parameter in node.Parameters) {
                parameters.AddNode(ToGraphNode(parameter, "parameter"));
            }

            root.AddNode(parameters);
        }

        var body = ToGraphNode(node.Value);

        body.AddProperty("tooltip", "body");

        root.AddNode(body);

        return root;
    }

    public static GraphNode ToGraphNode(ArrayLiteralNode node) {
        var root = new GraphNode(node.GetHashCode(), "array literal\\n(" + node.Content.Count + " item(s))");

        root.AddProperty("color", "teal");
        root.AddProperty("tooltip", "array init");

        var itemCounter = 1;

        foreach (var item in node.Content) {
            var itemNode = ToGraphNode((dynamic)item);

            itemNode.AddProperty("tooltip", "item " + itemCounter++);

            root.AddNode(itemNode);
        }

        return root;
    }

    public static GraphNode ToGraphNode(TypeCastNode node) {
        var root = new GraphNode(node.GetHashCode(), "type-cast");

        root.AddProperty("color", "purple");
        root.AddProperty("tooltip", "type-casting expression");

        var typeNode = ToGraphNode((dynamic)node.Type);

        typeNode.AddProperty("tooltip", "type");

        root.AddNode(typeNode);

        var operandNode = ToGraphNode((dynamic)node.Operand);

        operandNode.AddProperty("tooltip", "operand");

        root.AddNode(operandNode);

        return root;
    }

    public static GraphNode ToGraphNode(NamespaceNode node) {
        var root = new GraphNode(node.GetHashCode(), "namespace");

        root.AddProperty("color", "cornflowerblue");
        root.AddProperty("tooltip", "namespace declaration");

        var nameNode = ToGraphNode((dynamic)node.NamespaceName);

        nameNode.AddProperty("tooltip", "namespace name");

        root.AddNode(nameNode);

        return root;
    }

    public static GraphNode ToGraphNode(ImportNode node) {
        var root = new GraphNode(node.GetHashCode(), "import");

        root.AddProperty("color", "fuchsia");
        root.AddProperty("tooltip", "import statement");

        var fromNode = ToGraphNode((dynamic)node.FromStatement);

        root.AddNode(fromNode);

        var importsNode = new GraphNode(node.ImportsName.GetHashCode(), "imports\\nname");

        importsNode.AddProperty("color", "peru");
        importsNode.AddProperty("tooltip", "imports name");

        foreach (var import in node.ImportsName) {
            importsNode.AddNode(ToGraphNode((dynamic)import));
        }

        root.AddNode(importsNode);

        return root;
    }

    public static GraphNode ToGraphNode(FromNode node) {
        var root = new GraphNode(node.GetHashCode(), "from");

        root.AddProperty("color", "navy");
        root.AddProperty("tooltip", "from statement");

        var nameNode = ToGraphNode((dynamic)node.OriginName);

        nameNode.AddProperty("tooltip", "origin name");

        root.AddNode(nameNode);

        return root;
    }

    public static GraphNode ToGraphNode(ForeachNode node) {
        var root = new GraphNode(node.GetHashCode(), "foreach");

        root.AddProperty("color", "pink");
        root.AddProperty("tooltip", "foreach loop");

        var inNode = new GraphNode(node.InToken.GetHashCode(), "in");

        inNode.AddProperty("tooltip", "in iterator");

        inNode.AddNode(ToGraphNode((dynamic)node.ItemName));
        inNode.AddNode(ToGraphNode((dynamic)node.CollectionName));

        root.AddNode(inNode);

        var bodyNode = ToGraphNode(node.Body);

        root.AddNode(bodyNode);

        return root;
    }

    public static GraphNode ToGraphNode(ReturnNode node) {
        var root = new GraphNode(node.GetHashCode(), "return");

        root.AddProperty("color", "brown");
        root.AddProperty("tooltip", "return");

        if (node.IsReturningValue) {
            root.AddNode(ToGraphNode((dynamic)node.Value));
        }

        return root;
    }

    public static GraphNode ToGraphNode(ObjectCreationNode node) {
        var root = new GraphNode(node.GetHashCode(), "obj creation");

        root.AddProperty("color", "indigo");
        root.AddProperty("tooltip", "ctor/object creation");

        var classNameNode = ToGraphNode((dynamic)node.InvocationNode.FunctionName);

        classNameNode.AddProperty("color", "");
        classNameNode.AddProperty("tooltip", "class name");

        root.AddNode(classNameNode);

        if (node.InvocationNode.CallingParameters.Count == 0) {
            root.AddNode(new GraphNode(node.InvocationNode.CallingParameters.GetHashCode(), "(no args)"));

            return root;
        }

        var argsNode = new GraphNode("args");

        foreach (var parameter in node.InvocationNode.CallingParameters)
        {
            var paramNode = ToGraphNode((dynamic)parameter);

            paramNode.AddProperty("tooltip", "argument");

            argsNode.AddNode(paramNode);
        }

        root.AddNode(argsNode);

        return root;
    }

    public static GraphNode ToGraphNode(ForNode node) {
        var root = new GraphNode(node.GetHashCode(), "for loop");

        if (node.Header.Count != 0) {
            var headerNode = new GraphNode(node.Header.GetHashCode(), "header");

            headerNode.AddProperty("color", "deepskyblue");
            headerNode.AddProperty("tooltip", "for-loop header");

            foreach (var statement in node.Header) headerNode.AddNode(ToGraphNode((dynamic)statement));

            root.AddNode(headerNode);

        } else {
            root.AddNode(new GraphNode(node.Header.GetHashCode(), "(empty header)"));
        }

        root.AddNode(ToGraphNode(node.Body));

        return root;
    }

    public static GraphNode ToGraphNode(SimpleBlock block) {
        var root = new GraphNode(block.GetHashCode(), "block");

        root.AddProperty("color", "darkviolet");
        root.AddProperty("tooltip", nameof(SimpleBlock));

        foreach (var statement in block.Content) {
            var statementNode = ToGraphNode((dynamic)statement);

            //statementNode.AddProperty("tooltip", "statement");

            root.AddNode(statementNode);
        }

        return root;
    }


    public static GraphNode ToGraphNode(ComplexToken token) {
        var output = new GraphNode(token.GetHashCode(), token.Representation);

        output.AddProperty("color", "lightgrey");

        return output;
    }

    public static GraphNode ToGraphNode(ComplexToken token, string tooltip) {
        var output = ToGraphNode(token);

        output.AddProperty("tooltip", tooltip);

        return output;
    }
}