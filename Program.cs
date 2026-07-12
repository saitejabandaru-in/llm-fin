using System;
using LlmProof.Core;

namespace LlmProof.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running C# .NET LlmProof.Core Unit Tests...");

            // Test 1: Validation of Wald's SPRT logic
            var evaluator = new SprtEvaluator(alpha: 0.05, beta: 0.10, p0: 0.50, p1: 0.70);
            
            // Add sequential failures (which should lead to accepting H0)
            SprtDecision decision = SprtDecision.Continue;
            for (int i = 0; i < 20; i++)
            {
                decision = evaluator.AddSample(false);
                if (decision != SprtDecision.Continue) break;
            }

            if (decision != SprtDecision.AcceptH0)
            {
                throw new Exception($"Test Failed: Expected AcceptH0, got {decision}");
            }
            Console.WriteLine("Test 1 Passed: Correct H0 decision upon failures.");

            // Test 2: Validation of H1 selection
            var evaluator2 = new SprtEvaluator(alpha: 0.05, beta: 0.10, p0: 0.50, p1: 0.70);
            decision = SprtDecision.Continue;
            for (int i = 0; i < 20; i++)
            {
                decision = evaluator2.AddSample(true);
                if (decision != SprtDecision.Continue) break;
            }

            if (decision != SprtDecision.AcceptH1)
            {
                throw new Exception($"Test Failed: Expected AcceptH1, got {decision}");
            }
            Console.WriteLine("Test 2 Passed: Correct H1 decision upon successes.");

            Console.WriteLine("All 2/2 C# .NET tests passed successfully!");
        }
    }
}
