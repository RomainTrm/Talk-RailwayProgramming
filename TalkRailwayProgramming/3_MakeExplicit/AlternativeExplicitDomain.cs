namespace TalkRailwayProgramming._3_MakeExplicit;

public class AlternativeExplicitDomain
{
    private readonly Func<int, Task<Option<string>>> _dependency;
    public AlternativeExplicitDomain(Func<int, Task<Option<string>>> dependency) => _dependency = dependency;

    // Does compose poorly
    public async Task<Result<string, Error>> Run(int id)
    {
        var dependencyResult = await _dependency(id);
        return dependencyResult.Match(
            value =>
            {
                return StringToInt(value).Match(
                    integer =>
                    {
                        return EnsureIsPositive(integer).Match<Result<string, Error>>(
                            positiveInteger =>
                            {
                                var formattedString = Format(positiveInteger);
                                return new Ok<string, Error>(formattedString);
                            },
                            error => new Error<string, Error>(error));
                    },
                    error => new Error<string, Error>(error));
            },
            () => new Error<string, Error>(Error.UnknownValue));
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