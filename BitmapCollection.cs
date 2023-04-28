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
using System.Collections.ObjectModel;
using System.Drawing;

namespace loaddatfsh
{
    internal sealed class BitmapCollection : Collection<Bitmap>, IDisposable
    {
        private bool disposed;

        public BitmapCollection() : this(0)
        {
        }

        public BitmapCollection(int count) : base (new List<Bitmap>(count))
        {
            this.disposed = false;
        }

        protected override void ClearItems()
        {
            IList<Bitmap> items = Items;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    items[i].Dispose();
                    items[i] = null;
                }
            }

            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            if (Items[index] != null)
            {
                Items[index].Dispose();
                Items[index] = null;
            }

            base.RemoveItem(index);
        }

        public void SetCapacity(int capacity)
        {
            var list = (List<Bitmap>)Items;

            if (list.Capacity < capacity)
            {
                list.Capacity = capacity;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                if (disposing)
                {
                    IList<Bitmap> items = Items;

                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i] != null)
                        {
                            items[i].Dispose();
                            items[i] = null;
                        }
                    }
                }
            }
        }
    }
}
