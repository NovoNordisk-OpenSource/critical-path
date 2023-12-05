namespace NovoNordisk.CriticalPath.Tests;

public class CriticalPathTests
{
    [Fact]
    public void Should_Calculate_Costs()
    {
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

        var criticalPathMethod = new CriticalPathMethod();
        criticalPathMethod.Execute(activities);

        activities.Should().ContainSingle(_ => _.Name!.Equals("Finish")).Which.Should().Match<Activity>(_ => 
            _.Cost == 0 &&
            _.CriticalCost == 0 &&
            _.EarlyStart == 280 &&
            _.EarlyFinish == 280 &&
            _.LatestStart == 280 &&
            _.LatestFinish == 280 &&
            _.TotalFloat == 0 &&
            _.FreeFloat == 0

        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("C")).Which.Should().Match<Activity>(_ =>
            _.Cost == 90 &&
            _.CriticalCost == 90 &&
            _.EarlyStart == 190 &&
            _.EarlyFinish == 280 &&
            _.LatestStart == 190 &&
            _.LatestFinish == 280 &&
            _.TotalFloat == 0 &&
            _.FreeFloat == 0
        );
        
        activities.Should().ContainSingle(_ => _.Name!.Equals("G")).Which.Should().Match<Activity>(_ =>
            _.Cost == 40 &&
            _.CriticalCost == 40 &&
            _.EarlyStart == 120 &&
            _.EarlyFinish == 160 &&
            _.LatestStart == 240 &&
            _.LatestFinish == 280 &&
            _.TotalFloat == 120 &&
            _.FreeFloat == 120
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("F")).Which.Should().Match<Activity>(_ =>
            _.Cost == 20 &&
            _.CriticalCost == 20 &&
            _.EarlyStart == 120 &&
            _.EarlyFinish == 140 &&
            _.LatestStart == 260 &&
            _.LatestFinish == 280 &&
            _.TotalFloat == 140 &&
            _.FreeFloat == 140
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("B")).Which.Should().Match<Activity>(_ =>
            _.Cost == 90 &&
            _.CriticalCost == 180 &&
            _.EarlyStart == 100 &&
            _.EarlyFinish == 190 &&
            _.LatestStart == 100 &&
            _.LatestFinish == 190 &&
            _.TotalFloat == 0 &&
            _.FreeFloat == 0
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("E")).Which.Should().Match<Activity>(_ =>
            _.Cost == 20 &&
            _.CriticalCost == 60 &&
            _.EarlyStart == 100 &&
            _.EarlyFinish == 120 &&
            _.LatestStart == 220 &&
            _.LatestFinish == 240 &&
            _.TotalFloat == 120 &&
            _.FreeFloat == 0
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("A")).Which.Should().Match<Activity>(_ =>
            _.Cost == 50 &&
            _.CriticalCost == 230 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 50 &&
            _.LatestStart == 50 &&
            _.LatestFinish == 100 &&
            _.TotalFloat == 50 &&
            _.FreeFloat == 50
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("D")).Which.Should().Match<Activity>(_ =>
            _.Cost == 100 &&
            _.CriticalCost == 280 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 100 &&
            _.LatestStart == 0 &&
            _.LatestFinish == 100 &&
            _.TotalFloat == 0 &&
            _.FreeFloat == 0
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("Start")).Which.Should().Match<Activity>(_ =>
            _.Cost == 0 &&
            _.CriticalCost == 280 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 0 &&
            _.LatestStart == 0 &&
            _.LatestFinish == 0 &&
            _.TotalFloat == 0 &&
            _.FreeFloat == 0
        );
    }

    [Fact]
    public void Should_Return_Critical_Path()
    {
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

        var criticalPathMethod = new CriticalPathMethod();
        var criticalPath = criticalPathMethod.Execute(activities);

        criticalPath.Should().HaveCount(5);
        criticalPath[0].Should().Be(activityStart);
        criticalPath[1].Should().Be(activityD);
        criticalPath[2].Should().Be(activityB);
        criticalPath[3].Should().Be(activityC);
        criticalPath[4].Should().Be(activityEnd);
    }

    [Fact]
    public void Should_Fail_On_Cyclic_Dependency()
    {
        var activityEnd = new Activity("Finish", 0);
        var activityC = new Activity("C", 20, activityEnd);
        var activityB = new Activity("B", 40, activityC);
        var activityA = new Activity("A", 90, activityB);
        var activityStart = new Activity("Start", 0, activityA);
        activityC.Dependencies.Add(activityA);

        var activities = new HashSet<Activity>
        {
            activityEnd,
            activityC,
            activityB,
            activityA,
            activityStart,
        };

        var criticalPathMethod = new CriticalPathMethod();
        FluentActions.Invoking( () => criticalPathMethod.Execute(activities)).Should().Throw<CyclicDependencyException>();
    }

