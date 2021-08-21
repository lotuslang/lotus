using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TupleNode : ValueNode
{
    public new static readonly TupleNode NULL = new(Array.Empty<ValueNode>(), Token.NULL, Token.NULL, false);
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

    public ParenthesizedValueNode AsParenthsized()
        => new(
            Values[0],
            OpeningToken,
            ClosingToken,
            IsValid
        );

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}