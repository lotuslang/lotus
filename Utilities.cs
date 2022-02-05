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
        "continue",
        "break"
    };

    public static readonly HashSet<string> internalFunctions = new() {
        "print",
    };

    public static int GetNumberOfDigits(int i) {
        int count = 0;

        do count++; while ((i /= 10) >= 1);

        return count;
    }

    public static bool IsName(ValueNode node) => ASTHelper.IsName((StatementExpressionNode)node);

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
    public static string Join<T>(string separator, Func<T, string> convert, params T[] value) => Join(separator, convert, coll: value);

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
            else invalid.Add((TInvalid)item!);
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
			if (item is TMatch matched) yield return matched;
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

    public class Union<T, U>
    {
        readonly T? t;
        readonly U? u;
        readonly int tag;

        public Union(T item) { t = item; tag = 0; }
        public Union(U item) { u = item; tag = 1; }

        public TResult Match<TResult>(Func<T, TResult> f, Func<U, TResult> g) {
            switch (tag) {
                case 0: return f(t!);
                case 1: return g(u!);
                default: throw new Exception("Unrecognized tag value: " + tag);
            }
        }

        public static implicit operator Union<T, U>(T t) => new(t);
        public static implicit operator Union<T, U>(U u) => new(u);

        public static explicit operator T(Union<T, U> union) => union.t!;
        public static explicit operator U(Union<T, U> union) => union.u!;



        public override string ToString() => Match(t => t!.ToString(), u => u!.ToString())!;
    }

    public class Union<T, U, V>
    {
        readonly T? Item1;
        readonly U? Item2;
        readonly V? Item3;
        readonly int tag;

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

        public void Match<TResult>(Action<T> f, Action<U> g, Action<V> h) {
            switch (tag) {
                case 0:
                    f(Item1!);
                    break;
                case 1:
                    g(Item2!);
                    break;
                case 2:
                    h(Item3!);
                    break;
                default:
                    throw new Exception("Unrecognized tag value: " + tag);
            }
        }

        public static implicit operator Union<T, U, V>(T t) => new(t);
        public static implicit operator Union<T, U, V>(U u) => new(u);
        public static implicit operator Union<T, U, V>(V v) => new(v);

        public override string ToString() => Match(t => t!.ToString(), u => u!.ToString(), v => v!.ToString())!;
    }

    public sealed class None {}

    public sealed class Result<T> : Union<T, None>
    {
        public Result(T value) : base(value) { }

        public static implicit operator Result<T>(T t) => new(t);
    }
}