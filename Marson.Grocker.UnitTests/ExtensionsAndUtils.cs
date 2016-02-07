using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.UnitTests
{
    internal static class ExtensionsAndUtils
    {


        public static string[] ToLines(this string s)
        {
            var list = new List<string>();
            using (var reader = new StringReader(s))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }

            return list.ToArray();
        }


        //
        // See https://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform
        //

        private const double DoublePi = Math.PI * 2;

        private static double z0;
        private static double z1;
        private static bool generate;

        public static double NextGaussian(this Random random, double mean, double standardDeviation)
        {
            generate = !generate;
            if (!generate)
            {
                return z1 * standardDeviation + mean;
            }

            double u1, u2;
            do
            {
                u1 = random.NextDouble();
                u2 = random.NextDouble();
            } while (u1 <= double.MinValue);

            z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            z1 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            return z0 * standardDeviation + mean;
        }


    }
}
