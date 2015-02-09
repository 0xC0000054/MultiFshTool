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
