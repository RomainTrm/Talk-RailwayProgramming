namespace TalkRailwayProgramming._2_InitialDomain;

public class InitialDomain
{
    private readonly Func<int, Task<string>> _dependency;
    public InitialDomain(Func<int, Task<string>> dependency) => _dependency = dependency;

    public async Task<string> Run(int id)
    {
        var value = await _dependency(id);
        EnsureValueFound(value);
        
        var integer = StringToInt(value);
        EnsureIsPositive(integer);
        return Format(integer); 
    }

    private static void EnsureValueFound(string value)
    {
        if (value is null) throw new InvalidOperationException("No value for this id");
    }

    private static int StringToInt(string value)
    {
        return int.TryParse(value, out int i)
            ? i
            : throw new InvalidOperationException("Input string is not an integer");
    }

    private static void EnsureIsPositive(int value)
    {
        if (value <= 0)
            throw new InvalidOperationException("Input is not a positive integer");
    }

    private static string Format(int value)
    {
        return @$"""{value}"" is a positive value";
    }
}