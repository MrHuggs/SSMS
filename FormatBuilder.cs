using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    // This is a replacement for string builder, used when we format expressions:
    public class FormatBuilder
    {
        StringBuilder Builder = new StringBuilder();

        public void Append(string str, bool parenthesize = false)
        {
            if (parenthesize)
            {
                Builder.Append('(');
                Builder.Append(str);
                Builder.Append(')');
            }
            else
                Builder.Append(str);
        }

        public void Append<T>(T t, bool parenthesize = false)
        {
            Append(t.ToString(), parenthesize);
        }


        public void Clear()
        {
            Builder.Clear();
        }

        public override string ToString()
        {
            string raw = Builder.ToString();

            StringBuilder final_string = new StringBuilder();

            int l = raw.Length;
            int i = 0;

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
