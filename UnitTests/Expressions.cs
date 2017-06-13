using System;
using System.Collections.Generic;
using SSMS;


// Helper to build some complex expressions:
namespace UnitTests
{
    
    class Exp1
    {
        public Dictionary<SymNode, int> Orders = new Dictionary<SymNode, int>();
        public SymNode Root;

        public VarNode a, b, c, d, e, f, g, h;
        public PlusNode plus2, plus8;
        public ProdNode prod6, prod11;
        public CosNode cos10;
        public PowerNode exp11;

        public Exp1()
        {
            // Build the expression:
            //
            // (a+b)*(c+d*e+f)*cos(g^h)
            //
            // As we build the expression, record the index of the node in a dictionary, which
            // we can check as we iterate.
            //
            a = new VarNode("a");
            Orders[a] = 0;

            b = new VarNode("b");
            Orders[b] = 1;

            plus2 = new PlusNode(a, b);
            Orders[plus2] = 2;

            c = new VarNode("c");
            Orders[c] = 3;

            d = new VarNode("d");
            Orders[d] = 4;

            e = new VarNode("e");
            Orders[e] = 5;

            prod6 = new ProdNode(d, e);
            Orders[prod6] = 6;

            f = new VarNode("f");
            Orders[f] = 7;

            plus8 = new PlusNode(c, prod6, f);
            Orders[plus8] = 8;

            g = new VarNode("g");
            Orders[g] = 9;

            h = new VarNode("h");
            Orders[h] = 10;

            exp11 = new PowerNode(g, h);
            Orders[exp11] = 11;

            cos10 = new CosNode(exp11);
            Orders[cos10] = 12;

            prod11 = new ProdNode(plus2, plus8, cos10);
            Orders[prod11] = 13;

            Root = prod11;
        }
    }
}
