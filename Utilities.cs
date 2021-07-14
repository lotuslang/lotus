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
        "continue",
        "break"
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

    public class Union<T, U>
    {
        readonly T? Item1;
        readonly U? Item2;
        int tag;

        public Union(T item) { Item1 = item; tag = 0; }
        public Union(U item) { Item2 = item; tag = 1; }

        public TResult Match<TResult>(Func<T, TResult> f, Func<U, TResult> g) {
            switch (tag) {
                case 0: return f(Item1!);
                case 1: return g(Item2!);
                default: throw new Exception("Unrecognized tag value: " + tag);
            }
        }

        public static implicit operator Union<T, U>(T t) => new Union<T, U>(t);
        public static implicit operator Union<T, U>(U u) => new Union<T, U>(u);

        public override string ToString() => Match(t => t!.ToString(), u => u!.ToString())!;
    }

    public class Union<T, U, V>
    {
        readonly T? Item1;
        readonly U? Item2;
        readonly V? Item3;
        int tag;

        public Union(T item) { Item1 = item; tag = 0; }
        public Union(U item) { Item2 = item; tag = 1; }
        public Union(V item) { Item3 = item; tag = 2; }

        public TResult Match<TResult>(Func<T, TResult> f, Func<U, TResult> g, Func<V, TResult> h) {
            switch (tag) {
                case 0: return f(Item1!);
                case 1: return g(Item2!);
                case 2: return h(Item3!);
                default: throw new Exception("Unrecognized tag value: " + tag);
            }
        }

        public static implicit operator Union<T, U, V>(T t) => new Union<T, U, V>(t);
        public static implicit operator Union<T, U, V>(U u) => new Union<T, U, V>(u);
        public static implicit operator Union<T, U, V>(V v) => new Union<T, U, V>(v);

        public override string ToString() => Match(t => t!.ToString(), u => u!.ToString(), v => v!.ToString())!;
    }

    public sealed class None {}

    public sealed class Result<T> : Union<T, None>
    {
        public Result(T value) : base(value) { }

        public static implicit operator Result<T>(T t) => new Result<T>(t);
    }
}