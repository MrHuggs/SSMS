using System;
using NUnit.Framework;
using SSMS;

namespace UnitTests
{
    [TestFixture]
    public class NodeTests
    {
        [TestCase]
        public void FormatTest()
        {
            var prod = new ProdNode();

            prod.AddChild(new VarNode("b"));
            Assert.AreEqual("b", prod.ToString());

            prod.AddChild(new VarNode("a"));
            prod.AddChild(new VarNode("d"));
            prod.AddChild(new VarNode("c"));

            Assert.AreEqual("a*b*c*d", prod.ToString());

            var pp = new ProdNode();
            pp.AddChild(new VarNode("q"));
            pp.AddChild(new VarNode("r"));
            prod.AddChild(pp);
            Assert.AreEqual("a*b*c*d*(q*r)", prod.ToString());
            prod.RemoveChild(pp);

            var p = new PlusNode();
            p.AddChild(new VarNode("x"));
            Assert.AreEqual("x", p.ToString());
            p.AddChild(new VarNode("y"));
            Assert.AreEqual("x+y", p.ToString());
            prod.AddChild(p);
            Assert.AreEqual("a*b*c*d*(x+y)", prod.ToString());

            var p1 = new PlusNode();
            p1.AddChild(new VarNode("u"));
            p1.AddChild(new VarNode("v"));
            prod.AddChild(p1);
            Assert.AreEqual(prod.ToString(), "a*b*c*d*(u+v)*(x+y)");

            prod.AddChild(new ConstNode(14));
            prod.AddChild(new ConstNode(-4));
            Assert.AreEqual("-4*14*a*b*c*d*(u+v)*(x+y)", prod.ToString());

            var pn = new PowerNode(new VarNode("b"), new ConstNode(9));
            prod.AddChild(pn);
            Assert.AreEqual("-4*14*a*b*b^9*c*d*(u+v)*(x+y)", prod.ToString());

            prod.AddChild(new CosNode(p.DeepClone()));
            Assert.AreEqual("-4*14*a*b*b^9*c*d*(u+v)*(x+y)*cos(x+y)", prod.ToString());
        }

        [TestCase]
        public void IsZeroOrOneTest()
        {
            var c0 = new ConstNode(0);
            Assert.AreEqual(true, c0.IsZero());
            Assert.AreEqual(false, c0.IsOne());

            var c1 = new ConstNode(1);
            Assert.AreEqual(false, c1.IsZero());
            Assert.AreEqual(true, c1.IsOne());

            var c10 = new ConstNode(10);
            Assert.AreEqual(false, c10.IsZero());
            Assert.AreEqual(false, c10.IsOne());

            var cn = new CosNode(c0);
            Assert.AreEqual(false, cn.IsZero());
            Assert.AreEqual(true, cn.IsOne());

            var sn = new SinNode(c0);
            Assert.AreEqual(true, sn.IsZero());
            Assert.AreEqual(false, sn.IsOne());

            var prod = new ProdNode();
            prod.AddChild(c1);
            Assert.AreEqual(true, prod.IsOne());
            prod.AddChild(c1.DeepClone());
            Assert.AreEqual(true, prod.IsOne());
            prod.AddChild(c10);
            Assert.AreEqual(false, prod.IsOne());
            prod.AddChild(c0);
            Assert.AreEqual(true, prod.IsZero());

            var plus = new PlusNode();
            plus.AddChild(c0);
            Assert.AreEqual(true, plus.IsZero());
            plus.AddChild(c0.DeepClone());
            Assert.AreEqual(true, plus.IsZero());
            plus.AddChild(c1);
            Assert.AreEqual(true, plus.IsOne());
            plus.AddChild(c10);
            Assert.AreEqual(false, plus.IsOne());
            Assert.AreEqual(false, plus.IsZero());



        }
    }
}
