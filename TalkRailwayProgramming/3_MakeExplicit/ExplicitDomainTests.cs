namespace TalkRailwayProgramming._3_MakeExplicit;

public class ExplicitDomainTests
{
    private const int Id = 5;
    private static Func<int, Task<Option<string>>> BuildDependency(Option<string> expectedReturn)
        => _ => Task.FromResult(expectedReturn);

    [Theory]
    [InlineData("1")]
    [InlineData("5")]
    [InlineData("12")]
    public async Task ShouldFormatPositiveString(string value)
    {
        var sut = new ExplicitDomain(BuildDependency(new Some<string>(value)));
        var result = await sut.Run(Id);

        var expected = new Ok<string, Error>(@$"""{value}"" is a positive value");
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task ShouldFailWhenNoValueReturnedByDependency()
    {
        var sut = new ExplicitDomain(BuildDependency(new None<string>()));
        var result = await sut.Run(Id);

        var expected = new Error<string, Error>(Error.UnknownValue);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-2")]
    public async Task ShouldFailWhenNullOrNegativeString(string value)
    {
        var sut = new ExplicitDomain(BuildDependency(new Some<string>(value)));
        var result = await sut.Run(Id);

        var expected = new Error<string, Error>(Error.NotPositive);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Some text")]
    [InlineData("Tomorrow's temperature will be 3°C")]
    [InlineData("1.5")]
    public async Task ShouldFailWhenNotAnIntegerString(string value)
    {
        var sut = new ExplicitDomain(BuildDependency(new Some<string>(value)));
        var result = await sut.Run(Id);
        
        var expected = new Error<string, Error>(Error.NotInteger);
        Assert.Equal(expected, result);
    }
}