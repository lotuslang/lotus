using System;
using System.Linq;
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

    public static bool IsName(ValueNode node) {
        if (node is IdentNode) return true;

        if (node is OperationNode op && op.OperationType == OperationType.Access) {
            if (op.Operands.Count != 2) return false;

            // we only need to check the left-hand operand, because we know the right-hand operand
            // is an IdentNode, because an access operation is defined as :
            //  access-operation :
            //      value '.' identifier
            // hence, the left-hand side
            return IsName(op.Operands[0]);
        }

        return false;
    }

    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        where TKey : notnull
        => new ReadOnlyDictionary<TKey, TValue>(dic);

    public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        => new ReadOnlyCollection<T>(list);

    public static ReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> list)
        => list.ToList().AsReadOnly();

    [return: MaybeNull]
    public static T Find<T>(this ICollection<T> collection, Predicate<T> match) {
        if(match is null) {
            throw new ArgumentNullException(nameof(match));
        }

        foreach (var item in collection) {
            if (match(item)) return item;
        }

        return default(T)!;
    }
}