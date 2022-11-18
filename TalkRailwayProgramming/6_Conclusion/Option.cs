namespace TalkRailwayProgramming.Conclusion;

public abstract record Option<TValue>
{
    public abstract TResult Match<TResult>(
        Func<TValue, TResult> some,
        Func<TResult> none);
}

public sealed record Some<TValue>(TValue Value) : Option<TValue>
{
    public override TResult Match<TResult>(
        Func<TValue, TResult> some, 
        Func<TResult> none)
        => some(Value);
}

public sealed record None<TValue> : Option<TValue>
{
    public override TResult Match<TResult>(
        Func<TValue, TResult> some, 
        Func<TResult> none)
        => none();
}

public static class OptionExtensions
{
    public static Result<TValue, TError> ToResult<TValue, TError>(this Option<TValue> option, Func<TError> onError)
        => option.Match<Result<TValue, TError>>(
            value => new Ok<TValue, TError>(value),
            () => new Error<TValue, TError>(onError()));
}