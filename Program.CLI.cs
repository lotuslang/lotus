using System.IO;
using System.Threading.Tasks;

using System.CommandLine;

partial class Program
{
    static FileInfo _sourceCodeFile;

    static RootCommand BuildRootCommand() {
        var forceOption = new Option<bool>("--force", "Ignore compilation errors before executing commands");
        forceOption.AddAlias("-f");

        var fileArgument = new Argument<FileInfo>("input", "The file to read code from");
        fileArgument.SetDefaultValue(_sourceCodeFile);
        fileArgument.LegalFilePathsOnly();
        fileArgument.Arity = ArgumentArity.ZeroOrOne;
        fileArgument.AddValidator((result) => {
            if (result.Tokens.Count == 0)
                return;

            var filename = result.Tokens[0].Value;

            if (filename != "-" && !File.Exists(filename)) {
                result.ErrorMessage = result.LocalizationResources.FileDoesNotExist(filename);
            }
        });

        Command makeConstSubCmd(Action<Graph> act) {
            var cmd = new Command("const", "Apply constant-coloring to the AST graph");
            cmd.AddArgument(fileArgument);
            cmd.SetHandler(ConstHandlerFactory(act), fileArgument, forceOption);
            return cmd;
        }

        /*
        *   parsex silent [file.txt]
        */

        var silentVerb = new Command("silent", "Don't print anything to stdout (errors go to stderr)");
        silentVerb.AddArgument(fileArgument);
        silentVerb.SetHandler(
            (file, force) => Task.FromResult(force ? 0 : HandleParsing(file, out _)),
            fileArgument,
            forceOption
        );


        /*
        *   parsex print [file.txt]
        */

        var printVerb = new Command("print", "Reconstruct the source file from the AST and print it");
        printVerb.AddArgument(fileArgument);
        printVerb.SetHandler(PrintHandler, fileArgument, forceOption);


        /*
        *   parsex hash [file.txt]
        *   parsex hash const [file.txt]
        */

        var hashVerb = new Command("hash", "Print the hash of the AST graph") {
            fileArgument
        };

        hashVerb.SetHandler(
            GraphHandlerFactory(
                g => Console.WriteLine(g.GetHashCode())
            ),
            fileArgument,
            forceOption
        );

        hashVerb.AddCommand(makeConstSubCmd(g => Console.WriteLine(g.GetHashCode())));


        /*
        *   parsex graph [file.txt]
        *   parsex graph const [file.txt]
        */

        var graphVerb = new Command("graph", "Print graphviz code for the AST graph") {
            fileArgument
        };

        graphVerb.SetHandler(
            GraphHandlerFactory(
                g => Console.WriteLine(g.ToText())
            ),
            fileArgument,
            forceOption
        );

        graphVerb.AddCommand(makeConstSubCmd(g => Console.WriteLine(g.ToText())));

        /*
        *   parsex [--force] {silent, print, hash, graph}
        */

        var rootCommand = new RootCommand("A lotus parser/typechecker") {
            forceOption, // global option
            silentVerb,
            printVerb,
            hashVerb,
            graphVerb,
        };

        rootCommand.TreatUnmatchedTokensAsErrors = true;

        return rootCommand;
    }

    static Func<FileInfo, bool, Task<int>> GraphHandlerFactory(Action<Graph> act)
        => (file, force) => {
            var g = MakeGraph(file, force, out int exitCode);

            if (exitCode != 0 && !force) {
                return Task.FromResult(exitCode);
            }

            act(g);
            return Task.FromResult(0);
        };

    static Func<FileInfo, bool, Task<int>> ConstHandlerFactory(Action<Graph> act)
        => (file, force) => {
            var g = MakeGraph(
                file,
                Enumerable.OfType<TopLevelStatementNode>, // filter
                (n => ASTHelper.ShowConstants(n)), // transform
                force,
                out int exitCode
            );

            if (exitCode != 0 && !force) {
                return Task.FromResult(exitCode);
            }

            act(g);
            return Task.FromResult(0);
        };

    static Task<int> PrintHandler(FileInfo file, bool force) {
        var tokenizer = GetTokenizerForFile(file);
        var exitCode = HandleParsing(tokenizer, out var tlNodes);

        if (exitCode != 0 && !force) {
            return Task.FromResult(exitCode);
        }

        foreach (var node in tlNodes) {
            Console.Write(ASTHelper.PrintTopLevel(node));
        }

        string s;

        if ((s = ASTHelper.PrintToken(tokenizer.Current)).Length >= 2) {
            // print the last (EOF) token, which is not consumed by the parser
            Console.WriteLine(s[..^2]);
        }

        return Task.FromResult(0);
    }
}