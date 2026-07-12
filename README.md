# 📊 LlmProof.Finance (.NET)

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-blue.svg?style=flat-square&logo=dotnet" alt=".NET Version" />
  <img src="https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square" alt="License" />
</p>

`LlmProof.Finance` is an enterprise-grade AI Trading Agent & Portfolio Strategy Validator in C# .NET. It calculates Sharpe/Sortino ratios and uses Sequential Probability Ratio Testing (SPRT) to statistically validate if your AI agent's investment strategy beats a baseline benchmark.

---

## 🚀 Quick Start (C#)

```csharp
using LlmProof.Finance;

// Configure validator (alpha = 5%, beta = 10%)
var validator = new StrategyValidator(alpha: 0.05, beta: 0.10, p0: 0.50, p1: 0.65);

// Record daily outcomes (AI Strategy Return vs S&P 500 Return)
var decision = validator.RecordTradingDay(0.012, 0.005); // Beat benchmark!

if (decision == SprtDecision.AcceptH1)
{
    Console.WriteLine("AI strategy statistically beats the benchmark. Deploy to production!");
}

// Calculate metrics
double sharpe = StrategyValidator.CalculateSharpeRatio(returns);
double sortino = StrategyValidator.CalculateSortinoRatio(returns);
```

---

## 📄 License
MIT License.
