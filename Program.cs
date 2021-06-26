using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;

#pragma warning disable
class Program
{
    static void Main(string[] args) {
#if VS
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
#endif

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        if (args.Length == 0 || args[0] == "help") {
            Console.Error.WriteLine(@"
Usage : dotnet run -- [option]
        dotnet run
        parsex [option]
        parsex

    - help              display this help message
    - silent            don't print anything to stdout (still prints errors to stderr)
    - print             reconstruct the source file from the AST and print it
    - hash              print the hash of the AST
        - constant          print the hash of the constant-colored AST
    - constant          print each top-level expression and its ""constantness""
        - all               print ALL expressions (nested and top-level)
    - graph             print dot code of the AST
        - constant          print dot code of the AST, coloring each value node depending on its ""constantness""
            ");

            return;
        }

        var file = new Uri(Directory.GetCurrentDirectory() + "/./test.txt");

        // Initializes the tokenizer with the content of the "sample.txt" file
        var tokenizer = new LotusTokenizer(file);

        /*tokenizer = new LotusTokenizer(@"return (hello + world)");*/

        var parser = new TopLevelParser(tokenizer);

        var tlNodes = new List<TopLevelNode>();

        foreach (var node in parser) {
            tlNodes.Add(node as TopLevelNode);
        }

        //Console.Error.WriteLine(Logger.GetTextAt(new LocationRange(14, 22, 1, 1, "test.txt")));

        //Console.Write(g.ToText());

        //Console.WriteLine(ASTHelper.IsName(nodes[0]));

        if (Logger.HasErrors) {
            var count = Logger.ErrorCount;
            Logger.PrintAllErrors();
            Console.Error.WriteLine("In total, there were " + count + " errors. Please fix them.");
            return;
        }

        var statementNodes =
            tlNodes
                .Where(n => n is TopLevelStatementNode)
                .Select(n => (n as TopLevelStatementNode).Statement);

        var valueNodes =
            statementNodes
                .Where(n => n is StatementExpressionNode)
                .Select(n => (n as StatementExpressionNode).Value);

        if (args[0] == "graph") {
            var g = new Graph("AST");

            g.AddNodeProp("fontname", "Consolas, monospace");
            g.AddGraphProp("fontname", "Consolas, monospace");
            g.AddGraphProp("label", $"Abstract Syntax Tree of {file.LocalPath}\\n\\n");
            g.AddGraphProp("labelloc", "top");

            if (args.Length == 2) {
                if (args[1] == "constant") {
                    foreach (var node in statementNodes) {
                        g.AddNode(ASTHelper.ShowConstants(node));
                    }
                } else {
                    Console.WriteLine("Could not understand option " + args[1]);
                    return;
                }
            } else {
                foreach (var node in tlNodes) {
                    g.AddNode(ASTHelper.ToGraphNode(node));
                }
            }

            Console.WriteLine(g.ToText());
            return;
        }

        if (args[0] == "silent") return;

        if (args[0] == "constant") {
            IEnumerable<ValueNode> values;

            if (args.Length == 2) {
                if (args[1] == "all") {
                    values = statementNodes.SelectMany(ASTHelper.ExtractValue);
                } else {
                    Console.WriteLine("Could not understand option " + args[1]);
                    return;
                }
            } else {
                values = valueNodes;
            }

            foreach (var node in values) {
                Console.WriteLine(
                    ASTHelper.PrintValue(node as ValueNode)
                  + (ASTHelper.IsContant(node as ValueNode) ? " => is a constant." : " => isn't a constant.")
                );
            }

            return;
        }

        if (args[0] == "print") {
            foreach (var node in statementNodes) {
                Console.Write(ASTHelper.PrintStatement(node));
            }

            // print the last (EOF) token, which is not consumed by the parser
            Console.WriteLine(ASTHelper.PrintToken(tokenizer.Current)[..^2]);
            return;
        }

        if (args[0] == "hash") {
            var g = new Graph("AST");

            g.AddNodeProp("fontname", "Consolas, monospace");
            g.AddGraphProp("fontname", "Consolas, monospace");
            g.AddGraphProp("label", $"Abstract Syntax Tree of {file.LocalPath}\\n\\n");
            g.AddGraphProp("labelloc", "top");

            if (args.Length == 2) {
                if (args[1] == "constant") {
                    foreach (var node in statementNodes) {
                        g.AddNode(ASTHelper.ShowConstants(node));
                    }
                } else {
                    Console.WriteLine("Could not understand option " + args[1]);
                    return;
                }
            } else {
                foreach (var node in tlNodes) {
                    g.AddNode(ASTHelper.ToGraphNode(node));
                }
            }

            Console.WriteLine(g.GetHashCode());
            return;
        }

        Console.WriteLine("Could not understand option " + args[0]);
    }
}
#pragma warning restore