using System;
using System.Linq;
using System.Collections.Generic;

public interface IPrefixParselet<out T> where T : ValueNode
{
    T Parse(Parser parser, Token token);
}

public interface IInfixParselet<out T> where T : ValueNode
{
    T Parse(Parser parser, Token token, ValueNode left);
    Precedence Precedence { get; }
}

public interface IPostfixParselet<out T> : IInfixParselet<T> where T : ValueNode
{ }

public interface IStatementParselet<out T>  where T : StatementNode
{
    T Parse(Parser parser, Token token);
}