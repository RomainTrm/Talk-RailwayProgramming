namespace TalkRailwayProgramming._3_MakeExplicit;

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