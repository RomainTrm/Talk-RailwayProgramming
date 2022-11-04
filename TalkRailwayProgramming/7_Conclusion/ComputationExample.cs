using TalkRailwayProgramming._3_MakeExplicit;

namespace TalkRailwayProgramming._7_Conclusion;

public class ComputationExample : IExplicitDomain
{
    private readonly Func<int, Task<Option<string>>> _dependency;
    public ComputationExample(Func<int, Task<Option<string>>> dependency) => _dependency = dependency;

    public async Task<Result<string, Error>> Run(int id)
    {
        var value = await _dependency(id);
        return
            from stringValue in value.ToResult(() => Error.UnknownValue)
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
}