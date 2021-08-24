using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public record ImportNode(IList<ValueNode> Names, FromNode FromStatement, Token Token, bool IsValid = true)
: TopLevelNode(
    Token,
    new LocationRange(
        FromStatement.Location,
        Names.LastOrDefault()?.Location ?? Token.Location // this shouldn't happen anyway
    ),
    IsValid
)
{
    public new static readonly ImportNode NULL = new(Array.Empty<ValueNode>(), FromNode.NULL, Token.NULL, false);

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}
