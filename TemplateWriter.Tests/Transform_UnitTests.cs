using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMyVision;

namespace TemplateWriterTests
{
    [TestClass]
    public class TransformTests        
    {       
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

        [TestMethod]
        public void LoadObject()
        {
            var o = new
            {
                GroupKey = 1221,
                CompanyKey = 100
            };

            var tmp = "{GroupKey}_{CompanyKey}_{missing}";

            var tw = TemplateWriter.CreateFromObject(o);

            var x = tw.Transform(tmp);

            Assert.AreEqual($"{o.GroupKey}_{o.CompanyKey}_{{missing}}", x);
        }

        [TestMethod]
        public void DefaultConstructor()
        {
            var tw = new TemplateWriter();
            Assert.IsTrue(tw.Keys.Contains(nameof(TemplateWriter.GlobalVariables.Current)), "Current Key");
            Assert.IsTrue(tw.Keys.Contains(nameof(TemplateWriter.GlobalVariables.Index)), "Index Key");
        }

        [TestMethod]
        public void NullConstructor()
        {
            var tw = new TemplateWriter(null);
            Assert.IsFalse(tw.Keys.Any());
        }

        [TestMethod]
        public void AddDictionary()
        {
            var o = new
            {
                GroupKey = 1221,
                CompanyKey = 100
            };

            var dict = new System.Collections.Generic.Dictionary<string, object>()
            {
                ["GroupKey"]= o.GroupKey,
                ["CompanyKey"] = o.CompanyKey
            };

            var tmp = "{GroupKey}_{CompanyKey}_{missing}";

            var tw = new TemplateWriter(dict);


            var x = tw.Transform(tmp);

            Assert.AreEqual($"{o.GroupKey}_{o.CompanyKey}_{{missing}}", x);
            Assert.IsFalse(tw.Keys.Contains(nameof(TemplateWriter.GlobalVariables.Current)));
            Assert.IsFalse(tw.Keys.Contains(nameof(TemplateWriter.GlobalVariables.Index)));
        }

        [TestMethod]
        public void EmptyWriter()
        {
            var tw = TemplateWriter.Empty;
            Assert.IsFalse(tw.Keys.Any());
        }

        [TestMethod]
        public void Counter()
        {
            var tw = new TemplateWriter();
            tw.Add("Position");

            var outputs = new List<string>();

            foreach( var i in Enumerable.Range(10,5))
            {
                tw["Position"] = i;
                outputs.Add(tw.Transform($"{{{ TemplateWriter.GlobalVariables.Index }}}:{{Position}}"));                    
            }

            Assert.AreEqual("0:10|1:11|2:12|3:13|4:14", string.Join("|", outputs));            
        }

        [TestMethod]
        public void SeededCounter()
        {
            var tw = new TemplateWriter(seed: 5, increment: 10 );
            tw.Add("Position");

            var outputs = new List<string>();

            foreach (var i in Enumerable.Range(10, 5))
            {
                tw["Position"] = i;
                outputs.Add(tw.Transform($"{{{ TemplateWriter.GlobalVariables.Index }}}:{{Position}}"));
            }

            Assert.AreEqual("5:10|15:11|25:12|35:13|45:14", string.Join("|", outputs));
        }
    }
}
