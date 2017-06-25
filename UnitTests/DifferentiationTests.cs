using System.Collections.Generic;
using NUnit.Framework;
using SSMS;
using SSMS.Nodes;


namespace UnitTests
{
    [TestFixture]
    public class DifferentiationTests
    {
        [TestCase]
        public void SimpleDiff()
        {
            var tlist = TransformsList.Inst();

            var pn = new PowerNode(new VarNode("x"), new ConstNode(2));

            var rdif = pn.Differentiate("x");

            var result = tlist.Simplify(rdif);

            Assert.AreEqual("2*x", result.ToString());

            var prod = new ProdNode(new CosNode(new VarNode("x")), new SinNode(new VarNode("y")));
            Assert.AreEqual("cos(x)*sin(y)", prod.ToString());

            rdif = prod.Differentiate("x");
            result = tlist.Simplify(rdif);

            Assert.AreEqual("-sin(x)*sin(y)", result.ToString());


        }
    }
}
