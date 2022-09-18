using System.Collections.ObjectModel;

public static class Utils
{
    public static int GetNumberOfDigits(int i) {
        int count = 0;

        do count++; while ((i /= 10) >= 1);

        return count;
    }

    [DebuggerStepThrough]
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        where TKey : notnull
        => new(dic);

    [DebuggerStepThrough]
    public static ReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> list)
        => new(list.ToArray());

    [DebuggerStepThrough]
        public static string Join<T>(string separator, Func<T, string> convert, IEnumerable<T> coll) {
        var count = coll.Count();

        if (count == 0) {
            return "";
        } else if (count == 1) {
            return convert(coll.First());
        } else if (count < 20) {
            var output = "";

            foreach (var item in coll) output += convert(item) + separator;

            if (separator.Length != 0)
                output = output.Remove(output.Length - separator.Length);

            return output;
        } else {
            var output = new System.Text.StringBuilder();

            foreach (var item in coll) output.Append(convert(item)).Append(separator);

            if (separator.Length != 0)
                output = output.Remove(output.Length - separator.Length, separator.Length);

            return output.ToString();
        }
    }

    public static string GetDisplayName(this Type type) {
        if (!type.IsGenericType)
            return type.Name;

        return type.Name.Remove(type.Name.Length - 2)
             + '<'
             + Join(", ", GetDisplayName, type.GenericTypeArguments)
             + '>';
    }

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
                    Expected = "one of " + String.Join(", ", GetMatchingValues(validLvls))
                });
            }
        }

        return _accessLevel;
    }

    public static IEnumerable<TEnum> GetMatchingValues<TEnum>(this TEnum flag) where TEnum : struct, Enum {
        foreach (var value in Enum.GetValues<TEnum>()) {
            if (flag.HasFlag(value))
                yield return value;
        }
    }

    public static bool IsAsciiDigit(in char c) => (uint)(c - '0') <= 9;
}