using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemEqnBalancer
{
    class Matrix
    {
        private int rows;
        private int cols;
        private double[,] matrix;
        String[] left;
        String[] right;
        public Matrix(String s)
        {
            Equation eqn = new Equation(s);
            left = eqn.GetLeft();
            right = eqn.GetRight();
            //for (int i = 0; i < 2; i++)
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

            PopulateMatrix(results, left.Length, right.Length, eqn);
            //matrix = SolveMatrix(matrix);
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

        private void PopulateMatrix(List<String[,]> results, int lSize, int rSize, Equation eqn)
        {
             matrix = new double[eqn.elements.Count + 1, lSize + rSize + 1];

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
            //DisplayMatrix();
            //Console.WriteLine("//////////////////////");
            //return matrix;
        }

        double[] GetAdjunctCol()
        {
            double[] adjCol = new double[matrix.GetLength(0)];
            for (int i = 0; i < adjCol.Length; i++)
            {
                adjCol[i] = matrix[i, matrix.GetLength(1) - 1];
            }
            return adjCol;
        }

         double[] GetDiagonal()
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
            double temp;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                temp = matrix[i, i];
                lcm = LCM(lcm, temp);
            }
            return lcm;
        }

         int[] DetermineCoefficients(double[,] matrix)
        {
            double[] solution = GetAdjunctCol();
            double[] diagonal = GetDiagonal();
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


         string InsertCoeff(int[] answer, String[] left, String[] right)
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

        public string Solve()
        {
            //Equation eqn = new Equation(s);
            //String[] left = eqn.GetLeft();
            //String[] right = eqn.GetRight();
            ////for(int i = 0; i < 2; i++)
            ////{
            ////    Console.WriteLine("l " + left[i]);
            ////    Console.WriteLine("r " + right[i]);
            ////}

            //List<String[,]> results = new List<String[,]>();

            //for (int k = 0; k < left.Length; k++)
            //{
            //    results.Add(eqn.Parse(left[k]));
            //}

            //for (int k = 0; k < right.Length; k++)
            //{
            //    results.Add(eqn.Parse(right[k]));
            //}

            //double[,] matrix = PopulateMatrix(results, left.Length, right.Length, eqn);
            //matrix = SolveMatrix(matrix);
            //Console.WriteLine("???????????????????????");
            //DisplayMatrix(matrix);
            //Console.WriteLine("???????????????????????");
            SolveMatrix();
            return InsertCoeff(DetermineCoefficients(matrix), left, right);
        }

         private void SwapRows( int r1, int r2)
        {
            double temp;
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                temp = matrix[r1, i];
                matrix[r1, i] = matrix[r2, i];
                matrix[r2, i] = temp;
            }
            ///return matrix;
        }

        private void FindAndSwap( int currentRow)
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
                SwapRows( currentRow, rowToSwitch);
                return;
            }
            //return matrix;
        }

        private double LCMLeadingNum( int row1, int row2)//DELETE??????????
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

        private double[] FindScalars( int row1, int row2)
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

        private static double GCD(double x, double y)
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

        private void RowAddScalar1( int sourceRow, int destRow, double[] scalar)
        {
            if (Math.Abs(scalar[0]) < .001)
            {
                return;
            }
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                matrix[destRow, i] = matrix[sourceRow, i] * scalar[1] - matrix[destRow, i] * scalar[0];
            }
            return;
        }

        private double GCDofRow( int pos)
        {
            double gcd = matrix[pos, 0];
            for (int i = 1; i < matrix.GetLength(1); i++)
            {
                gcd = GCD(matrix[pos, i], gcd);
            }
            return gcd;
        }

        private void ToLowestTermsR( int pos)
        {
            double gcd = GCDofRow( pos);
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                matrix[pos, i] /= gcd;
            }
            //return matrix;
        }

        private int[] GetSigns()
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

        private void ToPositive()
        {
            int[] signs = GetSigns();

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = matrix[i, j] * signs[i];
                }
            }
            
        }

        private void ToLowestTermsM()
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                ToLowestTermsR(i);
            }
            //return matrix;
        }

        //static double[,] ToRowEchelonForm(double[,] matrix)
        public double[,] SolveMatrix()
        {
            //Console.WriteLine("ROW");
            double scalar = 0;
            double[] scalar1;
            //matrix = ToPositive(ToLowestTermsM(matrix));
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                FindAndSwap( i);
                for (int j = i + 1; j < matrix.GetLength(0); j++)
                {
                    //matrix = ToPositive(ToLowestTermsM(matrix));
                    //Console.WriteLine("\nI " + i + " j " + j);
                    //Console.WriteLine("FIRST");
                    //DisplayMatrix(matrix);
                    scalar1 = FindScalars( i, j);
                    //Console.WriteLine("Sclar " + scalar);
                    RowAddScalar1( i, j, scalar1);
                    //Console.WriteLine("LAST");
                    //DisplayMatrix(matrix);
                }
            }
            ToLowestTermsM();
            ToPositive();
            //Console.WriteLine("\n//////////////////////////////////////////////////////////\n");
            //DisplayMatrix();
            //Console.WriteLine("\n//////////////////////////////////////////////////////////\n");
            ToRedRowEchelon();
            // Console.WriteLine("\n//////////////////////////////////////////////////////////\n");
            ToLowestTermsM();
            ToPositive();
            return matrix;
        }



        private double[] FindPositionalScalars(int bottomRow, int topRow)
        {
            double[] scalars = new double[2];
            scalars[0] = matrix[bottomRow, bottomRow];
            scalars[1] = matrix[topRow, bottomRow];
            return scalars;
        }

        private void ToRedRowEchelon()
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
                    scalar1 = FindPositionalScalars(i, j);
                    //Console.WriteLine("Sclar " + scalar1[0] + " " + scalar1[1]);
                    RowAddScalar1( i, j, scalar1);
                    //Console.WriteLine("LAST");
                    //DisplayMatrix(matrix);
                }
            }
            //return matrix;
        }

        void DisplayMatrix()
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
    }
}
