using System.Text;
using System.Collections;

namespace Lotus.Extras.Graphs;

[DebuggerDisplay("{ID}:{Name}")]
public sealed class GraphNode : IEnumerable<GraphNode>, IEquatable<GraphNode>
{
    /// <summary>
    /// The unique identifier for this node.
    /// </summary>
    /// <value>A string representing the ID of this node.</value>
    public int ID { get; }
    private readonly string _stringID;

    /// <summary>
    /// The name of this node.
    /// </summary>
    /// <value>A string representing the name of this node.</value>
    public string Name { get; set; }

    /// <summary>
    /// The children of this node, i.e. the nodes this node points to.
    /// </summary>
    /// <value>A list of GraphNode this node is pointing to, (i.e. children).</value>
    public ImmutableArray<GraphNode>.Builder Children { get; }

    public Dictionary<string, string> Properties { get; }

    public GraphNode(string name) : this(new Random().Next(), name) { }

    public GraphNode(int id, string text) {
        ID = id;
        _stringID = ID.ToString();
        Name = text;
        Properties = new Dictionary<string, string>();
        Children = ImmutableArray.CreateBuilder<GraphNode>();
    }

    /// <summary>
    /// Add a child to this node.
    /// </summary>
    /// <param name="node">The node to add as a child.</param>
    public GraphNode Add(GraphNode node) {
        Children.Add(node);

        return this;
    }

    public GraphNode SetProperty(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return this;

        if (!Properties.TryAdd(property, value)) {
            Properties[property] = value;
        }

        return this;
    }

    public GraphNode SetColor(string color)
        => SetProperty("color", color);

    public GraphNode SetTooltip(string tooltipText)
        => SetProperty("tooltip", tooltipText);

    public GraphNode SetName(string name) {
        Name = name;
        return this;
    }

    public string ToText(HashSet<GraphNode> registry, int tabs = 2) {
        // Create a new string builder
        var strBuilder = new StringBuilder();

        // Declare the node : Append the id of the node, and set its label to `name`
        strBuilder.AppendLine().Append(new string('\t', tabs - 1)).Append(_stringID).Append(" [label=\"").Append(Name).Append('"');

        foreach (var property in Properties) {
            strBuilder.Append(',').Append(property.Key).Append("=\"").Append(property.Value).Append('"');
        }

        strBuilder.AppendLine("]");

        if (Children.Count == 0) return strBuilder.ToString();

        // For each node that is a children of this object
        foreach (var child in Children) {
            strBuilder.Append(new string('\t', tabs)).Append(_stringID).Append(" -- ").Append(child._stringID);

            // If this child wasn't aready processed, then append its text
            if (registry.Add(child))
                strBuilder.Append(new string('\t', tabs)).Append(child.ToText(registry, tabs + 1));
        }

        // Return the string builder
        return strBuilder.AppendLine().ToString();
    }

    public override int GetHashCode() => ID;

    public IEnumerator<GraphNode> GetEnumerator()
        => Children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public override bool Equals(object? obj)
        => Equals(obj as GraphNode);

    public bool Equals(GraphNode? node)
        => StructuralComparer.Equals(this, node);

    public static IEqualityComparer<GraphNode> IDComparer => EqualityComparer<GraphNode>.Default;
    public static IEqualityComparer<GraphNode> StructuralComparer => EqComparer.Instance;

    private sealed class EqComparer : EqualityComparer<GraphNode>
    {
        public static EqComparer Instance { get; } = new();

        private EqComparer() { }

        public override bool Equals(GraphNode? n1, GraphNode? n2)
            => n1 is null
                ? n2 is null
                : n2 is not null && GetHashCode(n1) == GetHashCode(n2);

        public override int GetHashCode(GraphNode n) {
            var code = new DeterministicHashCode();

            code.Add(n.Name, DeterministicStringComparer.Instance);

            foreach (var node in n.Children) {
                code.Add(node, StructuralComparer);
            }

            return code.ToHashCode();
        }
    }
}