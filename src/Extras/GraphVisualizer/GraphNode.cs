using System.Collections;
using System.Collections.ObjectModel;

namespace Lotus.Extras.Graphs;

[DebuggerDisplay("{ID}:{Name}")]
public sealed class GraphNode : IEnumerable<GraphNode>, IEquatable<GraphNode>
{
    /// <summary>
    /// The unique identifier for this node.
    /// </summary>
    public string ID { get; }
    private readonly int _numericID;

    /// <summary>
    /// The name of this node.
    /// </summary>
    public string Name { get; set; }

    private readonly List<GraphNode> _children;
    /// <summary>
    /// The children of this node, i.e. the nodes this node points to.
    /// </summary>
    public ReadOnlyCollection<GraphNode> Children => _children.AsReadOnly();

    public Dictionary<string, string> Properties { get; }

    public GraphNode(string name) : this(Random.Shared.Next(), name) { }

    public GraphNode(int id, string text) {
        _numericID = id;
        ID = _numericID.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        Name = text;
        Properties = new Dictionary<string, string>();
        _children = new List<GraphNode>();
    }

    /// <summary>
    /// Add a child to this node.
    /// </summary>
    /// <param name="node">The node to add as a child.</param>
    public GraphNode Add(GraphNode? node) {
        if (node is not null)
            _children.Add(node);

        return this;
    }

    public GraphNode SetProperty(string property, string value) {
        if (String.IsNullOrEmpty(property))
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

    public void AppendText(IndentedStringBuilder sb, HashSet<GraphNode> registry) {
        sb.AppendLine();

        // Declare the node: Append the id of the node, and set its label to `name`
        sb.Append(ID).Append(" [label=\"").Append(Name).Append('"');

        foreach (var property in Properties) {
            sb.Append(',').Append(property.Key).Append("=\"").Append(property.Value).Append('"');
        }

        sb.Append(']');
        sb.AppendLine();

        if (_children.Count == 0)
            return;

        sb.Indent++;

        // For each node that is a children of this object
        foreach (var child in _children) {
            sb.AppendLine();

            sb.Append(ID).Append(" -- ").Append(child.ID);

            // If this child wasn't already processed, then append its text
            if (registry.Add(child))
                child.AppendText(sb, registry);
        }

        sb.Indent--;
    }

    public override int GetHashCode() => _numericID;

    public IEnumerator<GraphNode> GetEnumerator()
        => Children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public override bool Equals(object? obj)
        => Equals(obj as GraphNode);

    public bool Equals(GraphNode? node)
        => node is not null && GetHashCode() == node.GetHashCode();

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
                code.Add(GetHashCode(node));
            }

            return code.ToHashCode();
        }
    }
}