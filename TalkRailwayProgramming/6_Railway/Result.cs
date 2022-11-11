namespace TalkRailwayProgramming;

public abstract record Result<TValue, TError>
{
    public abstract TResult Match<TResult>(
        Func<TValue, TResult> ok,
        Func<TError, TResult> error);
    
    public abstract Result<TResult, TError> Map<TResult>(
        Func<TValue, TResult> morphism);
    
    public abstract Result<TResult, TError> Bind<TResult>(
        Func<TValue, Result<TResult, TError>> morphism);
}

public sealed record Ok<TValue, TError>(TValue Value) : Result<TValue, TError>
{
    public override TResult Match<TResult>(
        Func<TValue, TResult> ok,
        Func<TError, TResult> error)
        => ok(Value);
    
    public override Result<TResult, TError> Map<TResult>(
        Func<TValue, TResult> morphism)
        => new Ok<TResult, TError>(morphism(Value));

    public override Result<TResult, TError> Bind<TResult>(
        Func<TValue, Result<TResult, TError>> morphism)
        => morphism(Value);
}

public sealed record Error<TValue, TError>(TError Value) : Result<TValue, TError>
{
    public override TResult Match<TResult>(
        Func<TValue, TResult> ok,
        Func<TError, TResult> error)
        => error(Value);
    
    public override Result<TResult, TError> Map<TResult>(
        Func<TValue, TResult> morphism)
        => new Error<TResult, TError>(Value);

    public override Result<TResult, TError> Bind<TResult>(
        Func<TValue, Result<TResult, TError>> morphism)
        => new Error<TResult, TError>(Value);
}