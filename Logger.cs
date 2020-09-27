using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class Logger
{

    public static Queue<(Exception, Location)> exceptions = new Queue<(Exception, Location)>();

    public static bool HasErrors => exceptions.Count != 0;

    public static void Log(string message, Location location)
        => Console.WriteLine($"{location}: {message}");

    public static void Warning(LotusException e)
        => Console.Error.WriteLine(e.CallerString + " : @ " + e.Position + "\t" + e.Message);

    public static void Error(Exception e, Location location)
        => exceptions.Enqueue((e, location));

    public static void Error(LotusException e)
        => exceptions.Enqueue((e, e.Position));

    public static void Error(string message, Location location)
        => exceptions.Enqueue((new Exception(message), location));

    public static Exception Fatal(Exception e)
        => e; // TODO: Do fancy stuff with method (like pretty-printing the exception)

    public static void PrintAllErrors() {
        foreach (var (exception, location) in exceptions) {
            Console.Error.WriteLine(exception.Message); // TODO: colour stuff

            Console.Error.WriteLine();

            if (location.filename != "<std>") {
                Console.Error.WriteLine(GetTextAt(location));
            } else {
                Console.Error.WriteLine("<Cannot print source code>");
            }

            Console.Error.WriteLine('\n');
        }
    }

    // !! WARNING !! I had to invoke dark powers to write those functions, hopefully without bugs, so edit with *extreme* care
    // It takes way longer to figure this out well enough than it looks, so please again : beware !
    private static string GetTextAt(Location position) {
        var (line, column, file) = position;

        var output = "";

        var pre2Line = GetLineAt(line - 2, file);

        if (pre2Line != "") output += pre2Line + '\n';

        var pre1Line = GetLineAt(line - 1, file);

        if (pre1Line != "") output += pre1Line + '\n';

        var actualLine = GetLineAt(line, file);

        if (actualLine == "") {
            output += "EOF";

            return output;
        }

        output += actualLine + '\n';

        var padding = new string(' ',
              Utilities.GetNumberOfDigits(line) // the space the line number takes
            + 3                                 // the space for " | "
            + (column - 1)                      // the space before the token
        );

        output += padding + "^ error starts here\n";

        var post1Line = GetLineAt(line + 1, file);

        if (post1Line != "") output += post1Line + '\n';

        var post2Line = GetLineAt(line + 2, file);

        if (post2Line != "") output += post2Line + '\n';

        return output;

    }

    // This took way longer than it looks, trust me.
    private static string GetLineAt(int line, string file) {

        var sourceText = File.ReadAllLines(file);

        if (line < 1 || line > sourceText.Length) return "";

        return line + " | " + sourceText[line - 1];
    }
}