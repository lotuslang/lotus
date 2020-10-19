using System.Collections.Generic;

public sealed class LotusTokenizer : Tokenizer
{

    public LotusTokenizer(StringConsumer stringConsumer) : base(stringConsumer, new LotusGrammar())
    { }

    public LotusTokenizer(IConsumer<char> consumer) : base(consumer, new LotusGrammar())
    { }

    public LotusTokenizer(IConsumer<Token> tokenConsumer) : base(tokenConsumer, new LotusGrammar())
    { }

    public LotusTokenizer(LotusTokenizer tokenizer) : base(tokenizer, tokenizer.Grammar)
    { }

    public LotusTokenizer(System.IO.FileInfo fileInfo) : this(new StringConsumer(fileInfo))
    { }

    public LotusTokenizer(IEnumerable<char> collection) : this(new StringConsumer(collection))
    { }

    public LotusTokenizer(IEnumerable<string> collection) : this(new StringConsumer(collection))
    { }
}