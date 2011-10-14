using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using FSHLib;

namespace loaddatfsh
{
    public class BlendBitmap
    {
       /// <summary>
       /// Blends the fsh bitmap and alpha images
       /// </summary>
       /// <param name="bmpitem">The bitmap item to blend</param>
       /// <returns>The blended bitmap or null</returns>
       public static Bitmap BlendBmp(BitmapItem bmpitem)
       {
            if (bmpitem.Bitmap != null && bmpitem.Alpha != null)
            {            
                Bitmap image = null;
                using (Bitmap temp = new Bitmap(bmpitem.Bitmap.Width, bmpitem.Bitmap.Height, PixelFormat.Format32bppArgb))
                {
                   

                    using (Bitmap bmpalpha = new Bitmap(bmpitem.Bitmap))
                    using (Bitmap colorbmp = new Bitmap(bmpitem.Alpha))
                    {
                        BitmapData colordata = colorbmp.LockBits(new Rectangle(0, 0, temp.Width, temp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                        BitmapData alphadata = bmpalpha.LockBits(new Rectangle(0, 0, temp.Width, temp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                        BitmapData bdata = temp.LockBits(new Rectangle(0, 0, temp.Width, temp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                        unsafe
                        {
                            byte* clrdata = (byte*)(void*)colordata.Scan0;
                            byte* aldata = (byte*)(void*)alphadata.Scan0;
                            byte* destdata = (byte*)(void*)bdata.Scan0;
                            int offset = bdata.Stride - temp.Width * 4;
                            int clroffset = colordata.Stride - temp.Width * 4;
                            int aloffset = alphadata.Stride - temp.Width * 4;
                            for (int y = 0; y < temp.Height; y++)
                            {
                                for (int x = 0; x < temp.Width; x++)
                                {
                                    destdata[3] = aldata[0]; // copy the blue alpha map channel to the blended image alpha channel
                                    destdata[0] = clrdata[0]; // blue 
                                    destdata[1] = clrdata[1]; // green 
                                    destdata[2] = clrdata[2]; // red

                                    destdata += 4;
                                    clrdata += 4;
                                    aldata += 4;
                                }

                            }

                        }
                        colorbmp.UnlockBits(colordata);
                        bmpalpha.UnlockBits(alphadata);                     
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
