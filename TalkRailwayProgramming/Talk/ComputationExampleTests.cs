using FluentAssertions;

namespace TalkRailwayProgramming.Talk;

public class ComputationExampleTests
{
    [Theory]
    [InlineData("1")] 
    [InlineData("5")] 
    [InlineData("12")]
    public void ShouldBeEquivalents(string value)
    {
        var computationResult = ComputationExample.RunComputation(new Some<string>(value));
        var bindResult = ComputationExample.RunWithBind(new Some<string>(value));
        
        computationResult.Should().Be(bindResult);
    }
    
    [Theory]
    [InlineData("1")] 
    [InlineData("5")] 
    [InlineData("12")]
    public void ShouldFormatPositiveString(string value)
    {
        var result = ComputationExample.RunComputation(new Some<string>(value));

        var expected = new Ok<string, ComputationExample.Error>(@$"""{value}"" is a positive integer: {value}");
        result.Should().Be(expected);
    }

    [Fact]
    public void ShouldFailWhenNoValueReturnedByDependency()
    {
        var result = ComputationExample.RunComputation(new None<string>());

        var expected = new Error<string, ComputationExample.Error>(ComputationExample.Error.UnknownValue);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("0")] 
    [InlineData("-2")]
    public void ShouldFailWhenNullOrNegativeString(string value)
    {
        var result = ComputationExample.RunComputation(new Some<string>(value));

        var expected = new Error<string, ComputationExample.Error>(ComputationExample.Error.NotPositive);
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("Some text")] 
    [InlineData("Tomorrow's temperature will be 3°C")] 
    [InlineData("1.5")]
    public void ShouldFailWhenNotAnIntegerString(string value)
    {
        var result = ComputationExample.RunComputation(new Some<string>(value));
        
        var expected = new Error<string, ComputationExample.Error>(ComputationExample.Error.NotInteger);
        result.Should().Be(expected);
    }
}