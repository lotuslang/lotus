using System.IO;
using System.Reflection;
using System.Globalization;

#pragma warning disable
class Program
{
    static int Main(string[] args) {

        // Lil hack for our visual studio (win and mac) users, whose IDE thinks it's a rebel
        // because it doesn't use the same working directory as literally every other
        // major IDE + the official fucking CLI. Used to love vs 2019, but honestly
        // I think I'm switching to vs code for most things and not loooking back.

        Directory.SetCurrentDirectory(
            Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .Parent
                .Parent
                .FullName
        );

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        var argList = args.ToList();

        var forced = argList.RemoveAll(arg => arg is "--force" or "-f") > 0;

        args = argList.ToArray();

        if (args.Length == 0 || args[0] == "help") {
            Console.Error.WriteLine(@"
Usage:  dotnet run -- [option]
        dotnet run
        parsex [option]
        parsex

    help              display this help message
    silent            don't print anything to stdout (errors go to stderr)
    print             reconstruct the source file from the AST and print it
    hash              print the hash of the AST
        constant          print the hash of the constant-colored AST
    graph             print dot code of the AST
        constant          print dot code of the AST with constant-coloring

Note: '--force' or '-f' ignores compilation errors before executing above commands
            ");

            return 1;
        }

        var file = new Uri(Directory.GetCurrentDirectory() + "/./test.txt");

        // Initializes the tokenizer with the content of the "sample.txt" file
        var tokenizer = new LotusTokenizer(file);

        /*
        tokenizer = new LotusTokenizer(@"return (new (string.hello)()[1] + hello)");
        //*/

        var parser = new TopLevelParser(tokenizer);

        var tlNodes = (parser.Select(node => node as TopLevelNode)).ToList();

        if (Logger.HasErrors) {
            var count = Logger.ErrorCount;
            Logger.PrintAllErrors();
            Console.Error.WriteLine("In total, there were " + count + " errors. Please fix them.");

            if (!forced)
                return 1;
        }

        if (args[0] == "graph") {
            Console.WriteLine(MakeGraph(tlNodes, args, file).ToText());
            return 0;
        }

        if (args[0] == "silent") return 0;

        if (args[0] == "print") {
            foreach (var node in tlNodes.Select(n => (n is TopLevelStatementNode stmt) ? stmt : null).Where(n => n is not null)) {
                Console.Write(ASTHelper.PrintStatement(node));
            }

            string s;

            if ((s = ASTHelper.PrintToken(tokenizer.Current)).Length >= 2) {
                // print the last (EOF) token, which is not consumed by the parser
                Console.WriteLine(s[..^2]);
            }

            return 0;
        }

        if (args[0] == "hash") {
            Console.WriteLine(MakeGraph(tlNodes, args, file).GetHashCode());
            return 0;
        }

        Console.WriteLine("Could not understand option " + args[0]);
        return 1;
    }

    static void AddGraphPrelude(Graph g, Uri file) {
        g.AddNodeProp("fontname", "Consolas, monospace");
        g.AddGraphProp("fontname", "Consolas, monospace");
        g.AddGraphProp("label", $"Abstract Syntax Tree of {Path.GetFileName(file.LocalPath)}\\n\\n");
        g.AddGraphProp("labelloc", "top");
    }

    static Graph MakeGraph(IEnumerable<TopLevelNode> nodes, string[] args, Uri file) {
        var statementNodes =
            nodes
                .Where(n => n is TopLevelStatementNode)
                .Select(n => (n as TopLevelStatementNode).Statement);

        var valueNodes =
            statementNodes
                .Where(n => n is StatementExpressionNode)
                .Select(n => (n as StatementExpressionNode).Value);

        var g = new Graph("AST");

        AddGraphPrelude(g, file);

        if (args.Length == 2) {
            if (args[1] == "constant") {
                foreach (var node in statementNodes) {
                    g.AddNode(ASTHelper.ShowConstants(node));
                }
            } else {
                Console.Error.WriteLine("Could not understand option " + args[1]);
                return new Graph("ERROR");
            }
        } else {
            foreach (var node in nodes) {
                g.AddNode(ASTHelper.ToGraphNode(node));
            }
        }

        return g;
    }
}
#pragma warning restore