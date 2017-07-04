﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS.Parser
{
    // Basic tokenizer for processing expression strings.
    //
    public enum TokenTypes
    {
        Plus,
        Minus,
        Wedge,          // Our symbols is $
        Times,
        Div,
        Exp,
        Comma,
        LParen,
        RParen,
        Differential,
        UnaryPlus,      // These are used as intermediate values by the parser
        UnaryMinus,     // but are not directly generated by the tokeninzer.
        Number,
        String,
        Function,
        End,
    }

    public struct Token
    {
        public TokenTypes Type;
        public double DoubleValue;      // For number
        public string StringValue;      // For string or function

        public string Print()
        {
            string result = Type.ToString();
            if (Type == TokenTypes.Number)
                result += " : " + DoubleValue.ToString();
            else if (Type == TokenTypes.String)
                result += " : " + StringValue;
            else if (Type == TokenTypes.Function)
                result += " : " + StringValue + "(function)";
            return result;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case TokenTypes.Plus:
                    return "+";
                case TokenTypes.Minus:
                    return "-";
                case TokenTypes.Wedge:
                    return @"/\";
                case TokenTypes.Times:
                    return "*";
                case TokenTypes.Div:
                    return "/";
                case TokenTypes.Exp:
                    return "^";
                case TokenTypes.Comma:
                    return ",";
                case TokenTypes.LParen:
                    return "(";
                case TokenTypes.RParen:
                    return ")";
                case TokenTypes.Differential:
                    return "d_";
                case TokenTypes.UnaryPlus:
                    return "u+";
                case TokenTypes.UnaryMinus:
                    return "u-";
                case TokenTypes.Number:
                    return StringValue;
                case TokenTypes.String:
                    return StringValue;
                case TokenTypes.Function:
                    return StringValue;
                case TokenTypes.End:
                    return "";
                default:
                    Debug.Assert(false);
                    return "<error>";
            }
        }
    }

    public static class TokenExtensions
    {
        public static string ConcatTokens(this List<Token> value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var v in value)
                sb.Append(v.ToString());

            return sb.ToString();
        }
    }
    
    public class Tokenizer
    {
        // This is list of function names, which will not be returned as variables (e.g. cos, sin)
        public HashSet<String> Functions;

        public Tokenizer(string buffer, HashSet<String> functions = null)
        {
            // If the input buffer has whitespace, that's okay.
            Buffer = buffer;
            Pos = 0;
            Functions = functions;
        }
        int Pos;
        string Buffer;

        char NextChar()
        {
            if (Pos == Buffer.Length)
                return (char)0;
            return Buffer[Pos++];
        }
        char PeekNextChar()
        {
            if (Pos == Buffer.Length)
                return (char)0;
            return Buffer[Pos];
        }

        void Advance()
        {
            Debug.Assert(Pos < Buffer.Length);
            Pos++;
        }
        void Backup()
        {
            Debug.Assert(Pos > 0);
            Pos--;
        }

        void Throw(string desc)
        {
            int len = Math.Min(Buffer.Length - Pos, 10);
            string full_desc = string.Format("Tokenizing error: {0}. Ocurred at \"{1}\".", desc, Buffer.Substring(Pos, len));
            throw new ApplicationException(full_desc);
        }

        public Token NextToken()
        {
            var next = new Token();

            char c;
            do
            {
                c = NextChar();
            } while (Char.IsWhiteSpace(c));

            switch (c)
            {
                case '+':
                    next.Type = TokenTypes.Plus;
                    return next;
                case '-':
                    next.Type = TokenTypes.Minus;
                    return next;
                case '$':
                    next.Type = TokenTypes.Wedge;
                    return next;
                case '*':
                    next.Type = TokenTypes.Times;
                    return next;
                case '/':
                    if (PeekNextChar() == '\\')
                    {
                        NextChar();
                        next.Type = TokenTypes.Wedge;
                    }
                    else
                        next.Type = TokenTypes.Div;
                    return next;
                case '^':
                    next.Type = TokenTypes.Exp;
                    return next;
                case ',':
                    next.Type = TokenTypes.Comma;
                    return next;
                case '(':
                    next.Type = TokenTypes.LParen;
                    return next;
                case ')':
                    next.Type = TokenTypes.RParen;
                    return next;
                case (char)0:
                    next.Type = TokenTypes.End;
                    break;
                default:
                    if (c == 'd')
                    {
                        if (PeekNextChar() == '_')
                        {
                            NextChar();
                            next.Type = TokenTypes.Differential;
                            break;
                        }
                    }


                    if (char.IsLetter(c))
                    {
                        Backup();
                        next.StringValue = ReadString();

                        if (Functions != null && Functions.Contains(next.StringValue))
                            next.Type = TokenTypes.Function;
                        else
                            next.Type = TokenTypes.String;
                    }
                    else if (char.IsDigit(c) || c == '.')
                    {
                        Backup();
                        ReadDouble(ref next);
                        next.Type = TokenTypes.Number;
                    }
                    else
                        Throw("Unknown characters");
                    break;
            }

            return next;
        }

        public List<Token> GetTokenList()
        {
            var l = new List<Token>();
            while (true)
            {
                Token t = NextToken();
                l.Add(t);
                if (t.Type == TokenTypes.End)
                    break;
            }
            return l;
        }

        void ReadDouble(ref Token token)
        {
            // Note that we don't support scientfic notation.  1.0E+10 will become 3 tokens.

            bool have_period = false;
            int start = Pos;

            for (;;)
            {
                char c = PeekNextChar();
                if (char.IsDigit(c))
                {
                    // continue advancing
                }
                else if (c == '.')
                {
                    if (have_period)
                        break;
                    have_period = true;
                    // continue advancing
                }
                else
                    break;

                Advance();
            }

            int len = Pos - start;
            if (len == 1 && have_period)
            {
                Backup();
                Throw("numbers should follow decimal point");
            }

            token.StringValue = Buffer.Substring(start, len);
            token.DoubleValue = double.Parse(token.StringValue);
        }

        string ReadString()
        {
            int start = Pos;

            for (;;)
            {
                char c = PeekNextChar();

                if (!char.IsLetter(c))
                    break;
                Advance();
            }

            string result = Buffer.Substring(start, Pos - start);
            return result;
        }
    }
}
