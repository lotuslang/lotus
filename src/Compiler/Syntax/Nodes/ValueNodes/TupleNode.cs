using System.Collections;
using System.Runtime.CompilerServices;

namespace Lotus.Syntax;

public sealed record TupleNode
: ValueNode, IEnumerable<ValueNode>
{
    public new static readonly TupleNode NULL = new(Tuple<ValueNode>.NULL);

    private readonly Tuple<ValueNode> _internalTuple;

    public ImmutableArray<ValueNode> Items => _internalTuple.Items;
    public Token OpeningToken => _internalTuple.OpeningToken;
    public Token ClosingToken => _internalTuple.ClosingToken;
    public int Count => _internalTuple.Count;

    public TupleNode(Tuple<ValueNode> tuple) : base(tuple.OpeningToken, tuple.Location, tuple.IsValid)
        => _internalTuple = tuple;

    public TupleNode(ImmutableArray<ValueNode> items, Token openingToken, Token closingToken)
        : this(new Tuple<ValueNode>(items, openingToken, closingToken)) { }

    /// <summary>
    /// <strong>TRUNCATES</strong> the tuple to the first element and turns it into a paren expression.
    /// </summary>
    internal ParenthesizedValueNode AsParenthesized()
        => new(
            Items.IsDefaultOrEmpty ? ValueNode.NULL : Items[0],
            OpeningToken,
            ClosingToken
        ) { IsValid = IsValid };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ImmutableArray<ValueNode>.Enumerator GetEnumerator() => _internalTuple.GetEnumerator();

    IEnumerator<ValueNode> IEnumerable<ValueNode>.GetEnumerator() => ((IEnumerable<ValueNode>)_internalTuple).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_internalTuple).GetEnumerator();

    public static explicit operator Tuple<ValueNode>(TupleNode node) => node._internalTuple;
}