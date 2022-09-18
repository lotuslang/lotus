namespace Lotus.Syntax;

public class Grammar : ReadOnlyGrammar
{
    // Parameters default to an empty dictionary if null or unspecified
    public Grammar(
        IDictionary<ExpressionKind, IPrefixParslet<ValueNode>>? prefixParslets = null,
        IDictionary<ExpressionKind, IInfixParslet<ValueNode>>? infixParslets = null,
        IDictionary<ExpressionKind, IPostfixParslet<ValueNode>>? postfixParslets = null,
        IDictionary<string, IStatementParslet<StatementNode>>? statementParslets = null,
        IDictionary<string, ITopLevelParslet<TopLevelNode>>? topLevelParslets = null,
        IDictionary<string, ExpressionKind>? expressionKinds = null
    )
        : base(
            prefixParslets,
            infixParslets,
            postfixParslets,
            statementParslets,
            topLevelParslets,
            expressionKinds
        )
    { }

    public Grammar(ReadOnlyGrammar grammar) : base(grammar)
    { }

    public Grammar RegisterPrefix(ExpressionKind kind, IPrefixParslet<ValueNode> parslet) {
        prefixParslets.Add(kind, parslet);

        return this;
    }

    public Grammar RegisterPrefixOperator(ExpressionKind kind, OperationType operationType) {
        RegisterPrefix(kind, new PrefixOperatorParslet(operationType));

        return this;
    }

    public Grammar RegisterInfixBinaryOperator(ExpressionKind kind, Precedence precedence, OperationType operationType) {
        RegisterInfix(kind, new BinaryOperatorParslet(precedence, operationType));

        return this;
    }

    public Grammar RegisterInfix(ExpressionKind kind, IInfixParslet<ValueNode> parslet) {
        infixParslets.Add(kind, parslet);

        return this;
    }

    public Grammar RegisterPostfixOperation(ExpressionKind kind, OperationType operationType) {
        RegisterPostfix(kind, new PostfixOperatorParslet(operationType));

        return this;
    }

    public Grammar RegisterPostfix(ExpressionKind kind, IPostfixParslet<ValueNode> parslet) {
        postfixParslets.Add(kind, parslet);

        return this;
    }

    public Grammar RegisterStatementParslet(string representation, IStatementParslet<StatementNode> parslet) {
        statementParslets.Add(representation, parslet);

        return this;
    }

    public Grammar RegisterTopLevelParslets(string representation, ITopLevelParslet<TopLevelNode> parslet) {
        topLevelParslets.Add(representation, parslet);

        return this;
    }

    public Grammar RegisterExpressionKind(string representation, ExpressionKind kind) {
        expressionKinds.Add(representation, kind);

        return this;
    }
}