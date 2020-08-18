public sealed class UsingParselet : IStatementParselet<UsingNode>
{
    public UsingNode Parse(Parser parser, Token usingToken) {
        if (!(usingToken is ComplexToken usingKeyword && usingToken == "using"))
            throw Logger.Fatal(new InvalidCallException(usingToken.Location));

        // TODO: if we see a '*' here (by tokenizer.peek for example), we could tell the user he should use `using` instead

        var importName = parser.ConsumeValue();

        var isValid = true;

        if (!(Utilities.IsName(importName) || importName is StringNode)) {
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