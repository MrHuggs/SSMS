using System;
using System.Collections.Generic;
using NUnit.Framework;
using SSMS;

namespace UnitTests
{
    [TestFixture]
    public class DistributiveTests
    {
        DistributiveTransform trans = new DistributiveTransform();


        [TestCase]
        public void SimpleDistributive()
        {

            var a = new VarNode("a");
            var b = new VarNode("b");
            var c = new VarNode("c");

            var plus = new PlusNode(b, c);
            var prod = new ProdNode(a, plus);

            var result = trans.Apply(prod);

            Assert.AreEqual("a*b+a*c", result.ToString());

        }

        [TestCase]
        public void E1Distributive()
        {
            var exp = new Exp1();
            SymNode result;

            // Starting expression is (a+b)*(c+d*e+f)*cos(g^h)
            result = trans.Apply(exp.Root);
            Assert.AreEqual("a*((c+d*e+f)*cos(g^h))+b*((c+d*e+f)*cos(g^h))", result.ToString());

            result = TransformsList.Inst().Expand(result);
            var _result = TransformsList.Inst().Expand(result);
            //
            // ***************************************************************
            // Need to add merging of prod nodes and sum nodes
            //
            Assert.AreEqual("(a*c+a*d*e+a*f)*cos(g^h)+(b*c+b*d*e+b*f)*cos(g^h)", result.ToString());
        }
    }
}
