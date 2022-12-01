namespace TalkRailwayProgramming.Talk;

public static class ComputationExample
{
    public static Result<string, Error> RunWithBind(Option<string> optionalString)
    {
        return optionalString.ToResult(() => Error.UnknownValue)
            .bind(stringValue => StringToInt(stringValue).Select(integer => (stringValue, integer)))
            .bind(x => EnsureIsPositive(x.integer).Select(positiveInteger => (x.stringValue, positiveInteger)))
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

public static class ResultComputationExtensions
{
    public static Result<TResult, TError> Select<TValue, TError, TResult>(
        this Result<TValue, TError> result,
        Func<TValue, TResult> morphism)
        => result.Match<Result<TResult, TError>>(
            ok => new Ok<TResult, TError>(morphism(ok)),
            error => new Error<TResult, TError>(error));

    public static Result<TResult, TError> bind<TValue, TResult, TError>(
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
        => bind(result, value => valueMorphism(value).Select(tmp => resultMorphism(value, tmp)));
    
    public static Result<TValue, TError> ToResult<TValue, TError>(this Option<TValue> option, Func<TError> onError)
        => option.Match<Result<TValue, TError>>(
            value => new Ok<TValue, TError>(value),
            () => new Error<TValue, TError>(onError()));
}