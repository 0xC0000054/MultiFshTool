using System;
using System.Collections;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;

namespace loaddatfsh
{
    // Implements the manual sorting of items by columns.
    class ListViewItemComparer : IComparer<ListViewItem>
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

        public int Compare(ListViewItem x, ListViewItem y)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }

            if (y == null)
            {
                throw new ArgumentNullException("y");
            }

            int returnVal = -1;
            if (numsort)
            {
                string xsub = x.Text.Substring(6, (x.Text.Length - 6));
                string ysub = y.Text.Substring(6, (y.Text.Length - 6));

                int numx = int.Parse(xsub, CultureInfo.InvariantCulture);
                returnVal = numx.CompareTo(int.Parse(ysub, CultureInfo.InvariantCulture));
            }
            else
            {
                returnVal = String.Compare(x.SubItems[col].Text,
                                       y.SubItems[col].Text, StringComparison.OrdinalIgnoreCase);
            }

            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }

}
