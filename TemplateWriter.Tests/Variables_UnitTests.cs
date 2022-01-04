using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMyVision;
using System;
using System.Collections.Generic;

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


        [TestMethod]
        public void Current_Guid()
        {
            var tw = CreateTemplateWriter();
            var value = tw.Transform($"{{{TemplateWriter.GlobalVariables.UUID}}}");

            Assert.AreEqual(Guid.Parse(value).ToString(), value);
        }
        [TestMethod]
        public void Current_GuidFormatted()
        {
            var tw = CreateTemplateWriter();
            var value = tw.Transform($"{{{TemplateWriter.GlobalVariables.UUID}:N}}");
            Assert.AreEqual(Guid.Parse(value).ToString("N"), value);
        }

        [TestMethod]
        public void Missing()
        {

            var tw = new TemplateWriter();
            Assert.AreEqual("{missing}", tw.Transform("{missing}"));

        }

        [TestMethod]
        public void CaseInsensitiveError()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                d.Add("Foo", "FOO");

                var tw = new TemplateWriter(d);
                tw.Add("foo", "foo");

            });
        }

        [TestMethod]
        public void CaseInsensitive()
        {

            var d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            d.Add("Foo", "FOO");

            var tw = new TemplateWriter(d);
            tw.Add("goo", "GOO");
            Assert.AreEqual("FOO FOO FOO", tw.Transform("{Foo} {foo} {FOO}"));
            Assert.AreEqual("GOO GOO GOO", tw.Transform("{Goo} {goo} {GOO}"));

        }

        [TestMethod]
        public void CaseSensitive()
        {
            var d = new Dictionary<string, object>();
            d.Add("Foo", "FOO");

            var tw = new TemplateWriter(d);
            tw.Add("foo", "foo");

            Assert.AreEqual("FOO foo", tw.Transform("{Foo} {foo}"));
        }
    }
}
