using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace loaddatfsh
{
    // Implements the manual sorting of items by columns.
    internal sealed class ListViewItemComparer : IComparer<ListViewItem>
    {
        private int col;
        private SortOrder order;

        public ListViewItemComparer(int column, SortOrder order)
        {
            this.col = column;
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

            int returnVal = StringLogicalComparer.Compare(x.SubItems[col].Text, y.SubItems[col].Text);

            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
            {     
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            }
            
            return returnVal;
        }
    }

}
