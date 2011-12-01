using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using FSHLib;
using System.Globalization;
using loaddatfsh.Properties;
using System.Text.RegularExpressions;
using FshDatIO;

namespace loaddatfsh
{
    internal partial class Multifshfrm : Form
    {
        public Multifshfrm()
        {
            InitializeComponent();
            this.dat = null;
            this.dirName = null;
            this.fshSize = null;
            this.fshFileName = string.Empty;
            this.curImage = null;
            this.mip64Fsh = null;
            this.mip32Fsh = null;
            this.mip16Fsh = null;
            this.mip8Fsh = null;
            this.bmpEntry = null;
            this.loadIsMip = false;
        }
        private string[] dirName;
        private string[] fshSize;
        private string fshFileName;
        private FSHImageWrapper curImage;
        private FSHImageWrapper mip64Fsh;
        private FSHImageWrapper mip32Fsh;
        private FSHImageWrapper mip16Fsh;
        private FSHImageWrapper mip8Fsh;
        private BitmapEntry bmpEntry;
        private const uint fshTypeID = 0x7ab50e44U;

        private void loadfsh_Click(object sender, EventArgs e)
        {

            if (openFshDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Load_Fsh(openFshDialog1.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text);
                }
            }
        }
        private bool loadIsMip; 
        private void Load_Fsh(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            if (fi.Exists)
            {
                if (fi.Extension.Equals(".fsh", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        bool success = false;
                        using (FileStream fstream = new FileStream(fi.FullName, FileMode.Open))
                        {
                            using (FSHImageWrapper tempimg = new FSHImageWrapper(fstream))
                            {
                                BitmapEntry tempitem = tempimg.Bitmaps[0];

                                fshFileName = filename;
                                ClearandReset(true);
                                if (origbmplist == null)
                                {
                                    origbmplist = new List<Bitmap>();
                                }
                                else
                                {
                                    foreach (var item in origbmplist)
                                    {
                                        item.Dispose();
                                    }
                                    origbmplist.Clear();
                                }
                                this.fshWriteCbGenMips = false;

                                foreach (var item in tempimg.Bitmaps)
                                {
                                    origbmplist.Add(item.Bitmap.Clone<Bitmap>());
                                }

                                if (tempitem.Bitmap.Width >= 128 && tempitem.Bitmap.Height >= 128)
                                {
                                    curImage = tempimg.Clone();
                                    RefreshImageLists();
                                    SetHdRadiosEnabled(curImage.Bitmaps[0]);
                                    success = true;
                                    tabControl1.SelectedTab = Maintab;

                                }
                                else if (tempitem.Bitmap.Width == 64 && tempitem.Bitmap.Height == 64)
                                {
                                    mip64Fsh = tempimg.Clone();
                                    RefreshMipImageList(mip64Fsh, bmp64Mip, alpha64Mip, blend64Mip, listViewMip64);
                                    ListViewItem item = listViewMip64.Items[0];
                                    item.Selected = true;
                                    loadIsMip = true;
                                    success = true;
                                    tabControl1.SelectedTab = mip64tab;
                                }
                                else if (tempitem.Bitmap.Width == 32 && tempitem.Bitmap.Height == 32)
                                {
                                    mip32Fsh = tempimg.Clone();
                                    RefreshMipImageList(mip32Fsh, bmp32Mip, alpha32Mip, blend32Mip, listViewMip32);
                                    ListViewItem item = listViewMip32.Items[0];
                                    item.Selected = true;
                                    loadIsMip = true;
                                    success = true;
                                    tabControl1.SelectedTab = mip32tab;
                                }
                                else if (tempitem.Bitmap.Width == 16 && tempitem.Bitmap.Height == 16)
                                {
                                    mip16Fsh = tempimg.Clone();
                                    RefreshMipImageList(mip16Fsh, bmp16Mip, alpha16Mip, blend16Mip, listViewMip16);
                                    ListViewItem item = listViewMip16.Items[0];
                                    item.Selected = true;
                                    loadIsMip = true;
                                    success = true;
                                    tabControl1.SelectedTab = mip16tab;
                                }
                                else if (tempitem.Bitmap.Width == 8 && tempitem.Bitmap.Height == 8)
                                {
                                    mip8Fsh = tempimg.Clone();
                                    RefreshMipImageList(mip8Fsh, bmp8Mip, alpha8Mip, blend8Mip, listViewMip8);
                                    ListViewItem item = listViewMip8.Items[0];
                                    item.Selected = true;
                                    loadIsMip = true;
                                    success = true;
                                    tabControl1.SelectedTab = mip8tab;
                                }
                            }
                            if (success)
                            {
                                RefreshBmpType();
                                SetHdRadiosEnabled(bmpEntry);
                                string tgistr = fi.FullName + ".TGI";
                                if (File.Exists(tgistr)) // check for a TGI file from Ilive's Reader
                                {
                                    using (StreamReader sr = new StreamReader(tgistr))
                                    {
                                        string line;
                                        bool groupread = false;
                                        bool instread = false;

                                        while ((line = sr.ReadLine()) != null)
                                        {
                                            if (!string.IsNullOrEmpty(line))
                                            {
                                                if (line.Equals("7ab50e44", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    if (!groupread)
                                                    {
                                                        tgiGroupTxt.Text = line;
                                                        groupread = true;
                                                    }
                                                    else if (!instread)
                                                    {
                                                        instStr = line;
                                                        tgiInstanceTxt.Text = instStr;
                                                        EndFormat_Refresh();
                                                        instread = true;
                                                    }
                                                }
                                            }
                                        }

                                    }
                                } 
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
            }
        }
        /// <summary>
        /// Saves a fsh using either FshWrite or FSHLib
        /// </summary>
        /// <param name="fs">The stream to save to</param>
        /// <param name="image">The image to save</param>
        private void SaveFsh(Stream fs, FSHImageWrapper image)
        {
            try
            {
                if (IsDXTFsh(image) && fshWriteCompCb.Checked)
                {
                    Fshwrite fw = new Fshwrite();
                    for (int i = 0; i < image.Bitmaps.Count; i++)
		            {
                        BitmapEntry bi = image.Bitmaps[i];
                        if ((bi.Bitmap != null && bi.Alpha != null) && bi.BmpType == FSHBmpType.DXT1 || bi.BmpType == FSHBmpType.DXT3)
                        {
                            if (useorigimage && origbmplist[i].Size == bi.Bitmap.Size)
                            {
                                fw.bmp.Add(origbmplist[i]);
                            }
                            else
                            {
                                fw.bmp.Add(bi.Bitmap);
                            }
                            fw.alpha.Add(bi.Alpha);
                            fw.dir.Add(Encoding.ASCII.GetBytes(bi.DirName));
                            fw.code.Add((int)bi.BmpType);
                        }
                    }
                    fw.WriteFsh(fs);
                }
                else
                {
                    image.Save(fs);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Test if the fsh only contains DXT1 or DXT3 items
        /// </summary>
        /// <param name="image">The image to test</param>
        /// <returns>True if successful otherwise false</returns>
        private bool IsDXTFsh(FSHImageWrapper image)
        {
            foreach (BitmapEntry item in image.Bitmaps)
            {
                if (item.BmpType != FSHBmpType.DXT3 && item.BmpType != FSHBmpType.DXT1)
                {
                   return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Sets the IsDirty flag on the loaded dat if it has changed
        /// </summary>
        private void SetLoadedDatIsDirty()
        {
            if (dat != null && datListView.SelectedItems.Count > 0)
            {
                if (!dat.IsDirty)
                {
                    dat.IsDirty = true;
                }
            }

        }
        /// <summary>
        /// Save the fsh and reload it
        /// </summary>
        private void Temp_fsh()
        {
            try
            {
                SetLoadedDatEnables();
                using (MemoryStream mstream = new MemoryStream())
                {
                    SaveFsh(mstream, curImage);
                    curImage = new FSHImageWrapper(mstream);
                }

                SetLoadedDatIsDirty();

                ClearFshlists();
                RefreshImageLists();

                RefreshBmpType();
            }
            catch (Exception)
            {
                throw;
            }

        }
        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            FSHImageWrapper image = GetImageFromSelectedTab(tabControl1.SelectedIndex);
            
            if (image != null && image.Bitmaps.Count > 0)
            {
                if (colorRadio.Checked)
                {
                    if (tabControl1.SelectedTab == Maintab)
                    {
                        RefreshBitmapList(curImage, listViewMain, BitmapList1);
                    }
                    else if (tabControl1.SelectedTab == mip64tab)
                    {
                        RefreshBitmapList(mip64Fsh, listViewMip64, bmp64Mip);
                    }
                    else if (tabControl1.SelectedTab == mip32tab)
                    {
                        RefreshBitmapList(mip32Fsh, listViewMip32, bmp32Mip);
                    }
                    else if (tabControl1.SelectedTab == mip16tab)
                    {
                        RefreshBitmapList(mip16Fsh, listViewMip16, bmp16Mip);
                    }
                    else if (tabControl1.SelectedTab == mip8tab)
                    {
                        RefreshBitmapList(mip8Fsh, listViewMip8, bmp8Mip);
                    }
                }
                else if (alphaRadio.Checked)
                {
                    if (tabControl1.SelectedTab == Maintab)
                    {
                        RefreshAlphaList(curImage, listViewMain, alphaList1);
                    }
                    else if (tabControl1.SelectedTab == mip64tab)
                    {
                        RefreshAlphaList(mip64Fsh, listViewMip64, alpha64Mip);
                    }
                    else if (tabControl1.SelectedTab == mip32tab)
                    {
                        RefreshAlphaList(mip32Fsh, listViewMip32, alpha32Mip);
                    }
                    else if (tabControl1.SelectedTab == mip16tab)
                    {
                        RefreshAlphaList(mip16Fsh, listViewMip16, alpha16Mip);
                    }
                    else if (tabControl1.SelectedTab == mip8tab)
                    {
                        RefreshAlphaList(mip8Fsh, listViewMip8, alpha8Mip);
                    }
                }
                else if (blendRadio.Checked)
                {
                    if (tabControl1.SelectedTab == Maintab)
                    {
                        RefreshBlendList(curImage, listViewMain, blendList1);
                    }
                    else if (tabControl1.SelectedTab == mip64tab)
                    {
                        RefreshBlendList(mip64Fsh, listViewMip64, blend64Mip);
                    }
                    else if (tabControl1.SelectedTab == mip32tab)
                    {
                        RefreshBlendList(mip32Fsh, listViewMip32, blend32Mip);
                    }
                    else if (tabControl1.SelectedTab == mip16tab)
                    {
                        RefreshBlendList(mip16Fsh, listViewMip16, blend16Mip);
                    }
                    else if (tabControl1.SelectedTab == mip8tab)
                    {
                        RefreshBlendList(mip8Fsh, listViewMip8, blend8Mip);
                    }
                }
            }
            else 
            {
                colorRadio.Checked = true;
                alphaRadio.Checked = false;
                blendRadio.Checked = false;
            }
            
        }
        
        /// <summary>
        /// Refreshes the list of bitmaps
        /// </summary>
        /// <param name="image">The image to refresh the list from</param>
        /// <param name="listview">The listview to add the images to</param>
        /// <param name="imglist">The ImageList containing the alpha bitmaps to use</param>
        private void RefreshBitmapList(FSHImageWrapper image, ListView listview, ImageList imglist)
        {
            if (listview.Items.Count > 0)
            {
                listview.Items.Clear();
            }
            listview.LargeImageList = imglist;
            listview.SmallImageList = imglist;

            for (int cnt = 0; cnt < image.Bitmaps.Count; cnt++)
            {
                ListViewItem alpha = new ListViewItem(Resources.BitmapNumberText + cnt.ToString(), cnt);
                listview.Items.Add(alpha);
            }
        }

        /// <summary>
        /// Refreshes the list of alpha bitmaps
        /// </summary>
        /// <param name="image">The image to refresh the list from</param>
        /// <param name="listview">The listview to add the images to</param>
        /// <param name="imglist">The ImageList containing the alpha bitmaps to use</param>
        private void RefreshAlphaList(FSHImageWrapper image, ListView listview, ImageList imglist)
        {
            if (listview.Items.Count > 0)
            {
                listview.Items.Clear();
            }
            listview.LargeImageList = imglist;
            listview.SmallImageList = imglist;
          
            for (int cnt = 0; cnt < image.Bitmaps.Count; cnt++)
            {
                ListViewItem alpha = new ListViewItem(Resources.AlphaNumberText + cnt.ToString(), cnt);
                listview.Items.Add(alpha);
            }
        }
        /// <summary>
        /// Refreshes the list of blended bitmaps
        /// </summary>
        /// <param name="image">The image to refresh the list from</param>
        /// <param name="listview">The listview to add the images to</param>
        /// <param name="imglist">The ImageList containing the blended bitmaps to use</param>
        private void RefreshBlendList(FSHImageWrapper image,ListView listview,ImageList imglist)
        {
            if (listview.Items.Count > 0)
            {
                listview.Items.Clear();
            }
            listview.LargeImageList = imglist;
            listview.SmallImageList = imglist; 
         
            for (int cnt = 0; cnt < image.Bitmaps.Count; cnt++)
            {
                ListViewItem blend = new ListViewItem(Resources.BlendNumberText + cnt.ToString(), cnt);
                listview.Items.Add(blend);
            }
        }
        private Bitmap Alphablend(BitmapEntry item)
        { 
            Bitmap blendbmp = new Bitmap(bmpEntry.Bitmap.Width, bmpEntry.Bitmap.Height);
            using(Graphics g = Graphics.FromImage(blendbmp))
	        {
                using (HatchBrush brush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.FromArgb(192, 192, 192)))
                {
		            g.FillRectangle(brush, new Rectangle(0, 0, blendbmp.Width, blendbmp.Height));
                }

                g.DrawImageUnscaled(BlendBitmap.BlendBmp(bmpEntry), new Rectangle(0, 0, blendbmp.Width, blendbmp.Height)); 
	        }
            return blendbmp;
        }
        private int typeindex = 0; // store previously selected fsh type
        private void listViewmain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMain.SelectedItems.Count > 0)
            {
                bmpEntry = curImage.Bitmaps[listViewMain.SelectedItems[0].Index];
                Settypeindex(bmpEntry);
                RefreshBmpType();

                sizeLbl.Text = fshSize[listViewMain.SelectedItems[0].Index];
                dirTxt.Text = dirName[listViewMain.SelectedItems[0].Index];
            }
        }
        private void Settypeindex(BitmapEntry item)
        {
            switch (item.BmpType) // set the stored bmp type
            {
                case FSHBmpType.TwentyFourBit:
                    typeindex = 0;
                    break;
                case FSHBmpType.ThirtyTwoBit:
                    typeindex = 1;
                    break;
                case FSHBmpType.DXT1:
                    typeindex = 2;
                    break;
                case FSHBmpType.DXT3:
                    typeindex = 3;
                    break;
            }
        }
       /// <summary>
        /// Check if the bitmap size is 128 x 128 or larger for the Replace function.
       /// </summary>
       /// <param name="bmp">The bitmap to check</param>
       /// <returns>True if the image is 128 x 128 or larger otherwise false</returns>
        private bool CheckReplaceBitmapSize(Bitmap bmp)
        {
            if (tabControl1.SelectedTab == Maintab)
            {
                if (bmp.Width >= 128 && bmp.Height >= 128)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(this, Resources.CheckReplaceBitmapSizeError, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Check the size of the files for images 64 x 64 or smaller
        /// </summary>
        /// <param name="files">The list of files to check</param>
        /// <returns>The filtered list of files</returns>
        private List<string> CheckSize(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {

                if (Path.GetExtension(list[i]).Equals(".png", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(list[i]).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    using (Bitmap b = new Bitmap(list[i]))
                    {
                        if (b.Width < 128 && b.Height < 128)
                        {
                            list.Remove(list[i]);
                        }
                    }
                }
                else
                {
                    list.Remove(list[i]);
                }
            }
            list.TrimExcess();
            list.Sort();
            return list;
        }
      
        private void addbtn_Click(object sender, EventArgs e)
        {
            openBitmapDialog1.Multiselect = true;
            if (openBitmapDialog1.ShowDialog(this) == DialogResult.OK)
            {
                List<string> list = new List<string>(openBitmapDialog1.FileNames);
                if (curImage == null && bmpEntry == null)
                {
                    NewFsh(list); // use the NewFsh function to create a new fsh if one does not exist
                }
                else
                {
                    AddbtnFiles(list, false);
                }
            }
        }

        private void addbtn_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private List<Bitmap> origbmplist = null;
        /// <summary>
        /// Adds a list files to an a new or existing image  
        /// </summary>
        /// <param name="files">The files to add</param>
        /// <param name="listfiltered">Has the list of files been previously been filtered</param>
        private void AddbtnFiles(List<string> files, bool listfiltered) 
        {
            try
            {
                if (curImage != null && bmpEntry != null)
                {
                    if (!listfiltered)
                    {
                        files = CheckSize(files);
                        files = TrimAlphaBitmaps(files);
                    }
                    for (int f = 0; f < files.Count; f++)
                    {
                        FileInfo fi = new FileInfo(files[f]);

                        BitmapEntry addbmp = new BitmapEntry();
                        string alphaPath = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.FullName) + "_a" + fi.Extension);

                        if (fi.Exists)
                        {
                            using (Bitmap bmp = new Bitmap(fi.FullName))
                            {
                                if (origbmplist == null)
                                {
                                    origbmplist = new List<Bitmap>();
                                }
                                origbmplist.Add(bmp.Clone<Bitmap>());
                                addbmp.Bitmap = bmp.Clone<Bitmap>();
                                this.fshWriteCbGenMips = true;

                                if (File.Exists(alphaPath))
                                {
                                    Bitmap alpha = new Bitmap(alphaPath);
                                    addbmp.Alpha = alpha;
                                    if (Checkhdimgsize(bmp) && fi.Name.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                    {
                                        addbmp.BmpType = FSHBmpType.ThirtyTwoBit;
                                        fshTypeBox.SelectedIndex = 1;
                                    }
                                    else
                                    {
                                        addbmp.BmpType = FSHBmpType.DXT3;
                                        fshTypeBox.SelectedIndex = 3;
                                    }

                                }
                                else if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
                                {
                                    Bitmap testbmp = GetAlphafromPng(bmp);
                                    addbmp.Alpha = testbmp;
                                    if (Checkhdimgsize(bmp) && fi.Name.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                    {
                                        addbmp.BmpType = FSHBmpType.ThirtyTwoBit;
                                        fshTypeBox.SelectedIndex = 1;
                                    }
                                    else
                                    {
                                        addbmp.BmpType = FSHBmpType.DXT3;
                                        fshTypeBox.SelectedIndex = 3;
                                    }
                                }
                                else
                                {
                                    addbmp.Alpha = GenerateAlpha(bmp);
                                    if (Checkhdimgsize(bmp) && fi.Name.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                    {
                                        addbmp.BmpType = FSHBmpType.TwentyFourBit;
                                        fshTypeBox.SelectedIndex = 0;
                                    }
                                    else
                                    {
                                        addbmp.BmpType = FSHBmpType.DXT1;
                                        fshTypeBox.SelectedIndex = 2;
                                    }
                                
                                }
                                if ((dirTxt.Text.Length > 0) && dirTxt.Text.Length == 4)
                                {
                                    addbmp.DirName = dirTxt.Text;
                                }
                                else
                                {
                                    addbmp.DirName = "FiSH";
                                }

                                if (bmp.Height < 256 && bmp.Width < 256)
                                {
                                    hdFshRadio.Enabled = false;
                                    hdBaseFshRadio.Enabled = false;
                                    regFshRadio.Checked = true;
                                }
                                else
                                {
                                    hdFshRadio.Enabled = true;
                                    hdBaseFshRadio.Enabled = true;
                                    if (bmpEntry.BmpType == FSHBmpType.ThirtyTwoBit)
                                    {
                                        hdFshRadio.Checked = true;
                                    }
                                    else if (bmpEntry.BmpType == FSHBmpType.TwentyFourBit)
                                    {
                                        hdBaseFshRadio.Checked = true;
                                    }
                                    else
                                    {
                                        regFshRadio.Checked = true;
                                    }
                                }

                                if (tabControl1.SelectedTab == Maintab)
                                {
                                    colorRadio.Checked = true;
                                    if (curImage == null)
                                    {
                                        curImage = new FSHImageWrapper();
                                    }
                                    curImage.Bitmaps.Add(addbmp);
                                    if (f == files.Count - 1)
                                    {
                                        Temp_fsh();
                                        mipbtn_Click(null, null);
                                    }
                                } 
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void addbtn_DragDrop(object sender, DragEventArgs e)
        {
            if (tabControl1.SelectedTab == Maintab)
            {
                List<string> files = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));
                if (curImage == null && bmpEntry == null)
                {
                    NewFsh(files); // use the NewFsh function to create a new fsh if one does not exist
                }
                else
                {
                    AddbtnFiles(files, false);
                }
            }
        }
        
        /// <summary>
        /// Rebuilds the Mip ImageLists
        /// </summary>
        /// <param name="image">The image to build the lists from</param>
        /// <param name="bmplist">The list to hold the Bitmap</param>
        /// <param name="alphalist">The list to hold the Alpha</param>
        /// <param name="blendlist">The list to hold the Blended Bitmap</param>
        /// <param name="list">The ListView to display the images</param>
        private void RefreshMipImageList(FSHImageWrapper image, ImageList bmplist, ImageList alphalist, ImageList blendlist, ListView list)
        {
            bmplist.Images.Clear();
            alphalist.Images.Clear();
            blendlist.Images.Clear(); 
            
            if (image.Bitmaps.Count > 1)
            {

                for (int cnt = 0; cnt < image.Bitmaps.Count; cnt++)
                {
                    bmpEntry = image.Bitmaps[cnt];
                    Reset24bitAlpha(bmpEntry);
                    bmplist.Images.Add(bmpEntry.Bitmap);
                    alphalist.Images.Add(bmpEntry.Alpha);
                    blendlist.Images.Add(Alphablend(bmpEntry));

                }

            }
            else
            {

                bmpEntry = image.Bitmaps[0];
                Reset24bitAlpha(bmpEntry);
                bmplist.Images.Add(bmpEntry.Bitmap);
                alphalist.Images.Add(bmpEntry.Alpha);
                blendlist.Images.Add(Alphablend(bmpEntry));
            }
            RefreshDirectory(image);
            
            list.BeginUpdate();
            if (colorRadio.Checked)
            {
                RefreshBitmapList(image, list, bmplist);
            }
            else if (alphaRadio.Checked)
            {
                RefreshAlphaList(image, list, alphalist);
            }
            else if (blendRadio.Checked)
            {
                RefreshBlendList(image, list, blendlist);
            }
            list.EndUpdate();

        }
        private unsafe void Reset24bitAlpha(BitmapEntry item)
        {
            if (item.BmpType == FSHBmpType.TwentyFourBit)
            {
                item.Alpha = new Bitmap(item.Bitmap.Width,item.Bitmap.Height, PixelFormat.Format24bppRgb);
                BitmapData bd = item.Alpha.LockBits(new Rectangle(0, 0, item.Alpha.Width, item.Alpha.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
                try
                {
                    for (int y = 0; y < bd.Height; y++)
                    {
                        byte* p = (byte*)bd.Scan0.ToPointer() + (y * bd.Stride);
                        for (int x = 0; x < bd.Width; x++)
                        {
                            p[0] = p[1] = p[2] = 255;
                            p += 3;
                        }
                    }
                }
                finally
                {
                    item.Alpha.UnlockBits(bd);
                }
            }
        }

        /// <summary>
        /// Refreshes the listviewMain ImageLists from the current fsh 
        /// </summary>
        private void RefreshImageLists()
        {
            if (curImage.Bitmaps.Count > 1)
            {

                remBtn.Enabled = true;
                for (int cnt = 0; cnt < curImage.Bitmaps.Count; cnt++)
                {
                    bmpEntry = curImage.Bitmaps[cnt];

                    Reset24bitAlpha(bmpEntry);
                    BitmapList1.Images.Add(bmpEntry.Bitmap);
                    alphaList1.Images.Add(bmpEntry.Alpha);
                    blendList1.Images.Add(Alphablend(bmpEntry));

                }
                  
            }
            else
            {
                remBtn.Enabled = false;
                bmpEntry = curImage.Bitmaps[0];
                Reset24bitAlpha(bmpEntry);
                BitmapList1.Images.Add(bmpEntry.Bitmap);
                alphaList1.Images.Add(bmpEntry.Alpha);
                blendList1.Images.Add(Alphablend(bmpEntry));

            } 
            
            RefreshDirectory(curImage);

            listViewMain.BeginUpdate();
            if (colorRadio.Checked)
            {
                RefreshBitmapList(curImage, listViewMain, BitmapList1);
            }
            else if (alphaRadio.Checked)
            {
                RefreshAlphaList(curImage, listViewMain, alphaList1);
            }
            else if (blendRadio.Checked)
            {
                RefreshBlendList(curImage, listViewMain, blendList1);
            }
            listViewMain.EndUpdate();
        }
        /// <summary>
        /// Refresh the fsh size and dir name for the input image 
        /// </summary>
        /// <param name="image">The input image</param>
        private void RefreshDirectory(FSHImageWrapper image)
        {
            dirName = new string[image.Bitmaps.Count];
            fshSize = new string[image.Bitmaps.Count];
            for (int i = 0; i < image.Bitmaps.Count; i++)
            {
                FSHDirEntry dir = image.GetDirectoryEntry(i);
                EntryHeader entryhead = image.GetEntryHeader(dir.offset);
                dirName[i] = Encoding.ASCII.GetString(dir.name);
                fshSize[i] = entryhead.Width.ToString(CultureInfo.CurrentCulture) + "x" + entryhead.Height.ToString(CultureInfo.CurrentCulture);
            }
        }
        private void dirTxt_TextChanged(object sender, EventArgs e)
        {
            if (curImage != null && tabControl1.SelectedTab == Maintab)
            {
                if (dirTxt.Text.Length > 0 && dirTxt.Text.Length == 4 && dirTxt.Text != bmpEntry.DirName)
	            {
                    bmpEntry.DirName = dirTxt.Text;
                    Temp_fsh();
                }
            }
        }
        
        private void remBtn_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == Maintab) 
            {
                if (listViewMain.SelectedItems.Count > 0)
                {
                    try
                    {
                        curImage.Bitmaps.Remove(bmpEntry); //remove the item and rebuild the mipmaps
                        Temp_fsh();
                        mipbtn_Click(null, null);
                        listViewMain.Items[0].Selected = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, this.Text);
                    }
                }
            }
        }
        private void repBtn_Click(object sender, EventArgs e)
        {
            if (curImage != null && bmpEntry != null)
            {
                if (listViewMain.SelectedItems.Count > 0)
                {
                        
                    Bitmap bmp = null;

                    try
                    {
                        BitmapEntry repBmp = new BitmapEntry();
                        bool bmpLoaded = false;
                        openBitmapDialog1.Multiselect = false;
                        string alphaMap = string.Empty;
                        string bmpFileName = string.Empty; // holds the filneame from the bmpBox TextBox or the OpenBitmapDialog 

                        if (bmpBox.Text.Length > 0 && File.Exists(bmpBox.Text))
                        {
                            bmp = new Bitmap(bmpBox.Text);
                            if (CheckReplaceBitmapSize(bmp))
                            {
                                repBmp.Bitmap = bmp.Clone(PixelFormat.Format24bppRgb);
                                bmpFileName = bmpBox.Text;
                                bmpLoaded = true;
                            }
                        }
                        else if (openBitmapDialog1.ShowDialog() == DialogResult.OK)
                        {
                            if (!Path.GetFileNameWithoutExtension(openBitmapDialog1.FileName).Contains("_a"))
                            {
                                bmp = new Bitmap(openBitmapDialog1.FileName);
                                alphaMap = Path.Combine(Path.GetDirectoryName(openBitmapDialog1.FileName), Path.GetFileNameWithoutExtension(openBitmapDialog1.FileName) + "_a" + Path.GetExtension(openBitmapDialog1.FileName));
                                if (CheckReplaceBitmapSize(bmp))
                                {
                                    repBmp.Bitmap = bmp.Clone(PixelFormat.Format24bppRgb);
                                    bmpFileName = openBitmapDialog1.FileName;
                                    bmpLoaded = true;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(this, Resources.repBmp_NewFileSelect_Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if (bmpLoaded)
                        {

                            if (alphaBox.Text.Length > 0 && File.Exists(alphaBox.Text))
                            {
                                using (Bitmap alpha = new Bitmap(alphaBox.Text))
                                {
                                    repBmp.Alpha = alpha.Clone(PixelFormat.Format24bppRgb);
                                }

                                if (Checkhdimgsize(bmp) && Path.GetFileName(bmpFileName).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                {
                                    repBmp.BmpType = FSHBmpType.ThirtyTwoBit;
                                    fshTypeBox.SelectedIndex = 1;
                                }
                                else
                                {
                                    repBmp.BmpType = FSHBmpType.DXT3;
                                    fshTypeBox.SelectedIndex = 3;
                                }
                            }
                            else if (!string.IsNullOrEmpty(alphaMap) && File.Exists(alphaMap))
                            {
                                Bitmap alpha = new Bitmap(alphaMap);
                                repBmp.Alpha = alpha;
                                if (Checkhdimgsize(bmp) && Path.GetFileName(bmpFileName).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                {
                                    repBmp.BmpType = FSHBmpType.ThirtyTwoBit;
                                    fshTypeBox.SelectedIndex = 1;
                                }
                                else
                                {
                                    repBmp.BmpType = FSHBmpType.DXT3;
                                    fshTypeBox.SelectedIndex = 3;
                                }
                            }
                            else if (Path.GetExtension(bmpFileName).Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
                            {
                                repBmp.Alpha = GetAlphafromPng(bmp);
                                if (Checkhdimgsize(bmp) && Path.GetFileName(bmpFileName).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                {
                                    repBmp.BmpType = FSHBmpType.ThirtyTwoBit;
                                    fshTypeBox.SelectedIndex = 1;
                                }
                                else
                                {
                                    repBmp.BmpType = FSHBmpType.DXT3;
                                    fshTypeBox.SelectedIndex = 3;
                                }
                            }
                            else
                            {
                                repBmp.Alpha = GenerateAlpha(bmp);
                                if (Checkhdimgsize(bmp) && Path.GetFileName(bmpFileName).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                {
                                    repBmp.BmpType = FSHBmpType.TwentyFourBit;
                                    fshTypeBox.SelectedIndex = 0;
                                }
                                else
                                {
                                    repBmp.BmpType = FSHBmpType.DXT1;
                                    fshTypeBox.SelectedIndex = 2;
                                }
                            }
                            if ((dirTxt.Text.Length > 0) && dirTxt.Text.Length == 4)
                            {
                                repBmp.DirName = dirTxt.Text;
                            }
                            else
                            {
                                repBmp.DirName = "FiSH";
                            }

                            curImage.Bitmaps.RemoveAt(listViewMain.SelectedItems[0].Index);
                            curImage.Bitmaps.Insert(listViewMain.SelectedItems[0].Index, repBmp);

                            Temp_fsh();
                            mipbtn_Click(null, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (bmp != null)
                        {
                            bmp.Dispose();
                            bmp = null;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, Resources.repBmp_NoImageSelected_Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        private void hdFshRadio_CheckedChanged(object sender, EventArgs e)
        {
            
            if (bmpEntry != null && bmpEntry.Bitmap != null)
            {
                if (bmpEntry.Bitmap.Width < 256 && bmpEntry.Bitmap.Height < 256)
                {
                    hdFshRadio.Checked = false;
                    hdBaseFshRadio.Checked = false;
                    regFshRadio.Checked = true;
                    hdFshRadio.Enabled = false;
                    hdBaseFshRadio.Enabled = false;
                }
                else
                {
                    if (curImage.Bitmaps.Count > 1)
                    {
                        hdFshRadio.Enabled = false;
                    }
                    else
                    {
                        hdFshRadio.Enabled = true;
                    }
                    hdBaseFshRadio.Enabled = true;
                    if (hdFshRadio.Checked)
                    {
                        bmpEntry.BmpType = FSHBmpType.ThirtyTwoBit;
                        fshTypeBox.SelectedIndex = 1;
                    }
                    else if (hdBaseFshRadio.Checked)
                    {
                        bmpEntry.BmpType = FSHBmpType.TwentyFourBit;
                        fshTypeBox.SelectedIndex = 0;
                    }
                    else if (regFshRadio.Checked)
                    {
                        if (bmpEntry.Alpha.GetPixel(0, 0).ToArgb() != Color.White.ToArgb())
                        {
                            bmpEntry.BmpType = FSHBmpType.DXT3; 
                            fshTypeBox.SelectedIndex = 3;
                        }
                        else
                        {
                            bmpEntry.BmpType = FSHBmpType.DXT1;
                            fshTypeBox.SelectedIndex = 2;
                        }
                    }
                }

            }
            else
            {
                hdFshRadio.Checked = false;
                hdBaseFshRadio.Checked = false;
                regFshRadio.Checked = true;
            }
        }
        /// <summary>
        /// Save and reload the mip images
        /// </summary>
        /// <param name="mipsize">The size of the image</param>
        private void Temp_Mips(int mipsize)
        {
            try
            {
                
                using (MemoryStream mstream = new MemoryStream())
                {
                    switch (mipsize)
                    {
                        case 64:
                            SaveFsh(mstream, mip64Fsh);
                            mip64Fsh = new FSHImageWrapper(mstream);
                            break;
                        case 32:
                            SaveFsh(mstream, mip32Fsh);
                            mip32Fsh = new FSHImageWrapper(mstream);
                            break;
                        case 16:
                            SaveFsh(mstream, mip16Fsh);
                            mip16Fsh = new FSHImageWrapper(mstream);
                            break;
                        case 8:
                            SaveFsh(mstream, mip8Fsh);
                            mip8Fsh = new FSHImageWrapper(mstream);
                            break;
                    }
                }

                switch (mipsize)
                { 
                    case 64:
                        RefreshMipImageList(mip64Fsh, bmp64Mip, alpha64Mip, blend64Mip, listViewMip64);
                        break;
                    case 32:
                        RefreshMipImageList(mip32Fsh, bmp32Mip, alpha32Mip, blend32Mip, listViewMip32);
                        break;
                    case 16:
                        RefreshMipImageList(mip16Fsh, bmp16Mip, alpha16Mip, blend16Mip, listViewMip16);
                        break;
                    case 8:
                        RefreshMipImageList(mip8Fsh, bmp8Mip, alpha8Mip, blend8Mip, listViewMip8);
                        break;
                }
                if (tabControl1.SelectedTab != Maintab)
                {
                    RefreshBmpType();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text);
            }

        }
        /// <summary>
        /// Creates the mip thumbnail using Graphics.DrawImage
        /// </summary>
        /// <param name="source">The Bitmap to draw</param>
        /// <param name="width">The width of the new bitmap</param>
        /// <param name="height">The height of the new bitmap</param>
        /// <returns>The new scaled Bitmap</returns>
        private Bitmap GetBitmapThumbnail(Bitmap source, int width, int height)
        {
            return SuperSample.SuperSample.GetBitmapThumbnail(source, width, height);
        }
        private bool mipsbtn_clicked = false;

        /// <summary>
        /// Generates the mimmaps for the zoom levels
        /// </summary>
        /// <param name="index">the index of the bitmap to scale</param>
        private void GenerateMips(int index)
        {
            
            Bitmap[] bmps = new Bitmap[4];
            Bitmap[] alphas = new Bitmap[4];

            try
            {
                BitmapEntry item = curImage.Bitmaps[index];
                // 0 = 8, 1 = 16, 2 = 32, 3 = 64

                bmps[0] = GetBitmapThumbnail(item.Bitmap, 8, 8);
                bmps[1] = GetBitmapThumbnail(item.Bitmap, 16, 16);
                bmps[2] = GetBitmapThumbnail(item.Bitmap, 32, 32);
                bmps[3] = GetBitmapThumbnail(item.Bitmap, 64, 64);
                //alpha
                alphas[0] = GetBitmapThumbnail(item.Alpha, 8, 8);
                alphas[1] = GetBitmapThumbnail(item.Alpha, 16, 16);
                alphas[2] = GetBitmapThumbnail(item.Alpha, 32, 32);
                alphas[3] = GetBitmapThumbnail(item.Alpha, 64, 64);

                for (int i = 0; i < 4; i++)
                {
                    if (bmps[i] != null && alphas[i] != null)
                    {
                        BitmapEntry mipitm = new BitmapEntry();
                        mipitm.Bitmap = bmps[i].Clone<Bitmap>();
                        mipitm.Alpha = alphas[i].Clone<Bitmap>();

                        if (!string.IsNullOrEmpty(item.DirName))
                        {
                            mipitm.DirName = item.DirName;
                        }
                        else
                        {
                            mipitm.DirName = "FiSH";
                        }
                        
                        if (item.BmpType == FSHBmpType.DXT3 || item.BmpType == FSHBmpType.ThirtyTwoBit)
                        {
                            mipitm.BmpType = FSHBmpType.DXT3;
                        }
                        else
                        {
                            mipitm.BmpType = FSHBmpType.DXT1;
                        }
                        if (mipitm.Bitmap.Width == 64 && mipitm.Bitmap.Height == 64)
                        {
                            if (mip64Fsh == null)
                            {
                                mip64Fsh = new FSHImageWrapper();
                            }
                            mip64Fsh.Bitmaps.Add(mipitm);
                            if (index == (curImage.Bitmaps.Count - 1))
                            {
                                Temp_Mips(64);
                            }
                        }
                        else if (mipitm.Bitmap.Width == 32 && mipitm.Bitmap.Height == 32)
                        {
                            if (mip32Fsh == null)
                            {
                                mip32Fsh = new FSHImageWrapper();
                            }
                            mip32Fsh.Bitmaps.Add(mipitm);
                            if (index == (curImage.Bitmaps.Count - 1))
                            {
                                Temp_Mips(32);
                            }
                        }
                        else if (mipitm.Bitmap.Width == 16 && mipitm.Bitmap.Height == 16)
                        {
                            if (mip16Fsh == null)
                            {
                                mip16Fsh = new FSHImageWrapper();
                            }
                            mip16Fsh.Bitmaps.Add(mipitm);
                            if (index == (curImage.Bitmaps.Count - 1))
                            {
                                Temp_Mips(16);
                            }
                        }
                        else if (mipitm.Bitmap.Width == 8 && mipitm.Bitmap.Height == 8)
                        {
                            if (mip8Fsh == null)
                            {
                                mip8Fsh = new FSHImageWrapper();
                            }
                            mip8Fsh.Bitmaps.Add(mipitm);
                            if (index == (curImage.Bitmaps.Count - 1))
                            {
                                Temp_Mips(8);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                for (int i = 0; i < 4; i++)
                {
                    if (bmps[i] != null)
                    {
                        bmps[i].Dispose();
                        bmps[i] = null;
                    }

                    if (alphas[i] != null)
                    {
                        alphas[i].Dispose();
                        alphas[i] = null;
                    }
                }
            }
        }
        private void mipbtn_Click(object sender, EventArgs e)
        {
            if ((curImage != null) && curImage.Bitmaps.Count >= 1)
            {
                try
                {
                    mip64Fsh = null;
                    mip32Fsh = null;
                    mip16Fsh = null;
                    mip8Fsh = null;
                    for (int b = 0; b < curImage.Bitmaps.Count; b++)
                    {
                        GenerateMips(b);
                    }
                    RefreshDirectory(curImage);
                    mipsbtn_clicked = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        /// <summary>
        /// Write the TGI file for The Reader
        /// </summary>
        private void WriteTgi(string filename, int zoom)
        {
            FileStream fs = new FileStream(filename + ".TGI", FileMode.OpenOrCreate, FileAccess.Write);

            try
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("7ab50e44\t\n");
                    sw.WriteLine(string.Format("{0:X8}", tgiGroupTxt.Text.ToString() + "\n"));
                    switch (zoom)
                    {
                        case 0:
                            sw.WriteLine(string.Format("{0:X8}", tgiInstanceTxt.Text.Substring(0, 7) + end8));
                            break;
                        case 1:
                            sw.WriteLine(string.Format("{0:X8}", tgiInstanceTxt.Text.Substring(0, 7) + end16));
                            break;
                        case 2:
                            sw.WriteLine(string.Format("{0:X8}", tgiInstanceTxt.Text.Substring(0, 7) + end32));
                            break;
                        case 3:
                            sw.WriteLine(string.Format("{0:X8}", tgiInstanceTxt.Text.Substring(0, 7) + end64));
                            break;
                        case 4:
                            sw.WriteLine(string.Format("{0:X8}", tgiInstanceTxt.Text.Substring(0, 7) + endreg));
                            break;

                    }
                }
                fs = null;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Dispose();
                    fs = null;
                }
            }


        }
        
        private void FshtypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FSHImageWrapper image = GetImageFromSelectedTab(tabControl1.SelectedIndex);
            if (bmpEntry != null && bmpEntry.Bitmap != null)
            {
                if (!Checkhdimgsize(bmpEntry.Bitmap))
                {
                    if (fshTypeBox.SelectedIndex == 0 || fshTypeBox.SelectedIndex == 1)
                    {
                        fshTypeBox.SelectedIndex = typeindex;
                    }
                }
             
                if (fshTypeBox.SelectedIndex == 0)
                {
                    hdBaseFshRadio.Checked = true;
                }
                else if (fshTypeBox.SelectedIndex == 1)
                {
                    hdFshRadio.Checked = true;
                }
                else
                {
                    regFshRadio.Checked = true;
                }
                switch (fshTypeBox.SelectedIndex)
                {
                    case 0:
                        bmpEntry.BmpType = FSHBmpType.TwentyFourBit;
                        break;
                    case 1:
                        bmpEntry.BmpType = FSHBmpType.ThirtyTwoBit;
                        break;
                    case 2:
                        bmpEntry.BmpType = FSHBmpType.DXT1;
                        break;
                    case 3:
                        bmpEntry.BmpType = FSHBmpType.DXT3;
                        break;
                }
            }
        }
        private FileStream mip_stream(string path, string addtopath)
        {
            return new FileStream(path + addtopath, FileMode.OpenOrCreate, FileAccess.Write);
        }
        private void saveFshBtn_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == Maintab)
            {
                if (curImage != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        curImage.IsCompressed = true;
                    }
                    else
                    {
                        curImage.IsCompressed = false;
                    }

                    if (!loadedDat && datListView.Items.Count == 0)
                    {
                        if (!mipsbtn_clicked)
                        {
                            mipbtn_Click(sender, e);
                        }
                        if (mipsbtn_clicked && mip64Fsh != null && mip32Fsh != null && mip16Fsh != null && mip8Fsh != null)
                        {
                            string filepath = Path.Combine(Path.GetDirectoryName(saveFshDialog1.FileName) + Path.DirectorySeparatorChar, Path.GetFileName(saveFshDialog1.FileName));
                            if (curImage.IsCompressed)
                            {
                                mip64Fsh.IsCompressed = true;
                                mip32Fsh.IsCompressed = true;
                                mip16Fsh.IsCompressed = true;
                                mip8Fsh.IsCompressed = true;
                            }
                            string ext = Path.GetExtension(saveFshDialog1.FileName);
                            using (FileStream m64 = mip_stream(filepath, "_s3" + ext))
                            {
                                SaveFsh(m64,mip64Fsh);
                            }
                            WriteTgi(filepath + "_s3" + ext, 3);

                            using (FileStream m32 = mip_stream(filepath, "_s2"+ ext))
                            {
                                SaveFsh(m32, mip32Fsh);
                            }
                            WriteTgi(filepath + "_s2" + ext, 2);

                            using (FileStream m16 = mip_stream(filepath, "_s1" + ext))
                            {
                                SaveFsh(m16, mip16Fsh);
                            }
                            WriteTgi(filepath + "_s1" + ext, 1);

                            using (FileStream m8 = mip_stream(filepath, "_s0" + ext))
                            {
                                SaveFsh(m8, mip8Fsh);
                            }
                            WriteTgi(filepath + "_s0" + ext, 0);
                        }
                        WriteTgi(saveFshDialog1.FileName, 4);
                    }
                    using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fs, curImage);
                    }
                }
            }
            else if (tabControl1.SelectedTab == mip64tab)
            {
                if (mip64Fsh != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        mip64Fsh.IsCompressed = true;
                    }
                    else
                    {
                        mip64Fsh.IsCompressed = false;
                    }
                    WriteTgi(saveFshDialog1.FileName, 3);
                    using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fs, mip64Fsh);
                    }
                }
            }
            else if (tabControl1.SelectedTab == mip32tab)
            {
                if (mip32Fsh != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        mip32Fsh.IsCompressed = true;
                    }
                    else
                    {
                        mip32Fsh.IsCompressed = false;
                    }
                    WriteTgi(saveFshDialog1.FileName, 2);
                    using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fs, mip32Fsh);
                    }
                }
            }
            else if (tabControl1.SelectedTab == mip16tab)
            {
                if (mip16Fsh != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        mip16Fsh.IsCompressed = true;
                    }
                    else
                    {
                        mip16Fsh.IsCompressed = false;
                    }
                    WriteTgi(saveFshDialog1.FileName, 1);
                    using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fs, mip16Fsh);
                    }
                }
            }
            else if (tabControl1.SelectedTab == mip8tab)
            {
                if (mip8Fsh != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        mip8Fsh.IsCompressed = true;
                    }
                    else
                    {
                        mip8Fsh.IsCompressed = false;
                    }
                    WriteTgi(saveFshDialog1.FileName, 0);
                    using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fs, mip8Fsh);   
                    }
                }
            }
        }
        private void saveBitmap(Bitmap bmp, PixelFormat format, string addtofilename)
        {
            ListView listv = new ListView();
            FSHImageWrapper image = new FSHImageWrapper();
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    listv = listViewMain;
                    image = curImage;
                    break;
                case 1:
                    listv = listViewMip64;
                    image = mip64Fsh;
                    break;
                case 2:
                    listv = listViewMip32;
                    image = mip32Fsh;
                    break;
                case 3:
                    listv = listViewMip16;
                    image = mip16Fsh;
                    break;
                case 4:
                    listv = listViewMip8;
                    image = mip8Fsh;
                    break;

            }
            if (listv.SelectedItems.Count > 0)
            {
                try
                {
                    string bitmapnum = image.Bitmaps.Count > 1 ? "-" + listv.SelectedItems[0].Index.ToString() : string.Empty;

                    if (!string.IsNullOrEmpty(fshFileName))
                    {
                        string name = string.Concat(fshFileName, bitmapnum, addtofilename, ".png");
                        using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            using (Bitmap tempbmp = bmp.Clone(format))
                            {
                                tempbmp.Save(fs, ImageFormat.Png);
                            }
                        }

                    }
                    else if (loadedDat && datListView.SelectedItems.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dat.FileName))
                        {
                            ListViewItem item = datListView.SelectedItems[0];
                            string fshname = Path.Combine(Path.GetDirectoryName(dat.FileName), "0x" + item.SubItems[2].Text);

                            string name = string.Concat(fshname, bitmapnum, addtofilename, ".png");

                            using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                using (Bitmap tempbmp = bmp.Clone(format))
                                {
                                    tempbmp.Save(fs, ImageFormat.Png);
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (image != null && image.Bitmaps.Count > 0)
                {
                    string prefix = string.Empty;
                    switch (addtofilename)
                    {
                        case "_a":
                            prefix = Resources.saveBitmap_Alpha_Prefix;
                            break;
                        case "_blend":
                            prefix = Resources.saveBitmap_Blended_Prefix;
                            break;
                    }
                    // use the prefix if it exists

                    string message = string.Format(Resources.saveBitmap_Error_Format, !string.IsNullOrEmpty(prefix) ? string.Concat(prefix," bitmap") : "bitmap");
                    MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void saveBmpBlendBtn_Click(object sender, EventArgs e)
        {
            if (bmpEntry != null && bmpEntry.Bitmap != null && bmpEntry.Alpha != null)
            {
                saveBitmap(BlendBitmap.BlendBmp(bmpEntry), PixelFormat.Format32bppArgb, "_blend");
            }
        }

        private void bmpSaveBtn_Click(object sender, EventArgs e)
        {
            if (bmpEntry != null && bmpEntry.Bitmap != null)
            {
                saveBitmap(bmpEntry.Bitmap, PixelFormat.Format24bppRgb, string.Empty);
            }
        }

        private void alphaSaveBtn_Click(object sender, EventArgs e)
        {
            if (bmpEntry != null && bmpEntry.Alpha != null)
            {
                saveBitmap(bmpEntry.Alpha, PixelFormat.Format24bppRgb, "_a");
            }
        }
        private void newFshBtn_Click(object sender, EventArgs e)
        {
            openBitmapDialog1.Multiselect = true;
            if (bmpBox.Text.Length > 0)
            {
                List<string> files = new List<string>();
                files.Add(bmpBox.Text);
                NewFsh(files);
            }
            else if (openBitmapDialog1.ShowDialog() == DialogResult.OK)
            {
               List<string> files = new List<string>(openBitmapDialog1.FileNames);
               NewFsh(files);
            }
            
        }
        private void alphaBtn_Click(object sender, EventArgs e)
        {
            if (openAlphaDialog1.ShowDialog() == DialogResult.OK)
            {
                alphaBox.Text = openAlphaDialog1.FileName;
            }
        }
        private void bmpBtn_Click(object sender, EventArgs e)
        {
            if (openBitmapDialog1.ShowDialog() == DialogResult.OK)
            {
                bmpBox.Text = openBitmapDialog1.FileName;
                string dirpath = Path.GetDirectoryName(openBitmapDialog1.FileName);
                string filename = Path.GetFileNameWithoutExtension(openBitmapDialog1.FileName);
                string alpha = Path.Combine(dirpath + Path.DirectorySeparatorChar, filename + "_a" + Path.GetExtension(openBitmapDialog1.FileName));
                if (File.Exists(alpha))
                {
                    alphaBox.Text = alpha;
                }
                else
                {
                    alphaBox.Text = null;
                }
            }
        }
        private static List<string> TrimAlphaBitmaps(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                FileInfo fi = new FileInfo(list[i]);
                if (fi.Name.Contains("_a"))
                {
                    list.Remove(list[i]);
                }
            }
            list.TrimExcess();
            list.Sort();
            return list;
        }
        private void NewFsh(List<string> files)
        {
            if (tabControl1.SelectedTab != Maintab)
            {
                curImage = new FSHImageWrapper();
                tabControl1.SelectedTab = Maintab; 
                ClearandReset(true);
            }

            if (bmpEntry != null)
            {
                bmpEntry.Dispose();
                bmpEntry = null;
            }
            bmpEntry = new BitmapEntry();
            try
            {
                files = CheckSize(files);
                files = TrimAlphaBitmaps(files);

                if (files.Count > 0)
                {
                    string alphaMap = string.Empty;
                    if (File.Exists(files[0]))
                    {
                         using (Bitmap bmp = new Bitmap(files[0]))
                         {
                             if (bmpBox.Text.Length <= 0)
                             {
                                 alphaMap = Path.Combine(Path.GetDirectoryName(files[0]), Path.GetFileNameWithoutExtension(files[0]) + "_a" + Path.GetExtension(files[0]));
                             }
                             bmpEntry.Bitmap = bmp.Clone<Bitmap>();

                             if (alphaBox.Text.Length > 0 && File.Exists(alphaBox.Text))
                             {
                                 bmpEntry.Alpha = new Bitmap(alphaBox.Text);
                                 if (Checkhdimgsize(bmp) && Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                 {
                                     bmpEntry.BmpType = FSHBmpType.ThirtyTwoBit;
                                     fshTypeBox.SelectedIndex = 1;
                                 }
                                 else
                                 {
                                     bmpEntry.BmpType = FSHBmpType.DXT3;
                                     fshTypeBox.SelectedIndex = 3;
                                 }
                             }
                             else if (!string.IsNullOrEmpty(alphaMap) && File.Exists(alphaMap))
                             {
                                 bmpEntry.Alpha = new Bitmap(alphaMap);
                                 if (Checkhdimgsize(bmp) && Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                 {
                                     bmpEntry.BmpType = FSHBmpType.ThirtyTwoBit;
                                     fshTypeBox.SelectedIndex = 1;
                                 }
                                 else
                                 {
                                     bmpEntry.BmpType = FSHBmpType.DXT3;
                                     fshTypeBox.SelectedIndex = 3;
                                 }
                             }
                             else if (Path.GetExtension(files[0]).Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
                             {

                                 bmpEntry.Alpha = GetAlphafromPng(bmp);
                                 if (Checkhdimgsize(bmp) && Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                 {
                                     bmpEntry.BmpType = FSHBmpType.ThirtyTwoBit;
                                     fshTypeBox.SelectedIndex = 1;
                                 }
                                 else
                                 {
                                     bmpEntry.BmpType = FSHBmpType.DXT3;
                                     fshTypeBox.SelectedIndex = 3;
                                 }
                             }
                             else
                             {
                                 if (bmp != null)
                                 {
                                     bmpEntry.Alpha = GenerateAlpha(bmp);
                                     if (Checkhdimgsize(bmp) && Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                     {
                                         bmpEntry.BmpType = FSHBmpType.TwentyFourBit;
                                         fshTypeBox.SelectedIndex = 0;
                                     }
                                     else
                                     {
                                         bmpEntry.BmpType = FSHBmpType.DXT1;
                                         fshTypeBox.SelectedIndex = 2;
                                     }
                                 }
                             }


                             if (dirTxt.Text.Length > 0 && dirTxt.Text.Length == 4)
                             {
                                 bmpEntry.DirName = dirTxt.Text;
                             }
                             else
                             {
                                 bmpEntry.DirName = "FiSH";
                             }

                             string fn = Path.GetFileNameWithoutExtension(files[0]);
                             if (fn.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                             {
                                 instStr = fn.Substring(2, 8);
                                 EndFormat_Refresh();
                             }
                             else if (tgiInstanceTxt.Text.Length <= 0)
                             {
                                 EndFormat_Refresh();
                             }
                             if (bmp.Height < 256 && bmp.Width < 256)
                             {
                                 hdFshRadio.Enabled = false;
                                 hdBaseFshRadio.Enabled = false;
                                 regFshRadio.Checked = true;
                             }
                             else
                             {
                                 hdFshRadio.Enabled = true;
                                 hdBaseFshRadio.Enabled = true;
                                 if (bmpEntry.BmpType == FSHBmpType.ThirtyTwoBit)
                                 {
                                     hdFshRadio.Checked = true;
                                 }
                                 else if (bmpEntry.BmpType == FSHBmpType.TwentyFourBit)
                                 {
                                     hdBaseFshRadio.Checked = true;
                                 }
                                 else
                                 {
                                     regFshRadio.Checked = true;
                                 }
                             }

                             if (bmp.Width >= 128 && bmp.Height >= 128)
                             {
                                 mipsbtn_clicked = false;

                                 if (origbmplist == null)
                                 {
                                     origbmplist = new List<Bitmap>();
                                 }
                                 else if (origbmplist.Count > 0)
                                 {
                                     foreach (var item in origbmplist)
                                     {
                                         item.Dispose();
                                     }
                                     origbmplist.Clear();
                                 }
                                 this.fshWriteCbGenMips = true;
                                 origbmplist.Add(bmpEntry.Bitmap.Clone<Bitmap>()); // store the original bitmap to use if switching between fshwrite and fshlib compression

                                 curImage = new FSHImageWrapper();
                                 curImage.Bitmaps.Add(bmpEntry);
                                 if (files.Count - 1 == 0)
                                 {
                                     Temp_fsh();
                                     mipbtn_Click(null, null);
                                 }
                             } 
                         }

                        if (files.Count - 1 > 0)
                        {
                            int cnt = files.Count - 1;
                            List<string> add = new List<string>(cnt);
                            add.AddRange(files.GetRange(1, cnt));
                            AddbtnFiles(add, true);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void newfshbtn_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
            
        }
        private void newfshbtn_DragDrop(object sender, DragEventArgs e)
        {
            List<string> list = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));
            NewFsh(list);
        }
        string instStr;
        string Groupidoverride = null;
        private Settings settings = null;
        private void LoadSettings()
        {
            try
            {
                settings = new Settings(Path.Combine(Application.StartupPath, @"Multifshview.xml"));
                compDatCb.Checked = bool.Parse(settings.GetSetting("compDatcb_checked", bool.TrueString).Trim());
                genNewInstCb.Checked = bool.Parse(settings.GetSetting("genNewInstcb_checked", bool.FalseString).Trim());
                ValidateGroupString(settings.GetSetting("GroupidOverride",string.Empty).Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, Resources.UnableToLoadSettings + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Validate the group id for invalid characters
        /// </summary>
        /// <param name="gid">The id to validate</param>
        private void ValidateGroupString(string gid)
        {
            if (!string.IsNullOrEmpty(gid))
            {
                if (gid.Length == 10)
                {
                    if (gid.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        gid = gid.Substring(2, 8);
                    }
                }
                if (gid.Length == 8)
                {
                    Regex rx = new Regex(@"^[A-Fa-f0-9]*$");
                    if (rx.IsMatch(gid))
                    {
                        Groupidoverride = gid;
                    }
                }
            }
        }
        private void ReloadGroupID()
        {
            string g = string.Empty; 
            if (!string.IsNullOrEmpty(Groupidoverride))
            {
                g = Groupidoverride;
            }
            else
            {
                g = "0986135E";
            }
            tgiGroupTxt.Text = g;
        }
        /// <summary>
        /// Generates a plain alpha map for the DXT1 and 24-bit fsh images.
        /// </summary>
        /// <param name="temp">The source bitmap to get the size from</param>
        /// <returns>The generated alpha map</returns>
        private unsafe static Bitmap GenerateAlpha(Bitmap temp)
        {
            Bitmap alpha = new Bitmap(temp.Width, temp.Height, PixelFormat.Format24bppRgb);
            BitmapData data = alpha.LockBits(new Rectangle(0, 0, alpha.Width, alpha.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int len = data.Stride * alpha.Height;
            byte[] pixelData = new byte[len];
            for (int i = 0; i < len; i++)
            {
                pixelData[i] = 255;
            }

            System.Runtime.InteropServices.Marshal.Copy(pixelData, 0, data.Scan0, len);

            alpha.UnlockBits(data);

            return alpha;
        }

        /// <summary>
        /// Gets the alpha map from a 32-bit png
        /// </summary>
        /// <param name="sourcepng">The source png</param>
        /// <returns>The resulting alpha map</returns>
        private unsafe static Bitmap GetAlphafromPng(Bitmap source)
        {
            Bitmap dest = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);

            BitmapData src = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dst = dest.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            byte* srcpxl = (byte*)src.Scan0.ToPointer();
            byte* dstpxl = (byte*)dst.Scan0.ToPointer();

            int srcofs = src.Stride - source.Width * 4;
            int dstofs = dst.Stride - dest.Width * 3;

            for (int y = 0; y < dest.Height; y++)
            {
                for (int x = 0; x < dest.Width; x++)
                {
                    dstpxl[0] = srcpxl[3];
                    dstpxl[1] = srcpxl[3];
                    dstpxl[2] = srcpxl[3];

                    srcpxl += 4; 
                    dstpxl += 3; 
                }
                srcpxl += srcofs;
                dstpxl += dstofs;
            }

            dest.UnlockBits(dst);
            source.UnlockBits(src);

            return dest;
        }

        private int CountPngArgs(string[] args)
        {
            int count = 0;
            
            for (int i = 0; i < args.Length; i++)
            {
                FileInfo fi = new FileInfo(args[i]);
                if (fi.Exists)
                {
                    if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                    {
                        count++;
                    }
                }
            }
         
            return count;
        }
        private void Multifshfrm_Load(object sender, EventArgs e)
        {
            fshTypeBox.SelectedIndex = 2;

            LoadSettings();

            if (tgiGroupTxt.Text.Length <= 0)
            {
                ReloadGroupID();
            }
            if (tgiInstanceTxt.Text.Length <= 0)
            {
                instStr = RandomHexString(7);
                EndFormat_Refresh();
            }
            //this.Text += string.Concat(" ", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            datNameTxt.Text = Resources.NoDatLoadedText;
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 0)
            {
                try
                {          
                    int pngcnt = CountPngArgs(args);
                    List<string> pnglist = null;

                    for (int i = 0; i < args.Length; i++)
                    {
                        FileInfo fi = new FileInfo(args[i]);
                        if (fi.Exists)
                        {
                            if (fi.Extension.Equals(".fsh", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                            {
                                Load_Fsh(fi.FullName);
                                break; // exit the loop if a fsh or dat file has been loaded
                            }
                            else if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                            {
                                if (pnglist == null)
                                {
                                    pnglist = new List<string>();
                                }
                                pnglist.Add(fi.FullName);
                                if (pnglist.Count == pngcnt)
                                {
                                    NewFsh(pnglist);
                                }
                            }
                            else if (fi.Extension.Equals(".dat", StringComparison.OrdinalIgnoreCase))
                            {
                                Load_Dat(fi.FullName);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (mip64Fsh == null && e.TabPage == mip64tab)
            {
                e.Cancel = true;
            }
            else if (mip32Fsh == null && e.TabPage == mip32tab)
            {
                e.Cancel = true;
            } 
            else if (mip16Fsh == null && e.TabPage == mip16tab)
            {
                e.Cancel = true;
            } 
            else if (mip8Fsh == null && e.TabPage == mip8tab)
            {
                e.Cancel = true;
            } 
        }
        private void RefreshBmpType()
        {
            switch (bmpEntry.BmpType)
            {
                case FSHBmpType.TwentyFourBit:
                    fshTypeBox.SelectedIndex = 0;
                    break;
                case FSHBmpType.ThirtyTwoBit:
                    fshTypeBox.SelectedIndex = 1;
                    break;
                case FSHBmpType.DXT1:
                    fshTypeBox.SelectedIndex = 2;
                    break;
                case FSHBmpType.DXT3:
                    fshTypeBox.SelectedIndex = 3;
                    break;
            }
        }
        private void DisableManageButtons(TabPage page)
        {
            if (page != Maintab)
            {
                addBtn.Enabled = false;
                remBtn.Enabled = false;
                repBtn.Enabled = false;
            }
            else
            {
                addBtn.Enabled = true;
                remBtn.Enabled = true;
                repBtn.Enabled = true;
            }
        }
        private void SetHdRadios(TabPage page)
        {
            if (page != Maintab)
            {
                hdFshRadio.Enabled = hdBaseFshRadio.Enabled = false;
            }
        }
        private bool savedFshWriteCbValue;
        private void DisableFshWriteCheckBox(TabPage page)
        {
            if (page != Maintab)
            {
                if (fshWriteCompCb.Enabled)
                {
                    savedFshWriteCbValue = fshWriteCompCb.Checked;
                    fshWriteCompCb.Checked = false;
                    fshWriteCompCb.Enabled = false; 
                }
            }
            else
            {
                if (!fshWriteCompCb.Enabled)
                {
                    fshWriteCompCb.Checked = savedFshWriteCbValue;
                    fshWriteCompCb.Enabled = true;
                }
            }

        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mipsbtn_clicked == false && loadIsMip == false)
            {
                if (tabControl1.SelectedTab == mip64tab || tabControl1.SelectedTab == mip32tab || tabControl1.SelectedTab == mip16tab || tabControl1.SelectedTab == mip8tab)
                {
                    tabControl1.SelectedTab = Maintab;
                }
            }
            else
            {
                DisableManageButtons(tabControl1.SelectedTab);
                SetHdRadios(tabControl1.SelectedTab);
                DisableFshWriteCheckBox(tabControl1.SelectedTab);
                if (mip64Fsh != null && mip64Fsh.Bitmaps.Count > 0 && tabControl1.SelectedTab == mip64tab)
                {
                    RefreshMipImageList(mip64Fsh, bmp64Mip, alpha64Mip, blend64Mip, listViewMip64);
                    listViewMip64.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(instStr, end64);
                }
                else if (mip32Fsh != null && mip32Fsh.Bitmaps.Count > 0 && tabControl1.SelectedTab == mip32tab)
                {
                    RefreshMipImageList(mip32Fsh, bmp32Mip, alpha32Mip, blend32Mip, listViewMip32);
                    bmpEntry = mip32Fsh.Bitmaps[0];
                    listViewMip32.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(instStr, end32);
                }
                else if (mip16Fsh != null && mip16Fsh.Bitmaps.Count > 0 && tabControl1.SelectedTab == mip16tab)
                {
                    RefreshMipImageList(mip16Fsh, bmp16Mip, alpha16Mip, blend16Mip, listViewMip16);
                    bmpEntry = mip16Fsh.Bitmaps[0];
                    listViewMip16.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(instStr, end16);
                }
                else if (mip8Fsh != null && mip8Fsh.Bitmaps.Count > 0 && tabControl1.SelectedTab == mip8tab)
                {
                    RefreshMipImageList(mip8Fsh, bmp8Mip, alpha8Mip, blend8Mip, listViewMip8);
                    bmpEntry = mip8Fsh.Bitmaps[0];
                    listViewMip8.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(instStr, end8);
                }
                else if (curImage != null && curImage.Bitmaps.Count > 0 && tabControl1.SelectedTab == Maintab)
                {
                    RefreshImageLists(); 
                    bmpEntry = curImage.Bitmaps[0];
                    SetHdRadiosEnabled(bmpEntry);
                    listViewMain.Items[0].Selected = true;
                    tgiInstanceTxt.Text = string.Concat(instStr, endreg);
                    RefreshBmpType();
                }
                
            }
        }

        private void listViewMip64_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMip64.SelectedItems.Count > 0)
            {
                bmpEntry = mip64Fsh.Bitmaps[listViewMip64.SelectedItems[0].Index];
                Settypeindex(bmpEntry);
                RefreshBmpType();
                sizeLbl.Text = fshSize[listViewMip64.SelectedItems[0].Index];
                dirTxt.Text = dirName[listViewMip64.SelectedItems[0].Index];
            }
        }

        private void listViewMip32_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMip32.SelectedItems.Count > 0)
            {
                bmpEntry = mip32Fsh.Bitmaps[listViewMip32.SelectedItems[0].Index];
                Settypeindex(bmpEntry);
                RefreshBmpType();
                sizeLbl.Text = fshSize[listViewMip32.SelectedItems[0].Index];
                dirTxt.Text = dirName[listViewMip32.SelectedItems[0].Index];
            }
        }

        private void listViewMip16_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMip16.SelectedItems.Count > 0)
            {
                bmpEntry = mip16Fsh.Bitmaps[listViewMip16.SelectedItems[0].Index];
                Settypeindex(bmpEntry);
                RefreshBmpType();
                sizeLbl.Text = fshSize[listViewMip16.SelectedItems[0].Index];
                dirTxt.Text = dirName[listViewMip16.SelectedItems[0].Index];
            }
        }

        private void listViewMip8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMip8.SelectedItems.Count > 0)
            {
                bmpEntry = mip8Fsh.Bitmaps[listViewMip8.SelectedItems[0].Index];
                Settypeindex(bmpEntry);
                RefreshBmpType();
                sizeLbl.Text = fshSize[listViewMip8.SelectedItems[0].Index];
                dirTxt.Text = dirName[listViewMip8.SelectedItems[0].Index];
            }
        }
        private Random ra = new Random(); 
        private string RandomHexString(int length)
        {
            const string numbers = "0123456789";
            const string hexcode = "ABCDEF";
            char[] charArray = new char[length];
            string hexstring = string.Empty;
           
            hexstring += numbers;
            hexstring += hexcode;
            string rangepath = Path.Combine(Application.StartupPath, @"instRange.txt");
            string lowerinst = string.Empty;
            string upperinst = string.Empty;
            if (File.Exists(rangepath))
            {                    
                string[] instarray = null;
                using (StreamReader sr = new StreamReader(rangepath))
                {
                    string line;
                    char[] splitchar = new char[] { ',' };
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            instarray = line.Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
                        }
                    }
                }
                if (instarray != null)
                {
                    string inst0 = instarray[0];
                    string inst1 = instarray[1];
                    if (inst0.Length == 10)
                    {
                        if (inst0.ToUpperInvariant().StartsWith("0X"))
                        {
                            lowerinst = inst0.Substring(2, 8);
                        }
                    }
                    else if (inst0.Length == 8)
                    {
                        lowerinst = inst0;
                    }
                    if (inst1.Length == 10)
                    {
                        if (inst1.ToUpperInvariant().StartsWith("0X"))
                        {
                            upperinst = inst1.Substring(2, 8);
                        }
                    }
                    else if (inst1.Length == 8)
                    {
                        upperinst = inst1;
                    }
                }

            }

            for (int c = 0; c < charArray.Length; c++)
            {
                int index;
                if (!string.IsNullOrEmpty(lowerinst) && !string.IsNullOrEmpty(upperinst))
                {
                    long lower = long.Parse(lowerinst, NumberStyles.HexNumber);
                    long upper = long.Parse(upperinst, NumberStyles.HexNumber);
                    double rn = (upper * 1.0 - lower * 1.0) * ra.NextDouble() + lower * 1.0;
                    string str =  Convert.ToInt64(rn).ToString("X").Substring(0,7);
                    return str;
                }
                else
                {
                    index = ra.Next(0, hexstring.Length);
                    charArray[c] = hexstring[index];
                }

            }
            return new string(charArray);
        }
        private void TgiGrouptxt_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) && ModifierKeys != Keys.Shift)
            {
                e.Handled = true;
                e.SuppressKeyPress = false;
            }
            else if (e.KeyCode == Keys.A || e.KeyCode == Keys.B || e.KeyCode == Keys.C || e.KeyCode == Keys.D || e.KeyCode == Keys.E ||
                e.KeyCode == Keys.F || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.Handled = true;
                e.SuppressKeyPress = false;
            }
            else
            {
                e.Handled = false;
                e.SuppressKeyPress = true;
            }
        }
        private char endreg;
        private char end64;
        private char end32;
        private char end16;
        private char end8;
        private void EndFormat_Refresh()
        { 
            if (inst0_4Rdo.Checked)
            {
                endreg = '4';
                end64 = '3';
                end32 = '2';
                end16 = '1';
                end8 = '0';
            }
            else if (inst5_9Rdo.Checked)
            {
                endreg = '9';
                end64 = '8';
                end32 = '7';
                end16 = '6';
                end8 = '5';
            }
            else if (instA_ERdo.Checked)
            {
                endreg = 'E';
                end64 = 'D';
                end32 = 'C';
                end16 = 'B';
                end8 = 'A';
            }
            
            if (string.IsNullOrEmpty(instStr))
            {
                if (!string.IsNullOrEmpty(tgiInstanceTxt.Text))
                {
                    instStr = tgiInstanceTxt.Text.Substring(0, 7);
                }
                else
                {
                    instStr = RandomHexString(7);
                }
            }

            if (instStr.Length > 7)
            {
                if (tabControl1.SelectedTab == mip64tab)
                {
                    if (instStr[7].Equals('D'))
                    {
                        instA_ERdo.Checked = true;
                    }
                    else if (instStr[7].Equals('8'))
                    {
                        inst5_9Rdo.Checked = true;
                    }
                    else if (instStr[7].Equals('3'))
                    {
                        inst0_4Rdo.Checked = true;
                    }
                }
                else if (tabControl1.SelectedTab == mip32tab)
                {
                    if (instStr[7].Equals('C'))
                    {
                        instA_ERdo.Checked = true;
                    }
                    else if (instStr[7].Equals('7'))
                    {
                        inst5_9Rdo.Checked = true;
                    }
                    else if (instStr[7].Equals('2'))
                    {
                        inst0_4Rdo.Checked = true;
                    }
                }
                else if (tabControl1.SelectedTab == mip16tab)
                {
                    if (instStr[7].Equals('B'))
                    {
                        instA_ERdo.Checked = true;
                    }
                    else if (instStr[7].Equals('6'))
                    {
                        inst5_9Rdo.Checked = true;
                    }
                    else if (instStr[7].Equals('2'))
                    {
                        inst0_4Rdo.Checked = true;
                    }
                }
                else if (tabControl1.SelectedTab == mip8tab)
                {
                    if (instStr[7].Equals('A'))
                    {
                        instA_ERdo.Checked = true;
                    }
                    else if (instStr[7].Equals('5'))
                    {
                        inst5_9Rdo.Checked = true;
                    }
                    else if (instStr[7].Equals('0'))
                    {
                        inst0_4Rdo.Checked = true;
                    }
                }
                else if (tabControl1.SelectedTab == Maintab)
                {
                    if (instStr[7].Equals('E'))
                    {
                        instA_ERdo.Checked = true;
                    }
                    else if (instStr[7].Equals('9'))
                    {
                        inst5_9Rdo.Checked = true;
                    }
                    else if (instStr[7].Equals('4'))
                    {
                        inst0_4Rdo.Checked = true;
                    }
                }
                instStr = instStr.Substring(0,7);
            }

            if (tabControl1.SelectedTab == Maintab)
            {
                tgiInstanceTxt.Text = string.Concat(instStr, endreg);
            }  
            else if (tabControl1.SelectedTab == mip64tab)
            {                
                tgiInstanceTxt.Text = string.Concat(instStr, end64);
            }
            else if (tabControl1.SelectedTab == mip32tab)
            {
                tgiInstanceTxt.Text = string.Concat(instStr, end32);
            }
            else if (tabControl1.SelectedTab == mip16tab)
            {
                tgiInstanceTxt.Text = string.Concat(instStr, end16);
            }
            else if (tabControl1.SelectedTab == mip8tab)
            {
                tgiInstanceTxt.Text = string.Concat(instStr, end8);
            }
     
        }
        private void EndFormat_CheckedChanged(object sender, EventArgs e)
        {
            EndFormat_Refresh();
        }
        /// <summary>
        /// Clears the ImageList and ListView of loaded fsh images
        /// </summary>
        private void ClearFshlists()
        {
            listViewMain.Items.Clear();
            listViewMip64.Items.Clear();
            listViewMip32.Items.Clear();
            listViewMip16.Items.Clear();
            listViewMip8.Items.Clear();
            BitmapList1.Images.Clear();
            bmp64Mip.Images.Clear();
            bmp32Mip.Images.Clear();
            bmp16Mip.Images.Clear();
            bmp8Mip.Images.Clear();
            alphaList1.Images.Clear();
            alpha64Mip.Images.Clear();
            alpha32Mip.Images.Clear();
            alpha16Mip.Images.Clear();
            alpha8Mip.Images.Clear();
            blendList1.Images.Clear();
            blend64Mip.Images.Clear();
            blend32Mip.Images.Clear();
            blend16Mip.Images.Clear();
            blend8Mip.Images.Clear();
        }
        private DatFile dat = null;
        private bool compress_datmips = false;
        private string origInst = null;
        private bool loadedDat = false;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Fsh")]
        private void Load_Dat(string fileName)
        {
            try
            {
                dat = new DatFile(fileName);
                datListView.Items.Clear();
                ClearFshlists();
                int fshnum = 0;
                this.Cursor = Cursors.WaitCursor;
                List<ListViewItem> items = new List<ListViewItem>(dat.Indexes.Count);
                try
                {
                    for (int i = 0; i < dat.Indexes.Count; i++)
                    {
                        DatIndex index = dat.Indexes[i];
                        if (index.Type == fshTypeID)
                        {

                            string istr = index.Instance.ToString("X8", CultureInfo.InvariantCulture);
                            if (istr.EndsWith("4", StringComparison.Ordinal) || istr.EndsWith("9", StringComparison.Ordinal)
                                || istr.EndsWith("E", StringComparison.Ordinal) || istr.EndsWith("0", StringComparison.Ordinal) ||
                                istr.EndsWith("5", StringComparison.Ordinal)  || istr.EndsWith("A", StringComparison.Ordinal))
                            {
                                try
                                {
                                    if (dat.CheckImageSize(index))
                                    {
                                        fshnum++;
                                        ListViewItem item1 = new ListViewItem(Resources.FshNumberText + fshnum.ToString(CultureInfo.CurrentCulture));

                                        item1.SubItems.Add(index.Group.ToString("X8"));
                                        item1.SubItems.Add(index.Instance.ToString("X8"));

                                        items.Add(item1);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                catch (FormatException)
                                {
                                    // Invalid or unsupported file, skip it
                                    continue;
                                }
                            }
                        }
                    }
                }
                finally
                {                    
                    this.Cursor = Cursors.Default;
                }

                items.TrimExcess();

                datListView.Items.AddRange(items.ToArray());



                if (datListView.Items.Count > 0)
                {
                    loadedDat = true;
                    DatRebuilt = false;
                    SetLoadedDatEnables();
                    datListView.Items[0].Selected = true;
                    datNameTxt.Text = Path.GetFileName(dat.FileName);
                }
                else
                {
                    string message = string.Format(Resources.NoImagesInDatFileError_Format, Path.GetFileName(fileName));
                    MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    loadedDat = false;
                    ClearandReset(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void loadDatbtn_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == Maintab)
            {
                if (openDatDialog1.ShowDialog() == DialogResult.OK)
                {
                    Load_Dat(openDatDialog1.FileName);
                }
            }
        }
        private bool DatRebuilt = false;
        /// <summary>
        /// Rebuild the dat with the new items
        /// </summary>
        /// <param name="inputdat">The dat file to build</param>
        private void RebuildDat(DatFile inputdat)
        {
            if (mipsbtn_clicked && mip64Fsh != null && mip32Fsh != null && mip16Fsh != null && mip8Fsh != null && curImage != null)
            {
                uint group = uint.Parse(tgiGroupTxt.Text,NumberStyles.HexNumber);
                uint[] instanceid = new uint[5];
                FshWrapper[] fshwrap = new FshWrapper[5];
                FSHImageWrapper[] fshimg = new FSHImageWrapper[5];
                fshimg[0] = mip8Fsh; fshimg[1] = mip16Fsh; fshimg[2] = mip32Fsh;
                fshimg[3] = mip64Fsh; fshimg[4] = curImage;
                instanceid[0] = uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end8,NumberStyles.HexNumber);
                instanceid[1] = uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end16, NumberStyles.HexNumber);
                instanceid[2] = uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end32, NumberStyles.HexNumber);
                instanceid[3] = uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end64, NumberStyles.HexNumber);
                instanceid[4] = uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + endreg, NumberStyles.HexNumber);
                if (inputdat == null)
                {
                    dat = new DatFile();
                }
                
                for (int i = 4; i >= 0; i--)
                {
                    
                    fshwrap[i] = new FshWrapper(fshimg[i]);
                    CheckInstance(inputdat, group, instanceid[i]);
                        
                    inputdat.Add(fshwrap[i], group, instanceid[i], compress_datmips);
                }
                DatRebuilt = true;
            }
        }

        /// <summary>
        /// Checks the dat for files with the same TGI id
        /// </summary>
        /// <param name="checkdat">The Dat to check</param>
        /// <param name="group">The group id to check</param>
        /// <param name="instance">The instance id to check</param>
        private void CheckInstance(DatFile checkdat,uint group, uint instance)
        {
            for (int n = 0; n < checkdat.Indexes.Count; n++)
            {
                DatIndex chkindex = checkdat.Indexes[n];
                if (chkindex.Type == fshTypeID && chkindex.Group == group && chkindex.IndexState != DatIndexState.New)
                {
                    if (chkindex.Instance == instance)
                    { 
                        checkdat.Remove(group, instance);
                    }
                }
            }
        }
        /// <summary>
        /// Saves the new or modified dat
        /// </summary>
        /// <param name="fileName">The fileName to save as</param>
        private void SaveDat(string filename)
        {
            try
            {
                dat.Save(filename);   
                
                datNameTxt.Text = Path.GetFileName(dat.FileName);

                dat.Close();
                dat = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!loadedDat && datListView.Items.Count == 0)
                {
                    ClearandReset(true);
                }
                else
                {
                    Load_Dat(dat.FileName); // reload the modified dat
                }
            }
        }
        private void saveDatbtn_Click(object sender, EventArgs e)
        {
            if (dat == null)
            {
                dat = new DatFile();
                DatRebuilt = false;
            }
            if (compDatCb.Checked && !compress_datmips)
            {
                compress_datmips = true; // compress the dat items
            }
            if (!mipsbtn_clicked)
            {
                mipbtn_Click(sender, e);
                RebuildDat(dat);
            }

            if (!genNewInstCb.Checked && !DatRebuilt)
            {
                RebuildDat(dat);
            }
           
            if (dat.Indexes.Count > 0)
            {
                if (!loadedDat && datListView.Items.Count == 0)
                {
                    if (saveDatDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        SaveDat(saveDatDialog1.FileName);
                    }
                }
                else
                {
                    SaveDat(dat.FileName);
                }
            }
        }

        private void genNewInstcb_CheckedChanged(object sender, EventArgs e)
        {
            if (dat != null && loadedDat)
            {
                settings.PutSetting("genNewInstcb_checked",genNewInstCb.Checked.ToString());
                if (genNewInstCb.Checked)
                {
                    for (int n = 0; n < dat.Indexes.Count; n++)
                    {
                        DatIndex addindex = dat.Indexes[n];
                        if (addindex.Type == fshTypeID)
                        {
                            string newinstance = null;

                            if (addindex.Instance == uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end8, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                newinstance = RandomHexString(7);
                                instStr = newinstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (addindex.Instance == uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end16, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                newinstance = RandomHexString(7);
                                instStr = newinstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (addindex.Instance == uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end32, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                newinstance = RandomHexString(7);
                                instStr = newinstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (addindex.Instance == uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end64, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                newinstance = RandomHexString(7);
                                instStr = newinstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (addindex.Instance == uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + endreg, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                newinstance = RandomHexString(7);
                                instStr = newinstance.Substring(0,7);
                                EndFormat_Refresh();
                            }
                        } 
                       
                    }
                    RebuildDat(dat);
                }
                else
                {
                    if (origInst != null)
                    {
                        if (instStr != origInst)
                        {
                            instStr = origInst;
                            EndFormat_Refresh();
                        }
                    }
                }
            }
        }

        private void newDatbtn_Click(object sender, EventArgs e)
        {
           if (loadedDat && datListView.Items.Count > 0)
           {
               ClearandReset(true);
               loadedDat = false;
           }
           else
           {
               ClearandReset(false);
           }
           this.dat = new DatFile();
           DatRebuilt = false;
           datNameTxt.Text = Resources.DatInMemoryText;       
           SetLoadedDatEnables();
        }

        private void SetHdRadiosEnabled(BitmapEntry entry)
        {
            if ((entry.Bitmap.Height >= 256 && entry.Bitmap.Width >= 256) && entry.BmpType == FSHBmpType.ThirtyTwoBit)
            {
                hdFshRadio.Enabled = true;
                hdBaseFshRadio.Enabled = true;
                hdBaseFshRadio.Checked = false;
                regFshRadio.Checked = false;
                hdFshRadio.Checked = true;
            }
            else if ((entry.Bitmap.Height >= 256 && entry.Bitmap.Width >= 256) && entry.BmpType == FSHBmpType.TwentyFourBit)
            {
                hdFshRadio.Enabled = true;
                hdBaseFshRadio.Enabled = true;
                hdFshRadio.Checked = false;
                regFshRadio.Checked = false;
                hdBaseFshRadio.Checked = true;
            }
            else
            {
                hdFshRadio.Enabled = false;
                hdBaseFshRadio.Enabled = false;
                regFshRadio.Checked = true;
            }
        }
        
        private void DatlistView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (datListView.SelectedItems.Count > 0)
            {
                try
                {
                    string group = datListView.SelectedItems[0].SubItems[1].Text;
                    string instance = datListView.SelectedItems[0].SubItems[2].Text;
                    ClearFshlists();
                    tgiGroupTxt.Text = group;
                    instStr = instance;
                    EndFormat_Refresh();
                    origInst = instance.Substring(0, 7);

                    if (datListView.SelectedItems[0].Tag == null)
                    {
                        uint grp = uint.Parse(group, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        uint inst = uint.Parse(instance, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        FshWrapper item = dat.LoadFile(grp, inst);
                        datListView.SelectedItems[0].Tag  = item;
                    }

                    FshWrapper fshitem = datListView.SelectedItems[0].Tag as FshWrapper;
                    if (fshitem.Compressed)
                    {
                        compress_datmips = true;
                        compDatCb.Checked = true;
                    }

                    if (fshitem.Image != null)
                    {

                        BitmapEntry tempEntry = fshitem.Image.Bitmaps[0];
 
                        curImage = fshitem.Image.Clone();
                        RefreshImageLists();
                        tabControl1.SelectedTab = Maintab;
                        
                        switch (tempEntry.BmpType)
                        {
                            case FSHBmpType.TwentyFourBit:
                                fshTypeBox.SelectedIndex = 0;
                                break;
                            case FSHBmpType.ThirtyTwoBit:
                                fshTypeBox.SelectedIndex = 1;
                                break;
                            case FSHBmpType.DXT1:
                                fshTypeBox.SelectedIndex = 2;
                                break;
                            case FSHBmpType.DXT3:
                                fshTypeBox.SelectedIndex = 3;
                                break;
                        }
                        SetHdRadiosEnabled(tempEntry);
                           
                        
                    }

                }
                catch (DatFileException dfex)
                {
                    MessageBox.Show(this, dfex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void compDatcb_CheckedChanged(object sender, EventArgs e)
        {
          settings.PutSetting("compDatcb_checked",compDatCb.Checked.ToString());
        }
        /// <summary>
        /// Clear the dat and fsh lists and reset
        /// </summary>
        /// <param name="clearLoadedFshFiles">clear the loaded fsh files</param>
        private void ClearandReset(bool clearLoadedFshFiles)
        {
            if (dat != null)
            {
                dat.Close();
                dat = null;
                datNameTxt.Text = Resources.NoDatLoadedText;
            }
            if (datListView.Items.Count > 0)
            {
                datListView.Items.Clear();
                fshWriteCompCb.Enabled = true;
            }
            if (clearLoadedFshFiles)
            {
                ClearFshlists();
                mipsbtn_clicked = false;
                if (curImage != null)
                {
                    curImage.Dispose();
                    curImage = null;
                }
                if (mip64Fsh != null)
                {
                    mip64Fsh.Dispose();
                    mip64Fsh = null;
                }
                if (mip32Fsh != null)
                {
                    mip32Fsh.Dispose();
                    mip32Fsh = null;
                }
                if (mip16Fsh != null)
                {
                    mip16Fsh.Dispose();
                    mip16Fsh = null;
                }
                if (mip8Fsh != null)
                {
                    mip8Fsh.Dispose();
                    mip8Fsh = null;
                }
                if (bmpEntry != null)
                {
                    bmpEntry.Dispose();
                    bmpEntry = null;
                    fshTypeBox.SelectedIndex = 2;
                    sizeLbl.Text = string.Empty;
                    dirTxt.Text = string.Empty;
                    ReloadGroupID();
                    instStr = string.Empty;
                    tgiInstanceTxt.Text = string.Empty;
                    if (origbmplist != null)
                    {
                        foreach (var item in origbmplist)
                        {
                            item.Dispose();
                        }
                        origbmplist.Clear();
                    }
                }
                hdFshRadio.Enabled = true;
                hdBaseFshRadio.Enabled = true;
            }
        }

        private void Multifshfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dat != null)
            {
                if (dat.IsDirty)
                {
                    switch (MessageBox.Show(this, Resources.SaveDatChangesText, this.Text, MessageBoxButtons.YesNoCancel))
                    {
                        case DialogResult.Yes:
                            dat.Save();
                            dat.Close();
                            break;
                        case DialogResult.No:
                            dat.Close();
                            break;
                        case DialogResult.Cancel:
                            e.Cancel = true;
                            break;
                    }
                }
                else
                {
                    dat.Close();
                }
            }
        }

        /// <summary>
        /// Check if the bitmap size is 256 x 256 or larger for the hd fsh
        /// </summary>
        /// <param name="b">The bitmap to check</param>
        /// <returns>True if the bitmap is 256 x 256 or larger otherwise false</returns>
        private bool Checkhdimgsize(Bitmap b)
        {
            if (b.Width >= 256 && b.Height >= 256)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private FSHImageWrapper GetImageFromSelectedTab(int index)
        {
            FSHImageWrapper image = null;
            switch (index)
            {
                case 0:
                    image = curImage;
                    break;
                case 1:
                    image = mip64Fsh;
                    break;
                case 2:
                    image = mip32Fsh;
                    break;
                case 3:
                    image = mip16Fsh;
                    break;
                case 4:
                    image = mip8Fsh;
                    break;
            }
            return image;
        }
        private void FshtypeBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            FSHImageWrapper image = GetImageFromSelectedTab(tabControl1.SelectedIndex);

            if (image != null && image.Bitmaps.Count > 0 && bmpEntry != null && bmpEntry.Bitmap != null)
            {
                if (!Checkhdimgsize(bmpEntry.Bitmap))
                {
                    if (e.Index == 0 || e.Index == 1)
                    {
                        // make the hd fsh items look disabled 
                        string text = cb.Items[e.Index].ToString();
                        e.DrawBackground();
                        e.Graphics.DrawString(text, e.Font, SystemBrushes.GrayText, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                        e.DrawFocusRectangle();
                    }
                    else
                    {
                        //leave the other items alone
                        string text = cb.Items[e.Index].ToString();
                        e.DrawBackground();
                        e.Graphics.DrawString(text, e.Font, SystemBrushes.WindowText, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                        e.DrawFocusRectangle();
                    }
                }
                else
                {
                    // draw it normally
                    string text = cb.Items[e.Index].ToString();
                    e.DrawBackground();
                    e.Graphics.DrawString(text, e.Font, SystemBrushes.WindowText, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                    e.DrawFocusRectangle();
                }
            }
            else
            {
                // draw it normally
                string text = cb.Items[e.Index].ToString();
                e.DrawBackground();
                e.Graphics.DrawString(text, e.Font, SystemBrushes.WindowText, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                e.DrawFocusRectangle();
            }
        }
        private int sortColumn = -1;
        private void DatlistView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                datListView.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (datListView.Sorting == SortOrder.Ascending)
                    datListView.Sorting = SortOrder.Descending;
                else
                    datListView.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            datListView.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.datListView.ListViewItemSorter = new ListViewItemComparer(e.Column,
                                                              datListView.Sorting);
        }
        private bool useorigimage = false;
        private bool fshWriteCbGenMips; // generate mips when the checkbox is changed.
        private void Fshwritecompcb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (curImage != null && curImage.Bitmaps.Count > 0 && origbmplist != null && !loadedDat && datListView.Items.Count == 0)
                {
                    useorigimage = true;

                    Temp_fsh();
                    if (fshWriteCbGenMips)
                    {
                        mipbtn_Click(null, null);
                    }
                    useorigimage = false; // reset it to false

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Set if the close dat button and fshwrite compression checkbox are enabled 
        /// </summary>
        private void SetLoadedDatEnables()
        { 
            if (!loadedDat && datListView.Items.Count == 0)
            {
                if (!fshWriteCompCb.Enabled)
                {
                    fshWriteCompCb.Enabled = true;
                }
                if (closeDatBtn.Enabled)
                {
                    closeDatBtn.Enabled = false;
                }
            }
            else
            {
                if (fshWriteCompCb.Enabled)
                {
                    if (fshWriteCompCb.Checked)
                    {
                        fshWriteCompCb.Checked = false;
                    }
                    fshWriteCompCb.Enabled = false;
                }
                if (!closeDatBtn.Enabled)
                {
                    closeDatBtn.Enabled = true;
                }
            }
        }

        private void closeDatbtn_Click(object sender, EventArgs e)
        {
            if (dat != null && loadedDat)
            {
                if (dat.IsDirty)
                {
                    switch (MessageBox.Show(this, Resources.SaveDatChangesText, this.Text, MessageBoxButtons.YesNo))
                    {
                        case DialogResult.Yes:
                            dat.Save();
                            dat.Close();
                            break;
                        case DialogResult.No:
                            dat.Close();
                            break;
                    }
                }
                else
                {
                    dat.Close();
                }
                ClearandReset(true);
                loadedDat = false;
                SetLoadedDatEnables();

            }
        }

    }
}