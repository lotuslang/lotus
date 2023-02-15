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
                var sectionConsumer = section.ToConsumer();

                var sectionParser = new ExpressionParser(sectionConsumer);

                sections.Add(sectionParser.Consume());

                if (sectionParser.Current.IsValid && sectionConsumer.Peek() != sectionConsumer.Default) {
                    var location = sectionConsumer.Position;

                    if (section.TokenCount > 0) {
                        location = new LocationRange(section.Tokens[0].Location, location);
                    }

                    Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                        Value = sectionConsumer.Consume(),
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