    // If there are two equal critical paths then both will have a total float of 0.
    // In this scenario the implementation will follow the first option in the list of activities given
    [Fact]
    public void Should_Handle_Two_Equal_Paths()
    {
        var activityEnd = new Activity("Finish", 0);
        var activityD = new Activity("D", 20, activityEnd);
        var activityC = new Activity("C", 10, activityD);
        var activityB = new Activity("B", 20, activityEnd);
        var activityA = new Activity("A", 10, activityB);
        var activityStart = new Activity("Start", 0, activityA, activityC);

        var activities = new HashSet<Activity>
        {
            activityEnd,
            activityA,
            activityB,
            activityC,
            activityD,
            activityStart,
        };

        var criticalPathMethod = new CriticalPathMethod();
        var criticalPath = criticalPathMethod.Execute(activities);

        criticalPath.Should().HaveCount(4);
        criticalPath[0].Should().Be(activityStart);
        criticalPath[1].Should().Be(activityA);
        criticalPath[2].Should().Be(activityB);
        criticalPath[3].Should().Be(activityEnd);
    }

    // If there are two paths that does not intersect, then both will have a total float of 0.
    // In this scenario the implementation will follow the first option in the list of activities given
    [Fact]
    public void Should_Handle_Two_Unrelated_Paths()
    {
        var activityEndA = new Activity("FinishAB", 0);
        var activityEndC = new Activity("FinishCD", 0);
        var activityD = new Activity("D", 20, activityEndC);
        var activityC = new Activity("C", 10, activityD);
        var activityB = new Activity("B", 20, activityEndA);
        var activityA = new Activity("A", 10, activityB);
        var activityStartA = new Activity("StartAB", 0, activityA);
        var activityStartC = new Activity("StartCD", 0, activityC);

        var activities = new HashSet<Activity>
        {
            activityEndA,
            activityEndC,
            activityA,
            activityB,
            activityC,
            activityD,
            activityStartA,
            activityStartC,
        };

        var criticalPathMethod = new CriticalPathMethod();
        var criticalPath = criticalPathMethod.Execute(activities);

        criticalPath.Should().HaveCount(4);
        criticalPath[0].Should().Be(activityStartA);
        criticalPath[1].Should().Be(activityA);
        criticalPath[2].Should().Be(activityB);
        criticalPath[3].Should().Be(activityEndA);
    }

    [Fact]
    public void Should_Ignore_Input_Order()
    {
        var activityEnd = new Activity("Finish", 0);
        var activityD = new Activity("D", 10, activityEnd);
        var activityC = new Activity("C", 20, activityD);
        var activityB = new Activity("B", 30, activityEnd);
        var activityA = new Activity("A", 40, activityB);
        var activityStart = new Activity("Start", 0, activityA, activityC);

        var activitiesOrderA = new HashSet<Activity> { activityEnd, activityA, activityB, activityC, activityD, activityStart};
        var activitiesOrderB = new HashSet<Activity> { activityEnd, activityD, activityC, activityB, activityA, activityStart };

        var criticalPathMethod = new CriticalPathMethod();
        var criticalPathA = criticalPathMethod.Execute(activitiesOrderA);
        var criticalPathB = criticalPathMethod.Execute(activitiesOrderB);

        criticalPathA.Should().Equal(criticalPathB);
    }

