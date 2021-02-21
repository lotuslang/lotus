using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

public static class Utilities
{
    public static readonly HashSet<string> keywords = new HashSet<string> {
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
    };

    public static readonly HashSet<string> internalFunctions = new HashSet<string> {
        "print",
    };

    public static int GetNumberOfDigits(int i) {
        int count = 0;

        do count++; while ((i /= 10) >= 1);

        return count;
    }

    public static bool IsName(ValueNode node) => ASTHelper.IsName(node);

    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        where TKey : notnull
        => new ReadOnlyDictionary<TKey, TValue>(dic);

    public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        => new ReadOnlyCollection<T>(list);

    public static ReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> list)
        => list.ToList().AsReadOnly();

    /// <summary>
    /// Tries to find an element in the collection that matches the condition
    /// </summary>
    /// <param name="collection">The collection to search in</param>
    /// <param name="match">The condition to search for</param>
    /// <returns>Either the first element that matches ; or default(T)</returns>
    [return: MaybeNull]
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
    public static bool Contains<T>(this ICollection<T> collection, Predicate<T> match) {
        if (match is null) {
            throw new ArgumentNullException(nameof(match));
        }

        foreach (var item in collection) {
            if (match(item)) return true;
        }

        return false;
    }

    public static string Join<T>(string separator, Func<T, string> convert, params T[] value) => Join(separator, convert, coll: value);

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

	public static (List<T> valid, List<T> invalid) Split<T>(this IEnumerable<T> list, Predicate<T> match) {
		var (valid, invalid) = (new List<T>(), new List<T>());

		foreach (var item in list) {
			if (match(item)) valid.Add(item);
			else invalid.Add(item);
		}

		return (valid, invalid);
	}

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

	public static (List<TMatch> valid, List<TOther> invalid) SplitByType<TMatch, TOther>(this IEnumerable<TOther> list) where TOther : class
		=> SplitByType<TOther, TMatch, TOther>(list);

	// the loops you have to jump through sometimes...
	public static IEnumerable<TMatch> WhereType<T, TMatch>(this IEnumerable<T> list) {
		foreach (var item in list) {
			if (item is TMatch matched) yield return matched;
		}
	}

    public static GraphNode Apply(this GraphNode node, Func<GraphNode, GraphNode> transform) {
        foreach (GraphNode child in node.Children) {
            child.Apply(transform);
        }

        return transform(node);
    }

	public static Stack<T> Clone<T>(this Stack<T> original) {
		var arr = new T[original.Count];
		original.CopyTo(arr, 0);
		Array.Reverse(arr);
		return new Stack<T>(arr);
	}
}