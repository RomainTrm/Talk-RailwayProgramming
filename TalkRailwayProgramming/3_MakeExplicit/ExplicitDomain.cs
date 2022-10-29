namespace TalkRailwayProgramming;

public class ExplicitDomain
{
    public enum Error { NotInteger, NotPositive }
    
    private static Result<int, Error> StringToInt(string value)
    {
        return int.TryParse(value, out int i)
            ? new Ok<int, Error>(i)
            : new Error<int, Error>(Error.NotInteger);
    }

    private static Result<int, Error> EnsureIsPositive(int value)
    {
        return value > 0 
            ? new Ok<int, Error>(value) 
            : new Error<int, Error>(Error.NotPositive);
    }

    private static string Format(int value)
    {
        return @$"""{value}"" is a positive value";
    }
        
    // Does compose poorly
    public static Result<string, Error> FormatPositiveString(string value)
    {
        var integer = StringToInt(value);
        if (integer is Error<int, Error> errorInteger) return new Error<string, Error>(errorInteger.Value); 
        
        var positiveInteger = EnsureIsPositive(GetValue(integer));
        if (positiveInteger is Error<int, Error> errorPositiveInteger) return new Error<string, Error>(errorPositiveInteger.Value);
        
        var formattedString = Format(GetValue(positiveInteger));
        return new Ok<string, Error>(formattedString); 
    }

    private static TValue GetValue<TValue, TError>(Result<TValue, TError> result)
    {
        var ok = (Ok<TValue, TError>)result;
        return ok.Value;
    }
}