using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Represents a for-loop statement
/// </summary>
public class ForNode : StatementNode
{
    public new static readonly ForNode NULL = new ForNode(ComplexToken.NULL, new StatementNode[0], SimpleBlock.NULL, Token.NULL, Token.NULL, false);

    public ReadOnlyCollection<StatementNode> Header { get; }

    public Token OpeningParenthesis { get; }

    public Token ClosingParenthesis { get; }

    public SimpleBlock Body { get; }

    public ForNode(ComplexToken forToken, IList<StatementNode> header, SimpleBlock body, Token openingParen, Token closingParen, bool isValid = true)
        : base(forToken, new LocationRange(forToken.Location, body.Location), isValid)
    {
        OpeningParenthesis = openingParen;
        ClosingParenthesis = closingParen;
        Header = header.AsReadOnly();
        Body = body;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
