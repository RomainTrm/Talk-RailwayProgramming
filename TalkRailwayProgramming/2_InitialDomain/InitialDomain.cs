namespace TalkRailwayProgramming._2_InitialDomain;

public static class InitialDomain
{
    public static string FormatPositiveString(string value)
    {
        var integer = StringToInt(value);
        EnsureIsPositive(integer);
        return Format(integer); 
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