using System.Collections.Generic;
using System.IO;

public class StatementParser : Parser
{
    public ExpressionParser ExpressionParser { get; protected set; }

    protected void Init() {
        ExpressionParser = new ExpressionParser(this);
    }

#nullable disable
    public StatementParser(IConsumer<Token> tokenConsumer) : base(tokenConsumer, new LotusGrammar()) {
        Init();
    }

    public StatementParser(IConsumer<StatementNode> nodeConsumer) : base(nodeConsumer, new LotusGrammar()) {
        Init();
    }

    public StatementParser(StringConsumer consumer) : this(new LotusTokenizer(consumer)) { }

    public StatementParser(IEnumerable<char> collection) : this(new LotusTokenizer(collection)) { }

    public StatementParser(FileInfo file) : this(new LotusTokenizer(file)) { }

    public StatementParser(Parser parser) : base(parser) {
        Init();
    }
#nullable enable

    public override StatementNode Peek()
        => new StatementParser(this).Consume();

    public override StatementNode[] Peek(int n) {
        var parser = new StatementParser(this);

        var output = new List<StatementNode>();

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.ToArray();
    }

    public override StatementNode Consume() {
        base.Consume();

        // Consume a token
        var currToken = Tokenizer.Consume();

        // if the token is EOF, return StatementNode.NULL
        if (currToken == Tokenizer.Default || currToken == "\u0003") {
            return (Current = Default);
        }

        if (Grammar.TryGetStatementParslet(currToken, out var parslet)) {
            Current = parslet.Parse(this, currToken);
        } else {
            Tokenizer.Reconsume();
            Current = ExpressionParser.Consume();
        }

        return Current;
    }
}