using System.Collections.Generic;
using NUnit.Framework;
using SSMS;


namespace UnitTests
{
    [TestFixture]
    public class IteratorTests
    {
        [TestCase]
        public void DFOTest()
        {
            // Dest the depth first post iterator by building up the expression:
            //
            // (a+b)*(c+d*e+f)*cos(g^h)
            //
            // As we build the expression, record the index of the node in a dictionary, which
            // we can check as we iterate.
            //
            var orders = new Dictionary<SymNode, int>();

            var a = new VarNode("a");
            orders[a] = 0;

            var b = new VarNode("b");
            orders[b] = 1;

            var plus2 = new PlusNode(a, b);
            orders[plus2] = 2;

            var c = new VarNode("c");
            orders[c] = 3;

            var d = new VarNode("d");
            orders[d] = 4;
            
            var e = new VarNode("e");
            orders[e] = 5;

            var prod6 = new ProdNode(d, e);
            orders[prod6] = 6;

            var f = new VarNode("f");
            orders[f] = 7;

            var plus8 = new PlusNode(c, prod6, f);
            orders[plus8] = 8;

            var g = new VarNode("g");
            orders[g] = 9;

            var h = new VarNode("h");
            orders[h] = 10;

            var exp11 = new PowerNode(g, h);
            orders[exp11] = 11;

            var cos10 = new CosNode(exp11);
            orders[cos10] = 12;

            var prod11 = new ProdNode(plus2, plus8, cos10);
            orders[prod11] = 13;

            string str = prod11.ToString();

            TreeIterator ti = new TreeIterator(prod11);

            int idx = 0;

            SymNode node;

            while ((node = ti.Next()) != null)
            {

                int expected = orders[node];
                Assert.AreEqual(expected, idx);
                idx++;
            }
            Assert.AreEqual(14, idx);
        }

    }
}
