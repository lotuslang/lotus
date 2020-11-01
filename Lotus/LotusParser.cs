using System.Collections.Generic;

public abstract class LotusParser : Parser
{

    public StatementParser StatementParser { get; }

    public ExpressionParser ExpressionParser { get; }

    public LotusParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, new LotusGrammar()) {
        StatementParser = new StatementParser(tokenConsumer);
        ExpressionParser = new ExpressionParser(tokenConsumer);
    }

    public LotusParser(IConsumer<StatementNode> nodeConsumer) : base(nodeConsumer, new LotusGrammar()) {
        StatementParser = new StatementParser(nodeConsumer);
        ExpressionParser = new ExpressionParser(nodeConsumer);
    }

    public LotusParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public LotusParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public LotusParser(System.IO.FileInfo file) : this(new LotusTokenizer(file)) { }
}