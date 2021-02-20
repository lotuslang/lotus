using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class TopLevelParser : Parser
{
    public StatementParser StatementParser { get; private set; }

    public ExpressionParser ExpressionParser => StatementParser.ExpressionParser;

    protected void Init() {
        StatementParser = new StatementParser(this);
    }

#nullable disable
    public TopLevelParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, LotusGrammar.Instance) {
        Init();
    }

    public TopLevelParser(IConsumer<Node> nodeConsumer) : base(nodeConsumer, LotusGrammar.Instance) {
        Init();
    }

    public TopLevelParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public TopLevelParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public TopLevelParser(FileInfo file) : this(new LotusTokenizer(file)) { }

    public TopLevelParser(Parser parser) : base(parser) {
        Init();
    }
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

    public bool Consume(out TopLevelNode result) {
        result = Consume(); // consume a StatementNode

        return result != Default;
    }

    public override TopLevelNode Consume() {
        base.Consume();

        
    }
}