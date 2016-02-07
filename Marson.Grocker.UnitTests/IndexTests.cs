using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marson.Grocker.Common;

namespace Marson.Grocker.UnitTests
{
    [TestClass]
    public class IndexTests
    {

        [TestMethod]
        public void ShouldCount5Lines()
        {
            CountLines(5);
        }

        [TestMethod]
        public void ShouldCount9Lines()
        {
            CountLines(9);
        }

        [TestMethod]
        public void ShouldCount196407Lines()
        {
            CountLines(196407);
        }

        private static void CountLines(int count)
        {
            using (var testData = new TestData())
            {
                var logFilePath = testData.GetLogFile(count);
                var logFile = LogFile.LoadFrom(logFilePath);
                Assert.AreEqual(count, logFile.Lines.Count);
            }
        }


    }
}
