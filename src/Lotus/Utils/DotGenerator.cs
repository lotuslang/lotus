using System.Text;
using System.Collections;

namespace Lotus.Utils;

[DebuggerDisplay("{name} ({RootNodes.Count} root nodes)")]
public sealed class Graph
{
    private readonly string name;

    /// <summary>
    /// The name of this graph.
    /// </summary>
    /// <value>A string that represents the name of this graph.</value>
    public ref readonly string Name => ref name;

    private readonly ImmutableArray<GraphNode>.Builder rootNodes;

    /// <summary>
    /// The nodes that are the roots of independent trees.
    /// </summary>
    /// <value>A List of GraphNode containing every root node.</value>
    public ImmutableArray<GraphNode> RootNodes => rootNodes.ToImmutable();

    private readonly Dictionary<string, string> graphProps;

    public ref readonly Dictionary<string, string> GraphProps => ref graphProps;

    private readonly Dictionary<string, string> nodeProps;

    public ref readonly Dictionary<string, string> NodeProps => ref nodeProps;

    private readonly Dictionary<string, string> edgeProps;

    public ref readonly Dictionary<string, string> EdgeProps => ref edgeProps;

    public Graph(string name) {
        this.name = name;
        rootNodes = ImmutableArray.CreateBuilder<GraphNode>();
        graphProps = new Dictionary<string, string>();
        nodeProps = new Dictionary<string, string>();
        edgeProps = new Dictionary<string, string>();
    }

    /// <summary>
    /// Adds a new node to this Graph as a root of an independent tree.
    /// </summary>
    /// <param name="node">The GraphNode to add.</param>
    public void AddNode(GraphNode node)
        => rootNodes.Add(node);

    /// <summary>
    /// A representation of this graph in the 'dot' language (https://www.graphviz.org/doc/info/lang.html).
    /// </summary>
    /// <returns>A single string of 'dot' representing this entire graph.</returns>
    public string ToText() {
        // Create a new string builder
        var strBuilder = new StringBuilder();

        // Append the keyword 'digraph' followed by the name of the graph, followed, on a new line, by an opening curly bracket
        strBuilder.Append("graph ").AppendLine(name);
        strBuilder.AppendLine("{");

        foreach (var property in graphProps) {
            strBuilder.Append('\t').Append(property.Key).Append("=\"").Append(property.Value).AppendLine("\"");
        }

        if (nodeProps.Count != 0) {
            strBuilder.AppendLine("\tnode[");

            foreach (var property in nodeProps) {
                strBuilder.Append("\t\t").Append(property.Key).Append("=\"").Append(property.Value).AppendLine("\"");
            }

            strBuilder.AppendLine("\t]\n");
        }

        if (edgeProps.Count != 0) {
            strBuilder.AppendLine("\tedge [");

            foreach (var property in edgeProps) {
                strBuilder.Append("\t\t").Append(property.Key).Append("=\"").Append(property.Value).AppendLine("\"");
            }

            strBuilder.AppendLine("\t]");
        }

        strBuilder.AppendLine();

        // A list of GraphNode to keep track of visited nodes
        var registry = new HashSet<GraphNode>();

        // For each independent tree in this graph
        foreach (var node in rootNodes) {
            // Add the root node to the registry
            _ = registry.Add(node);

            // Append the 'dot' representation of this root node to the graph's representation
            strBuilder.Append('\t').AppendLine(node.ToText(registry));
        }

        // Close the graph by a closing curly bracket on a new line
        strBuilder.Append("\n}\n//").Append(GetHashCode()).AppendLine();

        // Return the string builder
        return strBuilder.ToString();
    }

    public void AddGraphProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        graphProps[property] = value;
    }

    public void AddNodeProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        nodeProps[property] = value;
    }

    public void AddEdgeProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        edgeProps[property] = value;
    }

    public override int GetHashCode() => GetHashCode(true);

    public int GetHashCode(bool includeProps) {
        var code = new DeterministicHashCode();

        foreach (var node in rootNodes) {
            code.Add(node, GraphNode.StructuralComparer);
        }

        if (includeProps) {
            foreach (var props in new[] { GraphProps, NodeProps, EdgeProps }) {
                foreach (var prop in props) {
                    code.Add(DeterministicStringComparer.Instance.GetHashCode(prop.Key));
                    code.Add(DeterministicStringComparer.Instance.GetHashCode(prop.Value));
                }
            }
        }

        return code.ToHashCode();
    }
}

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
            strBuilder.Append(',').Append(property.Key).Append("=\"").Append(property.Value).Append('\"');
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

public struct DeterministicHashCode
{
    private int current;

    private static readonly DeterministicStringComparer stringComparer = DeterministicStringComparer.Instance;

    public void Add<T1>(T1 value) => current = (current * 21) + value!.GetHashCode();

    public void Add<T1>(T1 value, IEqualityComparer<T1> eq) => current = (current * 21) + eq.GetHashCode(value!);

    public static int Combine<T1>(T1 t1, string s) => (t1!.GetHashCode() * 21) + stringComparer.GetHashCode(s);

    public static int Combine<T1>(string s, T1 t1) => (stringComparer.GetHashCode(s) * 21) + t1!.GetHashCode();

    public static int Combine<T1, T2>(T1 t1, T2 t2) => (t1!.GetHashCode() * 21) + t2!.GetHashCode();

    public int ToHashCode() => current;
}