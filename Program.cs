﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ChemEqnBalancer
{
    
    class Program
    {
        static String[] GetRight(string eqn)
        {
            eqn = eqn.Substring(eqn.IndexOf("=") + 1);
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
            for (int i = 0; i < s.Length; i++)
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

            //COMMENTED ONLY FOR TESTING THIS NEEDS TO BE REVERTED!!!!!!!!!!!!!!!!!!!!!
            //for (int i = 0; i < numElems; i++)
            //{
            //    if (!MyGLobals.elements.Contains(elementsUsed[i]))
            //    {
            //        MyGLobals.elements.Add(elementsUsed[i]);
            //    }
            //}

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
        }//array

        private static int indexOf(string[,] s1, string s2)
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

        static double[,] PopulateMatrix(List<String[,]> results, int lSize, int rSize, Equation eqn)
        {
            double[,] matrix = new double[eqn.elements.Count + 1, lSize + rSize + 1];

            matrix[eqn.elements.Count, 0] = 1;
            matrix[eqn.elements.Count, lSize + rSize] = 1;
            for (int i = 0; i < eqn.elements.Count; i++)
            {
                int j = 0;
                for (; j < lSize; j++)
                {
                    if (indexOf(results[j], eqn.elements[i]) > -1)
                    {
                        matrix[i, j] = double.Parse(results[j][1, indexOf(results[j], eqn.elements[i])]);
                    }
                }
                for (; j < lSize + rSize; j++)
                {
                    if (indexOf(results[j], eqn.elements[i]) > -1)
                    {
                        matrix[i, j] = -int.Parse(results[j][1, indexOf(results[j], eqn.elements[i])]);
                    }
                }
            }
            //Console.WriteLine("//////////////////////");
            //DisplayMatrix(matrix);
            //Console.WriteLine("//////////////////////");
            return matrix;
        }

        static double[] GetAdjunctCol(double[,] matrix)
        {
            double[] adjCol = new double[matrix.GetLength(0)];
            for (int i = 0; i < adjCol.Length; i++)
            {
                adjCol[i] = matrix[i, matrix.GetLength(1) - 1];
            }
            return adjCol;
        }

        static double[] GetDiagonal(double[,] matrix)
        {
            double[] diagonal = new double[matrix.GetLength(0)];
            for (int i = 0; i < diagonal.Length; i++)
            {
                diagonal[i] = matrix[i, i];
            }
            return diagonal;
        }

        static double GetLCMDiagonal(double[,] matrix)
        {
            double lcm = matrix[0, 0];
            //DisplayMatrix(matrix);
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                lcm = LCM(lcm, matrix[i, i]);
            }
            return lcm;
        }

        static int[] DetermineCoefficients(double[,] matrix)
        {
            double[] solution = GetAdjunctCol(matrix);
            double[] diagonal = GetDiagonal(matrix);
            double lcm = GetLCMDiagonal(matrix);
            // Console.WriteLine("LCM " + lcm);

            //Console.WriteLine("FINAL COL \n" );
            for (int i = 0; i < solution.Length; i++)
            {
                //Console.Write(solution[i] + " ");
                solution[i] = lcm * solution[i];

            }


            for (int i = 0; i < solution.Length; i++)
            {
                //Console.WriteLine(solution[i]);
                solution[i] = solution[i] / diagonal[i];

            }

            int[] solutionI = new int[solution.Length];

            for (int i = 0; i < solution.Length; i++)
            {
                //Console.WriteLine(solution[i]);
                solutionI[i] = (int)(solution[i]);
            }
            return solutionI;
        }

        static double LCM(double a, double b)//
        {
            double lcm = 0;
            lcm = a * b;
            double gcd = GCD(a, b);
            return lcm / gcd;
        }


        static string InsertCoeff(int[] answer, String[] left, String[] right)
        {
            StringBuilder final = new StringBuilder();
            int i = 0;
            for (; i < left.Length; i++)
            {
                if (answer[i] > 1)
                {
                    left[i] = answer[i].ToString() + left[i];
                }
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

        static string Solve(string s)
        {
            Equation eqn = new Equation(s);
            String[] left = eqn.GetLeft();
            String[] right = eqn.GetRight();
            //for(int i = 0; i < 2; i++)
            //{
            //    Console.WriteLine("l " + left[i]);
            //    Console.WriteLine("r " + right[i]);
            //}

            List<String[,]> results = new List<String[,]>();

            for (int k = 0; k < left.Length; k++)
            {
                results.Add(eqn.Parse(left[k]));
            }

            for (int k = 0; k < right.Length; k++)
            {
                results.Add(eqn.Parse(right[k]));
            }

            double[,] matrix = PopulateMatrix(results, left.Length, right.Length, eqn);
            matrix = SolveMatrix(matrix);
            //Console.WriteLine("???????????????????????");
            //DisplayMatrix(matrix);
            //Console.WriteLine("???????????????????????");
            return InsertCoeff(DetermineCoefficients(matrix), left, right);
        }

        static double[,] SwapRows(double[,] matrix, int r1, int r2)
        {
            double temp;
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                temp = matrix[r1, i];
                matrix[r1, i] = matrix[r2, i];
                matrix[r2, i] = temp;
            }
            return matrix;
        }

        static double[,] FindAndSwap(double[,] matrix, int currentRow)
        {
            double smallest = double.MaxValue;
            int currentRowLeadingCoeffPos = -1;
            int first = matrix.GetLength(1) - 1;
            int rowToSwitch = currentRow;
            //Console.WriteLine("CR " + currentRow);
            //DisplayMatrix(matrix);
            //for(int i = currentRow; i < matrix.GetLength(1); i++)
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (Math.Abs(matrix[currentRow, i]) > 0.001)
                {
                    currentRowLeadingCoeffPos = i;
                    first = currentRowLeadingCoeffPos;
                    //Console.WriteLine("CRLCP " + currentRowLeadingCoeffPos + " row " + i + " v " + matrix[currentRow, i]);
                    break;
                }
            }
            if (currentRowLeadingCoeffPos > currentRow)
            {
                //Console.WriteLine("TEST CRLCP " + currentRowLeadingCoeffPos + " row " + currentRow );
                for (int i = currentRow; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrix[i, j] != 0)
                        {
                            currentRowLeadingCoeffPos = j;
                            break;
                        }
                    }
                    if (currentRowLeadingCoeffPos < first)
                    {
                        first = currentRowLeadingCoeffPos;
                        rowToSwitch = i;
                    }
                }
            }
            for (int i = first; i < matrix.GetLength(0); i++)
            {
                if (matrix[i, first] < smallest && Math.Abs(matrix[i, first]) > 0.001)
                {
                    smallest = matrix[i, first];
                    //Console.WriteLine("f " + matrix[i, first]);
                    rowToSwitch = i;
                }
            }
            if (currentRow != rowToSwitch)
            {
                //Console.WriteLine("SWAPPING FS");
                return SwapRows(matrix, currentRow, rowToSwitch);
            }
            return matrix;
        }

        static double LCMLeadingNum(double[,] matrix, int row1, int row2)//DELETE??????????
        {
            double lcm = 0;
            int posLCR1 = 0;
            int posLCR2 = 0;

            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (Math.Abs(matrix[row1, i]) > 0.001)
                {
                    posLCR1 = i;
                    break;
                }
            }

            if (Math.Abs(matrix[row2, posLCR1]) < 0.001)
            {
                return 0;
            }
            lcm = matrix[row1, posLCR1] * matrix[row2, posLCR1];
            double gcd = GCD(matrix[row1, posLCR1], matrix[row2, posLCR1]);
            return lcm / gcd;
        }

        static double[] FindScalars(double[,] matrix, int row1, int row2)
        {
            double[] lcm = new double[2];
            int posLCR1 = 0;
            int posLCR2 = 0;

            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (Math.Abs(matrix[row1, i]) > 0.001)
                {
                    posLCR1 = i;
                    lcm[0] = matrix[row1, i];
                    break;
                }
            }

            lcm[1] = matrix[row2, posLCR1];
            if (Math.Abs(matrix[row2, posLCR1]) < 0.001)
            {
                lcm[0] = 0;
                return lcm;
            }

            return lcm;
        }

        static double GCD(double x, double y)
        {
            double a = Math.Abs(x);
            double b = Math.Abs(y);
            double c;
            while (b > 0.001)
            {
                c = a % b;
                a = b;
                b = c;
            }
            return a;
        }

        static double[,] RowAddScalar1(double[,] matrix, int sourceRow, int destRow, double[] scalar)
        {
            if (Math.Abs(scalar[0]) < .001)
            {
                return matrix;
            }
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                matrix[destRow, i] = matrix[sourceRow, i] * scalar[1] - matrix[destRow, i] * scalar[0];
            }
            return matrix;
        }

        static double GCDofRow(double[,] matrix, int pos)
        {
            double gcd = matrix[pos, 0];
            for (int i = 1; i < matrix.GetLength(1); i++)
            {
                gcd = GCD(matrix[pos, i], gcd);
            }
            return gcd;
        }

        static double[,] ToLowestTermsR(double[,] matrix, int pos)
        {
            double gcd = GCDofRow(matrix, pos);
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                matrix[pos, i] /= gcd;
            }
            return matrix;
        }

        static int[] GetSigns(double[,] matrix)
        {
            int[] signs = new int[matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (Math.Abs(matrix[i, j]) > .001 && matrix[i, j] > 0)
                    {
                        signs[i] = 1;
                        break;
                    }
                    else if (Math.Abs(matrix[i, j]) > .001 && matrix[i, j] < 0)
                    {
                        signs[i] = -1;
                        break;
                    }
                }
            }

            return signs;
        }

        static double[,] ToPositive(double[,] matrix)
        {
            int[] signs = GetSigns(matrix);

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = matrix[i, j] * signs[i];
                }
            }
            return matrix;
        }

        static double[,] ToLowestTermsM(double[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                matrix = ToLowestTermsR(matrix, i);
            }
            return matrix;
        }

        //static double[,] ToRowEchelonForm(double[,] matrix)
        static double[,] SolveMatrix(double[,] matrix)
        {
            //Console.WriteLine("ROW");
            double scalar = 0;
            double[] scalar1;
            //matrix = ToPositive(ToLowestTermsM(matrix));
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                matrix = FindAndSwap(matrix, i);
                for (int j = i + 1; j < matrix.GetLength(0); j++)
                {
                    //matrix = ToPositive(ToLowestTermsM(matrix));
                    //Console.WriteLine("\nI " + i + " j " + j);
                    //Console.WriteLine("FIRST");
                    //DisplayMatrix(matrix);
                    scalar1 = FindScalars(matrix, i, j);
                    //Console.WriteLine("Sclar " + scalar);
                    matrix = RowAddScalar1(matrix, i, j, scalar1);
                    //Console.WriteLine("LAST");
                    //DisplayMatrix(matrix);
                }
            }

            matrix = ToPositive(ToLowestTermsM(matrix));
            // Console.WriteLine("\n//////////////////////////////////////////////////////////\n");
            // DisplayMatrix(matrix);
            //Console.WriteLine("\n//////////////////////////////////////////////////////////\n");
            matrix = ToRedRowEchelon(matrix);
            // Console.WriteLine("\n//////////////////////////////////////////////////////////\n");
            return ToPositive(ToLowestTermsM(matrix));
        }



        static double[] FindPositionalScalars(double[,] matrix, int bottomRow, int topRow)
        {
            double[] scalars = new double[2];
            scalars[0] = matrix[bottomRow, bottomRow];
            scalars[1] = matrix[topRow, bottomRow];
            return scalars;
        }

        static double[,] ToRedRowEchelon(double[,] matrix)
        {
            //Console.WriteLine("RED");
            double scalar = 0;
            double[] scalar1;

            for (int i = matrix.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    //Console.WriteLine("\nI " + i + " j " + j);
                    //Console.WriteLine("FIRST");
                    //DisplayMatrix(matrix);
                    scalar1 = FindPositionalScalars(matrix, i, j);
                    //Console.WriteLine("Sclar " + scalar1[0] + " " + scalar1[1]);
                    matrix = RowAddScalar1(matrix, i, j, scalar1);
                    //Console.WriteLine("LAST");
                    //DisplayMatrix(matrix);
                }
            }
            return matrix;
        }

        static void DisplayMatrix(double[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            String s1 = "CO2 + H2O = C6H12O6 + O2";
            String s2 = "CO2 + H2O2 = C6H12O6 + O2";
            String s3 = "C6H12O6 + O2 = CO2 + H2O";
            String s4 = "H2O6 = O2 + H2";
            String s5 = "O2 + H2 = H2O6";
            String answer;
            //Equation e = new Equation(s1);



            Console.WriteLine("\nInput: \n" + s1);
            Matrix m = new Matrix(s1);
            answer = m.Solve();
            Console.WriteLine("Output: \n" + answer);

            Console.WriteLine("\nInput: \n" + s2);
            m = new Matrix(s2);
            answer = m.Solve();
            Console.WriteLine("Output: \n" + answer);

            Console.WriteLine("\nInput: \n" + s3);
            m = new Matrix(s3);
            answer = m.Solve();
            Console.WriteLine("Output: \n" + answer);


            Console.WriteLine("\nInput: \n" + s4);
            m = new Matrix(s4);
            answer = m.Solve();
            Console.WriteLine("Output: \n" + answer);

            Console.WriteLine("\nInput: \n" + s5);
            m = new Matrix(s5);
            answer = m.Solve();
            Console.WriteLine("Output: \n" + answer);






            //Console.WriteLine("\nInput: \n" + s1);
            //answer = Solve(s1);
            //Console.WriteLine("Output: \n" + answer);

            //Console.WriteLine("\nInput: \n" + s2);
            //answer = Solve(s2);
            //Console.WriteLine("Output: \n" + answer);

            //Console.WriteLine("\nInput: \n" + s3);
            //answer = Solve(s3);
            //Console.WriteLine("Output: \n" + answer);


            //Console.WriteLine("\nInput: \n" + s4);
            //answer = Solve(s4);
            //Console.WriteLine("Output: \n" + answer);

            //Console.WriteLine("\nInput: \n" + s5);
            //answer = Solve(s5);
            //Console.WriteLine("Output: \n" + answer);




            //Console.WriteLine("Input: \n");
            //double[,] matrix = { { 1, 0, -6, 0 ,0 }, { 2, 1, -6, -2, 0 }, { 0, 2, -12, 0, 0 }, { 1, 0, 0, 0, 1 } };
            //double[,] matrix = { { 0, 2, -12, 0, 0 }, { 2, 1, -6, -2, 0 }, { 1, 0, -6, 0, 0 }, { 1, 0, 0, 0, 1 } };
            //double[,] matrix = { { 5, 0, -6, 8, 0 }, { 2, 1, -6, -2, 0 }, { 0, 2, -12, 0, 0 }, { 0, 0, 0, 5, 0 }, { 1, 0, 0, 0, 1 } };
            //matrix = FindAndSwap(matrix,0);
            //double[,] matrix = { { 109, -6, -6, -6 }, { -6, 74, -36, -36 }, { -6, -36, 74, -36 }, { -6, -36, -36, 74 } };
            //double[,] matrix = { { 1, 0, -6, 0, 0 }, { 6, 0, -1, -6, 0 }, { 0, 3, -4, -0, 0 }, { 1, 0, 0, 0, 1 } };
            //DisplayMatrix(matrix);
            //matrix = SolveMatrix(matrix);
            //Console.WriteLine("\nOutput: \n");
            //DisplayMatrix(matrix);

            Console.WriteLine("\nPRESS ANY KEY TO CONTINUE: \n");
            Console.ReadKey();

        }
    }
}