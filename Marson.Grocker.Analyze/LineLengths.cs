using System;
using System.Collections.Generic;
using System.Linq;

namespace Marson.Grocker.Analyze
{
    public class LineLengths : Frequencies<int>
    {
        public int Min
        {
            get
            {
                return dictionary.Keys.Min();
            }
        }

        public int Max
        {
            get
            {
                return dictionary.Keys.Max();
            }
        }

        public int Average
        {
            get
            {
                if (dictionary.Count > 0)
                {
                    return dictionary.Sum(p => p.Key * p.Value) / dictionary.Values.Sum();
                }
                else
                {
                    return -1;
                }
            }
        }

        public double Variance
        {
            get
            {
                double average = Average;
                return dictionary.Sum(p => (Math.Pow((double)p.Key - average, 2)) * (double)p.Value) / dictionary.Values.Sum();
            }
        }

        public double StandardDeviation
        {
            get
            {
                return Math.Sqrt(Variance);
            }
        }
    }
}