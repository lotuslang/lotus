using System.Collections.ObjectModel;

namespace Lotus.Extras.Graphs;

[DebuggerDisplay("{name} ({RootNodes.Count} root nodes)")]
public sealed class Graph
{
    /// <summary>
    /// The name of this graph.
    /// </summary>
    public string Name { get; set; }

    private readonly List<GraphNode> _rootNodes;
    /// <summary>
    /// The nodes that are the roots of independent trees.
    /// </summary>
    public ReadOnlyCollection<GraphNode> RootNodes => _rootNodes.AsReadOnly();

    public Dictionary<string, string> GraphProps { get; }

    public Dictionary<string, string> NodeProps { get; }

    public Dictionary<string, string> EdgeProps { get; }

    public Graph(string name) {
        Name = name;
        _rootNodes = new List<GraphNode>();
        GraphProps = new Dictionary<string, string>();
        NodeProps = new Dictionary<string, string>();
        EdgeProps = new Dictionary<string, string>();
    }

    /// <summary>
    /// Adds a new node to this Graph as a root of an independent tree.
    /// </summary>
    /// <param name="node">The GraphNode to add.</param>
    public void AddNode(GraphNode node)
        => _rootNodes.Add(node);

    /// <summary>
    /// A representation of this graph in the 'dot' language (https://www.graphviz.org/doc/info/lang.html).
    /// </summary>
    /// <returns>A single string of 'dot' representing this entire graph.</returns>
    public string ToText() {
        var sb = new IndentedStringBuilder();

        // Append the keyword 'digraph' followed by the name of the graph, followed, on a new line, by an opening curly bracket
        sb.Append("graph ").AppendLine(Name).Append('{');

        sb.Indent++;
        sb.AppendLine();

        foreach (var property in GraphProps) {
            sb.Append(property.Key).Append("=\"").Append(property.Value).Append('"');
            sb.AppendLine();
        }

        if (NodeProps.Count != 0) {
            sb.Append("node[");

            sb.Indent++;
            sb.AppendLine();

            foreach (var property in NodeProps) {
                sb.Append(property.Key).Append("=\"").Append(property.Value).Append('"');
                sb.AppendLine();
            }

            sb.Indent--;
            sb.AppendLine(']');
        }

        if (EdgeProps.Count != 0) {
            sb.Append("edge [");

            sb.Indent++;
            sb.AppendLine();

            foreach (var property in EdgeProps) {
                sb.Append(property.Key).Append("=\"").Append(property.Value).Append('"');
                sb.AppendLine();
            }

            sb.Indent--;
            sb.AppendLine(']');
        }

        // A list of GraphNode to keep track of visited nodes
        var registry = new HashSet<GraphNode>();

        foreach (var node in _rootNodes) {
            // If the node wasn't already processed
            if (registry.Add(node))
               node.AppendText(sb, registry);
        }

        sb.Indent--;

        // Close the graph by a closing curly bracket on a new line
        sb.AppendLine().Append('}')
          .AppendLine().Append("// ").Append(GetHashCode())
          .AppendLine();

        // Return the string builder
        return sb.ToString();
    }

    public void AddGraphProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        GraphProps[property] = value;
    }

    public void AddNodeProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        NodeProps[property] = value;
    }

    public void AddEdgeProp(string property, string value) {
        if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
            return;
        EdgeProps[property] = value;
    }

    public override int GetHashCode() => GetHashCode(true);

    public int GetHashCode(bool includeProps) {
        var code = new DeterministicHashCode();

        foreach (var node in _rootNodes) {
            code.Add(node, GraphNode.StructuralComparer);
        }

        if (includeProps) {
            foreach (var prop in GraphProps.Concat(NodeProps).Concat(EdgeProps)) {
                code.Add(DeterministicStringComparer.Instance.GetHashCode(prop.Key));
                code.Add(DeterministicStringComparer.Instance.GetHashCode(prop.Value));
            }
        }

        return code.ToHashCode();
    }
}