using System;
using System.Collections;
using System.Windows.Forms;
using System.Globalization;

namespace loaddatfsh
{
    // Implements the manual sorting of items by columns.
    class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        private bool numsort = false;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            numsort = (column == 0); // is the column number zero
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            if (numsort)
            {
                string xsub = ((ListViewItem)x).Text;
                string ysub = ((ListViewItem)y).Text;
                xsub = xsub.Substring(6, (xsub.Length - 6));
                ysub = ysub.Substring(6, (ysub.Length - 6));

                int numx = int.Parse(xsub, CultureInfo.InvariantCulture);
                returnVal = numx.CompareTo(int.Parse(ysub, CultureInfo.InvariantCulture));
            }
            else
            {
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                       ((ListViewItem)y).SubItems[col].Text, StringComparison.OrdinalIgnoreCase);
            }

            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }

}
