using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marson.Grocker.Common;
using System.IO;
using Moq;
using System.Diagnostics;

namespace Marson.Grocker.UnitTests
{
    /// <summary>
    /// Summary description for VirtualizingCollectionTests
    /// </summary>
    [TestClass]
    public class VirtualizingCollectionTests
    {
        private static TestData testData;
        private static string logFilePath;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            testData = new TestData();
            logFilePath = testData.GetLogFile(100);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (testData != null)
            {
                testData.Dispose();
                testData = null;
            }
        }

        [TestMethod]
        public void ShouldReadPages()
        {
            const int ItemsCount = 101;
            const int PageSize = 10;
            const int ExpectedPages = 11;

            Mock<IItemsProvider<string>> itemsProviderMock = new Mock<IItemsProvider<string>>();
            itemsProviderMock.Setup(p => p.GetCount()).Returns(ItemsCount);
            IList<string> vc = new VirtualizingCollection<string>(itemsProviderMock.Object, PageSize);

            int count = 0;
            foreach (var s in vc)
            {
                count++;
            }

            Assert.AreEqual(ItemsCount, count);
            Assert.AreEqual(ItemsCount, vc.Count);

            for (int i = 0; i < ExpectedPages; i++)
            {
                itemsProviderMock.Verify(p => p.GetRange(i * PageSize, PageSize), Times.Once());
            }
        }


        [TestMethod]
        public void ShouldReadWholeFile()
        {
            LogFile logFile = LogFile.LoadFrom(logFilePath);
            IItemsProvider<string> itemsProvider = new LogItemsProvider(logFile);
            IList<string> vc = new VirtualizingCollection<string>(itemsProvider, 10);

            Assert.AreEqual(100, logFile.Lines.Count);
            Assert.AreEqual(100, itemsProvider.GetCount());
            Assert.AreEqual(100, vc.Count);

            // Read the files side by side
            using (var stream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var reader = new StreamReader(stream))
            {
                int index = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Debug.WriteLine(index);
                    Assert.AreEqual(line, vc[index], "Failed at index " + index.ToString());
                    index++;
                }
            }
        }
    }
}
