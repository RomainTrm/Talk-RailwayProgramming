using TalkRailwayProgramming._3_MakeExplicit;

namespace TalkRailwayProgramming;

public static class ResultExtensions
{
    public static Result<TResult, TError> Select<TValue, TError, TResult>(
        this Result<TValue, TError> result, 
        Func<TValue, TResult> morphism)
        => result.Match<Result<TResult, TError>>(
            value => new Ok<TResult, TError>(morphism(value)),
            error => new Error<TResult, TError>(error));
    
    public static Result<TResult, TError> Bind<TValue, TError, TResult>(
        this Result<TValue, TError> result, 
        Func<TValue, Result<TResult, TError>> morphism)
        => result.Match(
            ok: morphism,
            error => new Error<TResult, TError>(error)); 
    
    // Only useful for Linq syntax
    public static Result<TResult, TError> SelectMany<TValue, TError, TTmp, TResult>(
        this Result<TValue, TError> result,
        Func<TValue, Result<TTmp, TError>> valueMorphism,
        Func<TValue, TTmp, TResult> resultMorphism)
        => result.Bind(value => valueMorphism(value).Select(tmp => resultMorphism(value, tmp)));
}