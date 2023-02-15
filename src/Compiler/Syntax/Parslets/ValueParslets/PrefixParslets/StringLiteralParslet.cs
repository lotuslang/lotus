namespace Lotus.Syntax;

public sealed class StringLiteralParslet : IPrefixParslet<StringNode>
{
    public static readonly StringLiteralParslet Instance = new();

    public StringNode Parse(ExpressionParser parser, Token token) {
        var strToken = token as StringToken;

        Debug.Assert(strToken is not null);

        if (strToken is ComplexStringToken complexString) {
            var sections = ImmutableArray.CreateBuilder<ValueNode>(complexString.CodeSections.Length);

            foreach (var section in complexString.CodeSections) {
                var sectionTokenizer = section.CreateTokenizer();

                var sectionParser = new ExpressionParser(sectionTokenizer);

                sections.Add(sectionParser.Consume());

                // if we can still consume, then there's too many tokens for an expression
                if (sectionParser.Current.IsValid && sectionTokenizer.TryConsume(out var extraToken)) {
                    var location = sectionTokenizer.Position;

                    if (section.TokenCount > 0) {
                        location = new LocationRange(section.Tokens[0].Location, location);
                    }

                    Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                        Value = extraToken,
                        Location = location,
                        Message = "Too many tokens in interpolated string's code section."
                                + " Sections should only contain ONE expression each.",
                        ExtraNotes
                                = "This probably means that you forgot a closing `}`,"
                                + " yet a following *valid* code section (the `{...}`"
                                + " parts of an interpolated string) \"closed\" the malformed one.\n"
                                + " If this isn't the case, then you probably wrote a statement instead of a value,"
                                + " which isn't allowed"
                    });
                }
            }

            return new ComplexStringNode(complexString, sections.MoveToImmutable());
        }

        return new StringNode(strToken);
    }
}
