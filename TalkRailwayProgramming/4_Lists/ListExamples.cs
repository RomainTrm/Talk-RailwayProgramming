namespace TalkRailwayProgramming.Lists;

public record AccountLine(Amount Amount)
{
    public AmountState EvaluateAmountState() => Amount.EvaluateAmountState();
}                       
    
public enum AmountState { Valid, Suspicious }      
    
public record Amount(decimal Value)
{
    public static Amount Add(Amount left, Amount right) => new(left.Value + right.Value);
    public static readonly Amount Zero = new(0m);     
    public AmountState EvaluateAmountState() => Value > 10_000m ? AmountState.Suspicious : AmountState.Valid;
}

public static class Initial
{
    public static Amount GetTotalAmountOfSuspiciousOperations(IReadOnlyList<IReadOnlyList<AccountLine>> months)
    {
        var allLines = GetAllLines(months);
        var suspiciousOperationsPerMonths = GetSuspiciousOperations(allLines);
        return GetTotalAmount(suspiciousOperationsPerMonths);                            
    }
    
    private static IEnumerable<AccountLine> GetAllLines(IEnumerable<IReadOnlyList<AccountLine>> months)
    {
        var result = new List<AccountLine>();
        foreach (var month in months)
        {
            foreach (var line in month)
            {
                result.Add(line);
            }
        }
        return result;
    }

    private static IEnumerable<AccountLine> GetSuspiciousOperations(IEnumerable<AccountLine> lines)
    {
        var result = new List<AccountLine>();
        foreach (var line in lines)
        {
            if (line.EvaluateAmountState() == AmountState.Suspicious)
                result.Add(line);
        }
        return result;
    }

    private static Amount GetTotalAmount(IEnumerable<AccountLine> lines)
    {
        var total = Amount.Zero;
        foreach (var line in lines)
        {
            var lineAmount = line.Amount;
            total = Amount.Add(total, lineAmount);
        }
        return total;
    }
}

// Same as initial, refactoring can be made only by using automated commands
public static class Reworked
{
    public static Amount GetTotalAmountOfSuspiciousOperations(IReadOnlyList<IReadOnlyList<AccountLine>> months)
    {
        var allLines = GetAllLines(months);
        var suspiciousOperationsPerMonths = GetSuspiciousOperations(allLines);
        return GetTotalAmount(suspiciousOperationsPerMonths);                            
    }
    
    private static IEnumerable<AccountLine> GetAllLines(IEnumerable<IReadOnlyList<AccountLine>> months)
    {
        return months.SelectMany(month => month).ToList();
    }

    private static IEnumerable<AccountLine> GetSuspiciousOperations(IEnumerable<AccountLine> lines)
    {
        return lines.Where(line => line.EvaluateAmountState() == AmountState.Suspicious).ToList();
    }

    private static Amount GetTotalAmount(IEnumerable<AccountLine> lines)
    {
        return lines.Select(line => line.Amount).Aggregate(Amount.Zero, Amount.Add);
    }
}

// Same as Reworked, refactoring can be made only by using automated commands and removing useless iterations
public static class OneLiner
{
    public static Amount GetTotalAmountOfSuspiciousOperations(IReadOnlyList<IReadOnlyList<AccountLine>> months)
    {
        return months
            .SelectMany(month => month)
            .Where(line => line.EvaluateAmountState() == AmountState.Suspicious)
            .Select(line => line.Amount)
            .Aggregate(Amount.Zero, Amount.Add);                            
    }
}