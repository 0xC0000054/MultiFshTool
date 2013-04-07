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
            Size listSize = list.ImageSize;
            if (listSize.Width > image.Width || listSize.Height > image.Height)
            {
                int width, height;

                int imageWidth = image.Width;
                int imageHeight = image.Height;

                if (imageWidth == imageHeight)
                {
                    width = height = imageHeight;
                }
                else
                {
                    // Figure out the ratio
                    double ratioX = (double)listSize.Width / (double)imageWidth;
                    double ratioY = (double)listSize.Height / (double)imageHeight;
                    double ratio = ratioX < ratioY ? ratioX : ratioY; // use whichever multiplier is smaller

                    // now we can get the new height and width
                    height = (int)(imageHeight * ratio);
                    width = (int)(imageWidth * ratio); 
                }

                list.ImageSize = new Size(width, height);
            }
            else
            {
                list.ImageSize = defaultSize;
            }
        }

        public static void ResetImageSize(this ImageList list)
        {
            list.ImageSize = defaultSize;
        }
    }
}
