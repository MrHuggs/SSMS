using System;
using System.Collections.Generic;
using SSMS;


// Helper to build some complex expressions:
namespace UnitTests
{
    
    class Exp1
    {
        public Dictionary<SymNode, int> PostOrders = new Dictionary<SymNode, int>();
        public Dictionary<SymNode, int> PreOrders = new Dictionary<SymNode, int>();
        public Dictionary<SymNode, SymNode> Parents = new Dictionary<SymNode, SymNode>();
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
            PostOrders[a] = 0;
            PreOrders[a] = 2;

            b = new VarNode("b");
            PostOrders[b] = 1;
            PreOrders[b] = 3;

            plus2 = new PlusNode(a, b);
            PostOrders[plus2] = 2;
            PreOrders[plus2] = 1;

            c = new VarNode("c");
            PostOrders[c] = 3;
            PreOrders[c] = 5;


            d = new VarNode("d");
            PostOrders[d] = 4;
            PreOrders[d] = 7;

            e = new VarNode("e");
            PostOrders[e] = 5;
            PreOrders[e] = 8;

            prod6 = new ProdNode(d, e);
            PostOrders[prod6] = 6;
            PreOrders[prod6] = 6;

            f = new VarNode("f");
            PostOrders[f] = 7;
            PreOrders[f] = 9;

            plus8 = new PlusNode(c, prod6, f);
            PostOrders[plus8] = 8;
            PreOrders[plus8] = 4;

            g = new VarNode("g");
            PostOrders[g] = 9;
            PreOrders[g] = 12;

            h = new VarNode("h");
            PostOrders[h] = 10;
            PreOrders[h] = 13;

            exp11 = new PowerNode(g, h);
            PostOrders[exp11] = 11;
            PreOrders[exp11] = 11;

            cos10 = new CosNode(exp11);
            PostOrders[cos10] = 12;
            PreOrders[cos10] = 10;

            prod11 = new ProdNode(plus2, plus8, cos10);
            PostOrders[prod11] = 13;
            PreOrders[prod11] = 0;

            Root = prod11;
        }
    }
}
