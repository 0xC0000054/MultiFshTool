using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace loaddatfsh
{
    // Implements the manual sorting of items by columns.
    internal sealed class ListViewItemComparer : Comparer<ListViewItem>
    {
        private readonly int col;
        private readonly SortOrder order;

        public ListViewItemComparer(int column, SortOrder order)
        {
            this.col = column;
            this.order = order;
        }

        public int Compare(ListViewItem x, ListViewItem y)
        {
            if (Object.ReferenceEquals(x, y))
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            int returnVal = StringLogicalComparer.Compare(x.SubItems[this.col].Text, y.SubItems[this.col].Text);

            // Determine whether the sort order is descending.
            if (this.order == SortOrder.Descending)
            {     
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            }
            
            return returnVal;
        }
    }

}
