using FluentAssertions;
using TalkRailwayProgramming._3_MakeExplicit;

namespace TalkRailwayProgramming._7_Conclusion;

public static class TraverseExtension
{
    public static Option<List<T>> Traverse<T>(this List<Option<T>> list)
        => list.Aggregate((Option<List<T>>)new None<List<T>>(), (aggregator, option) =>
        {
            return option.Match(
                someValue => aggregator.Match<Option<List<T>>>(
                    someAgg => new Some<List<T>>(someAgg.Append(someValue).ToList()),
                    none: () => new Some<List<T>>(new List<T>{ someValue })
                ),
                none: () =>aggregator);
        });
}

public class TraverseExample
{
    [Fact]
    public void ShouldTraverseList()
    {
        var input = new List<Option<int>>
        {
            new Some<int>(1),
            new Some<int>(2),
            new None<int>(),
            new Some<int>(3)
        };

        var result = input.Traverse();
        
        var expected = new Some<List<int>>(new List<int> { 1, 2, 3});
        result.Should().BeEquivalentTo(expected);
    }
    
    
    [Fact]
    public void ShouldTraverseListOfNone()
    {
        var input = new List<Option<int>>
        {
            new None<int>(),
            new None<int>(),
            new None<int>()
        };

        var result = input.Traverse();
        
        var expected = new None<List<int>>();
        result.Should().Be(expected);
    }
}