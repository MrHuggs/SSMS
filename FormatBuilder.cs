using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    // This is a replacement for string builder, used when we format expressions:
    class FormatBuilder
    {
        StringBuilder Builder = new StringBuilder();
        int ParenDepth = 0;

        public void Append(string str)
        {
            Builder.Append(str);
        }

        public void Append<T>(T t)
        {
            Builder.Append(t.ToString());
        }

        public void BeginParen()
        {
            ParenDepth++;
            Builder.Append("(");
        }

        public void EndParen()
        {
            ParenDepth--;
            Debug.Assert(ParenDepth >= 0);
            Builder.Append(")");
        }

        public void Clear()
        {
            Builder.Clear();
            ParenDepth = 0;
        }

        public override string ToString()
        {
            string raw = Builder.ToString();

            // Remove all white space:
            raw = raw.Replace(" ", "");

            StringBuilder final_string = new StringBuilder();

            int l = raw.Length;
            int i = 0;

            if (raw[0] == '(')      // If there is a leading parent, skip it.
            {
                i++;
                l--;
            }
            
            for (;  i < l - 1; i++)
            {
                // Turn +- and -= into just -:
                if ((raw[i] == '+' && raw[i + 1] == '-') ||
                    (raw[i] == '-' && raw[i + 1] == '+'))
                {
                    final_string.Append('-');
                    i++;
                    continue;
                }

                // Supress uneeded + after left paren:
                if (raw[i] == '+' && (i == 0 || raw[i -1] == '(') )
                {
                    continue;  
                }
                final_string.Append(raw[i]);
            }
            final_string.Append(raw[i]);

            //Console.WriteLine("Raw:  " + raw);
            //Console.WriteLine("Proc: " + final_string.ToString());

            return final_string.ToString();
        }
    }
}
