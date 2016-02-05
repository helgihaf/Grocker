using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marson.Grocker.Common;

namespace Marson.Grocker.UnitTests
{
    [TestClass]
    public class WindowTests
    {
        [TestMethod]
        public void ShouldCreateBasicWindow()
        {
            var logFile = LogFile.LoadFrom(FileNames.LogFile100);
            var window = logFile.CreateWindow();
            window.Length = 22;

            Assert.AreEqual(22, window.Length);
            Assert.AreEqual(5, window.PageCount);
        }

        [TestMethod]
        public void ShouldFindLastPage()
        {
            string[] lines = new string[]
                {
                    "[7.10.2015 22:41:17.129] [LAASSI] [LAASSI] [ActivityStop ] [Issue](594 ms) [769c8dbd-6c23-4c66-99a2-02bdfe5ea768] ParentId:[d963aee3-c4a0-4e96-9c14-25b9d3decae2] ActivityChainId:[53cf2568-e071-4474-81de-5dcb2c71af15]",
                    "[7.10.2015 22:41:17.145] [LAASSI] [LAASSI] [ActivityStop ] [Generating token](379 ms) [990cb996-ed7d-48dc-9ee5-be746d49a9a3] ParentId:[47ff040e-bf9f-4e35-8eec-6be3968f11d0] ActivityChainId:[e95e3eaf-b4cb-44a8-88ae-7522c716b168]",
                    "[7.10.2015 22:41:17.145] [LAASSI] [LAASSI] [ActivityStop ] [Issue](573 ms) [47ff040e-bf9f-4e35-8eec-6be3968f11d0] ParentId:[f5f87d0e-da4b-4a7e-91c5-23e60214d903] ActivityChainId:[e95e3eaf-b4cb-44a8-88ae-7522c716b168]",
                    "[7.10.2015 22:41:18.954] [LAASSI] [LAASSI] [ActivityStart] [Issue] [9312f62e-cb16-4210-aeb5-9a22080112dc] ParentId:[77e603b2-1fba-454c-8f3d-fd1296b9a728] ActivityChainId:[3a21590e-76ba-450c-a7cc-b50715931624]",
                    "[7.10.2015 22:41:19.001] [LAASSI] [LAASSI] [ActivityStart] [Issue] [1c1f65ef-9e48-4fea-869b-289759acfb24] ParentId:[42bdd3b2-54d7-47a7-9bc2-ca55b5f7024c] ActivityChainId:[c209b953-1b5a-4bac-8d15-6032b1bfb8c1]",
                    "[7.10.2015 22:41:19.142] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [44913770-ab79-4959-a56e-b313679a9db7] ParentId:[1c1f65ef-9e48-4fea-869b-289759acfb24] ActivityChainId:[c209b953-1b5a-4bac-8d15-6032b1bfb8c1]",
                    "[7.10.2015 22:41:19.157] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [a0a62fca-c95f-4fb5-9f29-1fbec516ccec] ParentId:[9312f62e-cb16-4210-aeb5-9a22080112dc] ActivityChainId:[3a21590e-76ba-450c-a7cc-b50715931624]",
                };
            var logFile = LogFile.LoadFrom(FileNames.LogFile100);
            var window = logFile.CreateWindow();
            window.Length = 7;
            window.LastPage();
            CompareLines(lines, window);
        }

