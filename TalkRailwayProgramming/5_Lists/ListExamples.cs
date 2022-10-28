namespace TalkRailwayProgramming;

public static class Initial
{
    public static Amount GetTotalAmountOfSuspiciousOperations(IReadOnlyList<MonthOperations> months)
    {
        var allLines = GetAllLines(months);
        var suspiciousOperationsPerMonths = GetSuspiciousOperations(allLines);
        return GetTotalAmount(suspiciousOperationsPerMonths);                            
    }
    
    private static IEnumerable<AccountLine> GetAllLines(IEnumerable<MonthOperations> months)
    {
        var result = new List<AccountLine>();
        foreach (var month in months)
        {
            foreach (var line in month.AccountLines)
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
    public static Amount GetTotalAmountOfSuspiciousOperations(IReadOnlyList<MonthOperations> months)
    {
        var allLines = GetAllLines(months);
        var suspiciousOperationsPerMonths = GetSuspiciousOperations(allLines);
        return GetTotalAmount(suspiciousOperationsPerMonths);                            
    }
    
    private static IEnumerable<AccountLine> GetAllLines(IEnumerable<MonthOperations> months)
    {
        return months.SelectMany(month => month.AccountLines).ToList();
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
    public static Amount GetTotalAmountOfSuspiciousOperations(IReadOnlyList<MonthOperations> months)
    {
        return months
            .SelectMany(month => month.AccountLines)
            .Where(line => line.EvaluateAmountState() == AmountState.Suspicious)
            .Select(line => line.Amount)
            .Aggregate(Amount.Zero, Amount.Add);                            
    }
}