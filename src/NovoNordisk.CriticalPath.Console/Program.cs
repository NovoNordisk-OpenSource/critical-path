using NovoNordisk.CriticalPath;
using Activity = NovoNordisk.CriticalPath.Activity;

var criticalPathMethod = new CriticalPathMethod();

var activityEnd = new Activity(name: "Finish", cost: 0);
var activityC = new Activity(name: "C", cost: 90, dependencies: activityEnd);
var activityG = new Activity(name: "G", cost: 40, dependencies: activityEnd);
var activityF = new Activity(name: "F", cost: 20, dependencies: activityEnd);
var activityB = new Activity(name: "B", cost: 90, dependencies: activityC);
var activityE = new Activity(name: "E", cost: 20, dependencies: [activityG, activityF]);
var activityA = new Activity(name: "A", cost: 50, dependencies: activityB);
var activityD = new Activity(name: "D", cost: 100, dependencies: [activityB, activityE]);
var activityStart = new Activity(name: "Start", cost: 0, dependencies: [activityA, activityD]);

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
