using System;
using System.Linq;
using System.Collections.Generic;

public interface IPrefixParselet
{
    StatementNode Parse(Parser parser, Token token);
}

public interface IInfixParselet
{
    StatementNode Parse(Parser parser, Token token, StatementNode left);
    Precedence Precedence { get; }
}

public interface IPostfixParselet : IInfixParselet
{ }