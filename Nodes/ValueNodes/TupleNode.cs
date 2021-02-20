using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TupleNode : ValueNode
{
    public new static readonly TupleNode NULL = new TupleNode(new ValueNode[0], Token.NULL, Token.NULL, false);
    public Token ClosingToken { get; }

    public Token OpeningToken => Token;

    public ReadOnlyCollection<ValueNode> Values { get; }

    public int Count => Values.Count;

    public TupleNode(IList<ValueNode> values, Token openingToken, Token closingToken, bool isValid = true)
        : base(openingToken, new LocationRange(openingToken.Location, closingToken.Location), isValid)
    {
        ClosingToken = closingToken;
        Values = values.AsReadOnly();
        Location = new LocationRange(openingToken.Location, closingToken.Location);
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(StatementVisitor<T> visitor) => visitor.Visit(this);
}