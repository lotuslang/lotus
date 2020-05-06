using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Grammar : ReadOnlyGrammar
{

    // Parameters default to an empty dictionary if null or unspecified
    public Grammar(
        IDictionary<ExpressionKind, IPrefixParselet<ValueNode>>? prefixParselets = null,
        IDictionary<ExpressionKind, IInfixParselet<ValueNode>>? infixParselets = null,
        IDictionary<ExpressionKind, IPostfixParselet<ValueNode>>? postfixParselets = null,
        IDictionary<string, ExpressionKind>? expressionKinds = null,
        ICollection<IToklet<Token>>? toklets = null,
        ICollection<ITriviaToklet<TriviaToken>>? triviaToklets = null
    )
        : base(
            prefixParselets,
            infixParselets,
            postfixParselets,
            expressionKinds,
            toklets,
            triviaToklets
        )
    { }

    public Grammar(ReadOnlyGrammar grammar) : base(grammar)
    { }

    public Grammar RegisterToklet(IToklet<Token> toklet) {
        toklets.Add(toklet);

        return this;
    }

    public Grammar RegisterTriviaToklet(ITriviaToklet<TriviaToken> toklet) {
        triviaToklets.Add(toklet);

        return this;
    }

    public Grammar RegisterPrefix(ExpressionKind kind, IPrefixParselet<ValueNode> parselet) {
        prefixParselets.Add(kind, parselet);

        return this;
    }

    public Grammar RegisterPrefixOperator(ExpressionKind kind, OperationType operationType) {
        RegisterPrefix(kind, new PrefixOperatorParselet(operationType));

        return this;
    }

    public Grammar RegisterInfixBinaryOperator(ExpressionKind kind, Precedence precedence, OperationType operationType) {
        RegisterInfix(kind, new BinaryOperatorParselet(precedence, operationType));

        return this;
    }

    public Grammar RegisterInfix(ExpressionKind kind, IInfixParselet<ValueNode> parselet) {
        infixParselets.Add(kind, parselet);

        return this;
    }

    public Grammar RegisterPostfixOperation(ExpressionKind kind, OperationType operationType) {
        RegisterPostfix(kind, new PostfixOperatorParselet(operationType));

        return this;
    }

    public Grammar RegisterPostfix(ExpressionKind kind, IPostfixParselet<ValueNode> parselet) {
        postfixParselets.Add(kind, parselet);

        return this;
    }

    public Grammar RegisterExpressionKind(string representation, ExpressionKind kind) {
        expressionKinds.Add(representation, kind);

        return this;
    }
}