    [Fact]
    public void Should_Handle_Two_Start_Activities()
    {
        var activityEnd = new Activity("Finish", 0);
        var activityC = new Activity("C", 60, activityEnd);
        var activityB = new Activity("B", 50, activityC);
        var activityA = new Activity("A", 30, activityC);
        var activityStartA = new Activity("StartA", 0, activityA);
        var activityStartB = new Activity("StartB", 0, activityB);

        var activities = new HashSet<Activity>
        {
            activityEnd,
            activityC,
            activityB,
            activityA,
            activityStartA,
            activityStartB,
        };

        var criticalPathMethod = new CriticalPathMethod();
        var criticalPath = criticalPathMethod.Execute(activities);

        criticalPath.Should().HaveCount(4);
        criticalPath[0].Should().Be(activityStartB);
        criticalPath[1].Should().Be(activityB);
        criticalPath[2].Should().Be(activityC);
        criticalPath[3].Should().Be(activityEnd);

        activities.Should().ContainSingle(_ => _.Name!.Equals("Finish")).Which.Should().Match<Activity>(_ =>
            _.Cost == 0 &&
            _.EarlyStart == 110 &&
            _.EarlyFinish == 110 &&
            _.LatestStart == 110 &&
            _.LatestFinish == 110 &&
            _.TotalFloat == 0 &&
            _.CriticalCost == 0
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("C")).Which.Should().Match<Activity>(_ =>
            _.Cost == 60 &&
            _.EarlyStart == 50 &&
            _.EarlyFinish == 110 &&
            _.LatestStart == 50 &&
            _.LatestFinish == 110 &&
            _.TotalFloat == 0 &&
            _.CriticalCost == 60
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("B")).Which.Should().Match<Activity>(_ =>
            _.Cost == 50 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 50 &&
            _.LatestStart == 0 &&
            _.LatestFinish == 50 &&
            _.TotalFloat == 0 &&
            _.CriticalCost == 110
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("A")).Which.Should().Match<Activity>(_ =>
            _.Cost == 30 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 30 &&
            _.LatestStart == 20 &&
            _.LatestFinish == 50 &&
            _.TotalFloat == 20 &&
            _.CriticalCost == 90
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("StartB")).Which.Should().Match<Activity>(_ =>
            _.Cost == 0 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 0 &&
            _.LatestStart == 0 &&
            _.LatestFinish == 0 &&
            _.TotalFloat == 0 &&
            _.CriticalCost == 110
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("StartA")).Which.Should().Match<Activity>(_ =>
            _.Cost == 0 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 0 &&
            _.LatestStart == 20 &&
            _.LatestFinish == 20 &&
            _.TotalFloat == 20 &&
            _.CriticalCost == 90
        );
    }

    [Fact]
    public void Should_Handle_Two_End_Activities()
    {
        var activityEndB = new Activity("FinishB", 0);
        var activityEndA = new Activity("FinishA", 0);
        var activityD = new Activity("D", 20, activityEndB);
        var activityC = new Activity("C", 20, activityEndA);
        var activityB = new Activity("B", 50, activityD);
        var activityA = new Activity("A", 30, activityC);
        var activityStart = new Activity("Start", 0, activityA, activityB);

        var activities = new HashSet<Activity>
        {
            activityEndB,
            activityEndA,
            activityD,
            activityC,
            activityB,
            activityA,
            activityStart,
        };

        var criticalPathMethod = new CriticalPathMethod();
        var criticalPath = criticalPathMethod.Execute(activities);

        criticalPath.Should().HaveCount(4);
        criticalPath[0].Should().Be(activityStart);
        criticalPath[1].Should().Be(activityB);
        criticalPath[2].Should().Be(activityD);
        criticalPath[3].Should().Be(activityEndB);

        activities.Should().ContainSingle(_ => _.Name!.Equals("FinishB")).Which.Should().Match<Activity>(_ =>
            _.Cost == 0 &&
            _.EarlyStart == 70 &&
            _.EarlyFinish == 70 &&
            _.LatestStart == 70 &&
            _.LatestFinish == 70 &&
            _.TotalFloat == 0 &&
            _.CriticalCost == 0
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("FinishA")).Which.Should().Match<Activity>(_ =>
            _.Cost == 0 &&
            _.EarlyStart == 50 &&
            _.EarlyFinish == 50 &&
            _.LatestStart == 70 &&
            _.LatestFinish == 70 &&
            _.TotalFloat == 20 &&
            _.CriticalCost == 0
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("D")).Which.Should().Match<Activity>(_ =>
            _.Cost == 20 &&
            _.EarlyStart == 50 &&
            _.EarlyFinish == 70 &&
            _.LatestStart == 50 &&
            _.LatestFinish == 70 &&
            _.TotalFloat == 0 &&
            _.CriticalCost == 20
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("C")).Which.Should().Match<Activity>(_ =>
            _.Cost == 20 &&
            _.EarlyStart == 30 &&
            _.EarlyFinish == 50 &&
            _.LatestStart == 50 &&
            _.LatestFinish == 70 &&
            _.TotalFloat == 20 &&
            _.CriticalCost == 20
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("B")).Which.Should().Match<Activity>(_ =>
            _.Cost == 50 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 50 &&
            _.LatestStart == 0 &&
            _.LatestFinish == 50 &&
            _.TotalFloat == 0 &&
            _.CriticalCost == 70
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("A")).Which.Should().Match<Activity>(_ =>
            _.Cost == 30 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 30 &&
            _.LatestStart == 20 &&
            _.LatestFinish == 50 &&
            _.TotalFloat == 20 &&
            _.CriticalCost == 50
        );

        activities.Should().ContainSingle(_ => _.Name!.Equals("Start")).Which.Should().Match<Activity>(_ =>
            _.Cost == 0 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 0 &&
            _.LatestStart == 0 &&
            _.LatestFinish == 0 &&
            _.TotalFloat == 0 &&
            _.CriticalCost == 70
        );
    }

