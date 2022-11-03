namespace TalkRailwayProgramming._3_MakeExplicit;

public enum Error { NotInteger, NotPositive, UnknownValue }

public class ExplicitDomain
{
    private readonly Func<int, Task<Option<string>>> _dependency;
    public ExplicitDomain(Func<int, Task<Option<string>>> dependency) => _dependency = dependency;

    // Does compose poorly
    public async Task<Result<string, Error>> Run(int id)
    {
        var dependencyResult = await _dependency(id);
        if (dependencyResult is None<string>) return new Error<string, Error>(Error.UnknownValue);

        var dependencyResultValue = ((Some<string>)dependencyResult).Value;
        var integer = StringToInt(dependencyResultValue);
        if (integer is Error<int, Error> errorInteger) return new Error<string, Error>(errorInteger.Value);

        var integerValue = ((Ok<int, Error>)integer).Value;
        var positiveInteger = EnsureIsPositive(integerValue);
        if (positiveInteger is Error<int, Error> errorPositiveInteger) return new Error<string, Error>(errorPositiveInteger.Value);
        
        var positiveIntegerValue = ((Ok<int, Error>)positiveInteger).Value;
        var formattedString = Format(positiveIntegerValue);
        return new Ok<string, Error>(formattedString); 
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