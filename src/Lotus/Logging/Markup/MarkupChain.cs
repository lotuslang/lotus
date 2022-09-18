using System.Collections;

internal sealed class MarkupChain : IEnumerable<Markup>
{
    [DebuggerDisplay("{Prev?.Value.DbgString() ?? \"\"} < {Value.DbgString()} > {Next?.Value.DbgString() ?? \"\"}")]
    internal sealed class MarkupNode {
        public Markup Value { get; init; }
        public MarkupNode? Next { get; set; }
        public MarkupNode? Prev { get; set; }

        public MarkupNode(Markup val, MarkupNode? next = null, MarkupNode? prev = null) {
            Value = val;
            Next = next;
            Prev = prev;
        }
    }

    public MarkupNode? First { get; private set; }

    public MarkupNode? Last => First?.Prev;

    public MarkupChain() {}

    public MarkupChain(MarkupChain chain) : this() {
        First = chain.First;
    }

    public MarkupChain(IEnumerable<Markup> markups) : this() {
        foreach (var item in markups)
            AddLast(item);
    }

    public void AddLast(Markup mk) {
        var node = new MarkupNode(mk);

        if (First is null) {
            First = node;
            First.Next = First.Prev = First;
        } else {
            node.Prev = First.Prev;
            node.Next = First;
            First.Prev!.Next = node;
            First.Prev = node;
        }
    }

    public void AddLast(MarkupNode node) {
        if (First is null) {
            First = node;
        } else {
            First.Prev!.Next = node;

            if (node.Prev is not null)
                node.Prev.Next = First;

            var newPrev = node.Prev;
            node.Prev = First.Prev;

            node.Next ??= First;

            First.Prev = newPrev;
        }
    }

    public void AddLast(MarkupChain chain) {
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
                //_current = consumer.Default;
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