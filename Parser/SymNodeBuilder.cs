using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SSMS.Nodes;


namespace SSMS.Parser
{
    public class SymNodeBuilder
    {

        static HashSet<string> Functions;

        static SymNodeBuilder()
        {
            Functions = new HashSet<string>();
            Functions.Add("sin");
            Functions.Add("cos");
        }


        public static SymNode ParseString(string str)
        {
            var t = new Tokenizer(str, Functions);
            var l = t.GetTokenList();

            ExpressionBuilder eb = new ExpressionBuilder(l);

            var enodes = eb.Parse();

            var result = CreateNodes(enodes);
            return result;
        }


        static public SymNode CreateNodes(List<ExpNode> expression)
        {
            if (expression.Count > 1)
            { 
                throw new ApplicationException(
                        string.Format("Source expression has multiple ({0}) root nodes.", expression.Count)
                        );
            }

            List<SymNode> results = new List<SymNode>();


            var result = ProcessExpNode(expression[0]);

            return result;
        }

        static SymNode ProcessExpNode(ExpNode enode)
        {
            List<SymNode> children = new List<SymNode>();

            foreach(var e in enode.Arguments)
                children.Add(ProcessExpNode(e));

            SymNode node;
            switch (enode.Type)
            {

                case TokenTypes.Plus:
                    node = new PlusNode(children[0], children[1]);
                    break;
                case TokenTypes.Minus:
                    node = new PlusNode(children[0],
                                        new ProdNode(new ConstNode(-1), children[1]));
                    break;
                case TokenTypes.Times:
                    node = new ProdNode(children[0], children[1]);
                    break;
                case TokenTypes.Div:
                    node = new ProdNode(children[0],
                                new PowerNode(children[1], new ConstNode(-1)));
                    break;
                case TokenTypes.Exp:
                    node = new PowerNode(children[0], children[1]);
                    break;
                case TokenTypes.Comma:
                    node = children[0];
                    break;
                case TokenTypes.UnaryPlus:
                    node = children[0];
                    break;
                case TokenTypes.UnaryMinus:
                    node = new ProdNode(new ConstNode(-1), children[0]);
                    break;
                case TokenTypes.Number:
                    node = new ConstNode(enode.Number);
                    break;
                case TokenTypes.String:
                    node = new VarNode(enode.String);
                    break;
                case TokenTypes.Function:
                    if (children.Count > 1)
                    {
                        throw new ApplicationException(
                                string.Format("Too many arguments for funcionts {0}.", enode.String)
                                );
                    }

                    switch (enode.String)
                    {
                        case "sin":
                            node = new SinNode(children[0]);
                            break;
                        case "cos":
                            node = new CosNode(children[0]);
                            break;
                        default:
                            Debug.Assert(false);
                            node = null;
                            break;
                    }
                    break;

                default:
                    Debug.Assert(false);
                    node = null;
                    break;
            }

            return node;
        }

    }
}
