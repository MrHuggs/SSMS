using System;
using NUnit.Framework;
using SSMS;
using SSMS.Nodes;
using SSMS.Parser;

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
            Assert.AreEqual("b", prod.ToStringSorted());

            prod.AddChild(new VarNode("a"));
            prod.AddChild(new VarNode("d"));
            prod.AddChild(new VarNode("c"));

            Assert.AreEqual("a*b*c*d", prod.ToStringSorted());

            var pp = new ProdNode();
            pp.AddChild(new VarNode("q"));
            pp.AddChild(new VarNode("r"));
            prod.AddChild(pp);
            Assert.AreEqual("a*b*c*d*(q*r)", prod.ToStringSorted());
            prod.RemoveChild(pp);

            var p = new PlusNode();
            p.AddChild(new VarNode("x"));
            Assert.AreEqual("x", p.ToStringSorted());
            p.AddChild(new VarNode("y"));
            Assert.AreEqual("x+y", p.ToStringSorted());
            prod.AddChild(p);
            Assert.AreEqual("a*b*c*d*(x+y)", prod.ToStringSorted());

            var p1 = new PlusNode();
            p1.AddChild(new VarNode("v"));
            p1.AddChild(new VarNode("u"));
            prod.AddChild(p1);
            Assert.AreEqual(prod.ToStringSorted(), "a*b*c*d*(u+v)*(x+y)");

            prod.AddChild(new ConstNode(14));
            prod.AddChild(new ConstNode(-4));
            Assert.AreEqual("-4*14*a*b*c*d*(u+v)*(x+y)", prod.ToStringSorted());

            var pn = new PowerNode(new VarNode("b"), new ConstNode(9));

            var prod2 = new ProdNode(pn, new VarNode("b"));
            Assert.AreEqual("b*b^9", prod2.ToStringSorted());

            prod.AddChild(pn);
            Assert.AreEqual("-4*14*a*b*b^9*c*d*(u+v)*(x+y)", prod.ToStringSorted());

            prod.AddChild(new CosNode(p.DeepClone()));
            Assert.AreEqual("-4*14*a*b*b^9*c*d*(u+v)*(x+y)*cos(x+y)", prod.ToStringSorted());
        }

        [TestCase]
        public void FormatOrderTest()
        {
            var a = new VarNode("a");
            var b = new VarNode("b");
            var c = new VarNode("c");
            var d = new VarNode("d");
            var e = new VarNode("e");
            var f = new VarNode("f");
            var g = new VarNode("g");
            var h = new VarNode("h");

            var plus = new PlusNode(new ProdNode(a, d, e), new ProdNode(b, c));
            Assert.AreEqual("a*d*e+b*c", plus.ToStringSorted());


            var cs = new CosNode(new PowerNode(g, h));

            plus = new PlusNode(new ProdNode(b, c, cs.DeepClone()), new ProdNode(d, a, e, cs.DeepClone()));

            // This shows the affect of sorting:
            Assert.AreEqual("b*c*cos(g^h)+d*a*e*cos(g^h)", plus.ToString());
            Assert.AreEqual("a*d*e*cos(g^h)+b*c*cos(g^h)", plus.ToStringSorted());
            Assert.AreEqual("a*d*e*cos(g^h)+b*c*cos(g^h)", plus.ToString());

            plus.AddChild(new ProdNode(a, c, cs.DeepClone()));
            Assert.AreEqual("a*c*cos(g^h)+a*d*e*cos(g^h)+b*c*cos(g^h)", plus.ToStringSorted());
        }

        [TestCase]
        public void DegenerateSortTest()
        {
            // Tests sorting where we have to compare inside a special function:
            var node = SymNodeBuilder.ParseString("cos(b)+cos(a)");
            Assert.AreEqual("cos(b)+cos(a)", node.ToString());
            Assert.AreEqual("cos(a)+cos(b)", node.ToStringSorted());

            node = SymNodeBuilder.ParseString("b^c*(4+e)+(4+e)*a^c");
            Assert.AreEqual("b^c*(4+e)+(4+e)*a^c", node.ToString());
            Assert.AreEqual("a^c*(4+e)+b^c*(4+e)", node.ToStringSorted());

            node = SymNodeBuilder.ParseString("b^c*(4+e)+(4+e)*a^c+a");
            Assert.AreEqual("b^c*(4+e)+(4+e)*a^c+a", node.ToString());
            Assert.AreEqual("a+a^c*(4+e)+b^c*(4+e)", node.ToStringSorted());

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
