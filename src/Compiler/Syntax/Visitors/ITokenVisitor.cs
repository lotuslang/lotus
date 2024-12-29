namespace Lotus.Syntax.Visitors;

public interface ITokenVisitor<out T>
{
    T Default(Token token);

    T Default(TriviaToken? token);

    T Visit(Token token) => Default(token);

    T Visit(BoolToken token) => Visit(token as Token);
    T Visit(CharToken token) => Visit(token as Token);
    T Visit(IdentToken token) => Visit(token as Token);
    T Visit(NumberToken token) => Visit(token as Token);
    T Visit(OperatorToken token) => Default(token);
    T Visit(StringToken token) => Visit(token as Token);

    T Visit(ComplexStringToken token) => Visit(token as Token);

    T Visit(TriviaToken? token) => Default(token);

    T Visit(CommentTriviaToken token) => Default(token);
    T Visit(NewlineTriviaToken token) => Visit(token as WhitespaceTriviaToken);
    T Visit(WhitespaceTriviaToken token) => Default(token);
    T Visit(CharacterTriviaToken token) => Default(token);
}