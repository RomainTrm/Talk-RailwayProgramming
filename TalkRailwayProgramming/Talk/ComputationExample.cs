namespace TalkRailwayProgramming.Talk;

public static class ComputationExample
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

public static class ResultComputationExtensions
{
    public static Result<TResult, TError> Select<TValue, TError, TResult>(
        this Result<TValue, TError> result,
        Func<TValue, TResult> morphism)
        => result.Match<Result<TResult, TError>>(
            ok => new Ok<TResult, TError>(morphism(ok)),
            error => new Error<TResult, TError>(error));

    private static Result<TResult, TError> Bind<TValue, TResult, TError>(
        this Result<TValue, TError> result,
        Func<TValue, Result<TResult, TError>> morphism)
        => result.Match(
            ok => morphism(ok),
            error => new Error<TResult, TError>(error));
    
    // Only useful for Linq syntax
    public static Result<TResult, TError> SelectMany<TValue, TError, TTmp, TResult>(
        this Result<TValue, TError> result,
        Func<TValue, Result<TTmp, TError>> valueMorphism,
        Func<TValue, TTmp, TResult> resultMorphism)
        => Bind(result, value => valueMorphism(value).Select(tmp => resultMorphism(value, tmp)));
    
    public static Result<TValue, TError> ToResult<TValue, TError>(this Option<TValue> option, Func<TError> onError)
        => option.Match<Result<TValue, TError>>(
            value => new Ok<TValue, TError>(value),
            () => new Error<TValue, TError>(onError()));
}