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
                int expected = exp.Orders[ti.Cur];
                Assert.AreEqual(expected, idx);

                idx++;
            }
            Assert.AreEqual(14, idx);
        }
            
    }
}
