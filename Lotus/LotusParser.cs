using System.Collections.Generic;

public sealed class LotusParser : Parser
{

    public LotusParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, new LotusGrammar()) { }

    public LotusParser(IConsumer<StatementNode> nodeConsumer) : base(nodeConsumer, new LotusGrammar()) { }

    public LotusParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public LotusParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public LotusParser(System.IO.FileInfo file) : this(new LotusTokenizer(file)) { }
}