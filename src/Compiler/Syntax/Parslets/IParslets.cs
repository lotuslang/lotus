namespace Lotus.Syntax;

public interface IPrefixParslet<out T> : IParslet<Token, T>
where T : ValueNode
{}

public interface IInfixParslet<out T> where T : ValueNode
{
    T Parse(Parser parser, Token token, ValueNode left);
    Precedence Precedence { get; }
}

public interface IPostfixParslet<out T> : IInfixParslet<T> where T : ValueNode
{ }

public interface IStatementParslet<out T> : IParslet<Token, T>
where T : StatementNode
{}

public interface ITopLevelParslet<out T> where T : TopLevelNode
{
    T Parse(Parser parser, Token token, ImmutableArray<Token> modifiers);
}

public interface IParslet<out TOut> : IParslet<None, TOut>
{
    TOut Parse(Parser parser);
    TOut IParslet<None, TOut>.Parse(Parser parser, None _) => Parse(parser);
}

public interface IParslet<in TArg, out TOut>
{
    TOut Parse(Parser parser, TArg arg);
}