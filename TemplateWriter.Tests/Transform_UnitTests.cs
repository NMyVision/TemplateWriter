using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMyVision;

namespace TemplateWriterTests
{
    [TestClass]
    public partial class TransformTests
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
        public void LoadAnonObject()
        {
            var o = new
            {
                GroupKey = 1221,
                CompanyKey = 100
            };

            var tmp = "{GroupKey}_{CompanyKey}_{missing}";

            var tw = TemplateWriter.Empty;
            tw.Load(o);

            var x = tw.Transform(tmp);

            Assert.AreEqual($"{o.GroupKey}_{o.CompanyKey}_{{missing}}", x);
        }

        [TestMethod]
        public void LoadObject()
        {
            var o = new Model("Jane", "Doe");

            var tmp = "{FirstName}_{LastName}_{Id}";

            var tw = TemplateWriter.Empty;
            tw.Load(o);

            var x = tw.Transform(tmp);

            Assert.AreEqual($"{o.FirstName}_{o.LastName}_{{Id}}", x);
        }

        [TestMethod]
        public void LoadObjectLoop()
        {
            var models = new[] { new Model("Jane", "Doe"), new Model("Jamis", "Doe") };

            var tmp = "{FirstName}_{LastName}_{Index}";

            var tw = new TemplateWriter();
            var index = 0;
            foreach (var o in models)
            {
                tw.Load(o);

                var x = tw.Transform(tmp);

                Assert.AreEqual($"{o.FirstName}_{o.LastName}_{index++}", x);
            }
        }

        [TestMethod]
        public void LoadInvalidObject()
        {
            Assert.ThrowsException<System.IO.InvalidDataException>(() =>
            {
                var tw = TemplateWriter.Empty;
                tw.Load("String");
            });

            Assert.ThrowsException<System.IO.InvalidDataException>(() =>
            {
                var tw = TemplateWriter.Empty;
                tw.Load(new Guid());
            });

            Assert.ThrowsException<System.IO.InvalidDataException>(() =>
            {
                var tw = TemplateWriter.Empty;
                tw.Load(new List<object>());
            });

        }


        [TestMethod]
        public void LoadKeyValuePair()
        {
            var source = new KeyValuePair<string, int>("Page",12);
            var tw = TemplateWriter.Empty;
            tw.Load(source);

            var x = tw.Transform("[{Page}]");
            Assert.AreEqual(x, "[12]");

        }
        //[TestMethod]
        //public void LoadToLookup()
        //{

        //    var tw = TemplateWriter.Empty;
        //    var lookup = Enumerable.Range(0, 3).ToLookup(x => x, x => $"{x}");
        //    tw.Load(lookup);
        //}

        [TestMethod]
        public void CreateFromObject()
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
            Assert.ThrowsException<NullReferenceException>(() => new TemplateWriter(null) );
        }

        [TestMethod]
        public void AddDictionary()
        {

            var GroupKey = 1221;
            var CompanyKey = 100;

            var dict = new Dictionary<string, object>()
            {
                ["GroupKey"] = GroupKey,
                ["CompanyKey"] = CompanyKey
            };

            var tmp = "{GroupKey}_{CompanyKey}_{missing}";

            var tw = new TemplateWriter(dict);


            var x = tw.Transform(tmp);

            Assert.AreEqual($"{GroupKey}_{CompanyKey}_{{missing}}", x);
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
            tw.Add("Position", 0);

            var outputs = new List<string>();

            foreach (var i in Enumerable.Range(10, 5))
            {
                tw["Position"] = i;
                outputs.Add(tw.Transform($"{{{ TemplateWriter.GlobalVariables.Index }}}:{{Position}}"));
            }

            Assert.AreEqual("0:10|1:11|2:12|3:13|4:14", string.Join("|", outputs));
        }

        [TestMethod]
        public void SeededCounter()
        {
            var tw = new TemplateWriter(seed: 5, increment: 10);
            tw.Add("Position", 0);

            var outputs = new List<string>();

            foreach (var i in Enumerable.Range(10, 5))
            {
                tw["Position"] = i;
                outputs.Add(tw.Transform($"{{{ TemplateWriter.GlobalVariables.Index }}}:{{Position}}"));
            }

            Assert.AreEqual("5:10|15:11|25:12|35:13|45:14", string.Join("|", outputs));
        }

        [TestMethod]
        public void Clear()
        {
            var o = new Model("Jane", "Doe");

            var tw = new TemplateWriter();
            
            tw.Load(o);

            tw.Clear();

            Assert.IsFalse(tw.Keys.Contains("FirstName"));
            Assert.IsFalse(tw.Keys.Contains("LastName"));


            Assert.IsTrue(tw.Keys.Contains("Current"));
            Assert.IsTrue(tw.Keys.Any());
        }

        [TestMethod]
        public void ClearAll()
        {
            var o = new Model("Jane", "Doe");

            var tw = new TemplateWriter();

            tw.Load(o);

            tw.Clear(true);

            Assert.IsFalse(tw.Keys.Any());
        }
    }
}
