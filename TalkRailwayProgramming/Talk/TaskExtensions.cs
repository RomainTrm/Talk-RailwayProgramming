namespace TalkRailwayProgramming.Talk;

public static class TaskExtensions
{
    public static Task<Result<TResult, TError>> Traverse<TValue, TResult, TError>(
        this Result<TValue, TError> result,
        Func<TValue, Task<TResult>> morphism)
    {
        return result.Match<Task<Result<TResult, TError>>>(
            ok: async value =>
            {
                var taskResult = await morphism(value);
                return new Ok<TResult, TError>(taskResult);
            },
            error: value =>
            {
                var error = new Error<TResult, TError>(value);
                return Task.FromResult<Result<TResult, TError>>(error);
            });
    }

    public static Task DoAsync<TValue, TError>(
        this Result<TValue, TError> result,
        Func<TValue, Task> action)
    {
        return result.Match(
            ok: value => action(value),
            error: _ => Task.CompletedTask);
    }
}