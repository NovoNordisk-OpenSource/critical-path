using NovoNordisk.CriticalPath;
using Activity = NovoNordisk.CriticalPath.Activity;

var criticalPathMethod = new CriticalPathMethod();

var activityEnd = new Activity("Finish", 0);
var activityC = new Activity("C", 90, activityEnd);
var activityG = new Activity("G", 40, activityEnd);
var activityF = new Activity("F", 20, activityEnd);
var activityB = new Activity("B", 90, activityC);
var activityE = new Activity("E", 20, activityG, activityF);
var activityA = new Activity("A", 50, activityB);
var activityD = new Activity("D", 100, activityB, activityE);
var activityStart = new Activity("Start", 0, activityA, activityD);

var activities = new HashSet<Activity>
{
    activityEnd,
    activityC,
    activityG,
    activityF,
    activityB,
    activityE,
    activityA,
    activityD,
    activityStart,
};

Console.WriteLine("Activities:");
PrintActivities(activities);

var criticalPath = criticalPathMethod.Execute(activities);

Console.WriteLine("Activities after calculations:");
PrintActivities(activities);

Console.WriteLine("Critical Activities:");
PrintActivities(criticalPath);

void PrintActivities(IEnumerable<Activity> activitiesToPrint)
{
    Console.WriteLine("Name\tCost\tCC\tTF\tFF\tES\tEF\tLS\tLF\tDependencies");
    foreach (var activity in activitiesToPrint)
    {
        Console.WriteLine($"{activity.Name}\t{activity.Cost}\t{activity.CriticalCost}\t{activity.TotalFloat}\t{activity.FreeFloat}\t" +
                          $"{activity.EarlyStart}\t{activity.EarlyFinish}\t{activity.LatestStart}\t{activity.LatestFinish}\t" +
                          $"{string.Join(", ", activity.Dependencies.Select(_ => _.Name))}");
    }
}
