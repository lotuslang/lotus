using System;
using System.Linq;
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
    protected IDictionary<ExpressionKind, IPrefixParselet<ValueNode>> prefixParselets;

    public virtual ReadOnlyDictionary<ExpressionKind, IPrefixParselet<ValueNode>> PrefixParselets {
        get => prefixParselets.AsReadOnly();
    }

    protected IDictionary<ExpressionKind, IInfixParselet<ValueNode>> infixParselets;

    public virtual ReadOnlyDictionary<ExpressionKind, IInfixParselet<ValueNode>> InfixParselets {
        get => infixParselets.AsReadOnly();
    }

    protected IDictionary<ExpressionKind, IPostfixParselet<ValueNode>> postfixParselets;

    public virtual ReadOnlyDictionary<ExpressionKind, IPostfixParselet<ValueNode>> PostfixParselets {
        get => postfixParselets.AsReadOnly();
    }

    protected IDictionary<string, ExpressionKind> expressionKinds;

    public virtual ReadOnlyDictionary<string, ExpressionKind> ExpressionKinds {
        get => expressionKinds.AsReadOnly();
    }

    protected ICollection<IToklet<Token>> toklets;

    public virtual ReadOnlyCollection<IToklet<Token>> Toklets {
        get => toklets.AsReadOnly();
    }

    protected ICollection<ITriviaToklet<TriviaToken>> triviaToklets;

    public virtual ReadOnlyCollection<ITriviaToklet<TriviaToken>> TriviaToklets {
        get => triviaToklets.AsReadOnly();
    }

#pragma warning disable CS8618
    public ReadOnlyGrammar(
        IDictionary<ExpressionKind, IPrefixParselet<ValueNode>>? prefixParselets = null,
        IDictionary<ExpressionKind, IInfixParselet<ValueNode>>? infixParselets = null,
        IDictionary<ExpressionKind, IPostfixParselet<ValueNode>>? postfixParselets = null,
        IDictionary<string, ExpressionKind>? expressionKinds = null,
        ICollection<IToklet<Token>>? toklets = null,
        ICollection<ITriviaToklet<TriviaToken>>? triviaToklets = null
    ) {
        Initialize(
            prefixParselets,
            infixParselets,
            postfixParselets,
            expressionKinds,
            toklets,
            triviaToklets
        );
    }
#pragma warning restore CS8618

    public ReadOnlyGrammar(ReadOnlyGrammar grammar)
        : this(
            grammar.prefixParselets,
            grammar.infixParselets,
            grammar.postfixParselets,
            grammar.expressionKinds,
            grammar.toklets,
            grammar.triviaToklets
        )
    { }

    /// <summary>
    /// Should only be called right after the (potentially derived) class' initialization
    /// </summary>
    /// <param name="grammar"></param>
    protected void Initialize(ReadOnlyGrammar grammar) {
        Initialize(
            grammar.prefixParselets,
            grammar.infixParselets,
            grammar.postfixParselets,
            grammar.expressionKinds,
            grammar.toklets,
            grammar.triviaToklets
        );
    }

    /// <summary>
    /// Should only be called right after the (potentially derived) class initialization
    /// </summary>
    /// <param name="grammar"></param>
    protected void Initialize(
        IDictionary<ExpressionKind, IPrefixParselet<ValueNode>>? prefixParselets = null,
        IDictionary<ExpressionKind, IInfixParselet<ValueNode>>? infixParselets = null,
        IDictionary<ExpressionKind, IPostfixParselet<ValueNode>>? postfixParselets = null,
        IDictionary<string, ExpressionKind>? expressionKinds = null,
        ICollection<IToklet<Token>>? toklets = null,
        ICollection<ITriviaToklet<TriviaToken>>? triviaToklets = null
    ) {
        this.prefixParselets = prefixParselets ?? new Dictionary<ExpressionKind, IPrefixParselet<ValueNode>>();
        this.infixParselets = infixParselets ?? new Dictionary<ExpressionKind, IInfixParselet<ValueNode>>();
        this.postfixParselets = postfixParselets ?? new Dictionary<ExpressionKind, IPostfixParselet<ValueNode>>();

        this.expressionKinds = expressionKinds ?? new Dictionary<string, ExpressionKind>();

        this.toklets = toklets ?? new List<IToklet<Token>>();
        this.triviaToklets = triviaToklets ?? new List<ITriviaToklet<TriviaToken>>();
    }

    public IToklet<Token> MatchToklet(StringConsumer input)
        => toklets.Find(toklet => toklet.Condition(new StringConsumer(input)))!;

    public ITriviaToklet<TriviaToken>? MatchTriviaToklet(StringConsumer input)
        => triviaToklets.Find(toklet => toklet.Condition(new StringConsumer(input)));

    public ExpressionKind GetExpressionKind(Token token) {

        if (token == null) return ExpressionKind.NotAnExpr;

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

    public IPrefixParselet<ValueNode> GetPrefixParselet(ExpressionKind kind)
        => prefixParselets[kind];

    public IPrefixParselet<ValueNode> GetPrefixParselet(Token token)
        => prefixParselets[GetExpressionKind(token)];

    public IInfixParselet<ValueNode> GetOperatorParselet(ExpressionKind kind)
        => infixParselets[kind];

    public IInfixParselet<ValueNode> GetOperatorParselet(Token token)
        => infixParselets[GetExpressionKind(token)];

    public IPostfixParselet<ValueNode> GetPostfixParselet(ExpressionKind kind)
        => postfixParselets[kind];

    public IPostfixParselet<ValueNode> GetPostfixParselet(Token token)
        => postfixParselets[GetExpressionKind(token)];

    public Precedence GetPrecedence(ExpressionKind kind) {
        if (!IsOperatorParselet(kind)) {
            return 0;
        }

        return GetOperatorParselet(kind).Precedence;
    }

    public Precedence GetPrecedence(Token token)
        => token != null ? GetPrecedence(GetExpressionKind(token)) : 0;

    public bool IsPrefix(ExpressionKind kind)
        => prefixParselets.ContainsKey(kind);

    public bool IsPostfix(ExpressionKind kind)
        => postfixParselets.ContainsKey(kind);

    public bool IsOperatorParselet(ExpressionKind kind)
        => infixParselets.ContainsKey(kind);
}