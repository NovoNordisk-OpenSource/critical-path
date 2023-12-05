# Critical Path Method
This is an implementation of the [Critical Path Method](https://hbr.org/1963/09/the-abcs-of-the-critical-path-method). 

# Usage
## Examples
See the `CriticalPath.Console` and the `NovoNordisk.CriticalPath.Tests` projects for working examples.

In general, create a `HashSet` of activities and use it as an argument to the `Execute(...)` function in the `CriticalPathMethod` object:

```C#
// Create activities
var activityEnd = new Activity("Finish", 0);
var activityC = new Activity("C", 90, activityEnd);
var activityG = new Activity("G", 40, activityEnd);
var activityF = new Activity("F", 20, activityEnd);
var activityB = new Activity("B", 90, activityC);
var activityE = new Activity("E", 20, activityG, activityF);
var activityA = new Activity("A", 50, activityB);
var activityD = new Activity("D", 100, activityB, activityE);
var activityStart = new Activity("Start", 0, activityA, activityD);

// Add activities to HashSet
var activities = new HashSet<Activity>
{
    activityEnd, activityC, activityG, activityF, activityB,
    activityE, activityA, activityD, activityStart,
};

// Calculate critical path
var criticalPathMethod = new CriticalPathMethod();
var criticalPath = criticalPathMethod.Execute(activities);
```

## Consume nuget Package in Other Projects
The Critical Path Method is distributed in a nuget package from the [Digital Foundation Nuget feed](https://novonordiskit.visualstudio.com/Digital%20Foundation/_artifacts/feed/Foundation_nuget). 

To consume from this feed make sure that the following package source is added to either Visual Studio or the given solution.

https://pkgs.dev.azure.com/novonordiskit/_packaging/Foundation_nuget%40Local/nuget/v3/index.json

This can be done in two ways:

1. from Visual Studio -> Tools -> Options... -> Nuget Package Manager.
2. Create a NuGet.config in your solution or project root folder. Example:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
	<add key="Digital Foundation Nuget" value="https://pkgs.dev.azure.com/novonordiskit/_packaging/Foundation_nuget%40Local/nuget/v3/index.json" />
  </packageSources>
</configuration>
```

Option 2 is preferred if you want to share the settings with the rest of the team and the build pipeline.

Your build pipeline agents should already have permission to read the feed. All you have to do is to make sure that you turned off "Limit job authorization scope to current project for non-release pipelines" 
in ADO -> Project Settings -> Pipelines Settings

# Notes
## Equal Critical Paths
If the graph contains two equal critical paths, and therefore both have a total float of 0, then the 
algorithm will return the first path in the given graph. It will not return both.

## Two Paths That Does Not Intersect
If the graph contains two paths that does not intersect, then they'll both have a total float of 0.
In that case the algorithm will return the first path in the given graph. It will not return both.

One could argue that it should return the one with the highest total cost, but this is not implemented. 
If this is the desired functionality then we should modify `CriticalActivities()` where the initial 
activity is found: `initialActivities.First(_ => _.TotalFloat == 0);`. 

## Free Float for the Final Task
We define this as 0. You could argue that it is infinite.

# How to Contribute
## Branching Strategy
Trunk based branching strategy is used. New features are added by creating feature branches that are then merged to main with a pull request. Pull requests requires the build pipeline to pass. Releases are done from release branches.

## Versioning
The nuget package must follow [semver.org](https://www.semver.org).

## Release Procedure
1. Create a release branch from main. Naiming convention is `release/x.y.z`.
1. Run the Publish pipeline from the given branch and give the version number as parameter to the pipeline.
1. The nuget package is stored in the Digital Foundation nuget feed.

# References
Based on:
* https://www.workamajig.com/blog/critical-path-method
* https://stackoverflow.com/a/9774655/2787333
* https://stackoverflow.com/questions/2985317/critical-path-method-algorithm
* https://www.codeproject.com/Articles/25312/Critical-Path-Method-Implementation-in-C
* https://github.com/elerch/Critical-Path-Extension-Method-for-.NET
