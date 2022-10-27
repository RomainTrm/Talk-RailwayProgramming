namespace TalkRailwayProgramming;

public abstract class Option<TValue> : IEquatable<Option<TValue>>
{
    public abstract TResult Match<TResult>(
        Func<TValue, TResult> some,
        Func<TResult> none);

    public abstract Option<TResult> Select<TResult>(Func<TValue, TResult> morphism);
    public abstract Option<TResult> Bind<TResult>(Func<TValue, Option<TResult>> morphism);
    public abstract Option<TResult> SelectMany<TTmp, TResult>(
        Func<TValue, Option<TTmp>> valueMorphism, 
        Func<TValue, TTmp, TResult> resultMorphism);

    #region Equatable pattern

    public bool Equals(Option<TValue>? other) => other != null;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Option<TValue>)obj);
    }

    public override int GetHashCode() => 1;

    #endregion
}

public sealed class Some<TValue> : Option<TValue>, IEquatable<Some<TValue>>
{
    private readonly TValue _value;
    public Some(TValue value) => _value = value;

    public override TResult Match<TResult>(
        Func<TValue, TResult> some, 
        Func<TResult> none)
        => some(_value);

    public override Option<TResult> Select<TResult>(
        Func<TValue, TResult> morphism)
        => new Some<TResult>(morphism(_value));

    public override Option<TResult> Bind<TResult>(
        Func<TValue, Option<TResult>> morphism)
        => morphism(_value);

    public override Option<TResult> SelectMany<TTmp, TResult>(
        Func<TValue, Option<TTmp>> valueMorphism,
        Func<TValue, TTmp, TResult> resultMorphism)
        => Bind(value => valueMorphism(value).Select(tmp => resultMorphism(value, tmp)));

    #region IEquatable pattern

    public bool Equals(Some<TValue>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _value != null && _value.Equals(other._value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is Some<TValue> some && Equals(some);
    }

    public override int GetHashCode() => _value?.GetHashCode() ?? 0;

    #endregion
}

public sealed class None<TValue> : Option<TValue>, IEquatable<None<TValue>>
{
    public override TResult Match<TResult>(
        Func<TValue, TResult> some, 
        Func<TResult> none)
        => none();

    public override Option<TResult> Select<TResult>(
        Func<TValue, TResult> morphism)
        => new None<TResult>();

    public override Option<TResult> Bind<TResult>(
        Func<TValue, Option<TResult>> morphism)
        => new None<TResult>();

    public override Option<TResult> SelectMany<TTmp, TResult>(
        Func<TValue, Option<TTmp>> valueMorphism,
        Func<TValue, TTmp, TResult> resultMorphism)
        => new None<TResult>();

    #region IEquatable pattern

    public bool Equals(None<TValue>? other) => other != null;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is None<TValue> none && Equals(none);
    }

    public override int GetHashCode() => 1;

    #endregion
}