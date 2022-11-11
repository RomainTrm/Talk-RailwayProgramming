using FluentAssertions;

namespace TalkRailwayProgramming.Talk;

public class ListsTests
{
    [Fact]
    public void SumSuspiciousAmounts()
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

        var result = Lists.GetTotalAmountOfSuspiciousOperations(months);
            
        var expected = new Amount(13100.50m + 11200.50m + 54320.10m);
        result.Should().Be(expected);
    }
}