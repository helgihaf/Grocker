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

    }
}
