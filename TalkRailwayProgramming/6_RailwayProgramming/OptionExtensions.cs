using TalkRailwayProgramming._3_MakeExplicit;

namespace TalkRailwayProgramming;

public static class OptionExtensions
{
    public static Option<TResult> Select<TValue, TResult>(this Option<TValue> option, Func<TValue, TResult> morphism)
        => option.Match<Option<TResult>>(
            value => new Some<TResult>(morphism(value)),
            () => new None<TResult>());
    
    public static Option<TResult> Bind<TValue, TResult>(this Option<TValue> option, Func<TValue, Option<TResult>> morphism)
        => option.Match(
            some: morphism,
            () => new None<TResult>());

    // Only useful for Linq syntax
    public static Option<TResult> SelectMany<TValue, TTmp, TResult>(
        this Option<TValue> option,
        Func<TValue, Option<TTmp>> valueMorphism,
        Func<TValue, TTmp, TResult> resultMorphism)
        => option.Bind(value => valueMorphism(value).Select(tmp => resultMorphism(value, tmp)));
}