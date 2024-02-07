using NovoNordisk.CriticalPath.Exceptions;

namespace NovoNordisk.CriticalPath;

internal class Graph
{
    private readonly ISet<Activity> _nodes;
    private readonly Dictionary<Activity, List<Activity>> _edgesFrom;
    private readonly Dictionary<Activity, List<Activity>> _edgesTo;
    
    private Graph(ISet<Activity> nodes, Dictionary<Activity, List<Activity>> edgesFrom, Dictionary<Activity, List<Activity>> edgesTo)
    {
        _nodes = nodes;
        _edgesFrom = edgesFrom;
        _edgesTo = edgesTo;
    }
    
    public static Graph CreateFromActivities(ISet<Activity> activities)
    {
        var edgesFrom = new Dictionary<Activity, List<Activity>>();
        var edgesTo = new Dictionary<Activity, List<Activity>>();
        var visited = new HashSet<Activity>();
        
        foreach (var activity in activities)
        {
            AddEdgesFromActivity(activity, edgesFrom, edgesTo, visited);
        }

        return new Graph(activities, edgesFrom, edgesTo);
    }
    
    private static void AddEdgesFromActivity(Activity activity, Dictionary<Activity, List<Activity>> edgesFrom, Dictionary<Activity, List<Activity>> edgesTo, ISet<Activity> visited)
    {
        if (!visited.Add(activity))
        {
            return;
        }

        foreach (var dependency in activity.Dependencies)
        {
            if (!edgesFrom.ContainsKey(activity))
            {
                edgesFrom.Add(activity, []);
            }
            
            if (!edgesTo.ContainsKey(dependency))
            {
                edgesTo.Add(dependency, []);
            }
            
            edgesFrom[activity].Add(dependency);
            edgesTo[dependency].Add(activity);
            AddEdgesFromActivity(dependency, edgesFrom, edgesTo, visited);
        }
    }
    
    internal IReadOnlyCollection<Activity> GetEdgesTo(Activity activity)
    {
        return _edgesTo.TryGetValue(activity, out var result) ? result : [];
    }
    
    internal IReadOnlyCollection<Activity> GetEdgesFrom(Activity activity)
    {
        return _edgesFrom.TryGetValue(activity, out var result) ? result : [];
    }

    internal IReadOnlyCollection<Activity> GetStartingNodes()
    {
        return _nodes.Where(n => !_edgesTo.ContainsKey(n)).ToList();
    }
    
    internal IReadOnlyList<Activity> TopologicalSort()
    {
        var activities = new List<Activity>();
        var edgesFrom = _edgesFrom.ToDictionary(entry => entry.Key, entry => new List<Activity>(entry.Value));
        var edgesTo = _edgesTo.ToDictionary(entry => entry.Key, entry => new List<Activity>(entry.Value));
        var startingNodes = new List<Activity>(GetStartingNodes());
        
        while (startingNodes.Any())
        {
            var node = startingNodes.First();
            startingNodes.Remove(node);
            
            activities.Add(node);
            
            if (!edgesFrom.TryGetValue(node, out var activitiesEdgeFromNode))
            {
                continue;
            }
            
            foreach (var activity in activitiesEdgeFromNode.ToList())
            {
                edgesFrom[node].Remove(activity);
                if (edgesFrom[node].Count <= 0)
                {
                    edgesFrom.Remove(node);
                }
                
                edgesTo[activity].Remove(node);
                if (edgesTo[activity].Count <= 0)
                {
                    edgesTo.Remove(activity);
                }
                
                if (!edgesTo.ContainsKey(activity) || edgesTo[activity].Count <= 0)
                {
                    startingNodes.Add(activity);
                }
            }
        }
        
        if (edgesFrom.Any())
        {
            throw new CyclicDependencyException("There is a cyclic dependency in the graph. A critical path cannot be determined");
        }

        return activities;
    }
}