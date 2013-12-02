using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathMaker {
    public class Table {
        private const string fieldSeparator = "@@";
        private const string lineSeparator = "##";
        int numRows = 0;
        int numColumns = 0;
        string[,] data = null;

        public Table(int rows, int columns) {
            data = new string[rows, columns];
            numRows = rows;
            numColumns = columns;

            for (int r = 0; r < numRows; r++)
                for (int c = 0; c < numColumns; c++)
                    data[r, c] = "";
        }

        public Table(string source) {
            if (source.Length != 0) {
                string[] lines = source.Split(new string[]{lineSeparator}, StringSplitOptions.None);

                numRows = lines.Length;

                int row = 0;
                foreach (string line in lines) {
                    string[] fields = line.Split(new string[] { fieldSeparator }, StringSplitOptions.None);

                    if (numColumns == 0)
                        numColumns = fields.Length;
                    else
                        System.Diagnostics.Debug.Assert(numColumns == fields.Length);
                    if (data == null)
                        data = new string[numRows, numColumns];

                    int column = 0;
                    foreach (string field in fields) {
                        data[row, column] = field;
                        column++;
                    }
                    row++;
                }
            }
        }

        public void DeleteColumn(int column) {
            int[] tbd = new int[1] { column };
            DeleteColumns(tbd);
        }

        public bool IsEmpty() {
            if (numRows == 0 || numColumns == 0)
                return true;
            return false;
        }

        public void DeleteColumns(int[] columns) {
            int newNumColumns = numColumns;
            for (int i = 0; i < columns.Length; i++)
                if (columns[i] < numColumns)
                    newNumColumns--;

            string[,] newData = new string[numRows, newNumColumns];
            for (int r = 0; r < numRows; r++) {
                int newColumn = 0;
                for (int c = 0; c < numColumns; c++) {
                    if (!columns.Contains(c))
                        newData[r, newColumn++] = data[r, c];
                }
            }
            numColumns = newNumColumns;
            data = newData;
        }

        public void DeleteRow(int row) {
            string[,] newData = new string[numRows - 1, numColumns];

            int newRow = 0;
            for (int r = 0; r < numRows; r++)
                if (r != row) {
                    for (int c = 0; c < numColumns; c++)
                        newData[newRow, c] = data[r, c];
                    newRow++;
                }
            data = newData;
            numRows = numRows - 1;
        }

        override public string ToString() {
            StringBuilder b = new StringBuilder();

            for (int r = 0; r < numRows; r++) {
                if (r > 0)
                    b.Append(lineSeparator); 
                for (int c = 0; c < numColumns; c++) {
                    if (c > 0)
                        b.Append(fieldSeparator); 
                    b.Append(data[r, c]);
                }
            }
            return b.ToString();
        }

        internal int AddColumn() {
            int newNumColumns = numColumns + 1;
            string[,] newData = new string[numRows, newNumColumns];
            for (int r = 0; r < numRows; r++) {
                for (int c = 0; c < numColumns; c++)
                    newData[r, c] = data[r, c];
                newData[r, newNumColumns - 1] = "";
            }
            numColumns = newNumColumns;
            data = newData;
            return numColumns - 1;
        }

        internal int AddRow() {
            int newNumRows = numRows + 1;
            string[,] newData = new string[newNumRows, numColumns];
            for (int c = 0; c < numColumns; c++) {
                for (int r = 0; r < numRows; r++)
                    newData[r, c] = data[r, c];
                newData[newNumRows - 1, c] = "";
            }
            numRows = newNumRows;
            data = newData;
            return numRows - 1;
                
        }

        internal void CopyColumn(int src, int dst) {
            for (int r = 0; r < numRows; r++)
                data[r, dst] = data[r, src];
        }

        internal int GetNumRows() {
            return numRows;
        }

        internal int GetNumColumns() {
            return numColumns;
        }

        internal string GetData(int row, int column) {
            System.Diagnostics.Debug.Assert(row < numRows && column < numColumns);
            string tmp = data[row, column];
            if (tmp == null)
                return "";
            else {
                tmp = tmp.Replace("\a", fieldSeparator);
                tmp = tmp.Replace("\f", lineSeparator);
                return tmp;
            }
        }

        internal void SetData(int row, int column, string value) {
            System.Diagnostics.Debug.Assert(row < numRows && column < numColumns);
            if (value == null)
                data[row, column] = "";
            else {
                value = value.Replace(fieldSeparator, "\a");
                value = value.Replace(lineSeparator, "\f");
                data[row, column] = value;
            }
        }

        internal void SwapColumns(int col1, int col2) {
            for (int r = 0; r < numRows; r++) {
                string tmp;

                tmp = data[r, col1];
                data[r, col1] = data[r, col2];
                data[r, col2] = tmp;
            }
        }

        internal void SwapRows(int row1, int row2) {
            for (int c = 0; c < numColumns; c++) {
                string tmp;

                tmp = data[row1, c];
                data[row1, c] = data[row2, c];
                data[row2, c] = tmp;
            }
        }
    }
}
