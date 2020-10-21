public sealed class UsingParselet : IStatementParselet<UsingNode>
{
    public UsingNode Parse(Parser parser, Token usingToken) {
        if (!(usingToken is ComplexToken usingKeyword && usingToken == "using"))
            throw Logger.Fatal(new InvalidCallException(usingToken.Location));

        var importName = parser.ConsumeValue();

        var isValid = true;

        if (!(importName is StringNode || Utilities.IsName(importName))) {
            Logger.Error(new UnexpectedValueTypeException(
                node: importName,
                context: "as a namespace in a using statement",
                expected: "string or name"
            ));

            isValid = false;
        }

        return new UsingNode(usingKeyword, importName, isValid);
    }
}