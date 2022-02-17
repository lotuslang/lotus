
public sealed class StringLiteralParslet : IPrefixParslet<StringNode>
{

    private static StringLiteralParslet _instance = new();
    public static StringLiteralParslet Instance => _instance;

	private StringLiteralParslet() : base() { }

    public StringNode Parse(ExpressionParser parser, Token token) {
        if (token is ComplexStringToken complexString) {
            var node = new ComplexStringNode(complexString, new List<ValueNode>());

            foreach (var section in complexString.CodeSections) {
                var endPos = (section.LastOrDefault()?.Location ?? parser.Position).GetLastLocation();

                var sectionConsumer = new Consumer<Token>(
                    section,
                    Token.NULL with { Location = endPos },
                    endPos.filename
                );

                var sectionParser = new ExpressionParser(sectionConsumer);

                node.AddSection(sectionParser.Consume());

                if (sectionParser.Current.IsValid && sectionConsumer.Peek() != sectionConsumer.Default) {
                    var location = sectionConsumer.Position;

                    if (section.Length >= 1) {
                        location = new LocationRange(section[0].Location, location);
                    }

                    Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                        Value = sectionConsumer.Consume(),
                        Location = location,
                        Message = "Too many tokens in interpolated string's code section."
                                + " Code sections should only contain a *single value* each.",
                        ExtraNotes =
                                 " This probably means that you forgot a closing `}`,"
                                +" yet a following *valid* code section (the `{...}`"
                                +" parts of an interpolated string) \"closed\" the malformed one.\n"
                                +" If this isn't the case, then you probably wrote a statement instead of a value,"
                                +" which isn't allowed"
                    });
                }
            }

            return node;
        }

        if (token is StringToken strToken) {
            return new StringNode(strToken);
        }

        throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, token.Location));
    }
}
