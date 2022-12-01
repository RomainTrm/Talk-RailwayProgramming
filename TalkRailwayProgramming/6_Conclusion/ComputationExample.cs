namespace TalkRailwayProgramming.Conclusion;

public static class ComputationExample
{
    public static Result<string, Error> RunWithBind(Option<string> optionalString)
    {
        return optionalString.ToResult(() => Error.UnknownValue)
            .Bind(stringValue => StringToInt(stringValue).Select(integer => (stringValue, integer)))
            .Bind(x => EnsureIsPositive(x.integer).Select(positiveInteger => (x.stringValue, positiveInteger)))
            .Select(x => @$"""{x.stringValue}"" is a positive integer: {x.positiveInteger}");
    }

    public static Result<string, Error> RunComputation(Option<string> optionalString)
    {
        return
            from stringValue in optionalString.ToResult(() => Error.UnknownValue)
            from integer in StringToInt(stringValue)
            from positiveInteger in EnsureIsPositive(integer)
            let formattedString = @$"""{stringValue}"" is a positive integer: {positiveInteger}"
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

    public enum Error { NotInteger, NotPositive, UnknownValue }
}