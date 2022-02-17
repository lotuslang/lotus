using System.IO;

public static class Logger
{
    public static Stack<LotusError> errorStack = new();

    public static int ErrorCount => errorStack.Count;

    public static bool HasErrors => errorStack.Count != 0;

    public static void Log(string message, LocationRange location)
        => Console.WriteLine($"{location}: {message}");

    public static void Warning(string message, LocationRange location)
        => Console.Error.WriteLine(location + " : " + message);

    public static void Warning(LotusError e)
        => Console.Error.WriteLine(e.Caller + " : \t" + e.Message);

    public static void Error(LotusError e)
        => errorStack.Push(e);

    public static Exception Fatal(Exception e) {
        if (ErrorCount > 0)
            PrintAllErrors();

        return e; // TODO: Do fancy stuff with method (like pretty-printing the exception)
    }

    public static void PrintAllErrors() {
        foreach (var error in errorStack.Reverse()) {

            Console.Error.WriteLine(error.Message); // TODO: colour stuff

            Console.Error.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;

            if (error is ILocalized eLoc && eLoc.Location.filename != "<std>") {
                Console.Error.WriteLine(GetTextAt(eLoc.Location));
            } else {
                Console.Error.WriteLine("<Cannot print source code>");
            }

            Console.ResetColor();

            Console.Error.WriteLine('\n');
        }
    }




    // !! WARNING !! I had to invoke dark powers to write those functions, hopefully without bugs, so edit with *extreme* care
    // It takes way longer to figure out well enough than it may look at first, so please again : beware !
    public static string GetTextAt(LocationRange range) {
        if (range.LineLength < 2) {
            if (range.ColumnLength < 2)
                return GetTextAtPoint(range.GetFirstLocation());
            else
                return GetTextAtLine(range);
        }

        return GetTextAtLines(range);
    }

    private static string GetTextAtLines(LocationRange range) {
        var (firstLine, lastLine, lineLength, firstColumn, _, _, file) = range;

        var output = "";

        var pre2Line = PrintLineAt(firstLine - 2, file);

        if (pre2Line != "") output += pre2Line + '\n';

        var pre1Line = PrintLineAt(firstLine - 1, file);

        if (pre1Line != "") output += pre1Line + '\n';

        /*var actualLine = PrintLineAt(firstLine, file, '>');

        if (actualLine == "") {
            output += "EOF";

            return output;
        }

        output += actualLine + '\n';

        var padding = new string(' ',
              Utilities.GetNumberOfDigits(firstLine) // the space the line number takes
            + 3                                      // the space for " > "
            + (firstColumn - 1)                      // the space before the token
        );

        output += padding + "^ error starts here\n";*/

        var padding = new string(' ',
              Utilities.GetNumberOfDigits(firstLine) // the space the line number takes
            + 3                                      // the space for " > "
            + (firstColumn - 1)                      // the space before the token
        );

        output += padding + "v error starts here\n";

        for (int i = 0; i < lineLength; i++) {
            var actualLine = PrintLineAt(firstLine + i, file, '>');

            if (actualLine == "") {
                output += "EOF";

                return output;
            }

            output += actualLine + '\n';
        }


        var post1Line = PrintLineAt(lastLine + 1, file);

        if (post1Line != "") output += post1Line + '\n';

        var post2Line = PrintLineAt(lastLine + 2, file);

        if (post2Line != "") output += post2Line + '\n';

        return output;
    }

    private static string GetTextAtLine(LocationRange range) {
        var (firstLine, _, _, firstColumn, _, columnLength, file) = range;

        var output = "";

        var pre2Line = PrintLineAt(firstLine - 2, file);

        if (pre2Line != "") output += pre2Line + '\n';

        var pre1Line = PrintLineAt(firstLine - 1, file);

        if (pre1Line != "") output += pre1Line + '\n';

        var padding = new string(' ',
              Utilities.GetNumberOfDigits(firstLine) // the space the line number takes
            + 3                                      // the space for " | "
            + (firstColumn - 1)                      // the space before the token
        );

        var actualLine = PrintLineAt(firstLine, file);

        if (actualLine == "") {
            output += "EOF";

            return output;
        }

        output += actualLine + '\n';

        output += padding + new string('^', columnLength) + " right here\n";


        var post1Line = PrintLineAt(firstLine + 1, file);

        if (post1Line != "") output += post1Line + '\n';

        var post2Line = PrintLineAt(firstLine + 2, file);

        if (post2Line != "") output += post2Line + '\n';

        return output;
    }

    private static string GetTextAtPoint(Location position) {
        var (line, column, file) = position;

        var output = "";

        var pre2Line = PrintLineAt(line - 2, file);

        if (pre2Line != "") output += pre2Line + '\n';

        var pre1Line = PrintLineAt(line - 1, file);

        if (pre1Line != "") output += pre1Line + '\n';

        var actualLine = PrintLineAt(line, file);

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

        var post1Line = PrintLineAt(line + 1, file);

        if (post1Line != "") output += post1Line + '\n';

        var post2Line = PrintLineAt(line + 2, file);

        if (post2Line != "") output += post2Line + '\n';

        return output;

    }

    private static string PrintLineAt(int line, string file) {

        var sourceText = File.ReadLines(file).Skip(line - 1).FirstOrDefault();

        if (line < 1 || sourceText == null) return "";

        return line + " | " + sourceText;
    }

    // This took way longer than it looks, trust me.
    private static string PrintLineAt(int line, string file, char separator) {

        var sourceText = File.ReadLines(file).Skip(line - 1).FirstOrDefault();

        if (line < 1 || sourceText == null) return "";

        return line + " " + separator + " " + sourceText;
    }
}