using System.IO;
using System.Reflection;
using System.Globalization;
using System.Threading.Tasks;

using System.CommandLine;
using System.CommandLine.Parsing;

//#pragma warning disable
partial class Program
{
    static FileInfo sourceCodeFile;

    static RootCommand BuildRootCommand() {
        var rootCommand = new RootCommand("A lotus parser/typechecker") {
            TreatUnmatchedTokensAsErrors = true
        };

        var forceOption = new Option<bool>("--force", "Ignore compilation errors before executing commands");
        forceOption.AddAlias("-f");

        var constantOption = new Option<bool>("--const", "Apply constant-coloring on the AST graph");
        constantOption.AddAlias("-c");

        var fileArgument = new Argument<FileInfo>("input", "The file to read code from");
        fileArgument.SetDefaultValue(sourceCodeFile);
        fileArgument.LegalFileNamesOnly();
        fileArgument.Arity = ArgumentArity.ZeroOrOne;
        fileArgument.AddValidator((result) => {
            if (result.Tokens.Count == 0)
                return;

            var filename = result.Tokens[0].Value;

            if (filename != "-" && !File.Exists(filename)) {
                result.ErrorMessage = result.LocalizationResources.FileDoesNotExist(filename);
            }
        });

        var silentVerb = new Command("silent", "Don't print anything to stdout (errors go to stderr)");
        silentVerb.AddArgument(fileArgument);
        silentVerb.SetHandler(SilentHandler, fileArgument, forceOption);

        var printVerb = new Command("print", "Reconstruct the source file from the AST and print it");
        printVerb.AddArgument(fileArgument);
        printVerb.SetHandler(PrintHandler, fileArgument, forceOption);

        var hashVerb = new Command("hash", "Print the hash of the AST graph");
        hashVerb.AddArgument(fileArgument);
        hashVerb.AddOption(constantOption);
        hashVerb.SetHandler(HashHandler, fileArgument, constantOption, forceOption);

        var graphVerb = new Command("graph", "Print graphviz code for the AST graph");
        graphVerb.AddArgument(fileArgument);
        graphVerb.AddOption(constantOption);
        graphVerb.SetHandler(GraphHandler, fileArgument, constantOption, forceOption);

        rootCommand.Add(forceOption);

        rootCommand.Add(silentVerb);
        rootCommand.Add(printVerb);
        rootCommand.Add(hashVerb);
        rootCommand.Add(graphVerb);

        //rootCommand.AddArgument(fileArgument);

        return rootCommand;
    }

    static Task<int> GraphHandler(FileInfo file, bool useConstants, bool force) {
        var tokenizer = GetTokenizerForFile(file);
        var exitCode = HandleParsing(tokenizer, out var tlNodes);

        if (exitCode != 0 && !force) {
            return Task.FromResult(exitCode);
        }

        Console.WriteLine(MakeGraph(tlNodes, useConstants, new Uri(file.FullName)).ToText());

        return Task.FromResult(0);
    }

    static Task<int> HashHandler(FileInfo file, bool useConstants, bool force) {
        var tokenizer = GetTokenizerForFile(file);
        var exitCode = HandleParsing(tokenizer, out var tlNodes);

        if (exitCode != 0 && !force) {
            return Task.FromResult(exitCode);
        }

        Console.WriteLine(MakeGraph(tlNodes, useConstants, new Uri(file.FullName)).GetHashCode());

        return Task.FromResult(0);
    }

    static Task<int> SilentHandler(FileInfo file, bool force)
        => Task.FromResult(force ? 0 : HandleParsing(file, out _));

    static Task<int> PrintHandler(FileInfo file, bool force) {
        var tokenizer = GetTokenizerForFile(file);
        var exitCode = HandleParsing(tokenizer, out var tlNodes);

        if (exitCode != 0 && !force) {
            return Task.FromResult(exitCode);
        }

        var stmtNodes = tlNodes.WhereType<TopLevelStatementNode>();

        foreach (var node in stmtNodes) {
            Console.Write(ASTHelper.PrintStatement(node));
        }

        string s;

        if ((s = ASTHelper.PrintToken(tokenizer.Current)).Length >= 2) {
            // print the last (EOF) token, which is not consumed by the parser
            Console.WriteLine(s[..^2]);
        }

        return Task.FromResult(0);
    }
}