/*
* This file is part of MultiFshTool, a tool for creating and editing FSH
* files that contain multiple images.
*
* Copyright (C) 2010-2017, 2023 Nicholas Hayes
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*
*/

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

        public override int Compare(ListViewItem x, ListViewItem y)
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
