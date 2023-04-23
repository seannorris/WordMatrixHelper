using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;

namespace WordMatrixHelper
{
    public partial class WordMatrixHelper : Form
    {
        public WordMatrixHelper()
        {
            InitializeComponent();
        }

        private decimal[,] ParseMatrix(string wordFormat)
        {
            var raggedArray = wordFormat.Split('(').Last().Split(')').First().Split('@').Select(row => row.Split('&').Select(x => decimal.Parse(x)).ToArray()).ToArray();

            var matrix = new decimal[raggedArray.Length, raggedArray[0].Length];
            for (var y = 0; y < raggedArray.Length; y++)
                for (var x = 0; x < raggedArray[y].Length; x++)
                    matrix[y, x] = raggedArray[y][x];

            return matrix;
        }

        private string EncodeMatrix(decimal[,] matrix)
        {
            var wordFormat = new StringBuilder();
            wordFormat.Append("[■(");
            for (var y = 0; y < matrix.GetLength(0); y++)
            {
                if (y > 0)
                    wordFormat.Append('@');

                for (var x = 0; x < matrix.GetLength(1); x++)
                {
                    if (x > 0)
                        wordFormat.Append('&');

                    wordFormat.Append(matrix[y,x]);
                }
            }

            wordFormat.Append(")]");
            return wordFormat.ToString();
        }

        private void textBox_Focused(object sender, EventArgs e)
        {
            if(!(sender is TextBox textBox) || string.IsNullOrEmpty(textBox.Text))
                return;

            textBox.SelectionStart = 0;
            textBox.SelectionLength = textBox.TextLength;
        }

        private decimal[,] GenerateIdentityMatrix(int n)
        {
            var matrix = new decimal[n,n];

            for (var y = 0; y < n; y++)
                for (var x = 0; x < n; x++)
                    matrix[y, x] = x == y ? 1 : 0;

            return matrix;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            outputTextBox.Text = EncodeMatrix(GenerateIdentityMatrix(2));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            outputTextBox.Text = EncodeMatrix(GenerateIdentityMatrix(3));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            outputTextBox.Text = EncodeMatrix(GenerateIdentityMatrix(4));
        }

        private string GenerateBlankMatrix(int n)
        {
            var wordFormat = new StringBuilder();
            wordFormat.Append("[■(");
            for (var y = 0; y < n; y++)
            {
                if (y > 0)
                    wordFormat.Append('@');

                for (var x = 1; x < n; x++)
                    wordFormat.Append('&');
            }

            wordFormat.Append(")]");
            return wordFormat.ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            outputTextBox.Text = GenerateBlankMatrix(2);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            outputTextBox.Text = GenerateBlankMatrix(3);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            outputTextBox.Text = GenerateBlankMatrix(4);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var a = ParseMatrix(textBox1.Text);
            var b = ParseMatrix(textBox2.Text);

            var matrix = new decimal[a.GetLength(0), a.GetLength(1)];

            for (var y = 0; y < a.GetLength(0); y++)
                for (var x = 0; x < a.GetLength(1); x++)
                    matrix[y, x] = a[y, x] + b[y, x];

            outputTextBox.Text = EncodeMatrix(matrix);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var a = ParseMatrix(textBox1.Text);
            var b = ParseMatrix(textBox2.Text);

            var matrix = new decimal[a.GetLength(0), a.GetLength(1)];

            for (var y = 0; y < a.GetLength(0); y++)
                for (var x = 0; x < a.GetLength(1); x++)
                    matrix[y, x] = a[y, x] - b[y, x];

            outputTextBox.Text = EncodeMatrix(matrix);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var a = ParseMatrix(textBox1.Text);
            var b = ParseMatrix(textBox2.Text);

            if (b.Length == 1)
            {
                var matrix1 = new decimal[a.GetLength(0), a.GetLength(1)];

                for (var y = 0; y < a.GetLength(0); y++)
                    for (var x = 0; x < a.GetLength(1); x++)
                        matrix1[y, x] = a[y, x] * b[0, 0];

                outputTextBox.Text = EncodeMatrix(matrix1);
                return;
            }

            var matrix = new decimal[a.GetLength(0), b.GetLength(1)];

            for (var y = 0; y < a.GetLength(0); y++)
            {
                for (var x = 0; x < b.GetLength(1); x++)
                {
                    decimal val = 0;
                    for (var i = 0; i < b.GetLength(0); i++)
                        val += a[y, i] * b[i, x];

                    matrix[y, x] = val;
                }
            }

            outputTextBox.Text = EncodeMatrix(matrix);
        }

