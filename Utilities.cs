using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

public static class Utilities
{
    public static readonly HashSet<string> keywords = new() {
        "var",
        "new",
        "func",
        "return",
        "for",
        "while",
        "foreach",
        "in",
        "if",
        "else",
        "from",
        "import",
        "using",
        "namespace",
        "enum",
        "continue",
        "break",
        "public",
        "internal",
        "protected",
        "private",
    };

    public static readonly HashSet<string> internalFunctions = new() {
        "print",
    };

    public static int GetNumberOfDigits(int i) {
        int count = 0;

        do count++; while ((i /= 10) >= 1);

        return count;
    }

    public static bool IsName(ValueNode node) => ASTHelper.IsName(node);

    [System.Diagnostics.DebuggerStepThrough]
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        where TKey : notnull
        => new(dic);

    [System.Diagnostics.DebuggerStepThrough]
    public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        => new(list);

    [System.Diagnostics.DebuggerStepThrough]
    public static ReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> list)
        => list.ToList().AsReadOnly();

    /// <summary>
    /// Tries to find an element in the collection that matches the condition
    /// </summary>
    /// <param name="collection">The collection to search in</param>
    /// <param name="match">The condition to search for</param>
    /// <returns>Either the first element that matches ; or default(T)</returns>
    [return: MaybeNull]
    [System.Diagnostics.DebuggerStepThrough]
    public static T? Find<T>(this ICollection<T> collection, Predicate<T> match) {
        if (match is null) {
            throw new ArgumentNullException(nameof(match));
        }

        foreach (var item in collection) {
            if (match(item)) return item;
        }

        return default(T);
    }

    /// <summary>
    /// Checks whether the collection contains an element that matches the condition
    /// </summary>
    /// <param name="collection">The collection to search in</param>
    /// <param name="match">The condition to check against for</param>
    [System.Diagnostics.DebuggerStepThrough]
    public static bool Contains<T>(this ICollection<T> collection, Predicate<T> match) {
        if (match is null) {
            throw new ArgumentNullException(nameof(match));
        }

        foreach (var item in collection) {
            if (match(item)) return true;
        }

        return false;
    }

    [System.Diagnostics.DebuggerStepThrough]
    public static string Join<T>(string separator, Func<T, string> convert, params T[] value)
        => Join(separator, convert, coll: value);

    [System.Diagnostics.DebuggerStepThrough]
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

            foreach (var item in coll) output.Append(convert(item) + separator);

            if (separator.Length != 0)
                output = output.Remove(output.Length - separator.Length, separator.Length);

            return output.ToString();
        }
    }

    [System.Diagnostics.DebuggerStepThrough]
	public static (List<T> valid, List<T> invalid) Split<T>(this IEnumerable<T> list, Predicate<T> match) {
		var (valid, invalid) = (new List<T>(), new List<T>());

		foreach (var item in list) {
			if (match(item)) valid.Add(item);
			else invalid.Add(item);
		}

		return (valid, invalid);
	}

    [System.Diagnostics.DebuggerStepThrough]
	public static (List<TValid> valid, List<TInvalid> invalid) SplitByType<TList, TValid, TInvalid>(this IEnumerable<TList> list)
		where TInvalid : class, TList
	{
		var (valid, invalid) = (new List<TValid>(), new List<TInvalid>());

		foreach (var item in list) {
			if (item is TValid validItem) valid.Add(validItem);
			else invalid.Add((item as TInvalid)!);
		}

		return (valid, invalid);
	}

    [System.Diagnostics.DebuggerStepThrough]
	public static (List<TMatch> valid, List<TOther> invalid) SplitByType<TMatch, TOther>(this IEnumerable<TOther> list) where TOther : class
		=> SplitByType<TOther, TMatch, TOther>(list);

	// the loops you have to jump through sometimes...
    [System.Diagnostics.DebuggerStepThrough]
	public static IEnumerable<TMatch> WhereType<TMatch>(this IEnumerable list) {
		foreach (var item in list) {
			if (item is TMatch matched)
                yield return matched;
		}
	}

    [System.Diagnostics.DebuggerStepThrough]
    public static GraphNode Apply(this GraphNode node, Func<GraphNode, GraphNode> transform) {
        foreach (var child in node.Children) {
            child.Apply(transform);
        }

        return transform(node);
    }

    [System.Diagnostics.DebuggerStepThrough]
	public static Stack<T> Clone<T>(this Stack<T> original) {
		var arr = new T[original.Count];
		original.CopyTo(arr, 0);
		Array.Reverse(arr);
		return new Stack<T>(arr);
	}

    public static string ToExpectedString(IEnumerable<TokenKind> expected)
        => '`' + String.Join("`, or `", expected) + '`';

    public static string ToExpectedString(IEnumerable<Type> expected)
        => '`' + String.Join("`, or `", expected.Select(type => type.Name)) + '`';

    public static string ToExpectedString(IEnumerable expected)
        => '\'' + String.Join("', or '", expected) + '\'';

    public static Uri RelativeToPWD(this Uri uri)
        => new Uri(System.IO.Directory.GetCurrentDirectory()).MakeRelativeUri(uri);

    public static string GetDisplayName(this Type type) {
        if (!type.IsGenericType)
            return type.Name;

        return type.Name.Remove(type.Name.Length - 2)
             + '<'
             + String.Join(", ", type.GenericTypeArguments.Select(t => t.GetDisplayName()))
             + '>';
    }
}