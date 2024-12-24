using System.Text;

namespace Lotus.Syntax.Visitors;

internal sealed partial class Printer : ITopLevelVisitor<string>
{
    public string Default(TopLevelNode node)
        => Print(node.Token);

    public string Visit(EnumNode node)
        => PrintModifiers(node.Modifiers)
         + Print(node.EnumToken)
         + Print(node.Name)
         + PrintTuple(node.Values, ",", Print);

    public string Visit(FunctionHeaderNode node) {
        var output = new StringBuilder();

        output.Append(Print(node.Token));
        output.Append(Print(node.Name));
        output.Append(Print(node.Parameters.OpeningToken));

        output.AppendJoin(",", node.Parameters.Items.Select(Print));

        output.Append(Print(node.Parameters.ClosingToken));

        if (node.HasReturnType) {
            output.Append(Print(node.ColonToken));
            output.Append(Print(node.ReturnType));
        }

        return output.ToString();
    }

    public string Visit(FunctionDefinitionNode node)
        => Print(node.Header) + Print(node.Body);

    public string Visit(ImportNode node)
        => Print(node.Token)
         + Print(node.Names.OpeningToken)
         + MiscUtils.Join(",", Print, node.Names)
         + Print(node.Names.ClosingToken)
         + (node.FromOrigin is null ? "" : Print(node.FromOrigin));

    public string Visit(NamespaceNode node)
        => PrintModifiers(node.Modifiers) + Print(node.Token) + Print(node.Name);

    public string Visit(UsingNode node)
        => Print(node.Token) + Print(node.Name);

    public string Visit(StructNode node)
        => PrintModifiers(node.Modifiers)
         + Print(node.Token)
         + Print(node.Name)
         + Print(node.Fields.OpeningToken)
         + MiscUtils.Join("; ", Print, node.Fields.Items)
         + (node.Fields.Count != 0 ? ";" : "")
         + Print(node.Fields.ClosingToken);

    public string Visit(TraitNode node) {
        var output = PrintModifiers(node.Modifiers)
                + Print(node.Token)
                + Print(node.Name)
                + Print(node.OpeningBracket);

        foreach (var func in node.Functions)
            output += Print(func) + ";";

        output += Print(node.ClosingBracket);

        return output;
    }

    public string Print(FromOrigin node)
        => Print(node.FromToken)
         + Print(node.OriginName);

    public string Print(StructField field)
        => PrintModifiers(field.Modifiers)
         + Print(field.Name)
         + ": "
         + Print(field.Type)
         + Print(field.EqualSign ?? Token.NULL)
         + Print(field.DefaultValue ?? ValueNode.NULL);

    public string Print(TypeDecName typeDec)
        => !typeDec.HasParent
                ? Print(typeDec.TypeName)
                : Print(typeDec.Parent)
                    + Print(typeDec.ColonToken)
                    + Print(typeDec.TypeName);

    public string Print(Tuple<TopLevelNode> tuple)
        => PrintTuple(tuple, "", Print);

    public string Print(TopLevelNode node)
        => node.Accept(this)
        + (SyntaxFacts.NeedsSemicolon(node) ? ";" : "");
}