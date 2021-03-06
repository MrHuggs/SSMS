﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using SSMS.Nodes;


namespace SSMS.Parser
{
    // Converts an expression tree into sym nodes.
    // Note that simplification and expansion are not performed.
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

            // This will trigger an exception if there are non-linear differntials:
            result.HasDifferential();

            result.AssertValid();
            result.CheckTree();
            return result;
        }

        public static SymNode SimplifyString(string str)
        {
            var result = ParseString(str);
            result = TransformsList.Inst().TrySimplify(result);
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
                case TokenTypes.Wedge:
                    node = new WedgeNode(children[0], children[1]);
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
                case TokenTypes.Differential:
                    node = new DNode(enode.String);
                    break;
                case TokenTypes.UnaryPlus:
                    node = children[0];
                    break;
                case TokenTypes.UnaryMinus:
                    if (children[0].Type == NodeTypes.Constant)
                    {
                        // Special case negating a constant to make the result nicer.
                        // This is not strictly necessary, and would be fixed by invoking Simplify() later.
                        ((ConstNode)children[0]).Value = -((ConstNode)children[0]).Value;
                        node = children[0];
                    }
                    else
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
