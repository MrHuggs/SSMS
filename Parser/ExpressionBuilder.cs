using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SSMS.Nodes;

namespace SSMS.Parser
{
    // Simple parser for creating expression trees from token lists.

    public class ExpNode 
    {
        public string String;
        public double Number;
        public List<ExpNode> Arguments = new List<ExpNode>();
        public TokenTypes Type;
        public bool FunctionTerminal;

        public void Print(string indent = "")
        {
            Console.WriteLine(indent + String);
            indent += "      ";
            foreach (var v in Arguments)
            {
                v.Print(indent);
            }
        }
    };

    public class ExpressionBuilder
    {
        public List<ExpNode> Output = new List<ExpNode>();

        // Actually, used as a stack of operators:
        List<TokenTypes> Operators = new List<TokenTypes>();

        List<Token> Tokens; // This is the input token stream.
        int Index;
        Token PeekNext() { return Tokens[Index]; }

        void Advance()
        {
            // Consume the next token. If we are at the end, just stay at the end token.
            Index = Math.Min(Index + 1, Tokens.Count - 1);
        }
        void Require(TokenTypes type)
        {
            if (PeekNext().Type != type)
                Throw(string.Format("Expected {0}", type.ToString()));
            Advance();
        }

        void Throw(string desc = "Syntax Error")
        {
            string full_desc = string.Format("Parsing error: {0}", desc);
            throw new ApplicationException(full_desc);
        }

        public ExpressionBuilder(List<Token> tokens)
        {
            Tokens = tokens;
            Debug.Assert(Tokens.Last().Type == TokenTypes.End);
            Index = 0;
        }
                
        void PopOperator()
        {
            int opcount = Operators.Count;
            if (opcount < 1)
                Throw("Missing operator");

            ExpNode en = new ExpNode();

            var op = Operators[opcount - 1];
            Operators.RemoveAt(opcount - 1);
            en.Type = op;
            en.String = op.ToString();

            // Depending on the operator we may need 1 or 2 arguments:
            int argcount = ArgCount(op);

            int count = Output.Count;
            if (count < argcount)
                Throw("Insufficent argmuents");

            if (argcount == 2)
            {
                en.Arguments.Add(Output[count - 2]);
                Output.RemoveAt(count - 2);
            }
            else
                Debug.Assert(argcount == 1);

            en.Arguments.Add(Output.Last());
            Output.RemoveAt(Output.Count - 1);

            Output.Add(en);
        }

        // This is the shunting yard algorithm, (https://en.wikipedia.org/wiki/Shunting-yard_algorithm),
        // enhanced to handle functions and unary +/-.
        public List<ExpNode> Parse()
        {
            for (;;)
            {
                Token next = PeekNext();
                Advance();

                if (next.Type == TokenTypes.End)
                    break;

                if (next.Type == TokenTypes.Differential)
                {
                    next = PeekNext();
                    if (next.Type != TokenTypes.String)
                        Throw("Differential must be followed by varialbe name");
                    Advance();

                    var nn = new ExpNode();
                    nn.String = next.StringValue;
                    nn.Type = TokenTypes.Differential;
                    Output.Add(nn);
                    continue;
                }

                if (next.Type == TokenTypes.Number)
                {
                    var nn = new ExpNode();
                    nn.String = next.ToString();
                    nn.Number = next.DoubleValue;
                    nn.Type = TokenTypes.Number;
                    Output.Add(nn);
                    continue;
                }

                if (next.Type == TokenTypes.String)
                {
                    var nn = new ExpNode();
                    nn.String = next.StringValue;
                    nn.Type = TokenTypes.String;
                    Output.Add(nn);
                    continue;
                }

                if (next.Type == TokenTypes.LParen)
                {
                    Operators.Add(TokenTypes.LParen);
                    continue;
                }

                if (next.Type == TokenTypes.Function)
                {
                    Require(TokenTypes.LParen);

                    var fn = new ExpNode();
                    fn.Type = TokenTypes.Function;
                    fn.FunctionTerminal = true;
                    fn.String = next.StringValue;
                    Output.Add(fn);

                    Operators.Add(TokenTypes.Function);
                    Operators.Add(TokenTypes.LParen);
                    continue;

                }

                if (next.Type == TokenTypes.RParen)
                {
                    ParseClosingParen();
                    continue;
                }

                if (ParseUnary(next))
                        continue;
                    

                if (next.Type >= TokenTypes.Plus && next.Type <= TokenTypes.Exp)
                {
                    ProcessOperator(next);
                    continue;
                }
            }

            // Input token stream has been processed - handle any remaining operators:
            while (Operators.Count > 0)
            {
                PopOperator();
            }

            return Output;
        }

