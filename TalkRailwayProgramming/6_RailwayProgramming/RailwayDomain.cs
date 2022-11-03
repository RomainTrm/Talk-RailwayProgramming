using TalkRailwayProgramming._3_MakeExplicit;

namespace TalkRailwayProgramming._6_RailwayProgramming;

public enum Error { NotInteger, NotPositive, UnknownValue }

public class RailwayDomain
{
    private readonly Func<int, Task<Option<string>>> _dependency;
    public RailwayDomain(Func<int, Task<Option<string>>> dependency) => _dependency = dependency;

    public async Task<Result<string, Error>> Run(int id)
    {
        var value = await _dependency(id);
        return value
            .ToResult(() => Error.UnknownValue)
            .Bind(StringToInt)
            .Bind(EnsureIsPositive)
            .Select(Format);
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