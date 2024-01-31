namespace NovoNordisk.CriticalPath;

/// <summary>
/// Tarjan's Strongly Connected Components
/// https://en.wikipedia.org/wiki/Tarjan%27s_strongly_connected_components_algorithm
/// </summary>
internal class StronglyConnectedComponents(Graph graph)
{
    private int _index = 0;
    private Stack<Activity> _stack = new();
    private Dictionary<Activity, int> _indexMap = new();
    private Dictionary<Activity, int> _lowLinkMap = new();
    
    public IReadOnlyCollection<IReadOnlyCollection<Activity>> Find()
    {
        _index = 0;
        _stack = new Stack<Activity>();

        var result = new List<List<Activity>>();

        foreach (var node in graph.Nodes)
        {
            if (!_indexMap.ContainsKey(node))
            {
                result.AddRange(StrongConnect(node));
            }
        }
        
        return result;
    }
    
    private List<List<Activity>> StrongConnect(Activity node)
    {
        _indexMap.Add(node, _index);
        _lowLinkMap.Add(node, _index);
        _index++;
        _stack.Push(node);
        var result = new List<List<Activity>>();

        var successors = graph.Edges
            .Where(e => e.Item1 == node)
            .Select(e => e.Item2);

        foreach (var s in successors)
        {
            if (!_indexMap.ContainsKey(s))
            {
                result.AddRange(StrongConnect(s));
                _lowLinkMap[node] = Math.Min(_lowLinkMap[node], _lowLinkMap[s]);
            }
            else
            {
                if (_stack.Contains(s))
                {
                    _lowLinkMap[node] = Math.Min(_lowLinkMap[node], _lowLinkMap[s]);
                }
            }
        }

        if (_lowLinkMap[node] == _indexMap[node])
        {
            var sccList = new List<Activity>();
            while (true)
            {
                var n = _stack.Pop();
                sccList.Add(n);
                if (node == n) break;
            }

            if (sccList.Count > 1)
            {
                result.Add(sccList);
            }
        }
        
        return result;
    }
}