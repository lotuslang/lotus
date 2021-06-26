public interface ITokenVisitor<T>
{
    T Default(Token token);

    T Default(TriviaToken token);


    T Visit(Token token) => Default(token);

    T Visit(BoolToken token) => Visit(token as ComplexToken);
    T Visit(ComplexStringToken token) => Visit(token as ComplexToken);
    T Visit(ComplexToken token) => Default(token);
    T Visit(IdentToken token) => Visit(token as ComplexToken);
    T Visit(NumberToken token) => Visit(token as ComplexToken);
    T Visit(OperatorToken token) => Default(token);
    T Visit(StringToken token) => Visit(token as ComplexToken);


    T Visit(TriviaToken token) => Default(token);

    T Visit(CommentTriviaToken token) => Default(token);
    T Visit(NewlineTriviaToken token) => Visit(token as WhitespaceTriviaToken);
    T Visit(WhitespaceTriviaToken token) => Default(token);
}