﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace loaddatfsh
{
    internal class Fshwrite
    {
        public Fshwrite()
        {
            bmplist = new List<Bitmap>();
            alphalist = new List<Bitmap>();
            dirnames = new List<byte[]>();
            codelist = new List<int>();
        }
        private enum SquishCompFlags
        {
            //! Use DXT1 compression.
            kDxt1 = (1 << 0),

            //! Use DXT3 compression.
            kDxt3 = (1 << 1),

            //! Use DXT5 compression.
            kDxt5 = (1 << 2),

            //! Use a very slow but very high quality colour compressor.
            kColourIterativeClusterFit = (1 << 8),

            //! Use a slow but high quality colour compressor (the default).
            kColourClusterFit = (1 << 3),

            //! Use a fast but low quality colour compressor.
            kColourRangeFit = (1 << 4),

            //! Use a perceptual metric for colour error (the default).
            kColourMetricPerceptual = (1 << 5),

            //! Use a uniform metric for colour error.
            kColourMetricUniform = (1 << 6),

        }
       
        private byte[] CompressImage(Bitmap image, int flags)
        {
            byte[] pixelData = new byte[image.Width * image.Height * 4];

            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* scan0 = (byte*)data.Scan0.ToPointer();
                int stride = data.Stride;
                for (int y = 0; y < image.Height; y++)
                {
                    byte* p = scan0 + (y * stride);
                    for (int x = 0; x < image.Width; x++)
                    {
                        int index = (y * image.Width * 4) + (x * 4);

                        pixelData[index] = p[2];
                        pixelData[index + 1] = p[1];
                        pixelData[index + 2] = p[0];
                        pixelData[index + 3] = p[3];

                        p += 4;
                    }
                }
            }
            image.UnlockBits(data);
            // Compute size of compressed block area, and allocate 
            int blockCount = ((image.Width + 3) / 4) * ((image.Height + 3) / 4);
            int blockSize = ((flags & (int)SquishCompFlags.kDxt1) != 0) ? 8 : 16;

            // Allocate room for compressed blocks
            byte[] blockData = new byte[blockCount * blockSize];

            // Invoke squish::CompressImage() with the required parameters
            CompressImageWrapper(pixelData, image.Width, image.Height, blockData, flags);

            // Return our block data to caller..
            return blockData;
        }
        private static bool Is64bit()
        {
            return IntPtr.Size == 8 ? true : false;
        }
        private sealed class Squish_32
        {
            [DllImport("Squish_Win32.dll")]
            internal static extern unsafe void SquishCompressImage(byte* rgba, int width, int height, byte* blocks, int flags);
        }
        private sealed class Squish_64
        {
            [DllImport("squish_x64.dll")]
            internal static extern unsafe void SquishCompressImage(byte* rgba, int width, int height, byte* blocks, int flags);
        }
        private static unsafe void CompressImageWrapper(byte[] rgba, int width, int height, byte[] blocks, int flags)
        {
            fixed (byte* RGBA = rgba)
            {
                fixed (byte* Blocks = blocks)
                {
                    if (Is64bit())
                    {
                        Squish_64.SquishCompressImage(RGBA, width, height, Blocks, flags);
                    }
                    else
                    {
                        Squish_32.SquishCompressImage(RGBA, width, height, Blocks, flags);
                    }
                }
            }
        }

        private Bitmap BlendDXTBitmap(Bitmap color, Bitmap alpha)
        {
            if (color == null)
                throw new ArgumentNullException("color", "color is null.");
            if (alpha == null)
                throw new ArgumentNullException("alpha", "alpha is null.");
            if (color.Size != alpha.Size)
                throw new ArgumentException("The color and alpha bitmaps must be equal size");

            Bitmap image = null;
            Bitmap temp = null;
            try
            {

                temp = new Bitmap(color.Width, color.Height, PixelFormat.Format32bppArgb);
                
                BitmapData colordata = color.LockBits(new Rectangle(0, 0, color.Width, color.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData alphadata = alpha.LockBits(new Rectangle(0, 0, alpha.Width, alpha.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
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
                            destdata[3] = aldata[0];
                            destdata[0] = clrdata[0];
                            destdata[1] = clrdata[1];
                            destdata[2] = clrdata[2];


                            destdata += 4;
                            clrdata += 4;
                            aldata += 4;
                        }
                        destdata += offset;
                        clrdata += clroffset;
                        aldata += aloffset;
                    }

                }
                color.UnlockBits(colordata);
                alpha.UnlockBits(alphadata);
                temp.UnlockBits(bdata);

                image = temp.Clone<Bitmap>();
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

        private List<Bitmap> bmplist = null;
        private List<Bitmap> alphalist = null;
        private List<byte[]> dirnames = null;
        private List<int> codelist = null;
        private static int GetBmpDataSize(Bitmap bmp, int code)
        {
            int ret = -1;
            switch (code)
            {
                case 0x60:
                    ret = (bmp.Width * bmp.Height) / 2; //Dxt1
                    break;
                case 0x61:
                    ret = (bmp.Width * bmp.Height); //Dxt3
                    break;
            }
            return ret;
        }
        public List<Bitmap> alpha
        {
            get 
            {
                return alphalist;
            }
            set
            {
                alphalist = value;
            }
        }
        public List<Bitmap> bmp
        {
            get
            {
                return bmplist;
            }
            set
            {
                bmplist = value;
            }
        }
        public List<byte[]> dir
        {
            get
            {
                return dirnames;
            }
            set
            {
                dirnames = value;
            }
        }
        public List<int> code
        {
            get
            {
                return codelist;
            }
            set
            {
                codelist = value;
            }
        }
        /// <summary>
        /// The function that writes the fsh
        /// </summary>
        /// <param name="output">The output file to write to</param>
        public unsafe void WriteFsh(Stream output)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (bmplist != null && bmplist.Count > 0 && alphalist != null && dirnames != null && codelist != null)
                {
                    //write header
                    ms.Write(Encoding.ASCII.GetBytes("SHPI"), 0, 4); // write SHPI id
                    ms.Write(BitConverter.GetBytes(0), 0, 4); // placeholder for the length
                    ms.Write(BitConverter.GetBytes(bmplist.Count), 0, 4); // write the number of bitmaps in the list

                    ms.Write(Encoding.ASCII.GetBytes("G264"), 0, 4); // header directory id
                    int count = bmplist.Count;
                    int fshlen = 16 + (8 * count); // fsh length
                    for (int i = 0; i < count; i++)
                    {
                        //write directory
                       // Debug.WriteLine("bmp = " + c.ToString() + " offset = " + fshlen.ToString());
                        ms.Write(dir[i], 0, 4); // directory id
                        ms.Write(BitConverter.GetBytes(fshlen), 0, 4); // Write the Entry offset 

                        fshlen += 16; // skip the entry header length
                        int bmplen = GetBmpDataSize(bmplist[i], codelist[i]);
                        fshlen += bmplen; // skip the bitmap length
                    }
                    for (int i = 0; i < count; i++)
                    {
                        Bitmap bmp = bmplist[i];
                        Bitmap alpha = alphalist[i];
                        int code = codelist[i];
                        // write entry header
                        ms.Write(BitConverter.GetBytes(code), 0, 4); // write the Entry bitmap code
                        ms.Write(BitConverter.GetBytes((ushort)bmp.Width), 0, 2); // write width
                        ms.Write(BitConverter.GetBytes((ushort)bmp.Height), 0, 2); //write height
                        for (int m = 0; m < 4; m++)
                        {
                            ms.Write(BitConverter.GetBytes((ushort)0), 0, 2);// write misc data
                        }

                        if (code == 0x60) //DXT1
                        {
                            Bitmap temp = BlendDXTBitmap(bmp, alpha);
                            int flags = (int)SquishCompFlags.kDxt1;
                            flags |= (int)SquishCompFlags.kColourIterativeClusterFit;
                            flags |= (int)SquishCompFlags.kColourMetricPerceptual;

                            byte[] data = CompressImage(temp, flags);
                            ms.Write(data, 0, data.Length);
                        }
                        else if (code == 0x61) // DXT3
                        {
                            Bitmap temp = BlendDXTBitmap(bmp, alpha);
                            int flags = (int)SquishCompFlags.kDxt3;
                            flags |= (int)SquishCompFlags.kColourIterativeClusterFit;
                            flags |= (int)SquishCompFlags.kColourMetricPerceptual;
                            byte[] data = CompressImage(temp, flags);
                            ms.Write(data, 0, data.Length);
                        }

                    }

                    ms.Position = 4L;
                    ms.Write(BitConverter.GetBytes((int)ms.Length), 0, 4); // write the files length
                    ms.WriteTo(output); // write the memory stream to the file
                }
            }
        }

    }
}
