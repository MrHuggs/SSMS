using NUnit.Framework;
using SSMS;


namespace UnitTests
{
    [TestFixture]
    public class SubstitutionTests
    {
        [TestCase]
        public void BasicSubstitutionTest()
        {

            var a = new VarNode("a");
            var ap = a.DeepClone();
            var c12 = new ConstNode(12);

            SymNode result;

            result = Substitution.Substitute(a, ap, c12);

            Assert.AreNotEqual(result, a);
            Assert.AreNotEqual(result, c12);
            Assert.IsTrue(result.IsEqual(c12));


        }
        [TestCase]
        public void SubstitutionTest()
        {
            var result = new Exp1().Root;

            var c10 = new ConstNode(10);

            result = Substitution.Substitute(result, new VarNode("c"), c10);
            result = Substitution.Substitute(result, new VarNode("d"), c10);
            result = Substitution.Substitute(result, new VarNode("e"), c10);

            Assert.AreEqual("(a+b)*(10+10*10+f)*cos(g^h)", result.ToString());
            result = result.FoldConstants();
            Assert.AreEqual("(a+b)*(110+f)*cos(g^h)", result.ToString());

        }

    }
}