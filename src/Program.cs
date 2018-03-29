using System;
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
        }//change once GJ is done

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
        } //change once GJ is done

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
        }//change once GJ is done

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
            int first = matrix.GetLength(1)- 1;
            int rowToSwitch = currentRow;
            //Console.WriteLine("CR " + currentRow);
            //DisplayMatrix(matrix);
            //for(int i = currentRow; i < matrix.GetLength(1); i++)
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if(Math.Abs(matrix[currentRow,i]) > 0.001)
                {
                    currentRowLeadingCoeffPos = i;
                    first = currentRowLeadingCoeffPos;
                    //Console.WriteLine("CRLCP " + currentRowLeadingCoeffPos + " row " + i + " v " + matrix[currentRow, i]);
                    break;
                }
            }
            if(currentRowLeadingCoeffPos > currentRow)
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
                    if(currentRowLeadingCoeffPos < first)
                    {
                        first = currentRowLeadingCoeffPos;
                        rowToSwitch = i;
                    }
                }
            }
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (matrix[i, first] < smallest && Math.Abs(matrix[i, first]) > 0.001)
                {
                    smallest = matrix[i, first];
                    //Console.WriteLine("f " + matrix[i, first]);
                    rowToSwitch = i;
                }
            }
            if (currentRow != rowToSwitch )
            {
                //Console.WriteLine("SWAPPING FS");
                return SwapRows(matrix, currentRow, rowToSwitch);
            }
            return matrix;
        }

        static double[,] Organize(double[,] matrix, int currentRow)
        {
            double smallest = double.MaxValue;
            int currentRowLeadingCoeffPos = -1;
            int first = matrix.GetLength(1) - 1;
            int rowToSwitch = currentRow;
            Console.WriteLine("CR " + currentRow);
            //for(int i = currentRow; i < matrix.GetLength(1); i++)
            DisplayMatrix(matrix);
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (Math.Abs(matrix[currentRow, i]) > 0.001)
                {
                    currentRowLeadingCoeffPos = i;
                    Console.WriteLine("CRLCP " + currentRowLeadingCoeffPos + " row " + i + " v " + matrix[currentRow, i]);
                    break;
                }
            }
            if (currentRowLeadingCoeffPos > currentRow)
            {
                Console.WriteLine("TEST CRLCP " + currentRowLeadingCoeffPos + " row " + currentRow);
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
            //for (int i = 0; i < matrix.GetLength(0); i++)
            //{
            //    if (matrix[i, first] < smallest && Math.Abs(matrix[i, first]) > 0.001)
            //    {
            //        smallest = matrix[i, first];
            //        Console.WriteLine("f " + matrix[i, first]);

            //        rowToSwitch = i;
            //    }
            //}
            if (currentRow != rowToSwitch)
            {
                return SwapRows(matrix, currentRow, rowToSwitch);
            }
            return matrix;
        }//delete????

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

            if(Math.Abs(matrix[row2, posLCR1]) < 0.001)
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

        static int DetermineGreaterLC(double[,] matrix, int row1, int row2)//DELETE????
        {
            int rowG = row1;
            DisplayMatrix(matrix);
            int index1 = int.MaxValue;
            int index2 = int.MaxValue;
            int index;
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (Math.Abs(matrix[row1, i]) > 0.001)
                {
                    index1 = i;
                    break;
                }
            }
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (Math.Abs(matrix[row2, i]) > 0.001)
                {
                    index2 = i;
                    break;
                }
            }
            index = Math.Min(index1, index2);
            //for(int i = 0; i < matrix.GetLength(1); i++)
            //{
            if (matrix[row2, index] > matrix[row1, index])
            {
                //Console.WriteLine("r2 " + row2 + " r1 " + row1 + " i " + i);
                ///Console.WriteLine(" second " + matrix[row2, i] + " first " + matrix[row1, i]);
                rowG = row2;
               // break;
            }
            //}
            //Console.WriteLine("rowG = " + rowG);
            return rowG;
        }

        static double[,] RowAddScalar(double[,] matrix, int sourceRow, int destRow, double scalar)//delete!!!!!
        {
            //if (DetermineGreaterLC(matrix, sourceRow, destRow) == sourceRow)
            //{
            //    Console.WriteLine("\nBEFORE SWAP\n");
            //    DisplayMatrix(matrix);
            //    matrix = SwapRows(matrix, sourceRow, destRow);
            //    Console.WriteLine("\nafter SWAP\n");
            //    DisplayMatrix(matrix);
            //}
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                //matrix[destRow, i] = matrix[sourceRow, i] * scalar  - matrix[destRow, i] * scalar;
                matrix[destRow, i] += matrix[sourceRow, i] * -scalar ;
            }
            return matrix;
        }

        static double[,] RowAddScalar1(double[,] matrix, int sourceRow, int destRow, double[] scalar)
        {
            //if (DetermineGreaterLC(matrix, sourceRow, destRow) == sourceRow)
            //{
            //    Console.WriteLine("\nBEFORE SWAP\n");
            //    DisplayMatrix(matrix);
            //    matrix = SwapRows(matrix, sourceRow, destRow);
            //    Console.WriteLine("\nafter SWAP\n");
            //    DisplayMatrix(matrix);
            //}
            if(Math.Abs(scalar[0]) < .001)
            {
                return matrix;
            }
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                matrix[destRow, i] = matrix[sourceRow, i] * scalar[1]  - matrix[destRow, i] * scalar[0];
            }
            return matrix;
        }

        static double GCDofRow(double[,] matrix, int pos)
        {
            double gcd = matrix[pos,0];
            for(int i = 1; i < matrix.GetLength(1); i++)
            {
                gcd = GCD(matrix[pos, i], gcd);
            }
            return gcd;
        }

        static double[,] ToLowestTermsR(double[,] matrix, int pos)
        {
            double gcd = GCDofRow(matrix, pos);
            for(int i = 0; i < matrix.GetLength(1); i++)
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
            for (int i = 0; i < matrix.GetLength(0); i++) {
                matrix = ToLowestTermsR(matrix, i);
            }
            return matrix;
        }

        static double[,] ToRowEchelonForm(double[,] matrix)
        {
            double scalar = 0;
            double[] scalar1;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                matrix = FindAndSwap(matrix, i);
                //for(int j = i + 1; j < matrix.GetLength(0); j++)
                //{
                //    scalar = LCMLeadingNum(matrix, i, j);
                //    matrix = RowAddScalar(matrix, i, j, scalar);
                //}
                for (int j = i + 1; j < matrix.GetLength(0); j++)
                {
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
            double scalar = 0;
            double[] scalar1;
            for (int i = matrix.GetLength(0) - 1; i >= 0; i--)
            {
                //matrix = FindAndSwap(matrix, i);
                //for(int j = i + 1; j < matrix.GetLength(0); j++)
                //{
                //    scalar = LCMLeadingNum(matrix, i, j);
                //    matrix = RowAddScalar(matrix, i, j, scalar);
                //}
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
            //String s = "CO2 + H2O2 = C6H12O6 + O2";
            //String s = "O2 + H2 = H2O6";
            //Console.WriteLine("Input: \n" + s);

            //String answer = Solve(s);
            //Console.WriteLine("\nOutput: \n" + answer);
            Console.WriteLine("Input: \n");
            double[,] matrix = { { 1, 0, -6, 0 ,0 }, { 2, 1, -6, -2, 0 }, { 0, 2, -12, 0, 0 }, { 1, 0, 0, 0, 1 } };
            //double[,] matrix = { { 0, 2, -12, 0, 0 }, { 2, 1, -6, -2, 0 }, { 1, 0, -6, 0, 0 }, { 1, 0, 0, 0, 1 } };
            //matrix = FindAndSwap(matrix,0);
            
            DisplayMatrix(matrix);
            matrix = ToRowEchelonForm(matrix);
            Console.WriteLine("\nOutput: \n");
            DisplayMatrix(matrix);

            Console.WriteLine("\nPRESS ANY KEY TO CONTINUE: \n");
            Console.ReadKey();

        }
    }
}
