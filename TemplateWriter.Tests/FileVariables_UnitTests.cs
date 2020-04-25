using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMyVision;

namespace TemplateWriterTests
{
    [TestClass]
    public class FileVariableTransformTests        
    {
        TemplateWriter tw;
        FileInfo fi;

        [TestInitialize]
        public void TestInit()
        {
            var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample.txt");            
            File.WriteAllText(filename, Guid.NewGuid().ToString("N"));
            fi = new FileInfo(filename);
            tw = TemplateWriter.Create(fi);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            fi.Delete();
        }


        [TestMethod]
        public void Name()
        {
            Assert.AreEqual(Path.GetFileNameWithoutExtension(fi.Name), tw.Transform($"{{{ TemplateWriter.GlobalFileVariables.Name }}}"));
 }
        [TestMethod]
        public void Filename()
        {
            Assert.AreEqual(Path.GetFileName(fi.Name), tw.Transform($"{{{ TemplateWriter.GlobalFileVariables.Filename }}}"));
        }
        [TestMethod]
        public void Extension()
        {
            Assert.AreEqual(fi.Extension, tw.Transform($"{{{ TemplateWriter.GlobalFileVariables.Extension }}}"));
        }
        [TestMethod]
        public void Fullname()
        {
            Assert.AreEqual(fi.FullName, tw.Transform($"{{{ TemplateWriter.GlobalFileVariables.Fullname }}}"));
        }
        [TestMethod]
        public void Directory()
        {
            Assert.AreEqual(fi.Directory.ToString(), tw.Transform($"{{{ TemplateWriter.GlobalFileVariables.Directory }}}"));
        }
        [TestMethod]
        public void Created()
        {
            Assert.AreEqual(fi.CreationTime.ToString("yyyyMMdd"), tw.Transform($"{{{ TemplateWriter.GlobalFileVariables.Created }:yyyyMMdd}}"));
        }
        [TestMethod]
        public void Modified()
        {
            Assert.AreEqual(fi.LastWriteTime.ToString("yyyyMMdd"), tw.Transform($"{{{ TemplateWriter.GlobalFileVariables.Modified }:yyyyMMdd}}"));
        }
    }
}
