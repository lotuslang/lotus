using System.IO;
using System.Runtime.CompilerServices;

[Serializable]
public class UnexpectedValueTypeException : LotusException
{
    public UnexpectedValueTypeException(LocationRange range, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"{range}: Unexpected value type encountered.  No more info available."
                + $" (from {Path.GetFileNameWithoutExtension(callerPath)}.{caller})",
            range,
            caller,
            callerPath
        ) { }

    public UnexpectedValueTypeException(string message, ValueNode node, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"{node.Location}: Unexpected {node.GetType().Name} ({node.Token.Representation}). {message}",
            node.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedValueTypeException(ValueNode node, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "", params Type[] expected)
        : base(
            $"{node.Location}: Unexpected {node.GetType().Name} ({node.Token.Representation})."
                + $"Expected `{String.Join("`, or `", expected.Select(type => type.Name))}`",
            node.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedValueTypeException(ValueNode node, string expected, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"{node.Location}: Unexpected {node.GetType().Name} ({node.Token.Representation}). Expected {expected}",
            node.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedValueTypeException(ValueNode node, string context, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "", params Type[] expected)
        : base(
            $"{node.Location}: Unexpected {node.GetType().Name} ({node.Token.Representation}) {context}."
                + $"Expected `{String.Join("`, or `", expected.Select(type => type.Name))}`",
            node.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedValueTypeException(ValueNode node, string context, string expected, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"{node.Location} : Unexpected {node.GetType().Name} ({node.Token.Representation}) {context}. Expected {expected}",
            node.Location,
            caller,
            callerPath
        ) { }
}