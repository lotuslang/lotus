using System;
using System.Linq;
using System.Collections.Generic;

public interface IPrefixParslet<out T> where T : ValueNode
{
    T Parse(ExpressionParser parser, Token token);
}

public interface IInfixParslet<out T> where T : ValueNode
{
    T Parse(ExpressionParser parser, Token token, ValueNode left);
    Precedence Precedence { get; }
}

public interface IPostfixParslet<out T> : IInfixParslet<T> where T : ValueNode
{ }

public interface IStatementParslet<out T> where T : StatementNode
{
    T Parse(StatementParser parser, Token token);
}

public interface ITopLevelParselet<out T> where T: TopLevelNode
{
    T Parse(TopLevelParser parser, Token token);
}