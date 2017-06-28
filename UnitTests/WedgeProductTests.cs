using System;
using System.Collections.Generic;
using NUnit.Framework;
using SSMS;
using SSMS.Parser;
using SSMS.Nodes;

namespace UnitTests.Parser
{
    [TestFixture]
    public class WedgeProductTests
    {

        [TestCase]
        public void DNodeSort()
        {
            string[] list1 = { "a", "b", "c", "d" };

            string[] list2 = { "d", "c", "b", "a" };

            string[] list3 = { "d", "b", "c", "a" };

            var dn1 = new DNode(list1);
            Assert.AreEqual(1, dn1.SortVars());

            var dn2 = new DNode(list2);
            Assert.AreEqual(1, dn2.SortVars());
            Assert.AreEqual(true, dn1.IsEqual(dn1));


            var dn3 = new DNode(list3);
            Assert.AreEqual(-1, dn3.SortVars());
            Assert.AreEqual(true, dn1.IsEqual(dn1));

        }
    }
}

