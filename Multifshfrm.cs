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
    public partial class Multifshfrm : Form
    {
        public Multifshfrm()
        {
            InitializeComponent();
        }
        private string[] dirname = null;
        private string[] fshsize = null;
        private string fshfilename = null;
        private FSHImage curimage = null;
        private BitmapItem bmpitem = null;

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
                    MessageBox.Show(this, ex.Message, this.Text);
                }
            }
        }
        private bool loadisMip = false; 
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
                            FSHImage tempimg = new FSHImage(fstream);
                            BitmapItem tempitem = new BitmapItem();
                            tempitem = (BitmapItem)tempimg.Bitmaps[0];

                            fshfilename = filename;
                            bmpitem = new BitmapItem();
                            ClearFshlists();

                            if (tempitem.Bitmap.Width >= 128 && tempitem.Bitmap.Height >= 128)
                            {
                                curimage = new FSHImage(fstream);
                                RefreshImageLists();
                                ListViewItem item = listViewMain.Items[0];
                                item.Selected = true;
                                success = true;
                                tabControl1.SelectedTab = Maintab;

                            }
                            else if (tempitem.Bitmap.Width == 64 && tempitem.Bitmap.Height == 64)
                            {

                                mip64fsh = new FSHImage(fstream);
                                RefreshMipImageList(mip64fsh, bmp64Mip, alpha64Mip, blend64Mip, listViewMip64);
                                ListViewItem item = listViewMip64.Items[0];
                                item.Selected = true;
                                loadisMip = true;
                                success = true;
                                tabControl1.SelectedTab = mip64tab;

                            }
                            else if (tempitem.Bitmap.Width == 32 && tempitem.Bitmap.Height == 32)
                            {
                                mip32fsh = new FSHImage(fstream);
                                RefreshMipImageList(mip32fsh, bmp32Mip, alpha32Mip, blend32Mip, listViewMip32);
                                ListViewItem item = listViewMip32.Items[0];
                                item.Selected = true;
                                loadisMip = true;
                                success = true;
                                tabControl1.SelectedTab = mip32tab;
                            }
                            else if (tempitem.Bitmap.Width == 16 && tempitem.Bitmap.Height == 16)
                            {
                                mip16fsh = new FSHImage(fstream);
                                RefreshMipImageList(mip16fsh, bmp16Mip, alpha16Mip, blend16Mip, listViewMip16);
                                ListViewItem item = listViewMip16.Items[0];
                                item.Selected = true;
                                loadisMip = true; 
                                success = true;
                                tabControl1.SelectedTab = mip16tab;
                            }
                            else if (tempitem.Bitmap.Width == 8 && tempitem.Bitmap.Height == 8)
                            {
                                mip8fsh = new FSHImage(fstream);
                                RefreshMipImageList(mip8fsh, bmp8Mip, alpha8Mip, blend8Mip, listViewMip8);
                                ListViewItem item = listViewMip8.Items[0];
                                item.Selected = true;
                                loadisMip = true;
                                success = true;
                                tabControl1.SelectedTab = mip8tab;
                            }
                            tempimg = null;
                        }
                        if (success)
                        {
                            RefreshBmpType();
                            if ((bmpitem.Bitmap.Height >= 256 && bmpitem.Bitmap.Width >= 256) && bmpitem.BmpType == FSHBmpType.ThirtyTwoBit)
                            {
                                hdfshRadio.Enabled = true;
                                hdBasetexrdo.Enabled = true;
                                hdBasetexrdo.Checked = false;
                                regFshrdo.Checked = false;
                                hdfshRadio.Checked = true;
                            }
                            else if ((bmpitem.Bitmap.Height >= 256 && bmpitem.Bitmap.Width >= 256) && bmpitem.BmpType == FSHBmpType.TwentyFourBit)
                            {
                                hdfshRadio.Enabled = true;
                                hdBasetexrdo.Enabled = true;
                                hdfshRadio.Checked = false;
                                regFshrdo.Checked = false;
                                hdBasetexrdo.Checked = true;
                            }
                            else
                            {
                                hdfshRadio.Enabled = false;
                                hdBasetexrdo.Enabled = false;
                                regFshrdo.Checked = true;
                            }
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
                                                    inststr = line;
                                                    tgiInstanceTxt.Text = inststr;
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
        private void SaveFsh(Stream fs, FSHImage image)
        {
            try
            {
                if (IsDXTFsh(image) && fshWriteCompCb.Checked)
                {
                    Fshwrite fw = new Fshwrite();
                    for (int i = 0; i < image.Bitmaps.Count; i++)
		            {
                        BitmapItem bi = (BitmapItem)image.Bitmaps[i];
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
                            fw.dir.Add(bi.DirName);
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
        private bool IsDXTFsh(FSHImage image)
        {
            foreach (BitmapItem bi in image.Bitmaps)
            {
                if (bi.BmpType != FSHBmpType.DXT3 && bi.BmpType != FSHBmpType.DXT1)
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
            if (dat != null && DatlistView1.SelectedItems.Count > 0)
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
                    SaveFsh(mstream, curimage);
                    curimage = new FSHImage(mstream);
                }

                SetLoadedDatIsDirty();

                bmpitem = new BitmapItem();
                ClearFshlists();
                RefreshImageLists();

                RefreshBmpType();

                listViewMain.Items[0].Selected = true;

            }
            catch (Exception)
            {
                throw;
            }

        }
        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            FSHImage image = GetImageFromSelectedTab(tabControl1.SelectedIndex);
            
            if (image != null && image.Bitmaps.Count > 0)
            {
                if (colorRadio.Checked)
                {
                    if (tabControl1.SelectedTab == Maintab)
                    {
                        RefreshBitmapList(curimage, listViewMain, BitmapList1);
                    }
                    else if (tabControl1.SelectedTab == mip64tab)
                    {
                        RefreshBitmapList(mip64fsh, listViewMip64, bmp64Mip);
                    }
                    else if (tabControl1.SelectedTab == mip32tab)
                    {
                        RefreshBitmapList(mip32fsh, listViewMip32, bmp32Mip);
                    }
                    else if (tabControl1.SelectedTab == mip16tab)
                    {
                        RefreshBitmapList(mip16fsh, listViewMip16, bmp16Mip);
                    }
                    else if (tabControl1.SelectedTab == mip8tab)
                    {
                        RefreshBitmapList(mip8fsh, listViewMip8, bmp8Mip);
                    }
                }
                else if (alphaRadio.Checked)
                {
                    if (tabControl1.SelectedTab == Maintab)
                    {
                        RefreshAlphaList(curimage, listViewMain, alphaList1);
                    }
                    else if (tabControl1.SelectedTab == mip64tab)
                    {
                        RefreshAlphaList(mip64fsh, listViewMip64, alpha64Mip);
                    }
                    else if (tabControl1.SelectedTab == mip32tab)
                    {
                        RefreshAlphaList(mip32fsh, listViewMip32, alpha32Mip);
                    }
                    else if (tabControl1.SelectedTab == mip16tab)
                    {
                        RefreshAlphaList(mip16fsh, listViewMip16, alpha16Mip);
                    }
                    else if (tabControl1.SelectedTab == mip8tab)
                    {
                        RefreshAlphaList(mip8fsh, listViewMip8, alpha8Mip);
                    }
                }
                else if (blendRadio.Checked)
                {
                    if (tabControl1.SelectedTab == Maintab)
                    {
                        RefreshBlendList(curimage, listViewMain, blendList1);
                    }
                    else if (tabControl1.SelectedTab == mip64tab)
                    {
                        RefreshBlendList(mip64fsh, listViewMip64, blend64Mip);
                    }
                    else if (tabControl1.SelectedTab == mip32tab)
                    {
                        RefreshBlendList(mip32fsh, listViewMip32, blend32Mip);
                    }
                    else if (tabControl1.SelectedTab == mip16tab)
                    {
                        RefreshBlendList(mip16fsh, listViewMip16, blend16Mip);
                    }
                    else if (tabControl1.SelectedTab == mip8tab)
                    {
                        RefreshBlendList(mip8fsh, listViewMip8, blend8Mip);
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
        private void RefreshBitmapList(FSHImage image, ListView listview, ImageList imglist)
        {
            if (listview.Items.Count > 0)
            {
                listview.Items.Clear();
            }
            listview.LargeImageList = imglist;
            listview.SmallImageList = imglist;

            for (int cnt = 0; cnt < image.Bitmaps.Count; cnt++)
            {
                ListViewItem alpha = new ListViewItem("bitmap # :" + cnt.ToString(), cnt);
                listview.Items.Add(alpha);
            }

            listview.Items[0].Selected = true;
        }

        /// <summary>
        /// Refreshes the list of alpha bitmaps
        /// </summary>
        /// <param name="image">The image to refresh the list from</param>
        /// <param name="listview">The listview to add the images to</param>
        /// <param name="imglist">The ImageList containing the alpha bitmaps to use</param>
        private void RefreshAlphaList(FSHImage image, ListView listview, ImageList imglist)
        {
            if (listview.Items.Count > 0)
            {
                listview.Items.Clear();
            }
            listview.LargeImageList = imglist;
            listview.SmallImageList = imglist;
          
            for (int cnt = 0; cnt < image.Bitmaps.Count; cnt++)
            {
                ListViewItem alpha = new ListViewItem("alpha # :" + cnt.ToString(), cnt);
                listview.Items.Add(alpha);
            }

            listview.Items[0].Selected = true;
        }
        /// <summary>
        /// Refreshes the list of blended bitmaps
        /// </summary>
        /// <param name="image">The image to refresh the list from</param>
        /// <param name="listview">The listview to add the images to</param>
        /// <param name="imglist">The ImageList containing the blended bitmaps to use</param>
        private void RefreshBlendList(FSHImage image,ListView listview,ImageList imglist)
        {
            if (listview.Items.Count > 0)
            {
                listview.Items.Clear();
            }
            listview.LargeImageList = imglist;
            listview.SmallImageList = imglist; 
         
            for (int cnt = 0; cnt < image.Bitmaps.Count; cnt++)
            {
                ListViewItem blend = new ListViewItem("blend # :" + cnt.ToString(), cnt);
                listview.Items.Add(blend);
            }
       
            listview.Items[0].Selected = true;

        }
        private Bitmap Alphablend(BitmapItem item)
        { 
            Bitmap blendbmp = new Bitmap(bmpitem.Bitmap.Width, bmpitem.Bitmap.Height);
            Graphics g = Graphics.FromImage(blendbmp);
            g.FillRectangle(new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.FromArgb(192, 192, 192)), new Rectangle(0, 0, blendbmp.Width, blendbmp.Height));
            BlendBitmap blbmp = new BlendBitmap();
            g.DrawImageUnscaled(blbmp.BlendBmp(bmpitem), new Rectangle(0, 0, blendbmp.Width, blendbmp.Height));
            return blendbmp;
        }
        private int typeindex = 0; // store previously selected fsh type
        private void listViewmain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMain.SelectedItems.Count > 0)
            {
                bmpitem = (BitmapItem)curimage.Bitmaps[listViewMain.SelectedItems[0].Index];
                Settypeindex(curimage, bmpitem);
                RefreshBmpType();

                Sizelbl.Text = fshsize[listViewMain.SelectedItems[0].Index];
                dirTxt.Text = dirname[listViewMain.SelectedItems[0].Index];
            }
        }
        private void Settypeindex(FSHImage image, BitmapItem item)
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
        /// Check if the bitmap size is 128 x 128 or larger
       /// </summary>
       /// <param name="bmp">The bitmap to check</param>
       /// <returns>True if the image is 128 x 128 or larger otherwise false</returns>
        private bool CheckSize(Bitmap bmp)
        {
            if (tabControl1.SelectedTab == Maintab)
            {
                if (bmp.Width >= 128 && bmp.Height >= 128)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(this, "The bitmap must be at least 128 x 128", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (curimage == null && bmpitem == null)
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
                if (curimage != null && bmpitem != null)
                {
                    if (!listfiltered)
                    {
                        files = CheckSize(files);
                        files = TrimAlphaBitmaps(files);
                    }
                    for (int f = 0; f < files.Count; f++)
                    {
                        FileInfo fi = new FileInfo(files[f]);

                        BitmapItem addbmp = new BitmapItem();
                        Bitmap bmp = null;
                        string alpath = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.FullName) + "_a" + fi.Extension);

                        if (fi.Exists)
                        {
                            bmp = new Bitmap(fi.FullName);
                            if (origbmplist == null)
                            {
                                origbmplist = new List<Bitmap>();
                            }
                            origbmplist.Add(bmp);
                            addbmp.Bitmap = bmp;
                        

                            if (File.Exists(alpath))
                            {
                                Bitmap alpha = new Bitmap(alpath);
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
                            else if (fi.Extension.ToLower().Equals(".png") && bmp.PixelFormat == PixelFormat.Format32bppArgb)
                            {
                                if (bmp != null)
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

                            }
                            else
                            {
                                if (bmp != null)
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
                            }
                            if ((dirTxt.Text.Length > 0) && dirTxt.Text.Length == 4)
                            {
                                addbmp.SetDirName(dirTxt.Text);
                            }
                            else
                            {
                                addbmp.SetDirName("FiSH");
                            }

                            if (bmp.Height < 256 && bmp.Width < 256)
                            {
                                hdfshRadio.Enabled = false;
                                hdBasetexrdo.Enabled = false;
                                regFshrdo.Checked = true;
                            }
                            else
                            {
                                hdfshRadio.Enabled = true;
                                hdBasetexrdo.Enabled = true;
                                if (bmpitem.BmpType == FSHBmpType.ThirtyTwoBit)
                                {
                                    hdfshRadio.Checked = true;
                                }
                                else if (bmpitem.BmpType == FSHBmpType.TwentyFourBit)
                                {
                                    hdBasetexrdo.Checked = true;
                                }
                                else
                                {
                                    regFshrdo.Checked = true;
                                }
                            }

                            if (tabControl1.SelectedTab == Maintab)
                            {
                                colorRadio.Checked = true;
                                if (curimage == null)
                                {
                                    curimage = new FSHImage();
                                }
                                curimage.Bitmaps.Add(addbmp);
                                curimage.UpdateDirty();
                                if (f == files.Count - 1)
                                {
                                    Temp_fsh();
                                    mipbtn_Click(null, null);
                                    listViewMain.Items[0].Selected = true;
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
                if (curimage == null && bmpitem == null)
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
        private void RefreshMipImageList(FSHImage image, ImageList bmplist, ImageList alphalist, ImageList blendlist, ListView list)
        {
            bmplist.Images.Clear();
            alphalist.Images.Clear();
            blendlist.Images.Clear(); 
            
            if (image.Bitmaps.Count > 1)
            {

                for (int cnt = 0; cnt < image.Bitmaps.Count; cnt++)
                {
                    bmpitem = (BitmapItem)image.Bitmaps[cnt];
                    Reset24bitAlpha(bmpitem);
                    bmplist.Images.Add(bmpitem.Bitmap);
                    alphalist.Images.Add(bmpitem.Alpha);
                    blendlist.Images.Add(Alphablend(bmpitem));

                }

            }
            else
            {

                bmpitem = (BitmapItem)image.Bitmaps[0];
                Reset24bitAlpha(bmpitem);
                bmplist.Images.Add(bmpitem.Bitmap);
                alphalist.Images.Add(bmpitem.Alpha);
                blendlist.Images.Add(Alphablend(bmpitem));
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
        private void Reset24bitAlpha(BitmapItem item)
        {
            if (item.BmpType == FSHBmpType.TwentyFourBit)
            {
                Bitmap alpha = new Bitmap(item.Bitmap.Width,item.Bitmap.Height);
                for (int y = 0; y < alpha.Height; y++)
                {
                    for (int x = 0; x < alpha.Width; x++)
                    {
                        alpha.SetPixel(x, y, Color.White);
                    }
                }
                item.Alpha = alpha;
            }
        }

        /// <summary>
        /// Refreshes the listviewMain ImageLists from the current fsh 
        /// </summary>
        private void RefreshImageLists()
        {
            if (curimage.Bitmaps.Count > 1)
            {

                remBtn.Enabled = true;
                for (int cnt = 0; cnt < curimage.Bitmaps.Count; cnt++)
                {
                    bmpitem = (BitmapItem)curimage.Bitmaps[cnt];

                    Reset24bitAlpha(bmpitem);
                    BitmapList1.Images.Add(bmpitem.Bitmap);
                    alphaList1.Images.Add(bmpitem.Alpha);
                    blendList1.Images.Add(Alphablend(bmpitem));

                }
                  
            }
            else
            {
                remBtn.Enabled = false;
                bmpitem = (BitmapItem)curimage.Bitmaps[0];
                Reset24bitAlpha(bmpitem);
                BitmapList1.Images.Add(bmpitem.Bitmap);
                alphaList1.Images.Add(bmpitem.Alpha);
                blendList1.Images.Add(Alphablend(bmpitem));

            } 
            
            RefreshDirectory(curimage);

            listViewMain.BeginUpdate();
            if (colorRadio.Checked)
            {
                RefreshBitmapList(curimage, listViewMain, BitmapList1);
            }
            else if (alphaRadio.Checked)
            {
                RefreshAlphaList(curimage, listViewMain, alphaList1);
            }
            else if (blendRadio.Checked)
            {
                RefreshBlendList(curimage, listViewMain, blendList1);
            }
            listViewMain.EndUpdate();
        }
        /// <summary>
        /// Refresh the fsh size and dir name for the input image 
        /// </summary>
        /// <param name="image">The input image</param>
        private void RefreshDirectory(FSHImage image)
        {
            dirname = new string[image.Bitmaps.Count];
            fshsize = new string[image.Bitmaps.Count];
            for (int dirnum = 0; dirnum < image.Bitmaps.Count; dirnum++)
            {
                FSHDirEntry dir = image.Directory[dirnum];
                FSHEntryHeader entryhead = new FSHEntryHeader();
                entryhead = image.GetEntryHeader(dir.offset);
                dirname[dirnum] = Encoding.ASCII.GetString(dir.name);
                fshsize[dirnum] = entryhead.width.ToString() + "x" + entryhead.height.ToString();
            }
        }
        private void dirTxt_TextChanged(object sender, EventArgs e)
        {
            if (curimage != null && tabControl1.SelectedTab == Maintab)
            {
                if (dirTxt.Text.Length > 0 && dirTxt.Text.Length == 4 && dirTxt.Text != Encoding.ASCII.GetString(bmpitem.DirName))
	            {
                    bmpitem.SetDirName(dirTxt.Text);
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
                        curimage.Bitmaps.Remove(bmpitem); //remove the item and rebuild the mipmaps
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
            if (curimage != null && bmpitem != null)
            {
                if (listViewMain.SelectedItems.Count > 0)
                {

                    try
                    {
                        BitmapItem repbmp = new BitmapItem();
                        Bitmap bmp = null;
                        bool bmploaded = false;
                        openBitmapDialog1.Multiselect = false;
                        string alphamap = string.Empty;
                        string bmpfilename = string.Empty; // holds the filename from the bmpBox TextBox or the OpenBitmapDialog 

                        if (bmpBox.Text.Length > 0 && File.Exists(bmpBox.Text))
                        {
                                bmp = new Bitmap(bmpBox.Text);
                                if (CheckSize(bmp))
                                {
                                    repbmp.Bitmap = bmp;
                                    bmpfilename = bmpBox.Text;
                                    bmploaded = true;
                                }
                        }
                        else if (openBitmapDialog1.ShowDialog() == DialogResult.OK)
                        {
                            if (!Path.GetFileNameWithoutExtension(openBitmapDialog1.FileName).Contains("_a"))
                            {
                                bmp = new Bitmap(openBitmapDialog1.FileName);
                                alphamap = Path.Combine(Path.GetDirectoryName(openBitmapDialog1.FileName), Path.GetFileNameWithoutExtension(openBitmapDialog1.FileName) + "_a" + Path.GetExtension(openBitmapDialog1.FileName));
                                if (CheckSize(bmp))
                                {
                                    repbmp.Bitmap = bmp;
                                    bmpfilename = openBitmapDialog1.FileName;
                                    bmploaded = true;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(this, Resources.repbmp_NewFileSelect_Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if (bmploaded)
                        {

                            if (Alphabox.Text.Length > 0 && File.Exists(Alphabox.Text))
                            {
                                Bitmap alpha = new Bitmap(Alphabox.Text);
                                repbmp.Alpha = alpha;
                                if (Checkhdimgsize(bmp) && Path.GetFileName(bmpfilename).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                {
                                    repbmp.BmpType = FSHBmpType.ThirtyTwoBit;
                                    fshTypeBox.SelectedIndex = 1;
                                }
                                else
                                {
                                    repbmp.BmpType = FSHBmpType.DXT3;
                                    fshTypeBox.SelectedIndex = 3;
                                }
                            }
                            else if (!string.IsNullOrEmpty(alphamap) && File.Exists(alphamap))
                            {
                                Bitmap alpha = new Bitmap(alphamap);
                                repbmp.Alpha = alpha;
                                if (Checkhdimgsize(bmp) && Path.GetFileName(bmpfilename).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                {
                                    repbmp.BmpType = FSHBmpType.ThirtyTwoBit;
                                    fshTypeBox.SelectedIndex = 1;
                                }
                                else
                                {
                                    repbmp.BmpType = FSHBmpType.DXT3;
                                    fshTypeBox.SelectedIndex = 3;
                                }
                            }
                            else if (Path.GetExtension(bmpfilename).Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
                            {
                                repbmp.Alpha = GetAlphafromPng(bmp);
                                if (Checkhdimgsize(bmp) && Path.GetFileName(bmpfilename).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                {
                                    repbmp.BmpType = FSHBmpType.ThirtyTwoBit;
                                    fshTypeBox.SelectedIndex = 1;
                                }
                                else
                                {
                                    repbmp.BmpType = FSHBmpType.DXT3;
                                    fshTypeBox.SelectedIndex = 3;
                                }
                            }
                            else
                            {
                                if (bmp != null)
                                {
                                    
                                    repbmp.Alpha = GenerateAlpha(bmp);
                                    if (Checkhdimgsize(bmp) && Path.GetFileName(bmpfilename).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                    {
                                        repbmp.BmpType = FSHBmpType.TwentyFourBit;
                                        fshTypeBox.SelectedIndex = 0;
                                    }
                                    else
                                    {
                                        repbmp.BmpType = FSHBmpType.DXT1;
                                        fshTypeBox.SelectedIndex = 2;
                                    }
                                }
                            }
                            if ((dirTxt.Text.Length > 0) && dirTxt.Text.Length == 4)
                            {
                                repbmp.SetDirName(dirTxt.Text);
                            }
                            else
                            {
                                repbmp.SetDirName("FiSH");
                            }
                            
                            curimage.Bitmaps.RemoveAt(listViewMain.SelectedItems[0].Index);
                            curimage.Bitmaps.Insert(listViewMain.SelectedItems[0].Index, repbmp);
                            curimage.UpdateDirty();
                                                        
                            Temp_fsh();
                            mipbtn_Click(null, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    }
                }
                else
                {
                    MessageBox.Show(this, Resources.repbmp_NoImageSelected_Error,this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                
            }
        }

        private void hdFshRadio_CheckedChanged(object sender, EventArgs e)
        {
            
            if (bmpitem != null && bmpitem.Bitmap != null)
            {
                if (bmpitem.Bitmap.Width < 256 && bmpitem.Bitmap.Height < 256)
                {
                    // MessageBox.Show(this, "A bitmap must be at least 256 x 256 to use High definition fsh", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    hdfshRadio.Checked = false;
                    hdBasetexrdo.Checked = false;
                    regFshrdo.Checked = true;
                    hdfshRadio.Enabled = false;
                    hdBasetexrdo.Enabled = false;
                }
                else
                {
                    if (curimage.Bitmaps.Count > 1)
                    {
                        hdfshRadio.Enabled = false;
                    }
                    else
                    {
                        hdfshRadio.Enabled = true;
                    }
                    hdBasetexrdo.Enabled = true;
                    if (hdfshRadio.Checked)
                    {
                        bmpitem.BmpType = FSHBmpType.ThirtyTwoBit;
                        fshTypeBox.SelectedIndex = 1;
                    }
                    else if (hdBasetexrdo.Checked)
                    {
                        bmpitem.BmpType = FSHBmpType.TwentyFourBit;
                        fshTypeBox.SelectedIndex = 0;
                    }
                    else if (regFshrdo.Checked)
                    {
                        if (bmpitem.Alpha.GetPixel(0, 0).ToArgb() == Color.Black.ToArgb())
                        {
                            bmpitem.BmpType = FSHBmpType.DXT3; 
                            fshTypeBox.SelectedIndex = 3;
                        }
                        else
                        {
                            bmpitem.BmpType = FSHBmpType.DXT1;
                            fshTypeBox.SelectedIndex = 2;
                        }
                    }
                }

            }
            else
            {
                hdfshRadio.Checked = false;
                hdBasetexrdo.Checked = false;
                regFshrdo.Checked = true;
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
                    if (mipsize == 64)
                    {
                        SaveFsh(mstream, mip64fsh);
                        mip64fsh = new FSHImage(mstream);
                    }
                    else if (mipsize == 32)
                    {
                        SaveFsh(mstream, mip32fsh);
                        mip32fsh = new FSHImage(mstream);
                    }
                    else if (mipsize == 16)
                    {
                        SaveFsh(mstream, mip16fsh);
                        mip16fsh = new FSHImage(mstream);
                    }
                    else if (mipsize == 8)
                    {
                        SaveFsh(mstream, mip8fsh);
                        mip8fsh = new FSHImage(mstream);
                    }
                }

                bmpitem = new BitmapItem();
                switch (mipsize)
                { 
                    case 64:
                        RefreshMipImageList(mip64fsh, bmp64Mip, alpha64Mip, blend64Mip, listViewMip64);
                        break;
                    case 32:
                        RefreshMipImageList(mip32fsh, bmp32Mip, alpha32Mip, blend32Mip, listViewMip32);
                        break;
                    case 16:
                        RefreshMipImageList(mip16fsh, bmp16Mip, alpha16Mip, blend16Mip, listViewMip16);
                        break;
                    case 8:
                        RefreshMipImageList(mip8fsh, bmp8Mip, alpha8Mip, blend8Mip, listViewMip8);
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
            /*Bitmap image = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(image)) // this is hopfully higher quality that GetThumbnailImage
            {
                gr.InterpolationMode = InterpolationMode.HighQualityBilinear;
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.CompositingQuality = CompositingQuality.HighQuality;
                gr.DrawImage(source, new Rectangle(0, 0, width, height));
            }*/
            Bitmap image = SuperSample.SuperSample.GetBitmapThumbnail(source, width, height); 
            return image;
        }
        private bool mipsbtn_clicked = false;
        private FSHImage mip64fsh = null;
        private FSHImage mip32fsh = null;
        private FSHImage mip16fsh = null;
        private FSHImage mip8fsh = null;
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
                BitmapItem item = (BitmapItem)curimage.Bitmaps[index];
                // 0 = 8, 1 = 16, 2 = 32, 3 = 64

                Bitmap bmp = new Bitmap(item.Bitmap);
                bmps[0] = GetBitmapThumbnail(bmp, 8, 8);
                bmps[1] = GetBitmapThumbnail(bmp, 16, 16);
                bmps[2] = GetBitmapThumbnail(bmp, 32, 32);
                bmps[3] = GetBitmapThumbnail(bmp, 64, 64);
                //alpha
                Bitmap alpha = new Bitmap(item.Alpha);
                alphas[0] = GetBitmapThumbnail(alpha, 8, 8);
                alphas[1] = GetBitmapThumbnail(alpha, 16, 16);
                alphas[2] = GetBitmapThumbnail(alpha, 32, 32);
                alphas[3] = GetBitmapThumbnail(alpha, 64, 64);

                for (int i = 0; i < 4; i++)
                {
                    if (bmps[i] != null && alphas[i] != null)
                    {
                        BitmapItem mipitm = new BitmapItem();
                        mipitm.Bitmap = bmps[i];
                        mipitm.Alpha = alphas[i];
                        mipitm.SetDirName(Encoding.ASCII.GetString(item.DirName));
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
                            if (mip64fsh == null)
                            {
                                mip64fsh = new FSHImage();
                            }
                            mip64fsh.Bitmaps.Add(mipitm);
                            mip64fsh.UpdateDirty();
                            if (index == (curimage.Bitmaps.Count - 1))
                            {
                                Temp_Mips(64);
                            }
                        }
                        else if (mipitm.Bitmap.Width == 32 && mipitm.Bitmap.Height == 32)
                        {
                            if (mip32fsh == null)
                            {
                                mip32fsh = new FSHImage();
                            }
                            mip32fsh.Bitmaps.Add(mipitm);
                            mip32fsh.UpdateDirty();
                            if (index == (curimage.Bitmaps.Count - 1))
                            {
                                Temp_Mips(32);
                            }
                        }
                        else if (mipitm.Bitmap.Width == 16 && mipitm.Bitmap.Height == 16)
                        {
                            if (mip16fsh == null)
                            {
                                mip16fsh = new FSHImage();
                            }
                            mip16fsh.Bitmaps.Add(mipitm);
                            mip16fsh.UpdateDirty();
                            if (index == (curimage.Bitmaps.Count - 1))
                            {
                                Temp_Mips(16);
                            }
                        }
                        else if (mipitm.Bitmap.Width == 8 && mipitm.Bitmap.Height == 8)
                        {
                            if (mip8fsh == null)
                            {
                                mip8fsh = new FSHImage();
                            }
                            mip8fsh.Bitmaps.Add(mipitm);
                            mip8fsh.UpdateDirty();
                            if (index == (curimage.Bitmaps.Count - 1))
                            {
                                using (MemoryStream mstream = new MemoryStream())
                                {
                                    SaveFsh(mstream, mip8fsh);
                                    mip8fsh = new FSHImage(mstream);
                                }
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
        }
        private void mipbtn_Click(object sender, EventArgs e)
        {
            if ((curimage != null) && curimage.Bitmaps.Count >= 1)
            {
                try
                {
                    mip64fsh = null;
                    mip32fsh = null;
                    mip16fsh = null;
                    mip8fsh = null;
                    for (int b = 0; b < curimage.Bitmaps.Count; b++)
                    {
                        GenerateMips(b);
                    }
                    RefreshDirectory(curimage);
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
            using (FileStream fs = new FileStream(filename + ".TGI", FileMode.OpenOrCreate, FileAccess.Write))
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
            }
        }
        
        private void FshtypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FSHImage image = GetImageFromSelectedTab(tabControl1.SelectedIndex);
            if (bmpitem != null && bmpitem.Bitmap != null)
            {
                if (!Checkhdimgsize(bmpitem.Bitmap))
                {
                    if (fshTypeBox.SelectedIndex == 0 || fshTypeBox.SelectedIndex == 1)
                    {
                        fshTypeBox.SelectedIndex = typeindex;
                    }
                }
             
                if (fshTypeBox.SelectedIndex == 0)
                {
                    hdBasetexrdo.Checked = true;
                }
                else if (fshTypeBox.SelectedIndex == 1)
                {
                    hdfshRadio.Checked = true;
                }
                else
                {
                    regFshrdo.Checked = true;
                }
                switch (fshTypeBox.SelectedIndex)
                {
                    case 0:
                        bmpitem.BmpType = FSHBmpType.TwentyFourBit;
                        break;
                    case 1:
                        bmpitem.BmpType = FSHBmpType.ThirtyTwoBit;
                        break;
                    case 2:
                        bmpitem.BmpType = FSHBmpType.DXT1;
                        break;
                    case 3:
                        bmpitem.BmpType = FSHBmpType.DXT3;
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
                if (curimage != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        curimage.IsCompressed = true;
                    }
                    else
                    {
                        curimage.IsCompressed = false;
                    }

                    if (!loadeddat && DatlistView1.Items.Count == 0)
                    {
                        if (!mipsbtn_clicked)
                        {
                            mipbtn_Click(sender, e);
                        }
                        if (mipsbtn_clicked && mip64fsh != null && mip32fsh != null && mip16fsh != null && mip8fsh != null)
                        {
                            string filepath = Path.Combine(Path.GetDirectoryName(saveFshDialog1.FileName) + Path.DirectorySeparatorChar, Path.GetFileName(saveFshDialog1.FileName));
                            if (curimage.IsCompressed)
                            {
                                mip64fsh.IsCompressed = true;
                                mip32fsh.IsCompressed = true;
                                mip16fsh.IsCompressed = true;
                                mip8fsh.IsCompressed = true;
                            }
                            string ext = Path.GetExtension(saveFshDialog1.FileName);
                            using (FileStream m64 = mip_stream(filepath, "_s3" + ext))
                            {
                                SaveFsh(m64,mip64fsh);
                            }
                            WriteTgi(filepath + "_s3" + ext, 3);

                            using (FileStream m32 = mip_stream(filepath, "_s2"+ ext))
                            {
                                SaveFsh(m32, mip32fsh);
                            }
                            WriteTgi(filepath + "_s2" + ext, 2);

                            using (FileStream m16 = mip_stream(filepath, "_s1" + ext))
                            {
                                SaveFsh(m16, mip16fsh);
                            }
                            WriteTgi(filepath + "_s1" + ext, 1);

                            using (FileStream m8 = mip_stream(filepath, "_s0" + ext))
                            {
                                SaveFsh(m8, mip8fsh);
                            }
                            WriteTgi(filepath + "_s0" + ext, 0);
                        }
                        WriteTgi(saveFshDialog1.FileName, 4);
                    }
                    using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fs, curimage);
                    }
                }
            }
            else if (tabControl1.SelectedTab == mip64tab)
            {
                if (mip64fsh != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        mip64fsh.IsCompressed = true;
                    }
                    else
                    {
                        mip64fsh.IsCompressed = false;
                    }
                    WriteTgi(saveFshDialog1.FileName, 3);
                    using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fs, mip64fsh);
                    }
                }
            }
            else if (tabControl1.SelectedTab == mip32tab)
            {
                if (mip32fsh != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        mip32fsh.IsCompressed = true;
                    }
                    else
                    {
                        mip32fsh.IsCompressed = false;
                    }
                    WriteTgi(saveFshDialog1.FileName, 2);
                    using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fs, mip32fsh);
                    }
                }
            }
            else if (tabControl1.SelectedTab == mip16tab)
            {
                if (mip16fsh != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        mip16fsh.IsCompressed = true;
                    }
                    else
                    {
                        mip16fsh.IsCompressed = false;
                    }
                    WriteTgi(saveFshDialog1.FileName, 1);
                    using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fs, mip16fsh);
                    }
                }
            }
            else if (tabControl1.SelectedTab == mip8tab)
            {
                if (mip8fsh != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        mip8fsh.IsCompressed = true;
                    }
                    else
                    {
                        mip8fsh.IsCompressed = false;
                    }
                    WriteTgi(saveFshDialog1.FileName, 0);
                    using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fs, mip8fsh);   
                    }
                }
            }
        }
        private void saveBitmap(Bitmap bmp, PixelFormat format, string addtofilename)
        {
            ListView listv = new ListView();
            FSHImage image = new FSHImage();
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    listv = listViewMain;
                    image = curimage;
                    break;
                case 1:
                    listv = listViewMip64;
                    image = mip64fsh;
                    break;
                case 2:
                    listv = listViewMip32;
                    image = mip32fsh;
                    break;
                case 3:
                    listv = listViewMip16;
                    image = mip16fsh;
                    break;
                case 4:
                    listv = listViewMip8;
                    image = mip8fsh;
                    break;

            }
            if (listv.SelectedItems.Count > 0)
            {
                try
                {
                    string bitmapnum = image.Bitmaps.Count > 1 ? "-" + listv.SelectedItems[0].Index.ToString() : string.Empty;
                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                    if (!string.IsNullOrEmpty(fshfilename))
                    {
                        string name = string.Concat(fshfilename, bitmapnum, addtofilename, ".png");
                        using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            using (Bitmap tempbmp = bmp.Clone(rect, format))
                            {
                                tempbmp.Save(fs, ImageFormat.Png);
                            }
                        }

                    }
                    else if (loadeddat && DatlistView1.SelectedItems.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dat.FileName))
                        {
                            ListViewItem item = DatlistView1.SelectedItems[0];
                            string fshname = Path.Combine(Path.GetDirectoryName(dat.FileName), "0x" + item.SubItems[2].Text);

                            string name = string.Concat(fshname, bitmapnum, addtofilename, ".png");

                            using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                using (Bitmap tempbmp = bmp.Clone(rect, format))
                                {
                                    tempbmp.Save(fs, ImageFormat.Png);
                                }
                            }

                        }
                    }
                    else
                    {
                        Debug.WriteLine("fshfilename is null");
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
            if (bmpitem != null && bmpitem.Bitmap != null && bmpitem.Alpha != null)
            {
                BlendBitmap blbmp = new BlendBitmap();
                saveBitmap(blbmp.BlendBmp(bmpitem), PixelFormat.Format32bppArgb, "_blend");
            }
        }

        private void bmpSaveBtn_Click(object sender, EventArgs e)
        {
            if (bmpitem != null && bmpitem.Bitmap != null)
            {
                saveBitmap(bmpitem.Bitmap, PixelFormat.Format24bppRgb, string.Empty);
            }
        }

        private void alphaSaveBtn_Click(object sender, EventArgs e)
        {
            if (bmpitem != null && bmpitem.Alpha != null)
            {
                saveBitmap(bmpitem.Alpha, PixelFormat.Format24bppRgb, "_a");
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
                Alphabox.Text = openAlphaDialog1.FileName;
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
                    Alphabox.Text = alpha;
                }
                else
                {
                    Alphabox.Text = null;
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
                curimage = new FSHImage();
                tabControl1.SelectedTab = Maintab; 
                ClearandReset(true);
            }

            bmpitem = new BitmapItem();
            Bitmap bmp = null;

            try
            {
                files = CheckSize(files);
                files = TrimAlphaBitmaps(files);

                if (files.Count > 0)
                {
                    string alphamap = string.Empty;
                    if (File.Exists(files[0]))
                    {
                        bmp = new Bitmap(files[0]);
                        if (bmpBox.Text.Length <= 0)
                        {
                            alphamap = Path.Combine(Path.GetDirectoryName(files[0]), Path.GetFileNameWithoutExtension(files[0]) + "_a" + Path.GetExtension(files[0]));
                        }
                        bmpitem.Bitmap = bmp;
                   
                        if (Alphabox.Text.Length > 0 && File.Exists(Alphabox.Text))
                        {
                            Bitmap alpha = new Bitmap(Alphabox.Text);
                            bmpitem.Alpha = alpha;
                            if (Checkhdimgsize(bmp) && Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                            {
                                bmpitem.BmpType = FSHBmpType.ThirtyTwoBit;
                                fshTypeBox.SelectedIndex = 1;
                            }
                            else
                            {
                                bmpitem.BmpType = FSHBmpType.DXT3;
                                fshTypeBox.SelectedIndex = 3;
                            }
                        }
                        else if (!string.IsNullOrEmpty(alphamap) && File.Exists(alphamap))
                        {
                            Bitmap alpha = new Bitmap(alphamap);
                            bmpitem.Alpha = alpha;
                            if (Checkhdimgsize(bmp) && Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                            {
                                bmpitem.BmpType = FSHBmpType.ThirtyTwoBit;
                                fshTypeBox.SelectedIndex = 1;
                            }
                            else
                            {
                                bmpitem.BmpType = FSHBmpType.DXT3;
                                fshTypeBox.SelectedIndex = 3;
                            }
                        }
                        else if (Path.GetExtension(files[0]).ToLowerInvariant().Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            
                            bmpitem.Alpha = GetAlphafromPng(bmp);
                            if (Checkhdimgsize(bmp) && Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                            {
                                bmpitem.BmpType = FSHBmpType.ThirtyTwoBit;
                                fshTypeBox.SelectedIndex = 1;
                            }
                            else
                            {
                                bmpitem.BmpType = FSHBmpType.DXT3;
                                fshTypeBox.SelectedIndex = 3;
                            }
                        }
                        else
                        {
                            if (bmp != null)
                            {
                                bmpitem.Alpha = GenerateAlpha(bmp);
                                if (Checkhdimgsize(bmp) && Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                {
                                    bmpitem.BmpType = FSHBmpType.TwentyFourBit;
                                    fshTypeBox.SelectedIndex = 0;
                                }
                                else
                                {
                                    bmpitem.BmpType = FSHBmpType.DXT1;
                                    fshTypeBox.SelectedIndex = 2;
                                }
                            }
                        }


                        if (dirTxt.Text.Length > 0 && dirTxt.Text.Length == 4)
                        {
                            bmpitem.SetDirName(dirTxt.Text);
                        }
                        else
                        {
                            bmpitem.SetDirName("FiSH");
                        }

                        string fn = Path.GetFileNameWithoutExtension(files[0]);
                        if (fn.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            inststr = fn.Substring(2, 8);
                            EndFormat_Refresh();
                        }
                        else if (tgiInstanceTxt.Text.Length <= 0)
                        {
                            EndFormat_Refresh();
                        }
                        if (bmp.Height < 256 && bmp.Width < 256)
                        {
                            hdfshRadio.Enabled = false;
                            hdBasetexrdo.Enabled = false;
                            regFshrdo.Checked = true;
                        }
                        else
                        {
                            hdfshRadio.Enabled = true;
                            hdBasetexrdo.Enabled = true;
                            if (bmpitem.BmpType == FSHBmpType.ThirtyTwoBit)
                            {
                                hdfshRadio.Checked = true;
                            }
                            else if (bmpitem.BmpType == FSHBmpType.TwentyFourBit)
                            {
                                hdBasetexrdo.Checked = true;
                            }
                            else
                            {
                                regFshrdo.Checked = true;
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
                                origbmplist.Clear();
                            }
                            origbmplist.Add(bmp); // store the original bitmap to use if switching between fshwrite and fshlib compression

                            curimage = new FSHImage();
                            curimage.Bitmaps.Add(bmpitem);
                            curimage.UpdateDirty();
                            if (files.Count - 1 == 0)
                            {
                                Temp_fsh();
                                mipbtn_Click(null, null);
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
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        string inststr;
        string Groupidoverride = null;
        private Settings settings = null;
        private void Loadsettings()
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
                MessageBox.Show(this, "unable to load settings: \n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (gid.ToLower().StartsWith("0x"))
                    {
                        gid = gid.Substring(2, 8);
                    }
                }
                if (gid.Length == 8)
                {
                    Regex rx = new Regex(@"^[A-Fa-f0-9]*$");
                    bool hexgid = rx.IsMatch(gid);
                    if (hexgid)
                    {
                        Groupidoverride = gid;
                    }
                }
            }
        }
        private void ReloadGroupid()
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

            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    FileInfo fi = new FileInfo(args[i]);
                    if (fi.Exists)
                    {
                        if (fi.Extension.Equals(".png") || fi.Extension.Equals(".bmp"))
                        {
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text);
            }
            return count;
        }
        private void Multifshfrm_Load(object sender, EventArgs e)
        {
            fshTypeBox.SelectedIndex = 2;

            Loadsettings();
/*#if DEBUG
            mipbtn.Visible = true;
#else
            mipbtn.Visible = false;
#endif*/
            if (tgiGroupTxt.Text.Length <= 0)
            {
                ReloadGroupid();
            }
            if (tgiInstanceTxt.Text.Length <= 0)
            {
                inststr = RandomHexString(7);
                //tgiInstancetxt.Text = inststr;   
                EndFormat_Refresh();

            }
            //this.Text += string.Concat(" ", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            datNameTxt.Text = "No dat loaded";
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 0)
            {
                bool loaded = false;
                int pngcnt = CountPngArgs(args);
                List<string> pnglist = null;

                try
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        FileInfo fi = new FileInfo(args[i]);
                        if (fi.Exists)
                        {
                            if (fi.Extension.Equals(".fsh") || fi.Extension.Equals(".qfs"))
                            {
                                if (!loaded)
                                {
                                    Load_Fsh(fi.FullName);
                                    loaded = true;
                                }
                            }
                            else if (fi.Extension.Equals(".png") || fi.Extension.Equals(".bmp"))
                            {
                                if (!loaded)
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
                            }
                            else if (fi.Extension.Equals(".dat"))
                            {
                                if (!loaded)
                                {
                                    Load_Dat(fi.FullName);
                                    loaded = true;
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
        }
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (mip64fsh == null && e.TabPage == mip64tab)
            {
                e.Cancel = true;
            }
            else if (mip32fsh == null && e.TabPage == mip32tab)
            {
                e.Cancel = true;
            } 
            else if (mip16fsh == null && e.TabPage == mip16tab)
            {
                e.Cancel = true;
            } 
            else if (mip8fsh == null && e.TabPage == mip8tab)
            {
                e.Cancel = true;
            } 
            /*else if (curimage == null && e.TabPage == Maintab)
            {
                e.Cancel = true;
            }*/
        }
        private void RefreshBmpType()
        {
            switch (bmpitem.BmpType)
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
                addbtn.Enabled = false;
                remBtn.Enabled = false;
                repBtn.Enabled = false;
            }
            else
            {
                addbtn.Enabled = true;
                remBtn.Enabled = true;
                repBtn.Enabled = true;
            }
        }
        private void SetHdRadios(TabPage page)
        {
            if (page != Maintab)
            {
                hdfshRadio.Enabled = hdBasetexrdo.Enabled = false;
            }
            else
            {
                hdfshRadio.Enabled = hdBasetexrdo.Enabled = true;
            }
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mipsbtn_clicked == false && loadisMip == false)
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
                if (mip64fsh != null && mip64fsh.Bitmaps.Count > 0 && tabControl1.SelectedTab == mip64tab)
                {
                    RefreshMipImageList(mip64fsh, bmp64Mip, alpha64Mip, blend64Mip, listViewMip64);
                    listViewMip64.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(inststr, end64);
                }
                else if (mip32fsh != null && mip32fsh.Bitmaps.Count > 0 && tabControl1.SelectedTab == mip32tab)
                {
                    RefreshMipImageList(mip32fsh, bmp32Mip, alpha32Mip, blend32Mip, listViewMip32);
                    bmpitem = (BitmapItem)mip32fsh.Bitmaps[0];
                    listViewMip32.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(inststr, end32);
                }
                else if (mip16fsh != null && mip16fsh.Bitmaps.Count > 0 && tabControl1.SelectedTab == mip16tab)
                {
                    RefreshMipImageList(mip16fsh, bmp16Mip, alpha16Mip, blend16Mip, listViewMip16);
                    bmpitem = (BitmapItem)mip16fsh.Bitmaps[0];
                    listViewMip16.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(inststr, end16);
                }
                else if (mip8fsh != null && mip8fsh.Bitmaps.Count > 0 && tabControl1.SelectedTab == mip8tab)
                {
                    RefreshMipImageList(mip8fsh, bmp8Mip, alpha8Mip, blend8Mip, listViewMip8);
                    bmpitem = (BitmapItem)mip8fsh.Bitmaps[0];
                    listViewMip8.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(inststr, end8);
                }
                else if (curimage != null && curimage.Bitmaps.Count > 0 && tabControl1.SelectedTab == Maintab)
                {
                    RefreshImageLists(); 
                    bmpitem = (BitmapItem)curimage.Bitmaps[0];
                    listViewMain.Items[0].Selected = true;
                    tgiInstanceTxt.Text = string.Concat(inststr, endreg);
                    RefreshBmpType();
                }
                
            }
        }

        private void listViewMip64_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMip64.SelectedItems.Count > 0)
            {
                bmpitem = (BitmapItem)mip64fsh.Bitmaps[listViewMip64.SelectedItems[0].Index];
                Settypeindex(mip64fsh, bmpitem);
                RefreshBmpType();
                Sizelbl.Text = fshsize[listViewMip64.SelectedItems[0].Index];
                dirTxt.Text = dirname[listViewMip64.SelectedItems[0].Index];
            }
        }

        private void listViewMip32_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMip32.SelectedItems.Count > 0)
            {
                bmpitem = (BitmapItem)mip32fsh.Bitmaps[listViewMip32.SelectedItems[0].Index];
                Settypeindex(mip32fsh, bmpitem);
                RefreshBmpType();
                Sizelbl.Text = fshsize[listViewMip32.SelectedItems[0].Index];
                dirTxt.Text = dirname[listViewMip32.SelectedItems[0].Index];
            }
        }

        private void listViewMip16_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMip16.SelectedItems.Count > 0)
            {
                bmpitem = (BitmapItem)mip16fsh.Bitmaps[listViewMip16.SelectedItems[0].Index];
                Settypeindex(mip16fsh, bmpitem);
                RefreshBmpType();
                Sizelbl.Text = fshsize[listViewMip16.SelectedItems[0].Index];
                dirTxt.Text = dirname[listViewMip16.SelectedItems[0].Index];
            }
        }

        private void listViewMip8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMip8.SelectedItems.Count > 0)
            {
                bmpitem = (BitmapItem)mip8fsh.Bitmaps[listViewMip8.SelectedItems[0].Index];
                Settypeindex(mip8fsh, bmpitem);
                RefreshBmpType();
                Sizelbl.Text = fshsize[listViewMip8.SelectedItems[0].Index];
                dirTxt.Text = dirname[listViewMip8.SelectedItems[0].Index];
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
                        if (inst0.ToLower().StartsWith("0x"))
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
                        if (inst1.ToLower().StartsWith("0x"))
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

            //bool copied = false;
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
            else if (e.KeyCode == Keys.A || e.KeyCode == Keys.B || e.KeyCode == Keys.C || e.KeyCode == Keys.D || e.KeyCode == Keys.E || e.KeyCode == Keys.F || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
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
                endreg = Convert.ToChar("4");
                end64 = Convert.ToChar("3");
                end32 = Convert.ToChar("2");
                end16 = Convert.ToChar("1");
                end8 = Convert.ToChar("0");
            }
            else if (inst5_9Rdo.Checked)
            {
                endreg = Convert.ToChar("9");
                end64 = Convert.ToChar("8");
                end32 = Convert.ToChar("7");
                end16 = Convert.ToChar("6");
                end8 = Convert.ToChar("5");
            }
            else if (instA_ERdo.Checked)
            {
                endreg = Convert.ToChar("E");
                end64 = Convert.ToChar("D");
                end32 = Convert.ToChar("C");
                end16 = Convert.ToChar("B");
                end8 = Convert.ToChar("A");
            }
            if (string.IsNullOrEmpty(inststr))
            {
                if (!string.IsNullOrEmpty(tgiInstanceTxt.Text))
                {
                    inststr = tgiInstanceTxt.Text.Substring(0, 7);
                }
                else
                {
                    inststr = RandomHexString(7);
                }
            }
            if (inststr.Length > 7)
            {
                if (tabControl1.SelectedTab == mip64tab)
                {
                    if (inststr[7].Equals(Convert.ToChar("D")))
                    {
                        instA_ERdo.Checked = true;
                    }
                    else if (inststr[7].Equals(Convert.ToChar("8")))
                    {
                        inst5_9Rdo.Checked = true;
                    }
                    else if (inststr[7].Equals(Convert.ToChar("3")))
                    {
                        inst0_4Rdo.Checked = true;
                    }
                }
                else if (tabControl1.SelectedTab == mip32tab)
                {
                    if (inststr[7].Equals(Convert.ToChar("C")))
                    {
                        instA_ERdo.Checked = true;
                    }
                    else if (inststr[7].Equals(Convert.ToChar("7")))
                    {
                        inst5_9Rdo.Checked = true;
                    }
                    else if (inststr[7].Equals(Convert.ToChar("2")))
                    {
                        inst0_4Rdo.Checked = true;
                    }
                }
                else if (tabControl1.SelectedTab == mip16tab)
                {
                    if (inststr[7].Equals(Convert.ToChar("B")))
                    {
                        instA_ERdo.Checked = true;
                    }
                    else if (inststr[7].Equals(Convert.ToChar("6")))
                    {
                        inst5_9Rdo.Checked = true;
                    }
                    else if (inststr[7].Equals(Convert.ToChar("2")))
                    {
                        inst0_4Rdo.Checked = true;
                    }
                }
                else if (tabControl1.SelectedTab == mip8tab)
                {
                    if (inststr[7].Equals(Convert.ToChar("A")))
                    {
                        instA_ERdo.Checked = true;
                    }
                    else if (inststr[7].Equals(Convert.ToChar("5")))
                    {
                        inst5_9Rdo.Checked = true;
                    }
                    else if (inststr[7].Equals(Convert.ToChar("0")))
                    {
                        inst0_4Rdo.Checked = true;
                    }
                }
                else if (tabControl1.SelectedTab == Maintab)
                {
                    if (inststr[7].Equals(Convert.ToChar("E")))
                    {
                        instA_ERdo.Checked = true;
                    }
                    else if (inststr[7].Equals(Convert.ToChar("9")))
                    {
                        inst5_9Rdo.Checked = true;
                    }
                    else if (inststr[7].Equals(Convert.ToChar("4")))
                    {
                        inst0_4Rdo.Checked = true;
                    }
                }
                inststr = inststr.Substring(0,7);
            }
            if (tabControl1.SelectedTab == mip64tab)
            {
                tgiInstanceTxt.Text = string.Concat(inststr, end64);
            }
            else if (tabControl1.SelectedTab == mip32tab)
            {
                tgiInstanceTxt.Text = string.Concat(inststr, end32);
            }
            else if (tabControl1.SelectedTab == mip16tab)
            {
                tgiInstanceTxt.Text = string.Concat(inststr, end16);
            }
            else if (tabControl1.SelectedTab == mip8tab)
            {
                tgiInstanceTxt.Text = string.Concat(inststr, end8);
            }
            else if (tabControl1.SelectedTab == Maintab)
            { 
                tgiInstanceTxt.Text = string.Concat(inststr, endreg);
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
        private string originst = null;
        private bool loadeddat = false;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Fsh")]
        private void Load_Dat(string filename)
        {
            try
            {
                dat = new DatFile(filename);
                DatlistView1.Items.Clear();
                ClearFshlists();
                int fshnum = 0;
                DatlistView1.BeginUpdate();
                for (int i = 0; i < dat.Indexes.Count; i++)
                {
                    DatIndex index = dat.Indexes[i]; 
                    if (index.Type == uint.Parse("7ab50e44", NumberStyles.HexNumber))
                    {

                        string istr = index.Instance.ToString("X8");
                        if (istr.EndsWith("4") || istr.EndsWith("9") || istr.EndsWith("E"))
                        {
                            FshWrapper wrap = dat.LoadFile(index.Group, index.Instance);

                            if (wrap.Image != null && ((BitmapItem)wrap.Image.Bitmaps[0]).Bitmap.Width >= 128 && ((BitmapItem)wrap.Image.Bitmaps[0]).Bitmap.Width >= 128)
                            {
                                fshnum++;
                                ListViewItem item1 = new ListViewItem("Fsh # " + fshnum.ToString());

                                item1.SubItems.Add(index.Group.ToString("X8"));
                                item1.SubItems.Add(index.Instance.ToString("X8"));
                                item1.Tag = wrap;
                                DatlistView1.Items.Add(item1);
                            }
                        }
                    }
                }
                DatlistView1.EndUpdate();

                if (DatlistView1.Items.Count > 0)
                {
                    loadeddat = true;
                    DatRebuilt = false;
                    SetLoadedDatEnables();
                    DatlistView1.Items[0].Selected = true;
                    datNameTxt.Text = Path.GetFileName(dat.FileName);
                }
                else
                {
                    string message = string.Format("No images loaded from {0}", Path.GetFileName(filename));
                    MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    loadeddat = false;
                    ClearandReset(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (mipsbtn_clicked && mip64fsh != null && mip32fsh != null && mip16fsh != null && mip8fsh != null && curimage != null)
            { 
                uint type = uint.Parse("7ab50e44",NumberStyles.HexNumber);
                uint group = uint.Parse(tgiGroupTxt.Text,NumberStyles.HexNumber);
                uint[] instanceid = new uint[5];
                FshWrapper[] fshwrap = new FshWrapper[5];
                FSHImage[] fshimg = new FSHImage[5];
                fshimg[0] = mip8fsh; fshimg[1] = mip16fsh; fshimg[2] = mip32fsh;
                fshimg[3] = mip64fsh; fshimg[4] = curimage;
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
                if (chkindex.Type == uint.Parse("7ab50e44",NumberStyles.HexNumber) && chkindex.Group == group && chkindex.Flags != DatIndexFlags.New)
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
        /// <param name="filename">The filename to save as</param>
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
                if (!loadeddat && DatlistView1.Items.Count == 0)
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
                if (!loadeddat && DatlistView1.Items.Count == 0)
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
            if (dat != null && loadeddat)
            {
                settings.PutSetting("genNewInstcb_checked",genNewInstCb.Checked.ToString());
                if (genNewInstCb.Checked)
                {
                    for (int n = 0; n < dat.Indexes.Count; n++)
                    {
                        DatIndex addindex = dat.Indexes[n];
                        if (addindex.Type == uint.Parse("7ab50e44", NumberStyles.HexNumber))
                        {
                            string newinstance = null;

                            if (addindex.Instance == uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end8, NumberStyles.HexNumber))
                            {
                                newinstance = RandomHexString(7);
                                inststr = newinstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (addindex.Instance == uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end16, NumberStyles.HexNumber))
                            {
                                newinstance = RandomHexString(7);
                                inststr = newinstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (addindex.Instance == uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end32, NumberStyles.HexNumber))
                            {
                                newinstance = RandomHexString(7);
                                inststr = newinstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (addindex.Instance == uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + end64, NumberStyles.HexNumber))
                            {
                                newinstance = RandomHexString(7);
                                inststr = newinstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (addindex.Instance == uint.Parse(tgiInstanceTxt.Text.Substring(0, 7) + endreg, NumberStyles.HexNumber))
                            {
                                newinstance = RandomHexString(7);
                                inststr = newinstance.Substring(0,7);
                                EndFormat_Refresh();
                            }
                        } 
                       
                    }
                    RebuildDat(dat);
                }
                else
                {
                    if (originst != null)
                    {
                        if (inststr != originst)
                        {
                            inststr = originst;
                            EndFormat_Refresh();
                        }
                    }
                }
            }
        }

        private void newDatbtn_Click(object sender, EventArgs e)
        {
           if (loadeddat && DatlistView1.Items.Count > 0)
           {
               ClearandReset(true);
               loadeddat = false;
           }
           else
           {
               ClearandReset(false);
           }
           this.dat = new DatFile();
           DatRebuilt = false;
           datNameTxt.Text = "Dat in Memory";       
           SetLoadedDatEnables();

        }
        
        private void DatlistView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DatlistView1.SelectedItems.Count > 0)
            {
                try 
                {
                    string group = DatlistView1.SelectedItems[0].SubItems[1].Text;
                    string instance = DatlistView1.SelectedItems[0].SubItems[2].Text;
                    ClearFshlists();
                    tgiGroupTxt.Text = group;
                    inststr = instance;
                    EndFormat_Refresh();
                    originst = instance.Substring(0,7);


                    FshWrapper fshitem = DatlistView1.SelectedItems[0].Tag as FshWrapper;
                    if (fshitem.Compressed)
                    {
                        compress_datmips = true;
                        compDatCb.Checked = true;
                    }

                    if (fshitem.Image != null)
                    {
                        using (MemoryStream fstream = new MemoryStream(fshitem.Image.RawData))
                        {
                            FSHImage image = new FSHImage(fstream);
                            BitmapItem tempitem = (BitmapItem)image.Bitmaps[0];
                            if (tempitem.Bitmap.Width >= 128 && tempitem.Bitmap.Height >= 128)
                            {
                                curimage = new FSHImage(fstream);
                                RefreshImageLists();
                                listViewMain.Items[0].Selected = true;
                                tabControl1.SelectedTab = Maintab;

                            }
                            switch (tempitem.BmpType)
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
                            if ((bmpitem.Bitmap.Height >= 256 && bmpitem.Bitmap.Width >= 256) && bmpitem.BmpType == FSHBmpType.ThirtyTwoBit)
                            {
                                hdfshRadio.Enabled = true;
                                hdBasetexrdo.Enabled = true;
                                hdBasetexrdo.Checked = false;
                                regFshrdo.Checked = false;
                                hdfshRadio.Checked = true;
                            }
                            else if ((bmpitem.Bitmap.Height >= 256 && bmpitem.Bitmap.Width >= 256) && bmpitem.BmpType == FSHBmpType.TwentyFourBit)
                            {
                                hdfshRadio.Enabled = true;
                                hdBasetexrdo.Enabled = true;
                                hdfshRadio.Checked = false;
                                regFshrdo.Checked = false;
                                hdBasetexrdo.Checked = true;
                            }
                            else
                            {
                                hdfshRadio.Enabled = false;
                                hdBasetexrdo.Enabled = false;
                                regFshrdo.Checked = true;
                            }
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text);
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
        /// <param name="clearloadedfsh">clear the loaded fsh</param>
        private void ClearandReset(bool clearloadedfsh)
        {
            if (dat != null)
            {
                dat.Close();
                dat = null;
                datNameTxt.Text = "No dat loaded";
            }
            if (DatlistView1.Items.Count > 0)
            {
                DatlistView1.Items.Clear();
            }
            if (clearloadedfsh)
            {
                ClearFshlists();
                mipsbtn_clicked = false;
                if (curimage != null)
                {
                    curimage = null;
                }
                if (mip64fsh != null)
                {
                    mip64fsh = null;
                }
                if (mip32fsh != null)
                {
                    mip32fsh = null;
                }
                if (mip16fsh != null)
                {
                    mip16fsh = null;
                }
                if (mip8fsh != null)
                {
                    mip8fsh = null;
                }
                if (bmpitem != null)
                {
                    bmpitem = null;
                    fshTypeBox.SelectedIndex = 2;
                    Sizelbl.Text = string.Empty;
                    dirTxt.Text = string.Empty;
                    ReloadGroupid();
                    inststr = string.Empty;
                    tgiInstanceTxt.Text = string.Empty;
                    if (origbmplist != null)
                    {
                        origbmplist.Clear();
                    }
                }
                hdfshRadio.Enabled = true;
                hdBasetexrdo.Enabled = true;
            }
        }

        private void Multifshfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dat != null)
            {
                if (dat.IsDirty)
                {
                    switch (MessageBox.Show(this, "Save changes to Dat?", "Save Dat?", MessageBoxButtons.YesNoCancel))
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
        private FSHImage GetImageFromSelectedTab(int index)
        {
            FSHImage image = null;
            switch (index)
            {
                case 0:
                    image = curimage;
                    break;
                case 1:
                    image = mip64fsh;
                    break;
                case 2:
                    image = mip32fsh;
                    break;
                case 3:
                    image = mip16fsh;
                    break;
                case 4:
                    image = mip8fsh;
                    break;
            }
            return image;
        }
        private void FshtypeBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            FSHImage image = GetImageFromSelectedTab(tabControl1.SelectedIndex);

            if (image != null && image.Bitmaps.Count > 0 && bmpitem != null && bmpitem.Bitmap != null)
            {
                if (!Checkhdimgsize(bmpitem.Bitmap))
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
        private void DatlistView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                DatlistView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (DatlistView1.Sorting == SortOrder.Ascending)
                    DatlistView1.Sorting = SortOrder.Descending;
                else
                    DatlistView1.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            DatlistView1.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.DatlistView1.ListViewItemSorter = new ListViewItemComparer(e.Column,
                                                              DatlistView1.Sorting);
        }
        private bool useorigimage = false;
        private void Fshwritecompcb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (curimage != null && curimage.Bitmaps.Count > 0 && origbmplist != null && !loadeddat && DatlistView1.Items.Count == 0)
                {
                    useorigimage = true;

                    Temp_fsh();
                    mipbtn_Click(null, null);

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
            if (!loadeddat && DatlistView1.Items.Count == 0)
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
            if (dat != null && loadeddat)
            {
                if (dat.IsDirty)
                {
                    switch (MessageBox.Show(this, "Save changes to Dat?", "Save Dat?", MessageBoxButtons.YesNo))
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
                loadeddat = false;
                SetLoadedDatEnables();

            }
        }

    }
    // Implements the manual sorting of items by columns.
    class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        private bool numsort = false;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            numsort = column == 0 ? true : false; // is the column number zero
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            if (numsort)
            {
                string xsub = ((ListViewItem)x).Text;
                string ysub = ((ListViewItem)y).Text;
                xsub = xsub.Substring(6, (xsub.Length - 6));
                ysub = ysub.Substring(6, (ysub.Length - 6));

                int numx = int.Parse(xsub);
                returnVal = numx.CompareTo(int.Parse(ysub));
            }
            else
            { 
             returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                    ((ListViewItem)y).SubItems[col].Text);
            }
           
            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
}