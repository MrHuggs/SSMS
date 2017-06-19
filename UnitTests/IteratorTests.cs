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
            var exp = new Exp1();

      
            TreeIterator ti = new TreeIterator(exp.Root);

            int idx = 0;

            while (ti.Next())
            {
                int expected = exp.PostOrders[ti.Cur];
                Assert.AreEqual(expected, idx);
                if (ti.Parent() != null)
                    Assert.IsTrue(ti.Parent().HasChild(ti.Cur));

                idx++;
            }
            Assert.AreEqual(14, idx);
        }

        [TestCase]
        public void DFOPreTest()
        {
            // Dest the depth first post iterator by building up the expression:
            var exp = new Exp1();


            TreeIteratorPre ti = new TreeIteratorPre(exp.Root);

            int idx = 0;

            while (ti.Next())
            {
                int expected = exp.PreOrders[ti.Cur];
                Assert.AreEqual(expected, idx);
                if (ti.Parent() != null)
                    Assert.IsTrue(ti.Parent().HasChild(ti.Cur));

                idx++;
            }
            Assert.AreEqual(14, idx);
        }
    }
}
