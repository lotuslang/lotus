using System.Collections.Generic;
using System.Collections.ObjectModel;

// /!\
// The design and structure of this class is very much a WiP
// (yes, even more than the rest of the project) !
// If you have any suggestions or feedback, please feel free to
// open an issue or a PR
// /!\

public class ReadOnlyGrammar
{
    protected IDictionary<ExpressionKind, IPrefixParslet<ValueNode>> prefixParslets;

    public virtual ReadOnlyDictionary<ExpressionKind, IPrefixParslet<ValueNode>> PrefixParslets {
        get => prefixParslets.AsReadOnly();
        init => prefixParslets = value;
    }

    protected IDictionary<ExpressionKind, IInfixParslet<ValueNode>> infixParslets;

    public virtual ReadOnlyDictionary<ExpressionKind, IInfixParslet<ValueNode>> InfixParslets {
        get => infixParslets.AsReadOnly();
        init => infixParslets = value;
    }

    protected IDictionary<ExpressionKind, IPostfixParslet<ValueNode>> postfixParslets;

    public virtual ReadOnlyDictionary<ExpressionKind, IPostfixParslet<ValueNode>> PostfixParslets {
        get => postfixParslets.AsReadOnly();
        init => postfixParslets = value;
    }

    protected IDictionary<StatementKind, IStatementParslet<StatementNode>> statementParslets;

    public virtual ReadOnlyDictionary<StatementKind, IStatementParslet<StatementNode>> StatementParslets {
        get => statementParslets.AsReadOnly();
        init => statementParslets = value;
    }

    protected IDictionary<string, ExpressionKind> expressionKinds;

    public virtual ReadOnlyDictionary<string, ExpressionKind> ExpressionKinds {
        get => expressionKinds.AsReadOnly();
        init => expressionKinds = value;
    }

    protected IDictionary<string, StatementKind> statementKinds;

    public virtual ReadOnlyDictionary<string, StatementKind> StatementKinds {
        get => statementKinds.AsReadOnly();
        init => statementKinds = value;
    }

    protected ICollection<IToklet<Token>> toklets;

    public virtual ReadOnlyCollection<IToklet<Token>> Toklets {
        get => toklets.AsReadOnly();
        init => toklets = value;
    }

    protected ICollection<ITriviaToklet<TriviaToken>> triviaToklets;

    public virtual ReadOnlyCollection<ITriviaToklet<TriviaToken>> TriviaToklets {
        get => triviaToklets.AsReadOnly();
        init => triviaToklets = value;
    }

#pragma warning disable CS8618
    public ReadOnlyGrammar() { }

    public ReadOnlyGrammar(
        IDictionary<ExpressionKind, IPrefixParslet<ValueNode>>? prefixParslets = null,
        IDictionary<ExpressionKind, IInfixParslet<ValueNode>>? infixParslets = null,
        IDictionary<ExpressionKind, IPostfixParslet<ValueNode>>? postfixParslets = null,
        IDictionary<StatementKind, IStatementParslet<StatementNode>>? statementParslets = null,
        IDictionary<string, ExpressionKind>? expressionKinds = null,
        IDictionary<string, StatementKind>? statementKinds = null,
        ICollection<IToklet<Token>>? toklets = null,
        ICollection<ITriviaToklet<TriviaToken>>? triviaToklets = null
    ) {
        Initialize(
            prefixParslets,
            infixParslets,
            postfixParslets,
            statementParslets,
            expressionKinds,
            statementKinds,
            toklets,
            triviaToklets
        );
    }
#pragma warning restore CS8618

    public ReadOnlyGrammar(ReadOnlyGrammar grammar)
        : this(
            grammar.prefixParslets,
            grammar.infixParslets,
            grammar.postfixParslets,
            grammar.statementParslets,
            grammar.expressionKinds,
            grammar.statementKinds,
            grammar.toklets,
            grammar.triviaToklets
        ) { }

    /// <summary>
    /// Should only be called right after the (potentially derived) class' initialization
    /// </summary>
    /// <param name="grammar"></param>
    protected void Initialize(ReadOnlyGrammar grammar) {
        Initialize(
            grammar.prefixParslets,
            grammar.infixParslets,
            grammar.postfixParslets,
            grammar.statementParslets,
            grammar.expressionKinds,
            grammar.statementKinds,
            grammar.toklets,
            grammar.triviaToklets
        );
    }

