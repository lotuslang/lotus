namespace Lotus.Syntax;

public sealed partial class Parser
{
    public ValueNode ConsumeValue(Precedence precedence = Precedence.Comma) {
        if (!System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack()) {
            Logger.Error(new InternalError(ErrorArea.Parser) {
                Message = "Expression's nesting level is too high for the compiler to deal with.",
                Location = Tokenizer.Position
            });

            return ValueNode.NULL;
        }

        var token = Tokenizer.Consume();
        var tokenKind = LotusFacts.GetExpressionKind(token);

        if (!LotusFacts.IsPrefixOrValueKind(tokenKind)) {
            string? notes = null;

            if (token.Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    Message = "Encountered an EOF where a value was expected",
                    Location = Position
                });
            } else {
                if (token.Kind == TokenKind.keyword)
                    notes = "You can't use " + token.Representation + " as an identifier/name, because it's a reserved keyword";
                else if (token.Kind == TokenKind.@operator)
                    notes = "The '" + token.Representation + "' operator cannot be used as a prefix (i.e. in front of an expression)";

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = token,
                    In = "an expression",
                    As = "a prefix or value literal",
                    Expected =
                          "a bool, "
                        + (token.Kind != TokenKind.@operator ? "prefix operator, " : "")
                        + "string, variable name, or number",
                    ExtraNotes = notes
                });

                if (token.Kind == TokenKind.semicolon)
                    Tokenizer.Reconsume();
            }

            return CreateFakeExpression(token);
        }

        var left = LotusFacts.GetPrefixParslet(tokenKind).Parse(this, token);

        if (!Tokenizer.TryConsume(out token))
            return left;

        tokenKind = LotusFacts.GetExpressionKind(token);

        if (LotusFacts.IsPostfixKind(tokenKind)) {
            left = LotusFacts.GetPostfixParslet(tokenKind).Parse(this, token, left);

            token = Tokenizer.Consume();
            tokenKind = LotusFacts.GetExpressionKind(token);
        }

        while (precedence < LotusFacts.GetPrecedence(tokenKind)) {
            left = LotusFacts.GetInfixParslet(tokenKind).Parse(this, token, left);

            token = Tokenizer.Consume();
            tokenKind = LotusFacts.GetExpressionKind(token);
        }

        Tokenizer.Reconsume();

        return left;
    }

    private static readonly TupleParslet<ValueNode> _defaultTupleParslet
        = new(static (parser) => parser.ConsumeValue());

    public Tuple<ValueNode> ConsumeTuple(uint expectedItemCount = 0) {
        var baseTuple = _defaultTupleParslet.Parse(this);

        var items = baseTuple.Items;

        // if expectedItemCount is 0, then it means there's no limit
        if (baseTuple.IsValid && expectedItemCount != 0 && expectedItemCount != items.Length) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = items.LastOrDefault() ?? CreateFakeExpression(),
                In = "a tuple",
                Location = items.LastOrDefault()?.Location ?? Position,
                Message = (items.Length > expectedItemCount ? "There were too many" : "There weren't enough")
                         + "values in this tuple.",
                Expected = expectedItemCount + " values, but got " + items.Length
            });

            baseTuple.IsValid = false;
        }

        return baseTuple;
    }
}