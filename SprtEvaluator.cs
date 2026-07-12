using System;

namespace LlmProof.Core
{
    public enum SprtDecision
    {
        Continue,
        AcceptH1, // Alternative Hypothesis: Model B is statistically superior
        AcceptH0  // Null Hypothesis: Model B is not statistically superior (or identical)
    }

    public class SprtEvaluator
    {
        public double Alpha { get; }
        public double Beta { get; }
        public double P0 { get; }
        public double P1 { get; }

        private readonly double _logLowerBound;
        private readonly double _logUpperBound;
        private double _logLikelihoodRatio = 0.0;

        public int SampleCount { get; private set; } = 0;
        public int SuccessCount { get; private set; } = 0;

        public SprtEvaluator(double alpha = 0.05, double beta = 0.10, double p0 = 0.50, double p1 = 0.70)
        {
            if (p0 <= 0 || p0 >= 1 || p1 <= 0 || p1 >= 1 || p0 >= p1)
                throw new ArgumentException("Hypothesis bounds p0 and p1 must satisfy 0 < p0 < p1 < 1.");
            if (alpha <= 0 || alpha >= 1 || beta <= 0 || beta >= 1)
                throw new ArgumentException("Error bounds alpha and beta must satisfy 0 < alpha, beta < 1.");

            Alpha = alpha;
            Beta = beta;
            P0 = p0;
            P1 = p1;

            // Wald's boundaries
            _logLowerBound = Math.Log(Beta / (1.0 - Alpha));
            _logUpperBound = Math.Log((1.0 - Beta) / Alpha);
        }

        /**
         * Records a single outcome: true for success (preferred model wins), false for failure.
         * Returns the current decision state.
         */
        public SprtDecision AddSample(bool isSuccess)
        {
            SampleCount++;
            if (isSuccess) SuccessCount++;

            double pSuccessRatio = P1 / P0;
            double pFailureRatio = (1.0 - P1) / (1.0 - P0);

            _logLikelihoodRatio += Math.Log(isSuccess ? pSuccessRatio : pFailureRatio);

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

        public double GetLogLikelihoodRatio() => _logLikelihoodRatio;
    }
}
