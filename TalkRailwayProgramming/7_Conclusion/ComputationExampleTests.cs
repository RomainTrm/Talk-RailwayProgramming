using FluentAssertions;

namespace TalkRailwayProgramming.Conclusion;

public class ComputationExampleTests
{
    [Theory]
    [InlineData("1")] 
    [InlineData("5")] 
    [InlineData("12")]
    public void ShouldFormatPositiveString(string value)
    {
        var result = ComputationExample.Run(new Some<string>(value));

        var expected = new Ok<string, ComputationExample.Error>(@$"""{value}"" is a positive value");
        result.Should().Be(expected);
    }

    [Fact]
    public void ShouldFailWhenNoValueReturnedByDependency()
    {
        var result = ComputationExample.Run(new None<string>());

        var expected = new Error<string, ComputationExample.Error>(ComputationExample.Error.UnknownValue);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("0")] 
    [InlineData("-2")]
    public void ShouldFailWhenNullOrNegativeString(string value)
    {
        var result = ComputationExample.Run(new Some<string>(value));

        var expected = new Error<string, ComputationExample.Error>(ComputationExample.Error.NotPositive);
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("Some text")] 
    [InlineData("Tomorrow's temperature will be 3°C")] 
    [InlineData("1.5")]
    public void ShouldFailWhenNotAnIntegerString(string value)
    {
        var result = ComputationExample.Run(new Some<string>(value));
        
        var expected = new Error<string, ComputationExample.Error>(ComputationExample.Error.NotInteger);
        result.Should().Be(expected);
    }
}