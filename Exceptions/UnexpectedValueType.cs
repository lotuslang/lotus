using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[Serializable]
public class UnexpectedValueType : Exception
{
    public UnexpectedValueType([CallerMemberName] string caller = "<unknown caller>", [CallerFilePath] string callerPath = "")
        : base($"Unexpected value type encountered. No more info available. (from {Path.GetFileNameWithoutExtension(callerPath)}.{caller})") { }

    public UnexpectedValueType(string message, ValueNode node)
        : base($"{node.Token.Location} : Unexpected {node.GetType().Name} ({node.Representation}). {message}") { }

    public UnexpectedValueType(ValueNode node, params Type[] expected)
        : base($"{node.Token.Location} : Unexpected {node.GetType().Name} ({node.Representation}). Expected `{String.Join("`, or `", expected.Select(type => type.Name))}`") { }

    public UnexpectedValueType(ValueNode node, string expected)
        : base($"{node.Token.Location} : Unexpected {node.GetType().Name} ({node.Representation}). Expected {expected}") { }

    public UnexpectedValueType(ValueNode node, string context, params Type[] expected)
        : base($"{node.Token.Location} : Unexpected {node.GetType().Name} ({node.Representation}) {context}. Expected `{String.Join("`, or `", expected.Select(type => type.Name))}`") { }

    public UnexpectedValueType(ValueNode node, string context, string expected)
        : base($"{node.Token.Location} : Unexpected {node.GetType().Name} ({node.Representation}) {context}. Expected {expected}") { }
}