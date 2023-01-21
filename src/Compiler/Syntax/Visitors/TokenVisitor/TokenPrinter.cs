using System.Text;

namespace Lotus.Syntax.Visitors;

internal sealed class TokenPrinter : ITokenVisitor<string>
{
    public string Visit(NumberToken token)
        => PrintLeadingTrivia(token) + token.Representation + PrintTrailingTrivia(token);

    public string Visit(StringToken token)
        => PrintLeadingTrivia(token) + '"' + token.Representation + '"' + PrintTrailingTrivia(token);

    public string Visit(ComplexStringToken token) {
        // the capacity is just the minimum + a guess
        var output = new StringBuilder("$\"", capacity: token.Representation.Length + (token.CodeSections.Length * 10));

        var str = token.Representation;

        // please don't ask, i was "in the flow" (and really hungry too so goodbye)
        for (var i = 0; i < token.Representation.Length; i++) {
            if (str[i] is not '{') {
                output.Append(str[i]);
                continue;
            }

            if (i + 1 < str.Length && str[i + 1] == '{') {
                output.Append(str[i]);
                continue;
            }

            // This code does three things :
            // 1. Extract the code section this piece of the string refers to
            // 2. Pretty print that code section
            // 3. Increment `i` by the right amount so that we don't double-parse or ignore anything

            // Example
            // Let's say we have the string "hello {name} !"
            // It will get translated by the tokenizer and parser as
            // ComplexStringNode("hello {0} !", codeSections = [ IdentNode("name") ])
            // and we have just lost the information of where `name` was in the string
            //
            // To pretty-print it, we have to put back "name" into its place while not ignoring any other character.
            // To do so, we first add every character before the "{0}" part to `output`.
            //
            // Note : the "{0}" part is called a "section marker" or simply a marker.
            //
            // 1. Then, when we arrive on '{', we initialize a counter that tracks how many digits this marker contains
            // In our case, it contains only 1 digit, so we have digitCount = 1. Finally, we extract the index from the
            // marker, by extracting a string (`sectionIndexStr`) and then parsing it into a number (`sectionIndex`).
            //
            // 2. Once we have the index, we can pretty-print the code section (`sectionStr`)
            //
            // 3. To clean-up, we have to increment `i` by the number of characters we handled ourselves (instead of through the main loop).
            // Since we consumed the index (which has digitCount = 1) and we don't want to print the closing '}' at the end of the marker,
            // we increment `i` by the number of digits + 1, to account for '}'

            var digitCount = 0;

            // increment digitCount while the character is not '}'
            while (str[digitCount + 1 + i] != '}') digitCount++;

            // extract the digits between '{' and '}'
            var sectionIndexStr = str.AsSpan(i + 1, digitCount);

            // parse the digit string into the index of code section this refers to.
            var sectionIndex = Int32.Parse(sectionIndexStr);

            output.Append('{');

            foreach (var sectionToken in token.CodeSections[sectionIndex])
                output.Append(Print(sectionToken));

            output.Append('}');

            i += digitCount + 1; // the `+ 1` is to account for the closing "}" at the end
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

    public string Default(TriviaToken? token) => Default(token is null ? Token.NULL : token as Token);
}