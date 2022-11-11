namespace TalkRailwayProgramming.MaitreD.Explicit;

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