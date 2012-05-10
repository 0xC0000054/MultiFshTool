using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace loaddatfsh
{
    internal static class BlendBitmap
    {
       /// <summary>
       /// Blends the fsh bitmap and alpha images
       /// </summary>
       /// <param name="bmpitem">The bitmap item to blend</param>
       /// <returns>The blended bitmap or null</returns>
       public static unsafe Bitmap BlendBmp(FshDatIO.BitmapEntry bmpitem)
       {
            if (bmpitem.Bitmap != null && bmpitem.Alpha != null)
            {            
                Bitmap image = null;
                using (Bitmap temp = new Bitmap(bmpitem.Bitmap.Width, bmpitem.Bitmap.Height, PixelFormat.Format32bppArgb))
                {
                    int width = temp.Width;
                    int height = temp.Height;

                    Rectangle lockRect = new Rectangle(0, 0, width, height);
                    BitmapData colordata = bmpitem.Bitmap.LockBits(lockRect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                    BitmapData alphadata = bmpitem.Alpha.LockBits(lockRect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                    BitmapData bdata = temp.LockBits(lockRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                    try
                    {
                        byte* clrScan0 = (byte*)colordata.Scan0.ToPointer();
                        byte* alScan0 = (byte*)alphadata.Scan0;
                        byte* destScan0 = (byte*)bdata.Scan0;
                        int stride = bdata.Stride;
                        int clrStride = colordata.Stride;
                        int alStride = alphadata.Stride;
                        for (int y = 0; y < height; y++)
                        {
                            byte* color = clrScan0 + (y * clrStride);
                            byte* alpha = alScan0 + (y * alStride);
                            byte* dest = destScan0 + (y * stride);
                            for (int x = 0; x < width; x++)
                            {
                                dest[3] = alpha[0]; // copy the blue alpha map channel to the blended image alpha channel
                                dest[0] = color[0]; // blue 
                                dest[1] = color[1]; // green 
                                dest[2] = color[2]; // red

                                dest += 4;
                                color += 3;
                                alpha += 3;
                            }

                        }
                    }
                    finally
                    {
                        bmpitem.Bitmap.UnlockBits(colordata);
                        bmpitem.Alpha.UnlockBits(alphadata);                     
                        temp.UnlockBits(bdata);
                    }

                    image = (Bitmap)temp.Clone();
                }

                return image;
            }
            else
            {
                return null;
            }
       }
    }
}