        void ProcessOperator(Token next)
        {
            int next_prec = Precendece(next.Type);

            for (;;)
            {
                if (Operators.Count == 0)
                    break;

                TokenTypes type = Operators.Last();
                int prec = Precendece(type);
                if (prec > next_prec || (prec == next_prec && IsLeftAssoc(next.Type)))
                {
                    PopOperator();
                }
                else
                    break;
            }
            Operators.Add(next.Type);
        }

        void ParseClosingParen()
        {
            for (;;)
            {
                if (Operators.Count == 0)
                    Throw("Umatched parenthesis");

                if (Operators.Last() == TokenTypes.LParen)
                {
                    Operators.RemoveAt(Operators.Count - 1);

                    if (Operators.Count > 0 && Operators.Last() == TokenTypes.Function)
                    {
                        Operators.RemoveAt(Operators.Count - 1);
                        for (int i = Output.Count - 1; ; i--)
                        {
                            Debug.Assert(i >= 0); // We should always be able to find a function call.
                            if (Output[i].FunctionTerminal == true)
                            {
                                Output[i].FunctionTerminal = false;
                                for (int j = i + 1; j < Output.Count; j++)
                                {
                                    Output[i].Arguments.Add(Output[j]);
                                }
                                while (Output.Count > i + 1)
                                    Output.RemoveAt(Output.Count - 1);
                                break;
                            }
                        }
                    }
                    break;
                }
                PopOperator();
            }
        }

        bool ParseUnary(Token next)
        {
            if (next.Type != TokenTypes.Plus && next.Type != TokenTypes.Minus)
                return false;

            bool unary = false;

            if (Index == 1)     // As we have already advanced, this means we are on the first token.
            {   // If the first token is +-, it must be unary:
                unary = true;
            }
            else
            {   // If the +/ follows any operator or (, is must be unary:
                var prev_token = Tokens[Index - 2].Type;
                if (prev_token >= TokenTypes.Plus && prev_token <= TokenTypes.LParen)
                {
                    unary = true;
                }
            }
            if (!unary)
                return false;

            Operators.Add(MakeUnary(next.Type));
            return true;
        }

        public void Print()
        {
            foreach (var v in Output)
            {
                v.Print();
            }
        }

        // Operator precedence table. This is not exactly the same as SymNode NodeTypes.
        static readonly int[] PrecedenceTable =
        {
                1, //Plus
                1, //Minus
                2, //Wedge
                3, //Times
                3, //Div
                4, //Exp
                0, // Comma
                -1,//LParen 
                -1,//RParen - should never be used
                6, //Differential
                5, //UnaryPlus
                5, //UnaryMinus
            };

        static int Precendece(TokenTypes type)
        {
            Debug.Assert(PrecedenceTable.Length == (int) TokenTypes.UnaryMinus + 1);
            return PrecedenceTable[(int)type];
        }
        static bool IsLeftAssoc(TokenTypes type)
        {
            return type != TokenTypes.Exp;
        }

        // Returns how many arguments a particular operator requires:
        static int ArgCount(TokenTypes type)
        {
            switch (type)
            {
                case TokenTypes.UnaryPlus:
                case TokenTypes.UnaryMinus:
                    return 1;
                default:
                    return 2;
            }
        }
        static TokenTypes MakeUnary(TokenTypes type)
        {
            switch (type)
            {
                case TokenTypes.Plus:
                    return TokenTypes.UnaryPlus;
                case TokenTypes.Minus:
                    return TokenTypes.UnaryMinus;
                default:
                    Debug.Assert(false);
                    return TokenTypes.End;
            }
        }
    }

}
