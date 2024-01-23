using NovoNordisk.CriticalPath.Exceptions;

namespace NovoNordisk.CriticalPath;

/// <summary>
/// Interface for the Critical Path Method.
/// </summary>
public interface ICriticalPathMethod
{
    /// <summary>
    /// This function calculates the given activities properties and finds the critical path through the dependency graph of the activities.
    /// </summary>
    /// <param name="activities">List of activities. Note that the HashSet is call-by-reference, so its properties will be modified by this function.</param>
    /// <returns>The activities on the critical path, ordered by their dependencies.</returns>
    List<Activity> Execute(HashSet<Activity> activities);
}   

/// <summary>
/// Implementation of the Critical Path Method.
/// </summary>
public class CriticalPathMethod : ICriticalPathMethod
{
    /// <summary>
    /// This function calculates the given activities properties and finds the critical path through the dependency graph of the activities.
    /// </summary>
    /// <param name="activities">List of activities. Note that the HashSet is call-by-reference, so its properties will be modified by this function.</param>
    /// <returns>The activities on the critical path, ordered by their dependencies.</returns>
    public List<Activity> Execute(HashSet<Activity> activities)
    {
        var edges = GetEdges(activities);
        var sortedActivities = TopologicalSort(activities, edges);
        CalculateCosts(sortedActivities, edges);
        var criticalActivities = CriticalActivities(activities, edges);

        return criticalActivities;
    }
    
    private static HashSet<Tuple<Activity, Activity>> GetEdges(HashSet<Activity> activities)
    {
        var edges = new HashSet<Tuple<Activity, Activity>>();
        var visited = new HashSet<Activity>();
        
        foreach (var activity in activities)
        {
            AddEdgesFromActivity(activity, edges, visited);
        }
        
        return edges;
    }

    private static void AddEdgesFromActivity(Activity activity, HashSet<Tuple<Activity, Activity>> edges, HashSet<Activity> visited)
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
    
    private static List<Activity> TopologicalSort(HashSet<Activity> nodes, HashSet<Tuple<Activity, Activity>> inputEdges)
    {
        var activities = new List<Activity>();
        var edges = new HashSet<Tuple<Activity, Activity>>(inputEdges);
        var startingNodes = new HashSet<Activity>(nodes.Where(n => edges.All(e => e.Item2.Equals(n) == false)));
        
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

    private static void CalculateCosts(IList<Activity> activities, HashSet<Tuple<Activity, Activity>> edges)
    {
        foreach (var activity in activities)
        {
            var edgesToActivity = edges.Where(e => e.Item2.Equals(activity)).ToList();
            
            var completionTime = edgesToActivity
                .Select(e => e.Item1.EarlyFinish)
                .DefaultIfEmpty(0)
                .Max();
            
            activity.EarlyStart = completionTime;
            activity.EarlyFinish = completionTime + activity.Cost;
        }

        foreach (var activity in activities.Reverse())
        {
            var critical = activity.Dependencies
                .Select(remainingActivityDependency => remainingActivityDependency.CriticalCost)
                .DefaultIfEmpty(0)
                .Max();

            activity.CriticalCost = critical + activity.Cost;
        }
        
        var criticalPathMaxCost = activities.Select(activity => activity.CriticalCost).Max();
        foreach (var activity in activities.Reverse())
        {
            var edgesFromActivity = edges.Where(e => e.Item1.Equals(activity)).ToList();

            var latestFinish = edgesFromActivity
                .Select(e => e.Item2.LatestStart)
                .DefaultIfEmpty(criticalPathMaxCost)
                .Min();

            activity.LatestFinish = latestFinish;
            activity.LatestStart = latestFinish - activity.Cost;
        }
    }
    
    private static List<Activity> CriticalActivities(HashSet<Activity> nodes, HashSet<Tuple<Activity, Activity>> edges)
    {
        var startingNodes = new HashSet<Activity>(nodes.Where(n => edges.All(e => e.Item2.Equals(n) == false)));
        var criticalActivities = new List<Activity>();

        var nextActivity = startingNodes.First(n => n.TotalFloat == 0);
        while (nextActivity != null)
        {
            criticalActivities.Add(nextActivity);
            nextActivity = nextActivity.Dependencies.FirstOrDefault(n => n.TotalFloat == 0);
        }

        return criticalActivities;
    }
}