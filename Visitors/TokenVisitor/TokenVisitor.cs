#nullable disable
#pragma warning disable IDE0034

public abstract class TokenVisitor<T>
{
    protected abstract T Default(Token token);

    protected abstract T Default(TriviaToken token);


    public virtual T Visit(Token token) => Default(token);

    public virtual T Visit(BoolToken token) => Visit(token as ComplexToken);
    public virtual T Visit(ComplexStringToken token) => Visit(token as ComplexToken);
    public virtual T Visit(ComplexToken token) => Default(token);
    public virtual T Visit(IdentToken token) => Visit(token as ComplexToken);
    public virtual T Visit(NumberToken token) => Visit(token as ComplexToken);
    public virtual T Visit(OperatorToken token) => Default(token);
    public virtual T Visit(StringToken token) => Visit(token as ComplexToken);


    public virtual T Visit(TriviaToken token) => Default(token);

    public virtual T Visit(CommentTriviaToken token) => Default(token);
    public virtual T Visit(NewlineTriviaToken token) => Visit(token as WhitespaceTriviaToken);
    public virtual T Visit(WhitespaceTriviaToken token) => Default(token);
}