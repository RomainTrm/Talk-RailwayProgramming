namespace TalkRailwayProgramming._5_Lists;

public record MonthOperations(List<AccountLine> AccountLines);
    
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

public static class Lists
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