using System;
using System.Collections.Generic;

public class TopLevelParser : Parser<TopLevelNode>
{
    public StatementParser StatementParser { get; private set; }

    public ExpressionParser ExpressionParser => StatementParser.ExpressionParser;

    public override TopLevelNode Current {
        get;
        protected set;
    } = ConstantDefault;

    public new static readonly TopLevelNode ConstantDefault = TopLevelNode.NULL;

    public override TopLevelNode Default => ConstantDefault with { Location = Position };

    protected void Init() {
        StatementParser = new StatementParser(Tokenizer);
        Current = ConstantDefault;
    }

#nullable disable
    public TopLevelParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, LotusGrammar.Instance)
        => Init();

    public TopLevelParser(IConsumer<TopLevelNode> nodeConsumer) : base(nodeConsumer, LotusGrammar.Instance)
        => Init();

    public TopLevelParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public TopLevelParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public TopLevelParser(Uri file) : this(new LotusTokenizer(file)) { }

    public TopLevelParser(Parser<TopLevelNode> parser) : base(parser, LotusGrammar.Instance)
        => Init();
#nullable enable

    public override TopLevelNode Peek()
        => new TopLevelParser(this).Consume();

    public override TopLevelNode[] Peek(int n) {
        var parser = new TopLevelParser(this);

        var output = new List<TopLevelNode>();

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.ToArray();
    }

    public override TopLevelNode Consume() {
        base.Consume();

        var currToken = Tokenizer.Consume();

        // if the token is EOF, return TopLevelNode.NULL
        if (currToken == Tokenizer.Default || currToken == "\u0003") {
            return (Current = Default);
        }

        if (Grammar.TryGetTopLevelParslet(currToken, out var parslet)) {
            Current = parslet.Parse(this, currToken);
        } else {
            Tokenizer.Reconsume();
            Current = new TopLevelStatementNode(StatementParser.Consume());
        }

        return Current;
    }

    public override TopLevelParser Clone() => new(this);
}