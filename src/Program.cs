﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ConsoleApp1
{
    public static class MyGLobals
    {
        public static List<String> elements = new List<string>();
        public static List<String[,]> results = new List<String[,]>();
        public static String[] left;
        public static String[] right;
    }
    class Program
    {
        static String[] GetRight(string eqn)
        {
            eqn =  eqn.Substring(eqn.IndexOf("=") + 1);
            int numPlus = NumOcc(eqn, "+");
            string[] s = new string[numPlus + 1];
            eqn = eqn.Trim();
            s = eqn.Split('+');
            for(int i = 0; i< s.Length; i++) {
                s[i] = s[i].Trim();
            }
            return s;
        }

        static String[] GetLeft(string eqn)
        {
            eqn = eqn.Substring(0, eqn.IndexOf("="));
            int numPlus = NumOcc(eqn, "+");
            string[] s = new string[numPlus + 1];
            eqn = eqn.Trim();
            s = eqn.Split('+');
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = s[i].Trim();
            }
            return s;
        }

        static int NumOcc(string s, string c)
        {
            int count = 0;
            for(int i = 0; i < s.Length; i++)
            {
                if (s.ElementAt(i).Equals(c))
                {
                    count++;
                }
            }
            return count;
        }
        
        static string[,] Parse(string s)
        {  
            char[] chem = s.ToCharArray();
            int[] numInChem = new int[s.Length];
            string[] elementsUsed = new string[s.Length];
            string temp = chem[0].ToString();
            int numElems = 0;
            int digit;

            for (int i = 1; i < s.Length; i++)
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
                if (!MyGLobals.elements.Contains(elementsUsed[i]))
                {
                    MyGLobals.elements.Add(elementsUsed[i]);
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

        private static int indexOf(string[] s1, string s2)
        {
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].Equals(s2))
                {
                    return i;
                }
            }
            return -1;
        }

        private static int indexOf(string[,] s1, string s2)
        {
            for (int i = 0; i < s1.Length/2; i++)
            {
                if (s1[0,i].Equals(s2))
                {
                    return i;
                }
            }
            return -1;
        }

        static double[,] PopulateMatrix(List<String[,]> results, int lSize, int rSize)
        {
            double[,] matrix = new double[MyGLobals.elements.Count + 1, lSize + rSize];
            matrix[MyGLobals.elements.Count, 0] = 1;
            for (int i = 0; i < MyGLobals.elements.Count; i++)
            {
                int j = 0;
                for (; j < lSize; j++)
                {
                    if (indexOf(results[j], MyGLobals.elements[i]) > -1)
                    {
                        matrix[i, j] = double.Parse(results[j][1, indexOf(results[j], MyGLobals.elements[i])]);
                    }
                }
                for (; j < lSize + rSize; j++)
                {
                    if (indexOf(results[j], MyGLobals.elements[i]) > -1)
                    {
                        matrix[i, j] = -int.Parse(results[j][1, indexOf(results[j], MyGLobals.elements[i])]);
                    }
                }
            }
            return matrix;
        }

        static int[] Reduce(double[,] matrix)
        {
            Matrix<double> m = Matrix<double>.Build.DenseOfArray(matrix);
            Console.WriteLine("m rows " + m.RowCount);
            Console.WriteLine("m cols " + m.ColumnCount);
            int blah = MyGLobals.elements.Count + 1;
            Console.WriteLine("v size " + blah);
            double[] vector = new double[MyGLobals.elements.Count + 1];
            vector[vector.Length - 1] = 1;

            Vector<double> aug = Vector<double>.Build.Dense(vector);
            Vector<double> x = m.Solve(aug);
            int[] solution = new int[x.Count];
            double lowest = x.ElementAt(0);
            double temp;

            for (int i = 1; i < solution.Length; i++)
            {
                if (lowest > x.ElementAt(i))
                    lowest = x.ElementAt(i);
            }
            
            for (int i = 0; i < solution.Length; i++)
            {
                temp = (x.ElementAt(i) / lowest);
                if ((1 - temp % 1) <= .001)
                {
                    temp += .001;
                }
                solution[i] = (int)(temp);
            }
            return solution;
        }

        static string InsertCoeff(int[] answer, String[] left, String[] right)
        {
            StringBuilder final = new StringBuilder();
            int i = 0;
            for (; i < left.Length; i++)
            {
                left[i] = answer[i].ToString() + left[i];
            }
            for (int j = 0; j < right.Length; j++)
            {
                if (answer[i] > 1)
                {
                    right[j] = answer[i].ToString() + right[j];
                }
                i++;
            }

            for (i = 0; i < left.Length; i++)
            {
                if (i > 0)
                {
                    final.Append(" + ");
                }
                final.Append(left[i]);
            }
            final.Append(" = ");

            for (i = 0; i < right.Length; i++)
            {
                if (i > 0)
                {
                    final.Append(" + ");
                }
                final.Append(right[i]);
            }
            return final.ToString();
        }

        static string Solve(string eqn)
        {
            String[] left = GetLeft(eqn);
            String[] right = GetRight(eqn);
            List<String[,]> results = new List<String[,]>();

            for (int k = 0; k < left.Length; k++)
            {
                results.Add(Parse(left[k]));
            }

            for (int k = 0; k < right.Length; k++)
            {
                results.Add(Parse(right[k]));
            }

            double[,] matrix = PopulateMatrix(results, left.Length, right.Length);

            return InsertCoeff(Reduce(matrix), left, right);
        }

        static void Main(string[] args)
        {
            //String s = "CO2 + H2O2 = C6H12O6 + O2";
            String s = "O2 + H2 = H2O6";
            Console.WriteLine("Input: \n" + s);

            String answer = Solve(s);
            Console.WriteLine("\nOutput: \n" + answer);

            Console.ReadKey();

        }
    }
}