    /// <summary>
    /// Should only be called right after the (potentially derived) class initialization
    /// </summary>
    /// <param name="grammar"></param>
    protected void Initialize(
        IDictionary<ExpressionKind, IPrefixParslet<ValueNode>>? prefixParslets = null,
        IDictionary<ExpressionKind, IInfixParslet<ValueNode>>? infixParslets = null,
        IDictionary<ExpressionKind, IPostfixParslet<ValueNode>>? postfixParslets = null,
        IDictionary<StatementKind, IStatementParslet<StatementNode>>? statementParslets = null,
        IDictionary<string, ExpressionKind>? expressionKinds = null,
        IDictionary<string, StatementKind>? statementKinds = null,
        ICollection<IToklet<Token>>? toklets = null,
        ICollection<ITriviaToklet<TriviaToken>>? triviaToklets = null
    ) {
        this.prefixParslets = prefixParslets ?? new Dictionary<ExpressionKind, IPrefixParslet<ValueNode>>();
        this.infixParslets = infixParslets ?? new Dictionary<ExpressionKind, IInfixParslet<ValueNode>>();
        this.postfixParslets = postfixParslets ?? new Dictionary<ExpressionKind, IPostfixParslet<ValueNode>>();
        this.statementParslets = statementParslets ?? new Dictionary<StatementKind, IStatementParslet<StatementNode>>();

        this.expressionKinds = expressionKinds ?? new Dictionary<string, ExpressionKind>();
        this.statementKinds = statementKinds ?? new Dictionary<string, StatementKind>();

        this.toklets = toklets ?? new List<IToklet<Token>>();
        this.triviaToklets = triviaToklets ?? new List<ITriviaToklet<TriviaToken>>();
    }

    public IToklet<Token> MatchToklet(StringConsumer input)
        => toklets.Find(toklet => toklet.Condition(new StringConsumer(input)))!;

    public ITriviaToklet<TriviaToken>? MatchTriviaToklet(StringConsumer input)
        => triviaToklets.Find(toklet => toklet.Condition(new StringConsumer(input)));

    public ExpressionKind GetExpressionKind(Token token) {

        if (token == null) return ExpressionKind.NotAnExpr;

        // fundamental expression kinds
        switch (token.Kind) {
            case TokenKind.ident:
                return ExpressionKind.Identifier;
            case TokenKind.number:
                return ExpressionKind.Number;
            case TokenKind.@bool:
                return ExpressionKind.Boolean;
            case TokenKind.@string:
            case TokenKind.complexString:
                return ExpressionKind.String;
        }

        return expressionKinds.TryGetValue(token, out ExpressionKind kind) ? kind : ExpressionKind.NotAnExpr;
    }

    public StatementKind GetStatementKind(Token token) {
        if (token == null) return StatementKind.NotAStatement;

        return statementKinds.TryGetValue(token, out StatementKind kind) ? kind : StatementKind.NotAStatement;
    }

    public IPrefixParslet<ValueNode> GetPrefixParslet(ExpressionKind kind)
        => prefixParslets[kind];

    public IPrefixParslet<ValueNode> GetPrefixParslet(Token token)
        => GetPrefixParslet(GetExpressionKind(token));

    public IInfixParslet<ValueNode> GetOperatorParslet(ExpressionKind kind)
        => infixParslets[kind];

    public IInfixParslet<ValueNode> GetOperatorParslet(Token token)
        => GetOperatorParslet(GetExpressionKind(token));

    public IPostfixParslet<ValueNode> GetPostfixParslet(ExpressionKind kind)
        => postfixParslets[kind];

    public IPostfixParslet<ValueNode> GetPostfixParslet(Token token)
        => GetPostfixParslet(GetExpressionKind(token));

    public IStatementParslet<StatementNode> GetStatementParslet(StatementKind kind)
        => statementParslets[kind];

    public IStatementParslet<StatementNode> GetStatementParslet(Token token)
        => GetStatementParslet(GetStatementKind(token));

    public Precedence GetPrecedence(ExpressionKind kind) {
        if (!IsOperatorParslet(kind)) {
            return 0;
        }

        return GetOperatorParslet(kind).Precedence;
    }

    public Precedence GetPrecedence(Token token)
        => token != null ? GetPrecedence(GetExpressionKind(token)) : 0;

    public bool IsPrefix(ExpressionKind kind)
        => prefixParslets.ContainsKey(kind);

    public bool IsPostfix(ExpressionKind kind)
        => postfixParslets.ContainsKey(kind);

    public bool IsOperatorParslet(ExpressionKind kind)
        => infixParslets.ContainsKey(kind);
}