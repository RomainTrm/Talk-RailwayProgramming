namespace TalkRailwayProgramming.Conclusion;

public class ComputationExample
{
    public static Result<string, Error> Run(Option<string> optionalString)
    {
        return
            from stringValue in optionalString.ToResult(() => Error.UnknownValue)
            from integer in StringToInt(stringValue)
            from positiveInteger in EnsureIsPositive(integer)
            let formattedString = Format(positiveInteger)
            select formattedString;
    }

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
    
    public enum Error { NotInteger, NotPositive, UnknownValue }
}