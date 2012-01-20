using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace loaddatfsh
{
    static class ImageListExtensions
    {
        private static readonly Size defaultSize = new Size(96, 96); 
        public static void ScaleListSize(this ImageList list, Bitmap image)
        {
            if (list.ImageSize.Width > image.Width || list.ImageSize.Height > image.Height)
            {
                int width = Math.Min(list.ImageSize.Width, image.Width);
                int height = Math.Min(list.ImageSize.Height, image.Height);

                list.ImageSize = new Size(width, height);
            }
            else
            {
                list.ImageSize = defaultSize;
            }
        }

    }
}
