using System.Collections.Generic;

public abstract class LotusParser : Parser
{
    public TopLevelParser TopLevelParser { get; }

    public StatementParser StatementParser { get; }

    public ExpressionParser ExpressionParser { get; }

    public LotusParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, LotusGrammar.Instance) {
        TopLevelParser = new TopLevelParser(tokenConsumer);
        StatementParser = new StatementParser(tokenConsumer);
        ExpressionParser = new ExpressionParser(tokenConsumer);
    }

    public LotusParser(IConsumer<Node> nodeConsumer) : base(nodeConsumer, LotusGrammar.Instance) {
        TopLevelParser = new TopLevelParser(nodeConsumer);
        StatementParser = TopLevelParser.StatementParser;
        ExpressionParser = StatementParser.ExpressionParser;
    }

    public LotusParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public LotusParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public LotusParser(System.IO.FileInfo file) : this(new LotusTokenizer(file)) { }
}