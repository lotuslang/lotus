using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[Serializable]
public class UnexpectedValueTypeException : LotusException
{
    public UnexpectedValueTypeException(Location location, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Unexpected value type encountered at location {location}.  No more info available."
                + $" (from {Path.GetFileNameWithoutExtension(callerPath)}.{caller})",
            location,
            caller,
            callerPath
        ) { }

    public UnexpectedValueTypeException(string message, ValueNode node, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Unexpected {node.GetType().Name} ({node.Representation}) at location {node.Token.Location}. {message}",
            node.Token.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedValueTypeException(ValueNode node, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "", params Type[] expected)
        : base(
            $"Unexpected {node.GetType().Name} ({node.Representation}) at location {node.Token.Location}."
                + $"Expected `{String.Join("`, or `", expected.Select(type => type.Name))}`",
            node.Token.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedValueTypeException(ValueNode node, string expected, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Unexpected {node.GetType().Name} ({node.Representation}) at location {node.Token.Location}. Expected {expected}",
            node.Token.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedValueTypeException(ValueNode node, string context, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "", params Type[] expected)
        : base(
            $"Unexpected {node.GetType().Name} ({node.Representation}) {context} at location {node.Token.Location}."
                + $"Expected `{String.Join("`, or `", expected.Select(type => type.Name))}`",
            node.Token.Location,
            caller,
            callerPath
        ) { }

    public UnexpectedValueTypeException(ValueNode node, string context, string expected, [CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base(
            $"Unexpected {node.GetType().Name} ({node.Representation}) {context} at location {node.Token.Location}. Expected {expected}",
            node.Token.Location,
            caller,
            callerPath
        ) { }
}