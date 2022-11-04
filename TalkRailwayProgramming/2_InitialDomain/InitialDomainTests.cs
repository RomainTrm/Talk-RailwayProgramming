using FluentAssertions;

namespace TalkRailwayProgramming._2_InitialDomain;

public class InitialDomainTests
{
    private const int Id = 5;
    private static Func<int, Task<string>> BuildDependency(string expectedReturn)
        => _ => Task.FromResult(expectedReturn);

    [Theory]
    [InlineData("1")]
    [InlineData("5")]
    [InlineData("12")]
    public async Task ShouldFormatPositiveString(string value)
    {
        var sut = new InitialDomain(BuildDependency(value));
        var result = await sut.Run(Id);

        var expected = @$"""{value}"" is a positive value";
        result.Should().Be(expected);
    }

    [Fact]
    public async Task ShouldFailWhenNoValueReturnedByDependency()
    {
        var sut = new InitialDomain(BuildDependency(null));
        var exception = await Record.ExceptionAsync(() => sut.Run(Id));

        exception.Should().BeOfType<InvalidOperationException>()
            .Which
            .Message.Should().Be("No value for this id");
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-2")]
    public async Task ShouldFailWhenNullOrNegativeString(string value)
    {
        var sut = new InitialDomain(BuildDependency(value));
        var exception = await Record.ExceptionAsync(() => sut.Run(Id));

        exception.Should().BeOfType<InvalidOperationException>()
            .Which
            .Message.Should().Be("Input is not a positive integer");
    }

    [Theory]
    [InlineData("Some text")]
    [InlineData("Tomorrow's temperature will be 3°C")]
    [InlineData("1.5")]
    public async Task ShouldFailWhenNotAnIntegerString(string value)
    {
        var sut = new InitialDomain(BuildDependency(value));
        var exception = await Record.ExceptionAsync(() => sut.Run(Id));

        exception.Should().BeOfType<InvalidOperationException>()
            .Which
            .Message.Should().Be("Input string is not an integer");
    }
}