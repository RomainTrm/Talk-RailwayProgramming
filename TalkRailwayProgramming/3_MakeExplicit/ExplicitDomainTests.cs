namespace TalkRailwayProgramming._3_MakeExplicit;

public class ExplicitDomainTests
{
    [Theory]
    [InlineData("1")]
    [InlineData("5")]
    [InlineData("12")]
    public void ShouldFormatPositiveString(string value)
    {
        var result = ExplicitDomain.FormatPositiveString(value);

        var expected = new Ok<string, ExplicitDomain.Error>(@$"""{value}"" is a positive value");
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-2")]
    public void ShouldFailWhenNullOrNegativeString(string value)
    {
        var result = ExplicitDomain.FormatPositiveString(value);

        var expected = new Error<string, ExplicitDomain.Error>(ExplicitDomain.Error.NotPositive);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Some text")]
    [InlineData("Tomorrow's temperature will be 3°C")]
    [InlineData("1.5")]
    public void ShouldFailWhenNotAnIntegerString(string value)
    {
        var result = ExplicitDomain.FormatPositiveString(value);

        var expected = new Error<string, ExplicitDomain.Error>(ExplicitDomain.Error.NotInteger);
        Assert.Equal(expected, result);
    }
}