        [TestMethod]
        public void ShouldFindFirstPage()
        {
            string[] lines = new string[]
                {
                    "[7.10.2015 22:40:27.536] [LAASSI] [LAASSI] [ActivityStart] [Issue] [cf408278-c626-4dc9-b9ab-134fc0eba9ed] ParentId:[0681e674-802a-497e-a7ca-95c79039dfe3] ActivityChainId:[9abb673a-e162-4df0-8c01-c265102b9353]",
                    "[7.10.2015 22:40:27.568] [LAASSI] [LAASSI] [ActivityStart] [Issue] [8e66eb24-28ab-4464-9b2c-843730e45585] ParentId:[a767b55b-76b3-422b-8189-6c0fe2f65c9a] ActivityChainId:[2e50c633-406e-49bc-89c0-772ca068f1b8]",
                    "[7.10.2015 22:40:27.677] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [023de19a-1a03-4364-b9f5-867be50edeef] ParentId:[cf408278-c626-4dc9-b9ab-134fc0eba9ed] ActivityChainId:[9abb673a-e162-4df0-8c01-c265102b9353]",
                    "[7.10.2015 22:40:27.771] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [c0bce1dc-ee71-4f8d-8bad-461d354a4f68] ParentId:[8e66eb24-28ab-4464-9b2c-843730e45585] ActivityChainId:[2e50c633-406e-49bc-89c0-772ca068f1b8]",
                    "[7.10.2015 22:40:28.176] [LAASSI] [LAASSI] [ActivityStop ] [Generating token](500 ms) [023de19a-1a03-4364-b9f5-867be50edeef] ParentId:[cf408278-c626-4dc9-b9ab-134fc0eba9ed] ActivityChainId:[9abb673a-e162-4df0-8c01-c265102b9353]",
                    "[7.10.2015 22:40:28.176] [LAASSI] [LAASSI] [ActivityStop ] [Issue](652 ms) [cf408278-c626-4dc9-b9ab-134fc0eba9ed] ParentId:[0681e674-802a-497e-a7ca-95c79039dfe3] ActivityChainId:[9abb673a-e162-4df0-8c01-c265102b9353]",
                    "[7.10.2015 22:40:28.192] [LAASSI] [LAASSI] [ActivityStop ] [Generating token](416 ms) [c0bce1dc-ee71-4f8d-8bad-461d354a4f68] ParentId:[8e66eb24-28ab-4464-9b2c-843730e45585] ActivityChainId:[2e50c633-406e-49bc-89c0-772ca068f1b8]",
                };
            var logFile = LogFile.LoadFrom(FileNames.LogFile100);
            var window = logFile.CreateWindow();
            window.Length = 7;
            window.FirstPage();
            CompareLines(lines, window);
        }

        [TestMethod]
        public void ShouldFindPrevAfterLastPage()
        {
            string[] lines = new string[]
                {
                    "[7.10.2015 22:41:14.181] [LAASSI] [LAASSI] [ActivityStop ] [Issue](606 ms) [5de55441-edc3-4dae-8664-0faa6b000e29] ParentId:[8574c866-f343-456e-81bf-afddaea658ce] ActivityChainId:[1d556ea5-4ec3-4e8c-af8b-b63366757520]",
                    "[7.10.2015 22:41:14.181] [LAASSI] [LAASSI] [ActivityStop ] [Issue](648 ms) [f8a6042b-0ad6-4e80-b8fb-d94a1125f3e3] ParentId:[dadbc325-b410-47c9-8ea8-1ede82d58e51] ActivityChainId:[a2dd6fd8-99fc-42dc-b1af-67cbeff0d9cb]",
                    "[7.10.2015 22:41:16.536] [LAASSI] [LAASSI] [ActivityStart] [Issue] [769c8dbd-6c23-4c66-99a2-02bdfe5ea768] ParentId:[d963aee3-c4a0-4e96-9c14-25b9d3decae2] ActivityChainId:[53cf2568-e071-4474-81de-5dcb2c71af15]",
                    "[7.10.2015 22:41:16.568] [LAASSI] [LAASSI] [ActivityStart] [Issue] [47ff040e-bf9f-4e35-8eec-6be3968f11d0] ParentId:[f5f87d0e-da4b-4a7e-91c5-23e60214d903] ActivityChainId:[e95e3eaf-b4cb-44a8-88ae-7522c716b168]",
                    "[7.10.2015 22:41:16.692] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [152ce427-0c88-4e60-9b5a-df8b1f935a80] ParentId:[769c8dbd-6c23-4c66-99a2-02bdfe5ea768] ActivityChainId:[53cf2568-e071-4474-81de-5dcb2c71af15]",
                    "[7.10.2015 22:41:16.770] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [990cb996-ed7d-48dc-9ee5-be746d49a9a3] ParentId:[47ff040e-bf9f-4e35-8eec-6be3968f11d0] ActivityChainId:[e95e3eaf-b4cb-44a8-88ae-7522c716b168]",
                    "[7.10.2015 22:41:17.114] [LAASSI] [LAASSI] [ActivityStop ] [Generating token](425 ms) [152ce427-0c88-4e60-9b5a-df8b1f935a80] ParentId:[769c8dbd-6c23-4c66-99a2-02bdfe5ea768] ActivityChainId:[53cf2568-e071-4474-81de-5dcb2c71af15]",
                };
            var logFile = LogFile.LoadFrom(FileNames.LogFile100);
            var window = logFile.CreateWindow();
            window.Length = 7;
            window.LastPage();
            window.PreviousPage();
            CompareLines(lines, window);
        }

