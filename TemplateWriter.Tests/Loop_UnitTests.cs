using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMyVision;

namespace TemplateWriterTests
{
    [TestClass]
    public class LoopTransformTests
    {

        [TestMethod]
        public void Loop()
        {

            var tw = new TemplateWriter();
            Assert.AreEqual("0", tw.Transform("{Index}"));
            Assert.AreEqual("1", tw.Transform("{Index}"));
        }

        [TestMethod]
        public void LoopWithNoIncrement()
        {

            var tw = new TemplateWriter(increment: 0);
            Assert.AreEqual("0", tw.Transform("{Index}"));
            Assert.AreEqual("0", tw.Transform("{Index}"));
        }

        [TestMethod]
        public void LoopManualIncrementWithManual()
        {

            var tw = new TemplateWriter();
            Assert.AreEqual("0", tw.Transform("{Index}"));
            Assert.AreEqual("1", tw.Transform("{Index}"));
            tw.Increment();
            Assert.AreEqual("3", tw.Transform("{Index}"));
        }

        [TestMethod]
        public void LoopManualIncrement()
        {

            var tw = new TemplateWriter(increment: 0);
            Assert.AreEqual("0", tw.Transform("{Index}"));
            Assert.AreEqual("0", tw.Transform("{Index}"));
            tw.Increment(); // since increment is set to 0... the default 
            Assert.AreEqual("0", tw.Transform("{Index}"));
        }

        [TestMethod]
        public void LoopManualIncrementAutoIncrementOff()
        {

            var tw = new TemplateWriter(autoIncrement: false);
            Assert.AreEqual("0", tw.Transform("{Index}"));
            Assert.AreEqual("0", tw.Transform("{Index}"));
            tw.Increment(); // since increment is set to 0... the default 
            Assert.AreEqual("1", tw.Transform("{Index}"));

        }

        [TestMethod]
        public void LoopManualIncrementValue()
        {

            var tw = new TemplateWriter(increment: 0);
            Assert.AreEqual("0", tw.Transform("{Index}"));
            Assert.AreEqual("0", tw.Transform("{Index}"));
            tw.Increment(5);
            Assert.AreEqual("5", tw.Transform("{Index}"));
        }
    }
}
