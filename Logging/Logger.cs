using System.IO;
using System.Text;

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
        => Console.Error.WriteLine("WARNING : " + Format(e));

    public static void Error(LotusError e)
        => errorStack.Push(e);

    public static Exception Fatal(Exception e) {
        if (ErrorCount > 0)
            PrintAllErrors();

        return e; // TODO: Do fancy stuff with method (like pretty-printing the exception)
    }

    public static void PrintAllErrors() {
        Console.Error.WriteLine("In total, there were " + ErrorCount + " error(s). Please fix them before proceeding.\n");

        var orderedErrorStack = errorStack.Reverse()/*.OrderBy(e => {
            if (e is ILocalized el) return el.Location;
            else return LocationRange.NULL;
        })*/;

        // TODO: we could optimize this function to call Format() in parallel and then Thread.WaitAll() to then print them
        // Problems :
        //      - They won't be in order anymore (because we'll have to use a ConcurrentBag)
        foreach (var error in orderedErrorStack) {
            Console.WriteLine();

            var areaString =
                  error.Area.ToName().ToUpper()
#if DEBUG
                + " @ "
                + GetCallerString(error)
#endif
                ;

            var frontCharCount = Console.WindowWidth - areaString.Length;

            if (frontCharCount > 2) {
                var backCharCount = Console.WindowWidth / 5;

                if (frontCharCount > backCharCount) {
                    areaString += " " + new string('-', backCharCount);

                    Console.Write(new string('-', frontCharCount - 2 - backCharCount));
                }

                Console.Error.Write(" " + areaString + " " + '\n');
            }

            Console.Error.Write(Format(error));
        }
    }

    public static string Format(LotusError error) {
        var sb = new StringBuilder();

        //* IDK if we should add this, but it can be useful in case the error doesn't have many properties
        var eType = error.GetType();

        if (eType.IsGenericType)
            sb.AppendLine(eType.Name.Remove(eType.Name.Length - 2) + '<' + String.Join(", ", eType.GenericTypeArguments.Select(t => t.Name)) + '>');
        else
            sb.AppendLine(eType.Name);//*/
        // Interfaces to implement :
        //      - ILocalized
        //      - IContextualized
        //      - UnexpectedError (abstract class)

        sb.AppendLine();

        if (error is UnexpectedError eUnx) {
            sb.AppendLine(FormatUnexpected(eUnx)).AppendLine();
        } else if (error is IContextualized eCtx) {
            // FormatUnexpected already takes care of context for us, so we only do it
            // if the error is not UnexpectedError
            sb.AppendLine(FormatContextualized(eCtx)).AppendLine();
        }

        if (error is ILocalized eLoc) {
            // TODO: Allow passing a short message to be displayed in the sample
            sb.AppendLine(FormatLocalized(eLoc));
        }

        sb.AppendLine();

        if (error.Message != null) {
            sb.AppendLine("\n" + error.Message);
        }

        if (error.ExtraNotes != null) {
            sb.AppendLine("\nNote: " + error.ExtraNotes);
        }

        sb.AppendLine();

        return sb.ToString();
    }

    public static string FormatUnexpected(UnexpectedError error) {
        var sb = new StringBuilder("Unexpected ");

        switch (error) {
            case IValued<Token> unxToken:
                var token = unxToken.Value;
                sb.Append(token.Kind + " '" + ASTHelper.PrintToken(token).Trim() + "'");
                break;
            case IValued<Node> unxNode:
                var node = unxNode.Value;
                sb.Append(
                      node.GetType().Name
                    + " '"
                    + (node.Location.LineLength < 100 ? ASTHelper.PrintNode(node).Trim() : "")
                    + "'"
                );
                break;
            case IValued<string> unxString:
                sb.Append("input '" + unxString.Value.Trim() + "'");
                break;
            case IValued<char> unxChar:
                var chr = unxChar.Value;
                if (chr == '\n' || chr == '\r') {
                    sb.Append("newline character.");
                } else if (chr == '\t') {
                    sb.Append("tabulation character.");
                } else if (Char.IsWhiteSpace(chr)) {
                    sb.Append("whitespace character '" + System.Text.RegularExpressions.Regex.Escape(chr.ToString()));
                } else {
                    sb.Append("character '" + chr + "'.");
                }
                break;
            default:
                sb.Append(String.Join(", ", error.GetType().GenericTypeArguments.Select(t => t.Name)));
                break;
        }

        if (error.In != null) sb.Append(" in " + error.In);

        if (error.As != null) sb.Append(" as " + error.As);

        error.Expected.Match(
            s => sb.Append("\nExpected " + s + "."),
            list => sb.Append("\nExpected one of :\n\t- " + String.Join("\n\t- ", list)),
            _ => sb
        );

        return sb.ToString();
    }

    public static string FormatContextualized(IContextualized error) {
        return "Error happened in " + error.In;
    }

    public static string FormatLocalized(ILocalized error) {
        var location = error.Location;
        var sb = new StringBuilder();
        var uri = new Uri(error.Location.filename);

        var relPath = uri.RelativeToPWD();

        sb.AppendLine($"\t--> {relPath}({location.firstLine}, {location.firstColumn})");

        if (!File.Exists(location.filename)) {
            string sourceCode;
            if (error is IValued<Node> eNode) {
                sourceCode = ASTHelper.PrintNode(eNode.Value);
            } else if (error is IValued<Token> eToken) {
                sourceCode = ASTHelper.PrintToken(eToken.Value);
            } else if (error is IValued<string> eString) {
                sourceCode = eString.Value;
            } else {
                sourceCode = "";
            }

            sb.AppendLine(
                FormatTextAt(
                    new LocationRange(
                        firstLine: 1,
                        lastLine: location.lastLine - location.firstLine + 1,
                        firstColumn: 1,
                        lastColumn: location.lastColumn - location.firstColumn + 1,
                        filename: location.filename
                    ),
                    new SourceCode(sourceCode)
                )
            );
            sb.Append("WARNING : This source code is approximated, because file " + location.filename + " could not be found");
        } else {
            sb.Append(FormatTextAt(location, new SourceCode(uri)));
        }

        return sb.ToString();
    }

    private static string FormatTextAt(LocationRange range) {
        if (!File.Exists(range.filename)) return "<couldn't print source code>";

        return FormatTextAt(range, new SourceCode(new Uri(range.filename)));
    }

    private static string FormatTextAt(LocationRange range, SourceCode text) {
        if (range.LineLength == 1) {
            if (range.ColumnLength == 1) return text.FormatTextAtPoint(range.GetFirstLocation());
            else return text.FormatTextAtLine(range);
        }

        return text.FormatTextAtLines(range);
    }

    private static string GetCallerString(LotusError error) => Path.GetFileNameWithoutExtension(error.CallerPath) + '.' + error.Caller;
}