using System;
using System.IO;

class Program
{
    static void Main(string[] _) {

		// lil hack for our vs 2019 users, which thinks it's a rebel becasue it doesn't use the same
		// working directory as literally every other major IDE + the official fucking CLI
		// used to love vs 2019, but honestly I think I'm switching to vs code for most things 
		// and not loooking back
		var file = new FileInfo(
			Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location))
				.Parent
				.Parent
				.FullName
			+ /*Path.DirectorySeparatorChar +*/ "/test.txt");

        // Initializes the tokenizer with the content of the "sample.txt" file
        var tokenizer = new LotusTokenizer(file);

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