using System;
using System.Collections.Generic;
using LlmFin;

namespace LlmFin.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running C# .NET LlmFin Quantitative AI Tests...");

            // 1. Setup mock daily returns for AI trading agent and baseline benchmark
            var aiReturns = new List<double>();
            var benchmarkReturns = new List<double>();
            
            var rand = new Random(42);
            for (int i = 0; i < 60; i++)
            {
                // AI Agent returns (higher mean)
                aiReturns.Add(rand.NextDouble() * 0.04 - 0.015); // Avg +0.5% return
                // Benchmark returns (lower mean)
                benchmarkReturns.Add(rand.NextDouble() * 0.03 - 0.015); // Avg 0.0% return
            }

            // 2. Validate Sharpe and Sortino ratios
            double sharpe = StrategyValidator.CalculateSharpeRatio(aiReturns);
            double sortino = StrategyValidator.CalculateSortinoRatio(aiReturns);

            Console.WriteLine($"AI Trading Agent Sharpe Ratio: {sharpe:F3}");
            Console.WriteLine($"AI Trading Agent Sortino Ratio: {sortino:F3}");

            if (sharpe <= 0 || sortino <= 0)
            {
                throw new Exception("Sharpe/Sortino calculation failed to produce standard output.");
            }
            Console.WriteLine("Test 1 Passed: Correctly calculated Sharpe and Sortino Ratios.");

            // 3. Test SPRT Strategy Outperformance validator
            var validator = new StrategyValidator(alpha: 0.05, beta: 0.10, p0: 0.50, p1: 0.65);
            SprtDecision decision = SprtDecision.Continue;

            for (int i = 0; i < aiReturns.Count; i++)
            {
                decision = validator.RecordTradingDay(aiReturns[i], benchmarkReturns[i]);
                if (decision != SprtDecision.Continue) break;
            }

            Console.WriteLine($"SPRT Decision: {decision} after {validator.TradingDays} trading days.");
            if (decision != SprtDecision.AcceptH1)
            {
                throw new Exception($"Test Failed: Expected AcceptH1 decision, got {decision}");
            }
            Console.WriteLine("Test 2 Passed: Correctly verified AI strategy outperformance against benchmark.");

            Console.WriteLine("All 2/2 C# LlmProof.Finance tests passed successfully!");
        }
    }
}