    [Fact]
    public void Should_Handle_End_Activity_With_Cost()
    {
        var activityEnd = new Activity("Finish", 50);
        var activityD = new Activity("D", 40, activityEnd);
        var activityC = new Activity("C", 30, activityD);
        var activityB = new Activity("B", 20, activityEnd);
        var activityA = new Activity("A", 10, activityB);
        var activityStart = new Activity("Start", 0, activityA, activityC);

        var activities = new HashSet<Activity>
        {
            activityEnd,
            activityA,
            activityB,
            activityC,
            activityD,
            activityStart,
        };

        var criticalPathMethod = new CriticalPathMethod();
        var criticalPath = criticalPathMethod.Execute(activities);

        criticalPath.Should().HaveCount(4);
        criticalPath[0].Should().Be(activityStart);
        criticalPath[1].Should().Be(activityC);
        criticalPath[2].Should().Be(activityD);
        criticalPath[3].Should().Be(activityEnd);
    }

    [Fact]
    public void Should_Handle_Two_Activities_With_Same_Properties()
    {
        var activityEnd = new Activity("Finish", 0);
        var activityC = new Activity("C", 90, activityEnd);
        var activityG = new Activity("C", 90, activityEnd);
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

        var criticalPathMethod = new CriticalPathMethod();
        criticalPathMethod.Execute(activities);

        activityEnd.Should().Match<Activity>(_ =>
            _.Cost == 0 &&
            _.CriticalCost == 0 &&
            _.EarlyStart == 280 &&
            _.EarlyFinish == 280 &&
            _.LatestStart == 280 &&
            _.LatestFinish == 280 &&
            _.TotalFloat == 0 &&
            _.FreeFloat == 0

        );

        activityC.Should().Match<Activity>(_ =>
            _.Cost == 90 &&
            _.CriticalCost == 90 &&
            _.EarlyStart == 190 &&
            _.EarlyFinish == 280 &&
            _.LatestStart == 190 &&
            _.LatestFinish == 280 &&
            _.TotalFloat == 0 &&
            _.FreeFloat == 0
        );

        activityG.Should().Match<Activity>(_ =>
            _.Cost == 90 &&
            _.CriticalCost == 90 &&
            _.EarlyStart == 120 &&
            _.EarlyFinish == 210 &&
            _.LatestStart == 190 &&
            _.LatestFinish == 280 &&
            _.TotalFloat == 70 &&
            _.FreeFloat == 70
        );

        activityF.Should().Match<Activity>(_ =>
            _.Cost == 20 &&
            _.CriticalCost == 20 &&
            _.EarlyStart == 120 &&
            _.EarlyFinish == 140 &&
            _.LatestStart == 260 &&
            _.LatestFinish == 280 &&
            _.TotalFloat == 140 &&
            _.FreeFloat == 140
        );

        activityB.Should().Match<Activity>(_ =>
            _.Cost == 90 &&
            _.CriticalCost == 180 &&
            _.EarlyStart == 100 &&
            _.EarlyFinish == 190 &&
            _.LatestStart == 100 &&
            _.LatestFinish == 190 &&
            _.TotalFloat == 0 &&
            _.FreeFloat == 0
        );

        activityE.Should().Match<Activity>(_ =>
            _.Cost == 20 &&
            _.CriticalCost == 110 &&
            _.EarlyStart == 100 &&
            _.EarlyFinish == 120 &&
            _.LatestStart == 170 &&
            _.LatestFinish == 190 &&
            _.TotalFloat == 70 &&
            _.FreeFloat == 0
        );

        activityA.Should().Match<Activity>(_ =>
            _.Cost == 50 &&
            _.CriticalCost == 230 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 50 &&
            _.LatestStart == 50 &&
            _.LatestFinish == 100 &&
            _.TotalFloat == 50 &&
            _.FreeFloat == 50
        );

        activityD.Should().Match<Activity>(_ =>
            _.Cost == 100 &&
            _.CriticalCost == 280 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 100 &&
            _.LatestStart == 0 &&
            _.LatestFinish == 100 &&
            _.TotalFloat == 0 &&
            _.FreeFloat == 0
        );

        activityStart.Should().Match<Activity>(_ =>
            _.Cost == 0 &&
            _.CriticalCost == 280 &&
            _.EarlyStart == 0 &&
            _.EarlyFinish == 0 &&
            _.LatestStart == 0 &&
            _.LatestFinish == 0 &&
            _.TotalFloat == 0 &&
            _.FreeFloat == 0
        );
    }
}
