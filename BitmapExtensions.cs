using System.Drawing;
using System.Drawing.Imaging;

namespace loaddatfsh
{
    static class BitmapExtensions
    {
        public static Bitmap Clone(this Bitmap bmp, PixelFormat format)
        {
            return bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), format);
        }

        public static Bitmap GetImageListThumbnail(this Bitmap bmp, Size imageListSize)
        {
            Bitmap image = null;
            Bitmap temp = null;

            try
            {
                temp = new Bitmap(imageListSize.Width, imageListSize.Height, bmp.PixelFormat);

                using (Graphics gr = Graphics.FromImage(temp))
                {
                    gr.DrawImage(bmp, GetThumbnailDisplayRectangle(bmp, imageListSize), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                }

                image = temp;
                temp = null;
            }
            finally
            {
                if (temp != null)
                {
                    temp.Dispose();
                    temp = null;
                }
            }

            return image;
        }

        public static Rectangle GetThumbnailDisplayRectangle(this Bitmap bmp, Size displaySize)
        {
            int x, y, width, height;

            double whRatio = (double)bmp.Width / bmp.Height;

            if (bmp.Width >= bmp.Height)
            {
                width = displaySize.Width;
                height = (int)(width / whRatio);
            }
            else
            {
                height = displaySize.Height;
                width = (int)(height * whRatio);
            }

            x = (displaySize.Width - width) / 2;
            y = (displaySize.Height - height) / 2;

            return new Rectangle(x, y, width, height);
        }
    }
}
