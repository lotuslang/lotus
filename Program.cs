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


        var tokenizer = new Tokenizer("-1");


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

        var g = new Graph("G");

        g.AddNodeProp("fontname", "Consolas, monospace");
        g.AddGraphProp("fontname", "Consolas, monospace");

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

        return output;
    }

    public static GraphNode ToGraphNode(StringNode node) {
        var output = new GraphNode(node.GetHashCode().ToString(), "\"'" + node.Representation.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\"") + "'\"");

        output.AddProperty("color", "orange");

        return output;
    }

    public static GraphNode ToGraphNode(StatementNode node) {
        var output = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Representation + "\"");

        output.AddProperty("color", "black");

        return output;
    }

    public static GraphNode ToGraphNode(OperationNode node) {

        var root = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Representation + "\"");

        root.AddProperty("color", "dodgerblue");

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

        root.AddNode(ToGraphNode((dynamic)node.Value));

        return root;
    }

    public static GraphNode ToGraphNode(DeclarationNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"var\"");

        root.AddProperty("color", "palegreen");

        root.AddNode(new GraphNode(node.Name.GetHashCode().ToString(), "\"" + node.Name + "\""));

        root.AddNode(ToGraphNode((dynamic)node.Value));

        return root;
    }

    public static GraphNode ToGraphNode(FunctionCallNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Name + "()\"");

        root.AddProperty("color", "tomato");

        foreach (var parameter in node.CallingParameters)
        {
            root.AddNode(ToGraphNode((dynamic)parameter));
        }

        return root;
    }

    public static GraphNode ToGraphNode(FunctionDeclarationNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"def " + node.Name.Representation + "\"");

        root.AddProperty("color", "indianred");

        var paramNode = new GraphNode(node.Parameters.GetHashCode().ToString(), "\"param\"");

        foreach (var parameter in node.Parameters) {
            paramNode.AddNode(new GraphNode(parameter.GetHashCode().ToString(), parameter.Representation));
        }

        root.AddNode(paramNode);

        root.AddNode(ToGraphNode((dynamic)node.Value));

        return root;
    }

    public static GraphNode ToGraphNode(SimpleBlock block) {
        var root = new GraphNode(block.GetHashCode().ToString(), "\"block\"");

        root.AddProperty("color", "green");

        foreach (var statement in block.Content) {
            root.AddNode(ToGraphNode((dynamic)statement));
        }

        return root;
    }
}