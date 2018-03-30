using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemEqnBalancer
{
    class Equation
    {
        public  List<String> elements = new List<string>();
        public  List<String[,]> results = new List<String[,]>();
        public static String[] left;
        public static String[] right;
        static string eqn;

        public Equation(string s)
        {
            eqn = s;
            //this.Parse();
        }

        public String[] GetRight()
        {
            string temp;
            temp = eqn.Substring(eqn.IndexOf("=") + 1);
            int numPlus = NumOcc(temp, "+");
            string[] s = new string[numPlus + 1];
            temp = temp.Trim();
            s = temp.Split('+');
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = s[i].Trim();
            }
            return s;
        }

         public String[] GetLeft()
        {
            string temp;
            temp = eqn.Substring(0, eqn.IndexOf("="));
            int numPlus = NumOcc(eqn, "+");
            string[] s = new string[numPlus + 1];
            temp = temp.Trim();
            s = temp.Split('+');
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = s[i].Trim();
            }
            return s;
        }

        int NumOcc(string s, string c)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s.ElementAt(i).Equals(c))
                {
                    count++;
                }
            }
            return count;
        }

        public string[,] Parse(string eqn)
        {
            char[] chem = eqn.ToCharArray();
            int[] numInChem = new int[eqn.Length];
            string[] elementsUsed = new string[eqn.Length];
            string temp = chem[0].ToString();
            int numElems = 0;
            int digit;

            for (int i = 1; i < eqn.Length; i++)
            {
                if (char.IsLower(chem[i]))
                {
                    temp = temp + chem[i].ToString();
                }
                else if (char.IsDigit(chem[i]))
                {
                    digit = int.Parse(chem[i].ToString());
                    if (i + 1 < chem.Length && char.IsDigit(chem[i + 1]))
                    {
                        digit = digit * 10 + int.Parse(chem[i + 1].ToString());
                        i++;
                    }
                    if (!elementsUsed.Contains(temp))
                    {
                        elementsUsed[numElems++] = temp;
                    }
                    numInChem[indexOf(elementsUsed, temp)] += digit - 1;//to account for sloppy double counting
                    digit = 0;
                }
                else if (char.IsUpper(chem[i]))
                {
                    if (!elementsUsed.Contains(temp))
                    {
                        elementsUsed[numElems++] = temp;
                    }
                    numInChem[indexOf(elementsUsed, temp)]++;
                    temp = chem[i].ToString();
                }
            }
            if (!elementsUsed.Contains(temp)) //Include last element if not already included
            {
                elementsUsed[numElems++] = temp;
            }
            numInChem[indexOf(elementsUsed, temp)]++;

            for (int i = 0; i < numElems; i++)
            {
                if (!elements.Contains(elementsUsed[i]))
                {
                    elements.Add(elementsUsed[i]);
                }
            }

            String[,] result = new String[2, numElems];
            for (int i = 0; i < numElems; i++)
            {
                result[0, i] = elementsUsed[i];
            }
            for (int i = 0; i < numElems; i++)
            {
                result[1, i] = numInChem[indexOf(elementsUsed, elementsUsed[i])].ToString();
            }

            return result;
        }

        private  int indexOf(string[] s1, string s2)
        {
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].Equals(s2))
                {
                    return i;
                }
            }
            return -1;
        }//array

        private  int indexOf(string[,] s1, string s2)
        {
            for (int i = 0; i < s1.Length / 2; i++)
            {
                if (s1[0, i].Equals(s2))
                {
                    return i;
                }
            }
            return -1;
        }//matrix
    }
}
