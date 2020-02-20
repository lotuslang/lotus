using System;
using System.IO;

class Program
{
    static void Main(string[] args) {

        // Initializes the tokenizer with the content of the "sample.txt" file
        var tokenizer = new Tokenizer(new FileInfo(Directory.GetCurrentDirectory() + "/sample.txt"));

        // Repeatedly consumes a token until we can't anymore
        /*while (tokenizer.Consume(out Token item)) {
            Console.WriteLine($"{item.Location} {item.Kind} : {item.Representation}");
        }*/

        //tokenizer = new Tokenizer("var a = 6");

        var parser = new Parser(tokenizer);

        /*File.WriteAllText(Directory.GetCurrentDirectory() + "/generated.txt", "");

        while (parser.Consume(out StatementNode node)) {
            File.AppendAllText(Directory.GetCurrentDirectory() + "/generated.txt", node.ToText(true));
        }

        var expr = "a = 5 + 6 * (max (lo, fi + 6) ^ 2)";
        Console.WriteLine(expr);
        Console.WriteLine(String.Join(" ", Parser.ToPostfixNotation(new Tokenizer(expr)).ConvertAll(t => {
            if (t == "++" || t == "--") {
                return ((t as OperatorToken).IsLeftAssociative ? "(postfix)" : "(prefix)") + (t.Representation);
            }

            return (t.Representation);
        })));

        var val = new Parser(new Tokenizer(expr)).ConsumeValue();*/

        var g = new Graph("AST");

        g.AddNodeProp("fontname", "Consolas, monospace");
        g.AddGraphProp("fontname", "Consolas, monospace");
        g.AddGraphProp("label", "Abstract Syntax Tree of sample.txt\\n\\n");
        g.AddGraphProp("labelloc", "top");

        while (parser.Consume(out StatementNode val)) {
            g.AddNode(ToGraphNode((dynamic)val));
        }

        Console.WriteLine(g.ToText());

        tokenizer = new Tokenizer(new FileInfo(Directory.GetCurrentDirectory() + "/sample.txt"));

        parser = new Parser(tokenizer);

        var interpreter = new Interpreter(parser);

        //interpreter.RunAll();
    }

    public static GraphNode ToGraphNode(ValueNode node) {
        var output = new GraphNode(node.GetHashCode(), "" + node.Representation + "");

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
        var output = new GraphNode(node.GetHashCode(), "" + node.Representation + "");

        output.AddProperty("color", "black");
        output.AddProperty("tooltip", nameof(StatementNode));

        return output;
    }

    public static GraphNode ToGraphNode(OperationNode node) {

        var root = new GraphNode(node.GetHashCode(), "" + node.Representation + "");

        root.AddProperty("color", "dodgerblue");
        root.AddProperty("tooltip", nameof(OperationNode));

        if (node.Representation == "++" || node.Representation == "--") {
            root = new GraphNode(node.GetHashCode(), (node.OperationType.EndsWith("post") ? "(postfix)" : "(prefix)") + node.Representation);
        }

        foreach (var child in node.Operands)
        {
            root.AddNode(ToGraphNode((dynamic)child));
        }

        return root;
    }

    public static GraphNode ToGraphNode(AssignmentNode node) {
        var root = new GraphNode(node.GetHashCode(), "" + node.Name + "");

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
            root = new GraphNode(node.GetHashCode(), name.Representation + "()");

        } else {
            root = new GraphNode(node.GetHashCode(), "call");

            var called = new GraphNode("function");

            called.AddNode(ToGraphNode((dynamic)node.FunctionName));

            root.AddNode(called);
        }

        root.AddProperty("color", "tomato");
        root.AddProperty("tooltip", "call to " + nameof(FunctionCallNode));

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

        var parameters = new GraphNode(node.Parameters.GetHashCode(), "param");

        parameters.AddProperty("tooltip", "parameters");

        foreach (var parameter in node.Parameters) {
            parameters.AddNode(ToGraphNode(parameter, "parameter"));
        }

        root.AddNode(parameters);

        var body = ToGraphNode(node.Value);

        body.AddProperty("tooltip", "body");

        root.AddNode(body);

        return root;
    }

    public static GraphNode ToGraphNode(ArrayLiteralNode node) {
        var root = new GraphNode(node.GetHashCode(), "[...] (" + node.Content.Count + " item(s))");

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

    public static GraphNode ToGraphNode(SimpleBlock block) {
        var root = new GraphNode(block.GetHashCode(), "block");

        root.AddProperty("color", "darkviolet");
        root.AddProperty("tooltip", nameof(SimpleBlock));

        foreach (var statement in block.Content) {
            var statementNode = ToGraphNode((dynamic)statement);

            statementNode.AddProperty("tooltip", "statement");

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