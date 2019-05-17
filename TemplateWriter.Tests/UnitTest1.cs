using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMyVision;

namespace TemplateWriterTests
{
    [TestClass]
    public class TransformTests        
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

        [TestMethod]
        public void Variables()
        {
            var tw = CreateTemplateWriter();
            tw.Add("GroupKey", 1221);
            tw.Add("CompanyKey", 101);

            var tmp = "{GroupKey}_{CompanyKey}_{missing}";

            Assert.AreEqual("1221_101_{missing}", tw.Transform(tmp));
        }


        [TestMethod]
        public void NumberCustomFormat()
        {
            var tw = CreateTemplateWriter();
            tw.Add("Number", 8001234567);

            var tmp = "{Number:000-000-0000}";

            Assert.AreEqual("800-123-4567", tw.Transform(tmp));
        }

        [TestMethod]
        public void Object()
        {
            var o = new
            {
                GroupKey = 1221,
                CompanyKey = 100
            };
             
            var tmp = "{GroupKey}_{CompanyKey}_{missing}";

            var x = TemplateWriter.Transform(tmp, o);

            Assert.AreEqual($"{o.GroupKey}_{o.CompanyKey}_{{missing}}", x);
        }
    }
}
