namespace TalkRailwayProgramming.Talk;

public abstract record Result<TValue, TError>
{
    public abstract TResult Match<TResult>(
        Func<TValue, TResult> ok,
        Func<TError, TResult> error);
}

public sealed record Ok<TValue, TError>(TValue Value) : Result<TValue, TError>
{
    public override TResult Match<TResult>(
        Func<TValue, TResult> ok,
        Func<TError, TResult> error)
        => ok(Value);
}

public sealed record Error<TValue, TError>(TError Value) : Result<TValue, TError>
{
    public override TResult Match<TResult>(
        Func<TValue, TResult> ok,
        Func<TError, TResult> error)
        => error(Value);
}

public static class ResultExtensions
{
    public static Result<TResult, TError> Map<TValue, TResult,TError>(
        this Result<TValue, TError> result,
        Func<TValue, TResult> morphism)
        => throw new System.NotImplementedException();

    public static Result<TResult, TError> Bind<TValue, TResult,TError>(
        this Result<TValue, TError> result,
        Func<TValue, Result<TResult, TError>> morphism)
        => throw new System.NotImplementedException();
}