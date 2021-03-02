using System.IO;
using System.Reflection;
using System.Globalization;

using System.CommandLine;

//#pragma warning disable
partial class Program
{
    static RootCommand _cli;
    static int Main(string[] args)
        => _cli.Invoke(args);

    static Program() {
        // Lil hack for our visual studio (win and mac) users, whose IDE thinks it's a rebel
        // because it doesn't use the same working directory as literally every other
        // major IDE + the official fucking CLI. Used to love vs 2019, but honestly
        // I think I'm switching to vs code for most things and not loooking back.
#if VS
        Directory.SetCurrentDirectory(
            Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .Parent
                .Parent
                .FullName
        );
#endif
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        sourceCodeFile = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "test.txt"));

        _cli = BuildRootCommand();
    }

    static void AddGraphPrelude(Graph g, FileInfo file)
        => AddGraphPrelude(g, Path.GetFileName(file.Name));

    private static void AddGraphPrelude(Graph g, Union<string, FileInfo, Uri> file) {
        var path = file.Match(
            str => str,
            info => info.Name,
            uri => uri.LocalPath
        );

        g.AddNodeProp("fontname", "Consolas, monospace");
        g.AddGraphProp("fontname", "Consolas, monospace");
        g.AddGraphProp("label", $"Abstract Syntax Tree of {path}\\n\\n");
        g.AddGraphProp("labelloc", "top");
    }

    static Graph MakeGraph(FileInfo file, bool force, out int exitCode) {
        var tokenizer = GetTokenizerForFile(file);
        exitCode = HandleParsing(tokenizer, out var tlNodes);

        var g = new Graph("AST");

        if (exitCode == 0 || force) {
            AddGraphPrelude(g, file);

            foreach (var node in tlNodes) {
                g.AddNode(ASTHelper.ToGraphNode(node));
            }
        }

        return g;
    }

    static Graph MakeGraph<T>(
        FileInfo file,
        Func<IEnumerable<TopLevelNode>, IEnumerable<T>> filter,
        Func<T, GraphNode> transform,
        bool force,
        out int exitCode
    ) {
        var tokenizer = GetTokenizerForFile(file);
        exitCode = HandleParsing(tokenizer, out var tlNodes);

        var g = new Graph("AST");

        if (exitCode == 0 || force) {
            var nodes = filter(tlNodes);

            AddGraphPrelude(g, file);

            foreach (var node in nodes) {
                g.AddNode(transform(node));
            }
        }

        return g;
    }

    static LotusTokenizer GetTokenizerForFile(FileInfo file) {
        LotusTokenizer tokenizer;

        var fileStr = file.ToString();

        if (fileStr == "-") {
            using var stdin = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);

            tokenizer = new LotusTokenizer(stdin.ReadToEnd());
        } else {
            tokenizer = new LotusTokenizer(new Uri(file.FullName));
        }

        return tokenizer;
    }

    static int HandleParsing(LotusTokenizer tokenizer, out List<TopLevelNode> nodes) {
        var parser = new TopLevelParser(tokenizer);

        nodes = parser.ToList();

        if (Logger.HasErrors) {
            Logger.PrintAllErrors();

            return 1;
        }

        return 0;
    }

    static int HandleParsing(FileInfo file, out List<TopLevelNode> nodes)
        => HandleParsing(GetTokenizerForFile(file), out nodes);
}
#pragma warning restore