        [TestMethod]
        public void ShouldFindNextAfterFirstPage()
        {
            string[] lines = new string[]
                {
                    "[7.10.2015 22:40:28.192] [LAASSI] [LAASSI] [ActivityStop ] [Issue](614 ms) [8e66eb24-28ab-4464-9b2c-843730e45585] ParentId:[a767b55b-76b3-422b-8189-6c0fe2f65c9a] ActivityChainId:[2e50c633-406e-49bc-89c0-772ca068f1b8]",
                    "[7.10.2015 22:40:30.454] [LAASSI] [LAASSI] [ActivityStart] [Issue] [3303c198-4fc3-4ff3-81de-635f6b140cd8] ParentId:[55123aeb-2271-4026-9353-fb62490901ed] ActivityChainId:[21937fdf-c25b-4c0f-9c09-2f9dbcc775b4]",
                    "[7.10.2015 22:40:30.516] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [9cad3b17-e03b-490a-8afb-57d507c49f7e] ParentId:[3303c198-4fc3-4ff3-81de-635f6b140cd8] ActivityChainId:[21937fdf-c25b-4c0f-9c09-2f9dbcc775b4]",
                    "[7.10.2015 22:40:30.875] [LAASSI] [LAASSI] [ActivityStop ] [Generating token](351 ms) [9cad3b17-e03b-490a-8afb-57d507c49f7e] ParentId:[3303c198-4fc3-4ff3-81de-635f6b140cd8] ActivityChainId:[21937fdf-c25b-4c0f-9c09-2f9dbcc775b4]",
                    "[7.10.2015 22:40:30.875] [LAASSI] [LAASSI] [ActivityStop ] [Issue](423 ms) [3303c198-4fc3-4ff3-81de-635f6b140cd8] ParentId:[55123aeb-2271-4026-9353-fb62490901ed] ActivityChainId:[21937fdf-c25b-4c0f-9c09-2f9dbcc775b4]",
                    "[7.10.2015 22:40:31.078] [LAASSI] [LAASSI] [ActivityStart] [Issue] [4ee4d074-f904-4d90-93a3-0e8f40b382f5] ParentId:[2ea762df-2de4-43f2-b79b-fcb3b2ebcd3f] ActivityChainId:[e194d807-c9ab-42e4-8827-86f4db345ef0]",
                    "[7.10.2015 22:40:31.156] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [1fdc5919-d22c-4276-9a44-e63628ab024d] ParentId:[4ee4d074-f904-4d90-93a3-0e8f40b382f5] ActivityChainId:[e194d807-c9ab-42e4-8827-86f4db345ef0]",
                };
            var logFile = LogFile.LoadFrom(FileNames.LogFile100);
            var window = logFile.CreateWindow();
            window.Length = 7;
            window.FirstPage();
            window.NextPage();
            CompareLines(lines, window);
        }

        private static void CompareLines(string[] lines, LogWindow window)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                Assert.AreEqual(lines[i], window.Lines[i]);
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
        //    var logFile = LogFile.LoadFrom(FileNames.LogFile100);
        //    var window = logFile.CreateWindow(7);
        //    window.CurrentPage = window.PageCount - 1;
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        Assert.AreEqual(lines[i], window.Lines[i].Text);
        //    }
        //}

    }
}
