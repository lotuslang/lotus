using System.IO;
using System.Reflection;
using System.Globalization;

using System.CommandLine;

//#pragma warning disable
partial class Program
{
    static int Main(string[] args)
        => BuildRootCommand().Invoke(args);

    static Program() {
        // Lil hack for our visual studio (win and mac) users, whose IDE thinks it's a rebel
        // because it doesn't use the same working directory as literally every other
        // major IDE + the official fucking CLI. Used to love vs 2019, but honestly
        // I think I'm switching to vs code for most things and not loooking back.
#pragma warning disable
        Directory.SetCurrentDirectory(
            Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .Parent
                .Parent
                .FullName
        );
#pragma warning restore
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        sourceCodeFile = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "test.txt"));
    }

    static void AddGraphPrelude(Graph g, Uri file) {
        g.AddNodeProp("fontname", "Consolas, monospace");
        g.AddGraphProp("fontname", "Consolas, monospace");
        g.AddGraphProp("label", $"Abstract Syntax Tree of {Path.GetFileName(file.LocalPath)}\\n\\n");
        g.AddGraphProp("labelloc", "top");
    }

    static Graph MakeGraph(IEnumerable<TopLevelNode> nodes, bool useConstants, Uri file) {
        var statementNodes =
            nodes
                .WhereType<TopLevelStatementNode>()
                .Select(n => n.Statement);

        var valueNodes =
            statementNodes
                .WhereType<StatementExpressionNode>()
                .Select(n => n.Value);

        var g = new Graph("AST");

        AddGraphPrelude(g, file);

        if (useConstants) {
            foreach (var node in statementNodes) {
                g.AddNode(ASTHelper.ShowConstants(node));
            }
        } else {
            foreach (var node in nodes) {
                g.AddNode(ASTHelper.ToGraphNode(node));
            }
        }

        return g;
    }

    static LotusTokenizer GetTokenizerForFile(FileInfo file) {
        LotusTokenizer tokenizer;

        var fileStr = file.ToString();

        if (fileStr == "-") {
            Console.WriteLine("Reading from stdin");

            using var stdin = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);

            tokenizer = new LotusTokenizer(stdin.ReadToEnd());
        } else {
            tokenizer = new LotusTokenizer(new Uri(file.FullName));
        }

        return tokenizer;
    }

    static int HandleParsing(LotusTokenizer tokenizer, out List<TopLevelNode> nodes) {
        var parser = new TopLevelParser(tokenizer);

        nodes = (parser.Select(node => node as TopLevelNode)).ToList();

        if (Logger.HasErrors) {
            var count = Logger.ErrorCount;
            Logger.PrintAllErrors();
            Console.Error.WriteLine("In total, there were " + count + " errors. Please fix them.");

            return 1;
        }

        return 0;
    }

    static int HandleParsing(FileInfo file, out List<TopLevelNode> nodes)
        => HandleParsing(GetTokenizerForFile(file), out nodes);
}
#pragma warning restore