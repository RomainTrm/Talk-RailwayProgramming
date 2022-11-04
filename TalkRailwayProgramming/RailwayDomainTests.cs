using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using TalkRailwayProgramming._3_MakeExplicit;
using TalkRailwayProgramming._6_RailwayProgramming;
using TalkRailwayProgramming._7_Conclusion;
using Error = TalkRailwayProgramming._3_MakeExplicit.Error;

using Factory = System.Func<TalkRailwayProgramming._3_MakeExplicit.Option<string>, TalkRailwayProgramming._3_MakeExplicit.IExplicitDomain>;

namespace TalkRailwayProgramming;

[SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Used to display test case name on test runner")]
public class RailwayDomainTests
{
    private const int Id = 5;

    public static IEnumerable<object[]> HappyPathTestCases() => BuildTestCases(
        "1", 
        "5", 
        "12");
    
    [Theory, MemberData(nameof(HappyPathTestCases))]
    public async Task ShouldFormatPositiveString(string implementationName, string value, Factory factory)
    {
        var sut = factory(new Some<string>(value));
        var result = await sut.Run(Id);

        var expected = new Ok<string, Error>(@$"""{value}"" is a positive value");
        result.Should().Be(expected);
    }

    
    public static IEnumerable<object[]> NoValueReturnedByDependencyTestCases() => GetImplementations().Select(x => new object[] { x.name, x.factory });

    [Theory, MemberData(nameof(NoValueReturnedByDependencyTestCases))]
    public async Task ShouldFailWhenNoValueReturnedByDependency(string implementationName, Factory factory)
    {
        var sut = factory(new None<string>());
        var result = await sut.Run(Id);

        var expected = new Error<string, Error>(Error.UnknownValue);
        result.Should().Be(expected);
    }


    public static IEnumerable<object[]> NullOrNegativeTestCases() => BuildTestCases(
        "0", 
        "-2");

    [Theory, MemberData(nameof(NullOrNegativeTestCases))]
    public async Task ShouldFailWhenNullOrNegativeString(string implementationName, string value, Factory factory)
    {
        var sut = factory(new Some<string>(value));
        var result = await sut.Run(Id);

        var expected = new Error<string, Error>(Error.NotPositive);
        result.Should().Be(expected);
    }
    

    public static IEnumerable<object[]> NotAnIntegerStringTestCases() => BuildTestCases(
        "Some text", 
        "Tomorrow's temperature will be 3°C", 
        "1.5");

    [Theory, MemberData(nameof(NotAnIntegerStringTestCases))]
    public async Task ShouldFailWhenNotAnIntegerString(string implementationName, string value, Factory factory)
    {
        var sut = factory(new Some<string>(value));
        var result = await sut.Run(Id);
        
        var expected = new Error<string, Error>(Error.NotInteger);
        result.Should().Be(expected);
    }
    

    private static IEnumerable<object[]> BuildTestCases(params object[] testCases) =>
        from implementation in GetImplementations()
        from testCase in testCases
        select new[] { implementation.name, testCase, implementation.factory };

    private static IEnumerable<(string name, Factory factory)> GetImplementations() =>
        new List<(string, Factory)>
        {
            ("Explicit domain", input => new ExplicitDomain(_ => Task.FromResult(input))),
            ("Alternative explicit domain", input => new AlternativeExplicitDomain(_ => Task.FromResult(input))),
            ("Railway domain", input => new RailwayDomain(_ => Task.FromResult(input))),
            ("Computation expression", input => new ComputationExample(_ => Task.FromResult(input)))
        };
}