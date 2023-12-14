using NovoNordisk.CriticalPath.Exceptions;

namespace NovoNordisk.CriticalPath;

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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Should be none-static so users can mock the usage in tests.")]
    public List<Activity> Execute(HashSet<Activity> activities)
    {
        CalculateCriticalCost(activities);
        CalculateLatest(activities);
        var initialActivities = ActivitiesWithNoPredecessors(activities);
        CalculateEarly(initialActivities);
        var criticalActivities = CriticalActivities(initialActivities);
        return criticalActivities;
    }

    // TODO: This could probably be re-written to a recursive function so we could get rid of the while loop
    // If there is a loop in the graph then this will keep looping forever. But we checked this earlier.
    private static List<Activity> CriticalActivities(HashSet<Activity> initialActivities)
    {
        var criticalActivities = new List<Activity>();

        var nextActivity = initialActivities.First(_ => _.TotalFloat == 0);
        while (nextActivity != null)
        {
            criticalActivities.Add(nextActivity);
            nextActivity = nextActivity.Dependencies.FirstOrDefault(_ => _.TotalFloat == 0);
        }

        return criticalActivities;
    }

    private static void CalculateCriticalCost(HashSet<Activity> activities)
    {
        var completed = new HashSet<Activity>(); // Activities whose critical cost has been calculated
        var remaining = new HashSet<Activity>(activities); // Activities whose critical cost needs to be calculated

        while (remaining.Any())
        {
            var progress = false;
            foreach (var remainingActivity in remaining)
            {
                //If all dependencies are completed (have a critical cost calculated) then we can complete this activity too
                var dependenciesNotCompleted = remainingActivity.Dependencies.Where(_ => !completed.Contains(_));
                if (dependenciesNotCompleted.Any())
                {
                    //Not all dependencies are completed, so move on to the next activity
                    continue;
                }

                var critical = 0;
                foreach (var remainingActivityDependency in remainingActivity.Dependencies)
                {
                    if (remainingActivityDependency.CriticalCost > critical)
                    {
                        critical = remainingActivityDependency.CriticalCost;
                    }
                }

                remainingActivity.CriticalCost = critical + remainingActivity.Cost;

                completed.Add(remainingActivity);
                remaining.Remove(remainingActivity);
                progress = true;
            }

            // If we haven't made any progress then a cycle must exist in the graph and we wont be able to calculate the critical path
            if (!progress)
            {
                throw new CyclicDependencyException(
                    "There is a cyclic dependency in the graph. A critical path cannot be determined");
            }
        }
    }

    private static void CalculateLatest(HashSet<Activity> activities)
    {
        var criticalPathMaxCost = activities.Select(activity => activity.CriticalCost).Max();
        foreach (var activity in activities)
        {
            activity.LatestStart = criticalPathMaxCost - activity.CriticalCost;
            activity.LatestFinish = activity.LatestStart + activity.Cost;
        }
    }

    private static HashSet<Activity> ActivitiesWithNoPredecessors(IReadOnlyCollection<Activity> activities)
    {
        var allSuccessors = activities.SelectMany(_ => _.Dependencies).Distinct();

        var activitiesWithNoPredecessors = new HashSet<Activity>(activities);
        activitiesWithNoPredecessors.ExceptWith(allSuccessors);

        return activitiesWithNoPredecessors;
    }

    private static void CalculateEarly(HashSet<Activity> initialActivities)
    {
        foreach (var initialActivity in initialActivities)
        {
            initialActivity.EarlyStart = 0;
            initialActivity.EarlyFinish = initialActivity.Cost;
            SetEarly(initialActivity);
        }
    }

    private static void SetEarly(Activity activity)
    {
        var completionTime = activity.EarlyFinish;
        foreach (var activityDependency in activity.Dependencies)
        {
            if (completionTime >= activityDependency.EarlyStart)
            {
                activityDependency.EarlyStart = completionTime;
                activityDependency.EarlyFinish = completionTime + activityDependency.Cost;
            }
            SetEarly(activityDependency);
        }
    }
}