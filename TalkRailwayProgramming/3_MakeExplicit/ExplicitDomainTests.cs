namespace TalkRailwayProgramming._3_MakeExplicit;

public class ExplicitDomainTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(12)]
    public void ShouldFormatPositiveString(int value)
    {
        var result = ExplicitDomain.Format(value);

        var expected = @$"""{value}"" is a positive value";
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void ShouldAcceptPositiveInt(int value)
    {
        var result = ExplicitDomain.EnsureIsPositive(value);
        
        var expected = new Ok<int, ExplicitDomain.Error>(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void ShouldFailWhenNullOrNegativeString(int value)
    {
        var result = ExplicitDomain.EnsureIsPositive(value);
        
        var expected = new Error<int, ExplicitDomain.Error>(ExplicitDomain.Error.NotPositive);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("5")]
    [InlineData("1")]
    [InlineData("0")]
    [InlineData("-5")]
    public void ShouldConvertIntegerString(string value)
    {
        var result = ExplicitDomain.StringToInt(value);

        var expected = new Ok<int, ExplicitDomain.Error>(int.Parse(value));
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Some text")]
    [InlineData("Tomorrow's temperature will be 3°C")]
    [InlineData("1.5")]
    public void ShouldFailWhenNotAnIntegerString(string value)
    {
        var result = ExplicitDomain.StringToInt(value);

        var expected = new Error<int, ExplicitDomain.Error>(ExplicitDomain.Error.NotInteger);
        Assert.Equal(expected, result);
    }
}