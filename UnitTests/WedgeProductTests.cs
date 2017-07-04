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

        WedgeNode MakeWedgeNode(string[] list)
        {
            var result = new WedgeNode();
            foreach (var s in list)
            {
                result.AddChild(new DNode(s));
            }
            return result;
        }


        [TestCase]
        public void DNodeSort()
        {
            string[] list1 = { "a", "b", "c", "d" };
            string[] list2 = { "d", "c", "b", "a" };
            string[] list3 = { "d", "b", "c", "a" };
            string[] list4 = { "d", "b", "b", "a" };


            var dn1 = MakeWedgeNode(list1);
            Assert.AreEqual(1, dn1.SortDNodes());
            Assert.AreEqual(false, dn1.IsZero());

            var dn2 = MakeWedgeNode(list2);
            Assert.AreEqual(1, dn2.SortDNodes());
            Assert.AreEqual(true, dn1.IsEqual(dn1));


            var dn3 = MakeWedgeNode(list3);
            Assert.AreEqual(-1, dn3.SortDNodes());
            Assert.AreEqual(true, dn1.IsEqual(dn1));


            var dn4 = MakeWedgeNode(list4);
            Assert.AreEqual(true, dn4.IsZero());

        }

        [TestCase]
        public void WedgeTests()
        {

        }
    }
}

