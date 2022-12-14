using System.Diagnostics.CodeAnalysis;
using FluentAssertions;

namespace TalkRailwayProgramming.Lists;

public class ListsTests
{
    [Theory, MemberData(nameof(GetImplementations))]
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Used to display test case name on test runner")]
    public void SumSuspiciousAmounts(string testCase, Func<List<IReadOnlyList<AccountLine>>, Amount> getTotalAmountOfSuspiciousOperations)
    {
        var months = new List<IReadOnlyList<AccountLine>>
        {
            new List<AccountLine>
            {
                new(new Amount(220.30m)),
                new(new Amount(13100.50m)) // Suspicious
            },
            new List<AccountLine>
            {
                new(new Amount(550.00m)),
                new(new Amount(11200.50m)), // Suspicious
                new(new Amount(1850.30m)),
                new(new Amount(550.00m)),
                new(new Amount(54320.10m)) // Suspicious
            }
        };

        var result = getTotalAmountOfSuspiciousOperations(months);
            
        var expected = new Amount(13100.50m + 11200.50m + 54320.10m);
        result.Should().Be(expected);
    }

    public static IEnumerable<object[]> GetImplementations() => new[]
    {
        new object[] { "Initial", Initial.GetTotalAmountOfSuspiciousOperations },
        new object[] { "Reworked", Reworked.GetTotalAmountOfSuspiciousOperations },
        new object[] { "OneLiner", OneLiner.GetTotalAmountOfSuspiciousOperations },
    };
}