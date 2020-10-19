public sealed class TokenPrinter : TokenVisitor<string>
{
    public override string Visit(ComplexStringToken token) => Default(token);

    public override string Visit(NumberToken token)
        => PrintLeadingTrivia(token) + token.Representation + PrintTrailingTrivia(token);

    public override string Visit(StringToken token)
        => PrintLeadingTrivia(token) + '"' + token.Representation + '"' + PrintTrailingTrivia(token);

    public override string Visit(Token token) {
        var output = PrintLeadingTrivia(token);

        if (token.Kind != TokenKind.EOF) {
            output += token.Representation;
        }

        output += PrintTrailingTrivia(token);

        return output;
    }


    public string PrintLeadingTrivia(Token token)
        => token.LeadingTrivia != null ? Print(token.LeadingTrivia) : "";

    public string PrintTrailingTrivia(Token token)
        => token.TrailingTrivia != null ? Print(token.TrailingTrivia) : "";

    public string Print(Token token) => token.Accept(this);

    protected override string Default(Token token) => Visit(token);

    protected override string Default(TriviaToken token) => Default(token as Token);
}