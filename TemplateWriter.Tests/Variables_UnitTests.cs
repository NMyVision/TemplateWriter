using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMyVision;

namespace TemplateWriterTests
{
    [TestClass]
    public class VariableTransformTests        
    {
        public TemplateWriter CreateTemplateWriter()
        {
            var dt = new DateTime(1980, 4, 6, 6, 30, 33);
            return new TemplateWriter(dt);
        }

        [TestMethod]
        public void Current()
        {
            var tw = CreateTemplateWriter();
            Assert.AreEqual("4/6/1980 6:30:33 AM", tw.Transform($"{{{TemplateWriter.GlobalVariables.Current}}}"));
        }

        [TestMethod]
        public void Current_Date()
        {
            var tw = CreateTemplateWriter();
            Assert.AreEqual("19800406", tw.Transform($"{{{TemplateWriter.GlobalVariables.Current_Date}}}"));
        }

        [TestMethod]
        public void Current_DateTime()
        {
            var tw = CreateTemplateWriter();
            Assert.AreEqual("19800406063033", tw.Transform($"{{{TemplateWriter.GlobalVariables.Current_DateTime}}}"));
        }

        [TestMethod]
        public void Current_Time()
        {
            var tw = CreateTemplateWriter();
            Assert.AreEqual("063033", tw.Transform($"{{{TemplateWriter.GlobalVariables.Current_Time}}}"));
        }

        [TestMethod]
        public void Current_UniqueDate()
        {
            var tw = CreateTemplateWriter();
            Assert.AreEqual("19800406063033000", tw.Transform($"{{{TemplateWriter.GlobalVariables.Current_UniqueDate}}}"));
        }
    }
}
