using NovoNordisk.CriticalPath.Exceptions;

namespace NovoNordisk.CriticalPath;

internal class Graph
{
    private ISet<Activity> Nodes { get; }
    private ISet<Tuple<Activity, Activity>> Edges { get; }

    private Graph(ISet<Activity> nodes, ISet<Tuple<Activity, Activity>> edges)
    {
        Nodes = nodes;
        Edges = edges;
    }
    
    public static Graph CreateFromActivities(ISet<Activity> activities)
    {
        var edges = new HashSet<Tuple<Activity, Activity>>();
        var visited = new HashSet<Activity>();
        
        foreach (var activity in activities)
        {
            AddEdgesFromActivity(activity, edges, visited);
        }

        return new Graph(activities, edges);
    }
    
    private static void AddEdgesFromActivity(Activity activity, ISet<Tuple<Activity, Activity>> edges, ISet<Activity> visited)
    {
        if (!visited.Add(activity))
        {
            return;
        }

        foreach (var dependency in activity.Dependencies)
        {
            edges.Add(new Tuple<Activity, Activity>(activity, dependency));
            AddEdgesFromActivity(dependency, edges, visited);
        }
    }
    
    internal IReadOnlyCollection<Activity> GetEdgesTo(Activity activity)
    {
        return Edges.Where(e => e.Item2.Equals(activity)).Select(e => e.Item1).ToList();
    }
    
    internal IReadOnlyCollection<Activity> GetEdgesFrom(Activity activity)
    {
        return Edges.Where(e => e.Item1.Equals(activity)).Select(e => e.Item2).ToList();
    }

    internal IReadOnlyCollection<Activity> GetStartingNodes()
    {
        return Nodes.Where(n => Edges.All(e => e.Item2.Equals(n) == false)).ToList();
    }
    
    internal IReadOnlyList<Activity> TopologicalSort()
    {
        var activities = new List<Activity>();
        var edges = new HashSet<Tuple<Activity, Activity>>(Edges);
        var startingNodes = new HashSet<Activity>(Nodes.Where(n => edges.All(e => e.Item2.Equals(n) == false)));
        
        while (startingNodes.Any())
        {
            var node = startingNodes.First();
            startingNodes.Remove(node);
            
            activities.Add(node);
            
            foreach (var edge in edges.Where(e => e.Item1.Equals(node)).ToList())
            {
                var activity = edge.Item2;
                edges.Remove(edge);
                
                if (edges.All(me => me.Item2.Equals(activity) == false))
                {
                    startingNodes.Add(activity);
                }
            }
        }
        
        if (edges.Any())
        {
            throw new CyclicDependencyException("There is a cyclic dependency in the graph. A critical path cannot be determined");
        }

        return activities;
    }
}