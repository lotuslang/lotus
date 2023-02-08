using System.Text;

namespace Lotus.Syntax.Visitors;

internal sealed class TokenPrinter : ITokenVisitor<string>
{
    public string Visit(NumberToken token)
        => PrintLeadingTrivia(token) + token.Representation + PrintTrailingTrivia(token);

    public string Visit(StringToken token)
        => PrintLeadingTrivia(token) + '"' + token.Representation + '"' + PrintTrailingTrivia(token);

    public string Visit(ComplexStringToken token) {
        if (token.CodeSections.Length == 0)
            return Visit((StringToken)token);

        // the capacity is just the minimum + a guess
        var output = new StringBuilder("$\"", capacity: token.Representation.Length + (token.CodeSections.Length * 10));

        var str = token.Representation;
        var sections = token.CodeSections;
        var currSectionIdx = 0;

        // please don't ask, i was "in the flow" (and really hungry too so goodbye)
        for (var i = 0; i < token.Representation.Length; i++) {
            if (currSectionIdx >= sections.Length || sections[currSectionIdx].StringOffset != i) {
                output.Append(str[i]);
                continue;
            }

            output.Append('{');

            foreach (var sectionToken in sections[currSectionIdx].Tokens)
                output.Append(Print(sectionToken));

            output.Append('}');

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

    public string PrintLeadingTrivia(Token token)
        => token.LeadingTrivia is not null ? Default(token.LeadingTrivia) : "";

    public string PrintTrailingTrivia(Token token)
        => token.TrailingTrivia is not null ? Default(token.TrailingTrivia) : "";

    public string Print(Token token) => token.Accept(this);

    public string Default(Token token) => Visit(token);

    public string Default(TriviaToken? token) => Default(token ?? Token.NULL);
}