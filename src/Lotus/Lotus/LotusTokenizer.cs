using Lotus.Text;
using Lotus.Syntax;

namespace Lotus;

public sealed class LotusTokenizer : Tokenizer
{
    public LotusTokenizer(StringConsumer stringConsumer) : base(stringConsumer, LotusGrammar.Instance)
    { }

    public LotusTokenizer(IConsumer<char> consumer) : base(consumer, LotusGrammar.Instance)
    { }

    public LotusTokenizer(IConsumer<Token> tokenConsumer) : base(tokenConsumer, LotusGrammar.Instance)
    { }

    public LotusTokenizer(LotusTokenizer tokenizer) : base(tokenizer, tokenizer.Grammar)
    { }

    public LotusTokenizer(System.Uri fileInfo) : this(new StringConsumer(fileInfo))
    { }

    public LotusTokenizer(IEnumerable<char> collection) : this(new StringConsumer(collection))
    { }
}