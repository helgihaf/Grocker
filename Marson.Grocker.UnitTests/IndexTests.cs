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
            var logFile = LogFile.LoadFrom(FileNames.LogFile5);
            Assert.AreEqual(5, logFile.Lines.Count);
        }

        [TestMethod]
        public void ShouldCount9Lines()
        {
            var logFile = LogFile.LoadFrom(FileNames.LogFile9);
            Assert.AreEqual(9, logFile.Lines.Count);
        }


        [TestMethod]
        public void ShouldCount196407Lines()
        {
            var logFile = LogFile.LoadFrom(FileNames.LogFile196407);
            Assert.AreEqual(196407, logFile.Lines.Count);
        }

        
        //[TestMethod]
        //public void ShouldCountABiggie()
        //{
        //    var logFile = LogFile.LoadFrom(@"D:\vinna\SyncToyLog\SyncToyLog.log");
        //    Assert.AreEqual(12322659, logFile.Lines.Count);
        //}

    }
}
