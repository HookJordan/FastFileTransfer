using System;
using System.Collections;
using System.Windows.Forms;
    

namespace FFT.Core.UI
{
    public class FancyListViewSort : IComparer
    {
        private int col;
        public FancyListViewSort()
        {
            col = 0;
        }
        public FancyListViewSort(int column)
        {
            col = column;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
            ((ListViewItem)y).SubItems[col].Text);
            return returnVal;
        }
    }
}
