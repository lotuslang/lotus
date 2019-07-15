using System;
using System.IO;

class Program
{
    static void Main(string[] args) {

        // Initializes the tokenizer with the content of the "sample.txt" file
        var tokenizer = new Tokenizer(new FileInfo(Directory.GetCurrentDirectory() + "/sample.txt"));

        // Temp variable to store the result of tokenizer.Consume
        var success = true;

        // Repeatedly consumes a token until we can't anymore
        while (success) {
            var token = tokenizer.Consume(out success);
            //Console.WriteLine($"{token.Location} {token.Kind} : {token.Representation}");
        }

        // Resets the tokenizer
        tokenizer = new Tokenizer(new FileInfo(Directory.GetCurrentDirectory() + "/sample.txt"));

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

        var val = parser.Consume();

        while (val != null) {
            g.AddNode(ToGraphNode((dynamic)val));
            val = parser.Consume();
        }

        Console.WriteLine(g.ToText());


        // DEBUG
        //tokens.ForEach(t => Console.WriteLine(t));

        // Initializes the parser with the (reseted) tokenizer
        //var parser = new Parser(tokenizer);
    }

    public static GraphNode ToGraphNode(ValueNode node) => new GraphNode(node.GetHashCode().ToString(), "\"" + node.Representation + "\"");

    public static GraphNode ToGraphNode(StringNode node) => new GraphNode(node.GetHashCode().ToString(), "\"'" + node.Representation + "'\"");

    public static GraphNode ToGraphNode(StatementNode node) => new GraphNode(node.GetHashCode().ToString(), "\"" + node.Representation + "\"");

    public static GraphNode ToGraphNode(OperationNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Representation + "\"");

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

    public static GraphNode ToGraphNode(AssignementNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Name + "\"");

        root.AddNode(ToGraphNode((dynamic)node.Value));

        return root;
    }

    public static GraphNode ToGraphNode(DeclarationNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"var\"");

        root.AddNode(new GraphNode(node.VariableName.GetHashCode().ToString(), "\"" + node.VariableName + "\""));

        root.AddNode(ToGraphNode((dynamic)node.Value));

        return root;
    }

    public static GraphNode ToGraphNode(FunctionNode node) {
        var root = new GraphNode(node.GetHashCode().ToString(), "\"" + node.Name + "()\"");

        root.AddNode(ToGraphNode((dynamic)node.Value));

        return root;
    }
}