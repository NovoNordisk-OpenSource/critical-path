namespace NovoNordisk.CriticalPath;

/// <summary>
/// An activity is a node in the flow of things that should be done. 
/// </summary>
public class Activity
{
    /// <summary>
    /// Id of the activity
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Name of the activity
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Cost of the activity.
    /// </summary>
    public int Cost { get; set; }

    /// <summary>
    /// The sum of costs for the most expensive path, including this activity
    /// </summary>
    public int CriticalCost { get; set; }

    /// <summary>
    /// The earliest start
    /// </summary>
    public int EarlyStart { get; set; }

    /// <summary>
    /// The earliest finish
    /// </summary>
    public int EarlyFinish { get; set; }

    /// <summary>
    /// The latest start
    /// </summary>
    public int LatestStart { get; set; }

    /// <summary>
    /// The latest finish
    /// </summary>
    public int LatestFinish { get; set; }

    /// <summary>
    /// The cost/slack of this action if the critical path is not followed. If this is 0 then this action is on the critical path
    /// </summary>
    public int TotalFloat => LatestStart - EarlyStart;

    /// <summary>
    /// The free cost/slack. If there are no successors then we define this as 0
    /// </summary>
    public int FreeFloat => Dependencies.Any() ? Dependencies.Select(_ => _.EarlyStart).Min() - EarlyFinish : 0;

    /// <summary>
    /// The activity successors
    /// </summary>
    public HashSet<Activity> Dependencies { get; set; }

    /// <summary>
    /// Create an activity and specify its dependencies. All other properties in this activity will be set during the calculation of the critical path.
    /// </summary>
    /// <param name="name">The name of the activity</param>
    /// <param name="cost">The cost of the activity</param>
    /// <param name="dependencies">The successors of this activity</param>
    public Activity(string name, int cost, params Activity[] dependencies)
    {
        Cost = cost;
        Name = name;
        Dependencies = new HashSet<Activity>(dependencies);
    }
}