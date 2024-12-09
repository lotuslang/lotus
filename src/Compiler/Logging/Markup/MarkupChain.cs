using System.Collections;

namespace Lotus.Text;

internal sealed class MarkupChain : IEnumerable<Markup>
{
    [DebuggerDisplay("{DbgStr()}")]
    internal sealed class MarkupNode {
        public Markup Value { get; init; }
        public MarkupNode Next { get; set; }
        public MarkupNode Prev { get; set; }

        public MarkupNode(Markup val, MarkupNode? next = null, MarkupNode? prev = null) {
            Value = val;
            Next = next ?? this;
            Prev = prev ?? this;
        }

        private string DbgStr() {
            var str = Value.DbgStr();

            if (Prev != this)
                str = Prev.Value.DbgStr() + " < " + str;

            if (Next != this)
                str += " > " + Next.Value.DbgStr();

            return str;
        }
    }

    public MarkupNode? First { get; private set; }

    [NotNullIfNotNull(nameof(First))]
    public MarkupNode? Last => First?.Prev;

    [MemberNotNullWhen(false, nameof(First))]
    public bool IsEmpty => First is null;

    public MarkupChain() {}

    public MarkupChain(IEnumerable<Markup> markups) : this() {
        foreach (var item in markups)
            AddLast(item);
    }

    public void AddLast(Markup mk) {
        if (First is null) {
            First = new(mk);
            return;
        }

        var node = new MarkupNode(mk, First, Last);
        Last!.Next = node;
        First.Prev = node;
    }

    public void AddLast(MarkupNode node) {
        if (First is null) {
            First = node;
        } else {
            var oldLast = Last!;
            // set last of this chain (first's backref) to the backref of the appended chain
            First.Prev = node.Prev;
            // set the forward ref of the old last node
            oldLast.Next = node;
            // set the backref of the appended node
            node.Prev = oldLast;
        }
    }

    public void Concat(MarkupChain chain) {
        if (chain.First is not null)
            AddLast(chain.First);
    }

    public IEnumerator<Markup> GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private class Enumerator : IEnumerator<Markup>
    {
        private readonly MarkupNode? _first;

        private MarkupNode? _nextNode;

        private Markup? _item;
        public Markup Current => _item!; // just like the dotnet impl lol

        object? IEnumerator.Current => _item;

        public Enumerator(MarkupChain chain) {
            _first = chain.First;
            _nextNode = _first;
        }

        public bool MoveNext() {
            if (_nextNode is null) {
                return false;
            }

            _item = _nextNode.Value;
            _nextNode = _nextNode!.Next;

            if (_nextNode == _first)
                _nextNode = null;

            return true;
        }

        public void Reset() => _nextNode = _first;
        // suggested by roslyn, don't know either :shrug:
        public void Dispose() => GC.SuppressFinalize(this);
    }
}