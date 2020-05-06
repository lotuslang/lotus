using System;
using System.IO;

class Program
{
    static void Main(string[] _) {

        // Initializes the tokenizer with the content of the "sample.txt" file
        var tokenizer = new LotusTokenizer(new FileInfo(Directory.GetCurrentDirectory() + "/test.txt"));

        tokenizer = new LotusTokenizer(@"
        def hey() {
            return !a && b || c ^^ d && !e
        }");

        var parser = new LotusParser(tokenizer);

        var g = new Graph("AST");

        g.AddNodeProp("fontname", "Consolas, monospace");
        g.AddGraphProp("fontname", "Consolas, monospace");
        g.AddGraphProp("label", $"Abstract Syntax Tree of {parser.Position.filename}\\n\\n");
        g.AddGraphProp("labelloc", "top");

        while (parser.Consume(out StatementNode node)) {
            g.AddNode(node.ToGraphNode());
        }

        Console.WriteLine(g.ToText());
    }
}