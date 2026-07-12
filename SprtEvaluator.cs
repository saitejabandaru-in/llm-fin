using System;
using System.Collections.Generic;
using System.Linq;

namespace LlmProof.Finance
{
    public enum SprtDecision
    {
        Continue,
        AcceptH1, // AI Strategy statistically beats the Benchmark portfolio
        AcceptH0  // AI Strategy is identical or underperforms the Benchmark portfolio
    }

    public class StrategyValidator
    {
        private readonly double _alpha;
        private readonly double _beta;
        private readonly double _p0; // Null hypothesis success rate (underperformance threshold)
        private readonly double _p1; // Alternative hypothesis success rate (outperformance target)
        
        private readonly double _logLowerBound;
        private readonly double _logUpperBound;
        private double _logLikelihoodRatio = 0.0;

        public int TradingDays { get; private set; } = 0;
        public int BenchmarkBeatenDays { get; private set; } = 0;

        public StrategyValidator(double alpha = 0.05, double beta = 0.10, double p0 = 0.50, double p1 = 0.65)
        {
            _alpha = alpha;
            _beta = beta;
            _p0 = p0;
            _p1 = p1;

            _logLowerBound = Math.Log(_beta / (1.0 - _alpha));
            _logUpperBound = Math.Log((1.0 - _beta) / _alpha);
        }

        /**
         * Evaluates a daily trading outcome against benchmark return.
         * Returns whether statistical significance has been reached to conclude strategy performance.
         */
        public SprtDecision RecordTradingDay(double strategyReturn, double benchmarkReturn)
        {
            TradingDays++;
            bool beatBenchmark = strategyReturn > benchmarkReturn;
            if (beatBenchmark) BenchmarkBeatenDays++;

            double pSuccessRatio = _p1 / _p0;
            double pFailureRatio = (1.0 - _p1) / (1.0 - _p0);

            _logLikelihoodRatio += Math.Log(beatBenchmark ? pSuccessRatio : pFailureRatio);

            if (_logLikelihoodRatio >= _logUpperBound)
            {
                return SprtDecision.AcceptH1;
            }
            else if (_logLikelihoodRatio <= _logLowerBound)
            {
                return SprtDecision.AcceptH0;
            }

            return SprtDecision.Continue;
        }

        /**
         * Calculate the Sharpe Ratio (Risk-Adjusted Return)
         */
        public static double CalculateSharpeRatio(IEnumerable<double> returns, double riskFreeRate = 0.02)
        {
            double[] returnsArray = returns.ToArray();
            if (returnsArray.Length < 2) return 0.0;

            double avgReturn = returnsArray.Average();
            double excessReturn = avgReturn - (riskFreeRate / 252.0); // Daily risk free rate

            double sumOfSquares = returnsArray.Select(r => Math.Pow(r - avgReturn, 2)).Sum();
            double stdDev = Math.Sqrt(sumOfSquares / (returnsArray.Length - 1));

            if (stdDev == 0.0) return 0.0;
            return (excessReturn / stdDev) * Math.Sqrt(252.0); // Annualized
        }

        /**
         * Calculate the Sortino Ratio (Downside Risk-Adjusted Return)
         */
        public static double CalculateSortinoRatio(IEnumerable<double> returns, double riskFreeRate = 0.02)
        {
            double[] returnsArray = returns.ToArray();
            if (returnsArray.Length < 2) return 0.0;

            double avgReturn = returnsArray.Average();
            double excessReturn = avgReturn - (riskFreeRate / 252.0);

            // Calculate downside deviation (only negative returns relative to target/risk-free)
            double dailyTarget = riskFreeRate / 252.0;
            double downsideSum = returnsArray
                .Select(r => r < dailyTarget ? Math.Pow(r - dailyTarget, 2) : 0.0)
                .Sum();
            
            double downsideDev = Math.Sqrt(downsideSum / returnsArray.Length);

            if (downsideDev == 0.0) return 0.0;
            return (excessReturn / downsideDev) * Math.Sqrt(252.0); // Annualized
        }

        public double GetLogLikelihoodRatio() => _logLikelihoodRatio;
    }
}