        decimal[,] FindInverse(decimal[,] matrix)
        {
            var identityMatrix = new decimal[matrix.GetLength(0), matrix.GetLength(1)];
            for (var row = 0; row < identityMatrix.GetLength(0); row++)
            {
                for (var col = 0; col < identityMatrix.GetLength(1); col++)
                {
                    identityMatrix[row, col] = (row == col) ? 1 : 0;
                }
            }

            var newMatrix = matrix.Clone() as decimal[,]; 

            for (var outerRow = 0; outerRow < newMatrix.GetLength(0) - 1; outerRow++)
            {
                if (newMatrix[outerRow, outerRow] == 0)
                {
                    var swapWith = newMatrix.GetLength(0) - 1;
                    while (newMatrix[outerRow, outerRow] == 0)
                    {
                        for (var col = 0; col < newMatrix.GetLength(1); col++)
                        {
                            var tempNewMatrix = newMatrix[outerRow, col];
                            var tempIdentityMatrix = identityMatrix[outerRow, col];
                            newMatrix[outerRow, col] = newMatrix[swapWith, col];
                            identityMatrix[outerRow, col] = identityMatrix[swapWith, col];
                            newMatrix[swapWith, col] = tempNewMatrix;
                            identityMatrix[swapWith, col] = tempIdentityMatrix;
                        }
                        swapWith--;
                    }
                }
                var divisor = newMatrix[outerRow, outerRow];
                for (var col = 0; col < newMatrix.GetLength(1); col++)
                {
                    newMatrix[outerRow, col] /= divisor;
                    identityMatrix[outerRow, col] /= divisor;
                }

                for (var innerRow = outerRow + 1; innerRow < newMatrix.GetLength(0); innerRow++)
                {
                    var multiplier = newMatrix[innerRow, outerRow];
                    for (var col = 0; col < newMatrix.GetLength(1); col++)
                    {
                        newMatrix[innerRow, col] -= newMatrix[outerRow, col] * multiplier;
                        identityMatrix[innerRow, col] -= identityMatrix[outerRow, col] * multiplier;
                    }
                }
            }


            var divisor2 = newMatrix[newMatrix.GetLength(0) - 1, newMatrix.GetLength(1) - 1];
            for (var col = 0; col < newMatrix.GetLength(1); col++)
            {
                newMatrix[newMatrix.GetLength(0) - 1, col] /= divisor2;
                identityMatrix[identityMatrix.GetLength(0) - 1, col] /= divisor2;
            }

            for (var outerRow = newMatrix.GetLength(0) - 2; outerRow >= 0; outerRow--)
            {
                for (var innerRow = newMatrix.GetLength(0) - 1; innerRow > outerRow; innerRow--)
                {
                    var multiplier = newMatrix[outerRow, innerRow];
                    for (var col = 0; col < newMatrix.GetLength(1); col++)
                    {
                        newMatrix[outerRow, col] -= newMatrix[innerRow, col] * multiplier;
                        identityMatrix[outerRow, col] -= identityMatrix[innerRow, col] * multiplier;
                    }
                }
            }

            return identityMatrix;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            outputTextBox.Text = EncodeMatrix(FindInverse(ParseMatrix(textBox1.Text)));
        }

        static decimal RoundToSignificantDigits(decimal val, int digits)
        {
            var truncateScale = (decimal) Math.Pow(10, digits + 3);
            var d = Math.Truncate(val * truncateScale) / truncateScale;
            if (d == 0)
                return 0;

            var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs((double)d))) + 1);
            return (decimal)(scale * Math.Round((double)d / scale, digits));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var sigFigs = (int)numericUpDown1.Value;
            var matrix = ParseMatrix(outputTextBox.Text);

            for (var y = 0; y < matrix.GetLength(0); y++)
                for (var x = 0; x < matrix.GetLength(1); x++)
                    matrix[y, x] = RoundToSignificantDigits(matrix[y, x], sigFigs);

            outputTextBox.Text = EncodeMatrix(matrix);
        }
    }
}
