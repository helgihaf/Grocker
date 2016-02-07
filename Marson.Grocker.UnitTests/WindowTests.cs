using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marson.Grocker.Common;

namespace Marson.Grocker.UnitTests
{
    [TestClass]
    public class WindowTests
    {
        private static TestData testData;
        private static string logFile100;
        private static string logFile5;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            testData = new TestData();
            logFile100 = testData.GetLogFile(100);
            logFile5 = testData.GetLogFile(5);
        }

        [TestMethod]
        public void ShouldCreateBasicWindow()
        {
            var logFile = LogFile.LoadFrom(logFile100);
            var window = logFile.CreateWindow();
            window.Length = 22;

            Assert.AreEqual(22, window.Length);
            Assert.AreEqual(5, window.PageCount);
        }

        [TestMethod]
        public void ShouldFindLastPage()
        {
            var logFile = LogFile.LoadFrom(logFile100);
            var window = logFile.CreateWindow();
            window.Length = 7;
            window.LastPage();
            CompareLines(TestData.LoadLines(logFile100, 100-7, 7), window);
        }

        [TestMethod]
        public void ShouldFindFirstPage()
        {
            var logFile = LogFile.LoadFrom(logFile100);
            var window = logFile.CreateWindow();
            window.Length = 7;
            window.FirstPage();
            CompareLines(TestData.LoadLines(logFile100, 0, 7), window);
        }

        [TestMethod]
        public void ShouldFindPrevAfterLastPage()
        {
            var logFile = LogFile.LoadFrom(logFile100);
            var window = logFile.CreateWindow();
            window.Length = 7;
            window.LastPage();
            window.PreviousPage();
            CompareLines(TestData.LoadLines(logFile100, 100 - 7 - 7, 7), window);
        }

        [TestMethod]
        public void ShouldFindNextAfterFirstPage()
        {
            var logFile = LogFile.LoadFrom(logFile100);
            var window = logFile.CreateWindow();
            window.Length = 7;
            window.FirstPage();
            window.NextPage();
            CompareLines(TestData.LoadLines(logFile100, 7, 7), window);
        }

        private static void CompareLines(string[] lines, LogWindow window)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                Assert.AreEqual(lines[i], window.Lines[i], "at i = " + i.ToString());
            }
        }




        //[TestMethod]
        //public void ShouldFindLastPage()
        //{
        //    string[] lines = new string[]
        //        {
        //            "[23.10.2015 09:26:44.513] [LAIHEH] [LAIHEH] [ActivityStart] [Issue] [269a7a4c-e235-4640-98a7-08b081dc4130] ParentId:[b9327fce-3da4-4182-abbf-f75493f5f4ea] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
        //            "[23.10.2015 09:26:44.560] [LAIHEH] [LAIHEH] [ActivityStart] [Generating token] [88f66500-7e1f-4a78-831a-c012a35a656f] ParentId:[269a7a4c-e235-4640-98a7-08b081dc4130] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
        //            "[23.10.2015 09:26:45.482] [LAIHEH] [LAIHEH] [ActivityStop ] [Generating token](925 ms) [88f66500-7e1f-4a78-831a-c012a35a656f] ParentId:[269a7a4c-e235-4640-98a7-08b081dc4130] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
        //            "[23.10.2015 09:26:45.482] [LAIHEH] [LAIHEH] [ActivityStop ] [Issue](973 ms) [269a7a4c-e235-4640-98a7-08b081dc4130] ParentId:[b9327fce-3da4-4182-abbf-f75493f5f4ea] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
        //            "[23.10.2015 09:27:29.877] [LAIHEH] [LAIHEH] [ActivityStart] [Issue] [35cbea68-867d-48e8-9bf2-625d103a6e9d] ParentId:[09dcdfe7-b3d3-4ff2-9187-c197370f1134] ActivityChainId:[09f5ff36-8ab0-419b-b11b-865478da041b]",
        //            "[23.10.2015 09:26:28.126] [L37441$] [L37441$] [ActivityStart] [Starting STS - 2.4] [67b9b2c5-3ac5-43e6-b7df-eca584324a6e] ParentId:[6fcf9b72-7877-478c-bee0-1e3cb8d230a8] ActivityChainId:[b999946c-101d-4472-ba63-65c186c9e973]",
        //            "[23.10.2015 09:26:28.137] [L37441$] [L37441$] [ActivityStart] [Starting endpoint for STS-2.4] [24f22d48-04d8-4c9c-ad59-add3cef74f40] ParentId:[67b9b2c5-3ac5-43e6-b7df-eca584324a6e] ActivityChainId:[d24110ac-f179-40d7-b3f2-7415376104b3]",
        //        };
        //    var logFile = LogFile.LoadFrom(logFile100);
        //    var window = logFile.CreateWindow(7);
        //    window.CurrentPage = window.PageCount - 1;
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        Assert.AreEqual(lines[i], window.Lines[i].Text);
        //    }
        //}

    }
}
