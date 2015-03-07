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
    }
}
