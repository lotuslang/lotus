namespace Lotus.Syntax;

public sealed class ComplexStringParslet : IPrefixParslet<ComplexStringNode>
{
    public static readonly ComplexStringParslet Instance = new();

    public ComplexStringNode Parse(Parser parser, Token token) {
        var complexString = token as ComplexStringToken;

        Debug.Assert(complexString is not null);

        var sections = ImmutableArray.CreateBuilder<ValueNode>(complexString.CodeSections.Length);

        foreach (var section in complexString.CodeSections) {
            var sectionTokenizer = section.CreateTokenizer();

            var sectionParser = new Parser(sectionTokenizer);

            sections.Add(sectionParser.ConsumeValue());

            // if we can still consume, then there's too many tokens for an expression
            if (sectionParser.Current.IsValid && !sectionTokenizer.EndOfStream) {
                var extraToken = sectionTokenizer.Consume();
                var location = extraToken.Location;

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
}
