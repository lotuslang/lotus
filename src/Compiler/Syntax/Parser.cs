namespace Lotus.Syntax;

public sealed partial class Parser
{
    public Tokenizer Tokenizer { get; }

    public LocationRange Position => Current.Location;

    public bool EndOfStream => Tokenizer.EndOfStream;

    private Node _curr = Node.NULL;
    public Node Current => _curr;

    public Parser(Tokenizer tokenizer) {
        Tokenizer = tokenizer;
        _curr = _curr with { Location = tokenizer.Position };
    }

    public Parser(TextStream stream) : this(new Tokenizer(stream)) { }

    private Parser(Parser parser) : this(parser.Tokenizer) {
        _curr = parser.Current;
    }

    internal Parser Clone() => new(this);

    public bool TryConsumeValue<TNode>([NotNullWhen(true)] out TNode? output, out ValueNode asNode) where TNode : ValueNode {
        asNode = ConsumeValue();
        output = asNode as TNode;
        return output is not null;
    }

    public Result<TNode> TryConsumeValue<TNode>(out ValueNode asNode) where TNode : ValueNode
        => (asNode = ConsumeValue()) as TNode ?? Result<TNode>.Error;

    public TNode ConsumeValue<TNode>(TNode defaultVal, Action<Node> errorHandler) where TNode : ValueNode {
        var errCount = Logger.ErrorCount;
        if (!TryConsumeValue<TNode>(out var output, out var val)) {
            while (errCount < Logger.ErrorCount)
                _ = Logger.errorStack.Pop();

            errorHandler(val);
        }

        return output ?? defaultVal;
    }

    public TNode ConsumeValue<TNode>(TNode defaultVal, string? @in = null, string? @as = null) where TNode : ValueNode
        => ConsumeValue<TNode>(
            defaultVal,
            val =>
                Logger.Error(new UnexpectedError<Node>(ErrorArea.Parser) {
                    Value = val,
                    In = @in,
                    As = @as,
                    Expected = typeof(TNode).Name
                })
        );

    public bool TryConsumeEitherValues<TNode1, TNode2>(Union<TNode1, TNode2> defaultVal, out Union<TNode1, TNode2> res, out ValueNode asNode)
        where TNode1 : ValueNode
        where TNode2 : ValueNode {
        asNode = ConsumeValue();

        switch (asNode) {
            case TNode1 t1:
                res = t1;
                return true;
            case TNode2 t2:
                res = t2;
                return true;
            default:
                res = defaultVal;
                return false;
        }
    }

    public Result<Union<T1, T2>> TryConsumeEitherValues<T1, T2>(out ValueNode asNode)
        where T1 : ValueNode
        where T2 : ValueNode
        => (asNode = ConsumeValue()) switch {
            T1 t1 => new(t1),
            T2 t2 => new(t2),
            _ => Result<Union<T1, T2>>.Error
        };
}