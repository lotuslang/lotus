public interface IPrefixParslet<out T> : IParslet<ExpressionParser, Token, T>
where T : ValueNode
{}

public interface IInfixParslet<out T> where T : ValueNode
{
    T Parse(ExpressionParser parser, Token token, ValueNode left);
    Precedence Precedence { get; }
}

public interface IPostfixParslet<out T> : IInfixParslet<T> where T : ValueNode
{ }

public interface IStatementParslet<out T> : IParslet<StatementParser, Token, T>
where T : StatementNode
{}

public interface ITopLevelParslet<out T> : IParslet<TopLevelParser, Token, T>
where T : TopLevelNode
{}

public interface IParslet<in TParser, out TOut> : IParslet<TParser, None, TOut>
{
    TOut Parse(TParser parser);
    TOut IParslet<TParser, None, TOut>.Parse(TParser parser, None arg) => Parse(parser);
}

public interface IParslet<in TParser, in TArg, out TOut>
{
    TOut Parse(TParser parser, TArg arg);
}