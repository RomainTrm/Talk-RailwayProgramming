namespace TalkRailwayProgramming;

public class ExplicitDomain
{
    public enum Error { NotInteger, NotPositive }
    
    public static Result<int, Error> StringToInt(string value)
    {
        return int.TryParse(value, out int i)
            ? new Ok<int, Error>(i)
            : new Error<int, Error>(Error.NotInteger);
    }

    public static Result<int, Error> EnsureIsPositive(int value)
    {
        return value > 0 
            ? new Ok<int, Error>(value) 
            : new Error<int, Error>(Error.NotPositive);
    }

    public static string Format(int value)
    {
        return @$"""{value}"" is a positive value";
    }
        
    // Does not compose anymore
    // public static string FormatPositiveString(string value)
    // {
    //     var integer = StringToInt(value);
    //     EnsureIsPositive(integer);
    //     return Format(integer); 
    // }
}