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
        if (token.CodeSections.Length == 0)
            return Visit((StringToken)token);

        // the capacity is just the minimum + a guess
        var output = new StringBuilder("$\"", capacity: token.Representation.Length + (token.CodeSections.Length * 10));

        var str = token.Representation;
        var sections = token.CodeSections;
        var currSectionIdx = 0; // tracks the next code section we could insert

        for (var i = 0; i < token.Representation.Length; i++) {
            if (currSectionIdx >= sections.Length || sections[currSectionIdx].StringOffset != i) {
                output.Append(str[i]);
                continue;
            }

            output.Append('{');

            foreach (var sectionToken in sections[currSectionIdx].Tokens)
                output.Append(Print(sectionToken));

            output.Append('}');

            i++; // we don't wanna consume the '}' in the repr
            currSectionIdx++;
        }

        return output.ToString() + '"';
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