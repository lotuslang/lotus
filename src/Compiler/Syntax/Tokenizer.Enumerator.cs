using System.Collections;

namespace Lotus.Syntax;

public sealed partial class Tokenizer : IEnumerable<Token>
{
    public IEnumerator<Token> GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    private class Enumerator : IEnumerator<Token>
    {
        private readonly Tokenizer _tokenizer;

        private Token _curr = Token.NULL;
        public Token Current => _curr;
        object IEnumerator.Current => Current;

        public Enumerator(Tokenizer tokenizer) => _tokenizer = tokenizer;

        public bool MoveNext() => _tokenizer.Consume(out _curr);
        public void Reset() => throw new NotImplementedException();
        public void Dispose() => throw new NotImplementedException();
    }
}