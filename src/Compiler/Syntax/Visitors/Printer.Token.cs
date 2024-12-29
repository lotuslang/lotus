using System.Text;

namespace Lotus.Syntax.Visitors;

internal sealed partial class Printer : ITokenVisitor<string>
{
    public string Visit(NumberToken token)
        => PrintLeadingTrivia(token) + token.Representation + PrintTrailingTrivia(token);

    public string Visit(CharToken token)
        => PrintLeadingTrivia(token) + "'" + token.Representation + "'" + PrintTrailingTrivia(token);

    public string Visit(StringToken token)
        => PrintLeadingTrivia(token) + '"' + token.Representation + '"' + PrintTrailingTrivia(token);

    public string Visit(ComplexStringToken token) {
        if (token.CodeSections.IsEmpty) {
            return PrintLeadingTrivia(token)
                + '"'
                + token.Representation
                + '"'
                + PrintTrailingTrivia(token);
        }

        // the capacity is just the minimum + a guess
        var output = new StringBuilder(PrintLeadingTrivia(token));
        output.Append("$\"");

        foreach (var (text, code) in Enumerable.Zip(token.TextSections, token.CodeSections)) {
            output.Append(text);

            output.Append('{');
            foreach (var sectionToken in code.Tokens)
                output.Append(Print(sectionToken));
            output.Append('}');
        }

        // there's always one more text section than code (even if it might be empty)
        Debug.Assert(token.TextSections.Length == token.CodeSections.Length + 1);
        output.Append(token.TextSections[^1]);

        output.Append('"');
        output.Append(PrintTrailingTrivia(token));

        return output.ToString();
    }

    public string Visit(Token token) {
        var output = PrintLeadingTrivia(token);

        if (token.Kind != TokenKind.EOF) {
            output += token.Representation;
        }

        output += PrintTrailingTrivia(token);

        return output;
    }

    public string PrintLeadingTrivia(Token token) {
        if (!PrintTrivia || token.LeadingTrivia is null)
            return "";

        return Default(token.LeadingTrivia);
    }

    public string PrintTrailingTrivia(Token token) {
        if (!PrintTrivia || token.TrailingTrivia is null)
            return "";

        return Default(token.TrailingTrivia);
    }

    public string Print(Token token) => token.Accept(this);

    public string Default(Token token) => Visit(token);

    public string Default(TriviaToken? token) => Default(token ?? Token.NULL);
}