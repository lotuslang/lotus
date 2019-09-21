using System;
using System.IO;

class Program
{
    static void Main(string[] args) {

        // Initializes the tokenizer with the content of the "sample.txt" file
        //var tokenizer = new Tokenizer(new FileInfo(Directory.GetCurrentDirectory() + "/sample.txt"));

        // Repeatedly consumes a token until we can't anymore
        /* while (tokenizer.Consume(out Token item)) {
            Console.WriteLine($"{item.Location} {item.Kind} : {item.Representation}");
        }*/


        var tokenizer = new Tokenizer("6 + arr[6 + 3] - 9");


        var parser = new Parser(tokenizer);

        /* var expr = "a = 5 + 6 * (max (lo, fi + 6) ^ 2)";
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
        g.AddGraphProp("label", "Abstract Syntax Tree of a Block program\\n\\n");
        g.AddGraphProp("labelloc", "top");

        StatementNode val = parser.ConsumeValue();

        g.AddNode(ToGraphNode((dynamic)val));

        /*while (val != null) {
            g.AddNode(ToGraphNode((dynamic)val));
            val = parser.Consume();
        }*/

        Console.WriteLine(g.ToText());

        // DEBUG
        //tokens.ForEach(t => Console.WriteLine(t));

        // Initializes the parser with the ('reseted') tokenizer
        //var parser = new Parser(tokenizer);
    }

    public static GraphNode ToGraphNode(ValueNode node) {
        var output = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Representation + "\"");

        output.AddProperty("color", "lightgrey");
        output.AddProperty("tooltip", "ValueNode");

        return output;
    }

    public static GraphNode ToGraphNode(StringNode node) {
        var output = new GraphNode(node.GetHashCode().ToString(), "\"'" + node.Representation.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\"") + "'\"");

        output.AddProperty("color", "orange");
        output.AddProperty("tooltip", "StringNode");

        return output;
    }

    public static GraphNode ToGraphNode(StatementNode node) {
        var output = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Representation + "\"");

        output.AddProperty("color", "black");
        output.AddProperty("tooltip", "StatementNode");

        return output;
    }

    public static GraphNode ToGraphNode(OperationNode node) {

        var root = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Representation + "\"");

        root.AddProperty("color", "dodgerblue");
        root.AddProperty("tooltip", "OperationNode");

        if (node.Representation == "++" || node.Representation == "--") {
            root = new GraphNode(node.GetHashCode().ToString(), ((node as OperationNode).OperationType.EndsWith("post") ? "\"(postfix)" : "\"(prefix)") + node.Representation + "\"");
        }

        if (node.Representation == "_") {
            root = new GraphNode(node.GetHashCode().ToString(), "\"-\"");
        }

        foreach (var child in (node as OperationNode).Operands)
        {
            root.AddNode(ToGraphNode((dynamic)child));
        }

        return root;
    }

    public static GraphNode ToGraphNode(AssignmentNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Name + "\"");

        root.AddProperty("color", "teal");
        root.AddProperty("tooltip", "AssignmentNode");

        root.AddNode(ToGraphNode((dynamic)node.Value));

        return root;
    }

    public static GraphNode ToGraphNode(DeclarationNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"var\"");

        root.AddProperty("color", "palegreen");
        root.AddProperty("tooltip", "DeclarationNode");

        var nameNode = new GraphNode(node.Name.GetHashCode().ToString(), "\"" + node.Name + "\"");

        nameNode.AddProperty("tooltip", "name");

        root.AddNode(nameNode);

        root.AddNode(ToGraphNode((dynamic)node.Value));

        return root;
    }

    public static GraphNode ToGraphNode(FunctionCallNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Name + "()\"");

        root.AddProperty("color", "tomato");
        root.AddProperty("tooltip", "FunctionCallNode");

        foreach (var parameter in node.CallingParameters)
        {
            var paramNode = ToGraphNode((dynamic)parameter);

            paramNode.AddProperty("tooltip", "argument");

            root.AddNode(paramNode);
        }

        return root;
    }

    public static GraphNode ToGraphNode(FunctionDeclarationNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"def " + node.Name.Representation + "\"");

        root.AddProperty("color", "indianred");
        root.AddProperty("tooltip", "FunctionDeclarationNode");

        var parameters = new GraphNode(node.Parameters.GetHashCode().ToString(), "\"param\"");

        parameters.AddProperty("tooltip", "parameters");

        foreach (var parameter in node.Parameters) {
            var paramNode = new GraphNode(parameter.GetHashCode().ToString(), parameter.Representation);

            paramNode.AddProperty("tooltip", "parameter");

            parameters.AddNode(paramNode);
        }

        root.AddNode(parameters);

        var value = ToGraphNode((dynamic)node.Value);

        value.AddProperty("tooltip", "body");

        root.AddNode(value);

        return root;
    }

    public static GraphNode ToGraphNode(SimpleBlock block) {
        var root = new GraphNode(block.GetHashCode().ToString(), "\"block\"");

        root.AddProperty("color", "green");
        root.AddProperty("tooltip", "SimpleBlock");

        foreach (var statement in block.Content) {
            var statementNode = ToGraphNode((dynamic)statement);

            statementNode.AddProperty("tooltip", "statement");

            root.AddNode(statementNode);
        }

        return root;
    }
}