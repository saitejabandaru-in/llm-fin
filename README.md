# 📊 LlmProof.Core (.NET)

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-blue.svg?style=flat-square&logo=dotnet" alt=".NET Version" />
  <img src="https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square" alt="License" />
</p>

`LlmProof.Core` is an enterprise-grade Wald's Sequential Probability Ratio Testing (SPRT) statistical evaluator for LLM and AI agent comparison in C# .NET. It lets you statistically compare LLM responses and stop testing early as soon as mathematical significance is reached, saving up to 50%+ in token evaluation costs.

---

## 🚀 Quick Start (C#)

```csharp
using LlmProof.Core;

// Configure evaluator (alpha = 5%, beta = 10%)
var evaluator = new SprtEvaluator(alpha: 0.05, beta: 0.10, p0: 0.50, p1: 0.70);

// Record outcomes (true for preferred model win, false otherwise)
evaluator.AddSample(true);
evaluator.AddSample(true);

var decision = evaluator.AddSample(true);

if (decision == SprtDecision.AcceptH1)
{
    Console.WriteLine("Alternative model is statistically superior. Stop testing early!");
}
```

---

## 📄 License
MIT License.
