using System.Diagnostics.CodeAnalysis;

namespace TalkRailwayProgramming;

public static class ListExamples
{
    public record AccountLine(DateTime Date, Amount Amount)
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
        public static Amount GetTotalAmountOfSuspiciousOperations(IReadOnlyList<AccountLine> lines)
        {
            var suspiciousOperations = GetSuspiciousOperations(lines);
            return GetTotalAmount(suspiciousOperations);                            
        }

        private static IReadOnlyList<AccountLine> GetSuspiciousOperations(IReadOnlyList<AccountLine> lines)
        {
            var result = new List<AccountLine>();
            foreach (var line in lines)
            {
                if (line.EvaluateAmountState() == AmountState.Suspicious)
                    result.Add(line);
            }
            return result;
        }

        private static Amount GetTotalAmount(IReadOnlyList<AccountLine> lines)
        {
            var total = Amount.Zero;
            foreach (var line in lines)
            {
                total = Amount.Add(total, line.Amount);
            }
            return total;
        }
    }
    
    // Same as initial, refactoring can be made only by using Resharper commands
    public static class Reworked
    {
        public static Amount GetTotalAmountOfSuspiciousOperations(IReadOnlyList<AccountLine> lines)
        {
            var suspiciousOperations = GetSuspiciousOperations(lines);
            return GetTotalAmount(suspiciousOperations);                            
        }

        private static IReadOnlyList<AccountLine> GetSuspiciousOperations(IReadOnlyList<AccountLine> lines) =>
            lines
                .Where(line => line.EvaluateAmountState() == AmountState.Suspicious)
                .ToList();                                                          

        private static Amount GetTotalAmount(IReadOnlyList<AccountLine> lines) =>
            lines
                .Select(line => line.Amount)                                        
                .Aggregate(Amount.Zero, Amount.Add);
    }
    
    // Same as Reworked, refactoring can be made only by using Resharper commands
    public static class OneLiner
    {
        public static Amount GetTotalAmountOfSuspiciousOperations(IReadOnlyList<AccountLine> lines)
        {
            return lines
                .Where(line => line.EvaluateAmountState() == AmountState.Suspicious)
                .Select(line => line.Amount)                                        
                .Aggregate(Amount.Zero, Amount.Add);
        }
    }
    
    public class ListExamplesTests
    {
        [Theory, MemberData(nameof(GetImplementations))]
        [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Used to display test case name on test runner")]
        public void SumSuspiciousAmounts(string testCase, Func<List<AccountLine>, Amount> getTotalAmountOfSuspiciousOperations)
        {
            var lines = new List<AccountLine>
            {
                new(DateTime.Today, new Amount(550.00m)),
                new(DateTime.Today, new Amount(11200.50m)),
                new(DateTime.Today, new Amount(1850.30m)),
                new(DateTime.Today, new Amount(550.00m)),
                new(DateTime.Today, new Amount(54320.10m))
            };

            var result = getTotalAmountOfSuspiciousOperations(lines);
            
            var expected = new Amount(65520.60m);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> GetImplementations() => new[]
        {
            new object[] { "Initial", Initial.GetTotalAmountOfSuspiciousOperations },
            new object[] { "Reworked", Reworked.GetTotalAmountOfSuspiciousOperations },
            new object[] { "OneLiner", OneLiner.GetTotalAmountOfSuspiciousOperations }
        };
    }
}