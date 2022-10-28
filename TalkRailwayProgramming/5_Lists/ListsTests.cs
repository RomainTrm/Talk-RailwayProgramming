using System.Diagnostics.CodeAnalysis;

namespace TalkRailwayProgramming;

public class ListsTests
{
    [Theory, MemberData(nameof(GetImplementations))]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Used to display test case name on test runner")]
    public void SumSuspiciousAmounts(string testCase, Func<List<MonthOperations>, Amount> getTotalAmountOfSuspiciousOperations)
    {
        var months = new List<MonthOperations>
        {
            new (DateTime.Today.AddMonths(-1).Month, DateTime.Today.AddMonths(-1).Year, new List<AccountLine>
            {
                new(DateTime.Today, new Amount(220.30m)),
                new(DateTime.Today, new Amount(13100.50m)) // Suspicious
            }),
            new (DateTime.Today.Month, DateTime.Today.Year, new List<AccountLine>
            {
                new(DateTime.Today, new Amount(550.00m)),
                new(DateTime.Today, new Amount(11200.50m)), // Suspicious
                new(DateTime.Today, new Amount(1850.30m)),
                new(DateTime.Today, new Amount(550.00m)),
                new(DateTime.Today, new Amount(54320.10m)) // Suspicious
            })
        };

        var result = getTotalAmountOfSuspiciousOperations(months);
            
        var expected = new Amount(13100.50m + 11200.50m + 54320.10m);
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> GetImplementations() => new[]
    {
        new object[] { "Initial", Initial.GetTotalAmountOfSuspiciousOperations },
        new object[] { "Reworked", Reworked.GetTotalAmountOfSuspiciousOperations },
        new object[] { "OneLiner", OneLiner.GetTotalAmountOfSuspiciousOperations },
        new object[] { "Talk", Lists.GetTotalAmountOfSuspiciousOperations }
    };
}