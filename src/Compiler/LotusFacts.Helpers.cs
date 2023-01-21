using Lotus.Syntax;
using Lotus.Semantic;

namespace Lotus;

public static partial class LotusFacts
{
    internal static bool IsStartOfNumber(char c, char nextChar)
        => Char.IsAsciiDigit(c)
        || (c is '.' && Char.IsAsciiDigit(nextChar));

    public static bool NeedsSemicolon(StatementNode node)
        => node is not (
                   ElseNode
                or ForeachNode
                or ForNode
                or FunctionDeclarationNode
                or IfNode
                or WhileNode
            );

    public static AccessLevel GetAccess(string str)
        => str switch {
            "public" => AccessLevel.Public,
            "internal" => AccessLevel.Internal,
            "private" => AccessLevel.Private,
            _ => AccessLevel.Default,
        };

    public static AccessLevel GetAccessAndValidate(
        Token accessToken,
        AccessLevel defaultLvl,
        AccessLevel validLvls
    ) {
        var _accessLevel = defaultLvl;

        if (accessToken != Token.NULL) {
            _accessLevel = GetAccess(accessToken);

            if ((_accessLevel & validLvls) == AccessLevel.Default) {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = accessToken,
                    As = "an access modifier",
                    Message = "The " + accessToken + " modifier is not valid here",
                    Expected = "one of " + String.Join(", ", validLvls.GetMatchingValues())
                });
            }
        }

        return _accessLevel;
    }
}