using System.Collections;

internal sealed class MarkupChain : IEnumerable<Markup>
{
    [System.Diagnostics.DebuggerDisplay("{Prev?.Value.ToString() ?? \"\"} < {Value.ToString()} > {Next?.Value.ToString() ?? \"\"}")]
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

    private MarkupNode? head;

    public MarkupNode? First {
        get => head;
        private set => head = value;
    }

    public MarkupNode? Last => head?.Prev;

    public MarkupChain() {}

    public MarkupChain(MarkupChain chain) : this() {
        head = chain.head;
    }

    public MarkupChain(IEnumerable<Markup> markups) : this() {
        foreach (var item in markups)
            AddLast(item);
    }

    public void AddLast(Markup mk) {
        var node = new MarkupNode(mk);

        if (head is null) {
            head = node;
            head.Next = head.Prev = head;
        } else {
            node.Prev = head.Prev;
            node.Next = head;
            head.Prev!.Next = node;
            head.Prev = node;
        }
    }

    public void AddLast(MarkupNode node) {
        if (head is null) {
            head = node;
        } else {
            head.Prev!.Next = node;

            if (node.Prev is not null)
                node.Prev.Next = head;


            var newPrev = node.Prev;
            node.Prev = head.Prev;

            if (node.Next is null)
                node.Next = head;

            head.Prev = newPrev;
        }
    }

    public void AddLast(MarkupChain chain) {
        if (chain.head is not null)
            AddLast(chain.head);
    }

    public IEnumerator<Markup> GetEnumerator() => new MarkupChainEnumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal class MarkupChainEnumerator : IEnumerator<Markup>
    {
        private readonly MarkupChain chain;

        private MarkupNode? _nextNode;

        private Markup? _item;
        public Markup Current => _item!; // just like the dotnet impl lol

        object? IEnumerator.Current => _item;

        public MarkupChainEnumerator(MarkupChain chain) {
            this.chain = chain;
            _nextNode = chain.head;
        }


        public bool MoveNext() {
            if (_nextNode is null) {
                //_current = consumer.Default;
                return false;
            }

            _item = _nextNode.Value;
            _nextNode = _nextNode!.Next;

            if (_nextNode == chain.head)
                _nextNode = null;

            return true;
        }

        public void Reset() => throw new NotImplementedException();
        // suggested by roslyn, don't know either :shrug:
        public void Dispose() => GC.SuppressFinalize(this);
    }
}