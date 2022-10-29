namespace TalkRailwayProgramming._2_InitialDomain;

public class InitialDomainTests
{
    [Theory]
    [InlineData("1")]
    [InlineData("5")]
    [InlineData("12")]
    public void ShouldFormatPositiveString(string value)
    {
        var result = InitialDomain.FormatPositiveString(value);

        var expected = @$"""{value}"" is a positive value";
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-2")]
    public void ShouldFailWhenNullOrNegativeString(string value)
    {
        var exception = Record.Exception(() => InitialDomain.FormatPositiveString(value));

        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("Input is not a positive integer", exception.Message);
    }

    [Theory]
    [InlineData("Some text")]
    [InlineData("Tomorrow's temperature will be 3°C")]
    [InlineData("1.5")]
    public void ShouldFailWhenNotAnIntegerString(string value)
    {
        var exception = Record.Exception(() => InitialDomain.FormatPositiveString(value));

        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("Input string is not an integer", exception.Message);
    }
}