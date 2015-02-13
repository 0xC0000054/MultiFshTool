using FshDatIO;
using loaddatfsh.Properties;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace loaddatfsh
{
    internal partial class Multifshfrm : Form
    {
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
        private TaskbarManager manager;
        private JumpList jumpList;
        private bool loadIsMip;
        private BitmapCollection origbmplist;

        private Random ra;
        private Nullable<long> lowerInstRange;
        private Nullable<long> upperInstRange;

        private string instStr;
        private string groupIDOverride = null;
        private Settings settings = null;

        private char endreg;
        private char end64;
        private char end32;
        private char end16;
        private char end8;
        private bool savedFshWriteCbValue;
        private bool useOriginalImage;
        /// <summary>
        /// Used to disable the fshwrite mipmap generation for loaded fsh images.
        /// </summary>
        private bool fshWriteCbGenMips;

        private DatFile dat;
        private string origInst;
        private List<ListViewItem> datListViewItems;
        private bool mipsBuilt;
        private bool datRebuilt;
        private int sortColumn;

        /// <summary>
        /// Used to disable Fshwrite Compression on processors that do not support SSE.
        /// </summary>
        private bool fshWriteCompressionEnabled;

        private const string AlphaMapSuffix = "_a";
        private static Regex hexadecimalRegex;

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
            this.datListViewItems = new List<ListViewItem>();
            this.origbmplist = null;
            this.ra = new Random();
            this.useOriginalImage = false;
            this.mipsBuilt = false;
            this.sortColumn = -1;
            this.fshWriteCompressionEnabled = true;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (TaskbarManager.IsPlatformSupported)
                {
                    manager = TaskbarManager.Instance;
                    manager.ApplicationId = "MultiFshTool";
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
        }

        private void loadfsh_Click(object sender, EventArgs e)
        {
            if (openFshDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadFsh(openFshDialog1.FileName);
                }
                catch (FileNotFoundException fnfex)
                {
                    ShowErrorMessage(fnfex.Message);
                }
                catch (FormatException fex)
                {
                    ShowErrorMessage(fex.Message);
                }
                catch (IOException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (NotSupportedException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (SecurityException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
        }

        private void ReadTgi(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                const int Type = 0;
                const int Group = 1;
                const int Instance = 2;

                int nextLine = Type;
                string line = sr.ReadLine();
                while (line != null)
                {
                    line = line.Trim();
                    if (line.Length > 0)
                    {
                        switch (nextLine)
                        {
                            case Type:
                                if (!line.Equals("7ab50e44", StringComparison.OrdinalIgnoreCase))
                                {
                                    return;
                                }

                                nextLine = Group;
                                break;
                            case Group:
                                if (ValidateHexString(line))
                                {
                                    this.tgiGroupTxt.Text = line;
                                }
                                break;
                            case Instance:
                                if (ValidateHexString(line))
                                {
                                    this.tgiInstanceTxt.Text = line;
                                }
                                return;
                            default:
                                break;
                        }
                    }
                    line = sr.ReadLine();
                }

            }
        }

        private void LoadFsh(string fileName)
        {
            if (File.Exists(fileName))
            {
                string ext = Path.GetExtension(fileName);

                if (ext.Equals(".fsh", StringComparison.OrdinalIgnoreCase) || ext.Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    try
                    {
                        bool success = false;

                        using (FSHImageWrapper tempimg = new FSHImageWrapper(fs))
                        {
                            fs = null;
                            BitmapEntry tempitem = tempimg.Bitmaps[0];

                            fshFileName = fileName;
                            ClearandReset(true);
                            if (origbmplist == null)
                            {
                                origbmplist = new BitmapCollection();
                            }
                            
                            this.fshWriteCbGenMips = false;

                            int width = tempitem.Bitmap.Width;
                            int height = tempitem.Bitmap.Height;

                            if (width >= 128 || height >= 128)
                            {
                                curImage = tempimg.Clone();
                                RefreshImageLists();
                                success = true;
                                tabControl1.SelectedTab = Maintab;
                            }
                            else if (width == 64 || height == 64)
                            {
                                mip64Fsh = tempimg.Clone();
                                RefreshMipImageList(mip64Fsh, bmp64Mip, alpha64Mip, blend64Mip, listViewMip64);
                                loadIsMip = true;
                                success = true;
                                tabControl1.SelectedTab = mip64tab;
                            }
                            else if (width == 32 || height == 32)
                            {
                                mip32Fsh = tempimg.Clone();
                                RefreshMipImageList(mip32Fsh, bmp32Mip, alpha32Mip, blend32Mip, listViewMip32);
                                loadIsMip = true;
                                success = true;
                                tabControl1.SelectedTab = mip32tab;
                            }
                            else if (width == 16 || height == 16)
                            {
                                mip16Fsh = tempimg.Clone();
                                RefreshMipImageList(mip16Fsh, bmp16Mip, alpha16Mip, blend16Mip, listViewMip16);
                                loadIsMip = true;
                                success = true;
                                tabControl1.SelectedTab = mip16tab;
                            }
                            else if (width == 8 || height == 8)
                            {
                                mip8Fsh = tempimg.Clone();
                                RefreshMipImageList(mip8Fsh, bmp8Mip, alpha8Mip, blend8Mip, listViewMip8);
                                loadIsMip = true;
                                success = true;
                                tabControl1.SelectedTab = mip8tab;
                            }

                            if (success)
                            {
                                origbmplist.SetCapacity(tempimg.Bitmaps.Count);
                                foreach (var item in tempimg.Bitmaps)
                                {
                                    origbmplist.Add(item.Bitmap.Clone<Bitmap>());
                                }
                                RefreshBmpType();

                                // Check for a TGI file from Ilive's Reader
                                string tgiPath = fileName + ".TGI";
                                if (File.Exists(tgiPath))
                                {
                                    ReadTgi(tgiPath);
                                }
                                SetSaveButtonsEnabled(true);
                            }

                        }
                    }
                    catch (Exception)
                    {
                        throw;
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

                bool fshWriteCompression = fshWriteCompCb.Checked;

                if (embeddedMipmapsCb.Checked)
                {
                    IEnumerable<BitmapEntry> items = curImage.Bitmaps.Where(b => b.EmbeddedMipmapCount == 0);
                    foreach (var item in items)
                    {
                        item.CalculateMipmapCount();
                    }
                }

                if (image.IsDXTFsh() && fshWriteCompression && useOriginalImage)
                {
                    using (FSHImageWrapper fsh = new FSHImageWrapper())
                    {
                        BitmapEntryCollection entries = image.Bitmaps;

                        for (int i = 0; i < entries.Count; i++)
                        {
                            BitmapEntry entry = entries[i].Clone();
                            entry.Bitmap = origbmplist[i].Clone(PixelFormat.Format24bppRgb);

                            fsh.Bitmaps.Add(entry);
                        }

                        fsh.Save(fs, true);
                    }
                }
                else
                {
                    image.Save(fs, fshWriteCompression);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Save the fsh and reload it
        /// </summary>
        private void ReloadCurrentImage()
        {
            try
            {
                SetLoadedDatEnables();
                using (MemoryStream mstream = new MemoryStream())
                {
                    SaveFsh(mstream, curImage);
                    mstream.Position = 0L;
                    curImage = new FSHImageWrapper(mstream);
                }

                if ((dat != null) && datListView.SelectedIndices.Count > 0)
                {
                    dat.IsDirty = true;
                }

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
                    switch (tabControl1.SelectedIndex)
                    {
                        case 0:
                            RefreshBitmapList(curImage, listViewMain, bitmapList);
                            break;
                        case 1:
                            RefreshBitmapList(mip64Fsh, listViewMip64, bmp64Mip);
                            break;
                        case 2:
                            RefreshBitmapList(mip32Fsh, listViewMip32, bmp32Mip);
                            break;
                        case 3:
                            RefreshBitmapList(mip16Fsh, listViewMip16, bmp16Mip);
                            break;
                        case 4:
                            RefreshBitmapList(mip8Fsh, listViewMip8, bmp8Mip);
                            break;
                    }
                }
                else if (alphaRadio.Checked)
                {
                    switch (tabControl1.SelectedIndex)
                    {
                        case 0:
                            RefreshBitmapList(curImage, listViewMain, alphaList);
                            break;
                        case 1:
                            RefreshBitmapList(mip64Fsh, listViewMip64, alpha64Mip);
                            break;
                        case 2:
                            RefreshBitmapList(mip32Fsh, listViewMip32, alpha32Mip);
                            break;
                        case 3:
                            RefreshBitmapList(mip16Fsh, listViewMip16, alpha16Mip);
                            break;
                        case 4:
                            RefreshBitmapList(mip8Fsh, listViewMip8, alpha8Mip);
                            break;
                    }
                }
                else if (blendRadio.Checked)
                {
                    switch (tabControl1.SelectedIndex)
                    {
                        case 0:
                            RefreshBitmapList(curImage, listViewMain, blendList);
                            break;
                        case 1:
                            RefreshBitmapList(mip64Fsh, listViewMip64, blend64Mip);
                            break;
                        case 2:
                            RefreshBitmapList(mip32Fsh, listViewMip32, blend32Mip);
                            break;
                        case 3:
                            RefreshBitmapList(mip16Fsh, listViewMip16, blend16Mip);
                            break;
                        case 4:
                            RefreshBitmapList(mip8Fsh, listViewMip8, blend8Mip);
                            break;
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
        private static void RefreshBitmapList(FSHImageWrapper image, ListView listview, ImageList imglist)
        {
            if (listview.Items.Count > 0)
            {
                listview.Items.Clear();
            }
            listview.LargeImageList = imglist;
            listview.SmallImageList = imglist;
            int count = image.Bitmaps.Count;
            for (int i = 0; i < count; i++)
            {
                ListViewItem alpha = new ListViewItem(Resources.BitmapNumberText + i.ToString(CultureInfo.CurrentCulture), i);
                listview.Items.Add(alpha);
            }
        }

        /// <summary>
        /// Refreshes the list of alpha bitmaps
        /// </summary>
        /// <param name="image">The image to refresh the list from</param>
        /// <param name="listview">The listview to add the images to</param>
        /// <param name="imglist">The ImageList containing the alpha bitmaps to use</param>
        private static void RefreshAlphaList(FSHImageWrapper image, ListView listview, ImageList imglist)
        {
            if (listview.Items.Count > 0)
            {
                listview.Items.Clear();
            }
            listview.LargeImageList = imglist;
            listview.SmallImageList = imglist;

            int count = image.Bitmaps.Count;
            for (int i = 0; i < count; i++)
            {
                ListViewItem alpha = new ListViewItem(Resources.AlphaNumberText + i.ToString(CultureInfo.CurrentCulture), i);
                listview.Items.Add(alpha);
            }
        }
        /// <summary>
        /// Refreshes the list of blended bitmaps
        /// </summary>
        /// <param name="image">The image to refresh the list from</param>
        /// <param name="listview">The listview to add the images to</param>
        /// <param name="imglist">The ImageList containing the blended bitmaps to use</param>
        private static void RefreshBlendList(FSHImageWrapper image, ListView listview, ImageList imglist)
        {
            if (listview.Items.Count > 0)
            {
                listview.Items.Clear();
            }
            listview.LargeImageList = imglist;
            listview.SmallImageList = imglist;

            int count = image.Bitmaps.Count;
            for (int i = 0; i < count; i++)
            {
                ListViewItem blend = new ListViewItem(Resources.BlendNumberText + i.ToString(CultureInfo.CurrentCulture), i);
                listview.Items.Add(blend);
            }
        }

        private static Bitmap AlphaBlend(BitmapEntry item, Size displaySize)
        {
            Bitmap image = new Bitmap(displaySize.Width, displaySize.Height);
            using (Graphics g = Graphics.FromImage(image))
            {
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                using (HatchBrush brush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.FromArgb(192, 192, 192)))
                {
                    g.FillRectangle(brush, rect);
                }
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (Bitmap blended = BlendBitmap.BlendBmp(item))
                {
                    g.DrawImage(blended, rect);
                }
            }

            return image;
        }

        private void listViewmain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewMain.SelectedItems.Count > 0)
            {
                int index = listViewMain.SelectedItems[0].Index;
                this.bmpEntry = curImage.Bitmaps[index];
                RefreshBmpType();

                switch (bmpEntry.BmpType)
                {
                    case FshImageFormat.ThirtyTwoBit:
                        hdFshRadio.Checked = true;
                        break;
                    case FshImageFormat.TwentyFourBit:
                        hdBaseFshRadio.Checked = true;
                        break;
                    default:
                        regFshRadio.Checked = true;
                        break;
                }


                this.embeddedMipmapsCb.Checked = this.bmpEntry.EmbeddedMipmapCount > 0;

                this.sizeLbl.Text = this.fshSize[index];
                this.dirTxt.Text = this.dirName[index];
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
                if (bmp.Width >= 128 || bmp.Height >= 128)
                {
                    return true;
                }
                else
                {
                    ShowErrorMessage(Resources.CheckReplaceBitmapSizeError);
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Check the size of the files for images 64 x 64 or smaller and removes the alpha images.
        /// </summary>
        /// <param name="files">The list of files to check</param>
        /// <returns>The filtered list of files</returns>
        private static List<string> CheckSizeAndTrimAlpha(List<string> list)
        {
            list.RemoveAll(new Predicate<string>(delegate(string file)
            {
                string ext = Path.GetExtension(file);
                if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    if (Path.GetFileName(file).IndexOf("_a", StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        using (Bitmap b = new Bitmap(file))
                        {
                            if (b.Width >= 128 || b.Height >= 128)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }));

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
                    AddFilesToImage(list, false);
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

        /// <summary>
        /// Adds a list files to an a new or existing image  
        /// </summary>
        /// <param name="files">The files to add</param>
        /// <param name="listFiltered">Has the list of files been previously been filtered</param>
        private void AddFilesToImage(List<string> files, bool listFiltered)
        {
            try
            {
                if (curImage == null)
                {
                    curImage = new FSHImageWrapper();
                }

                if (tabControl1.SelectedTab != Maintab)
                {
                    tabControl1.SelectedTab = Maintab;
                }

                if (!listFiltered)
                {
                    files = CheckSizeAndTrimAlpha(files);
                }

                if (origbmplist == null)
                {
                    origbmplist = new BitmapCollection();
                }

                int lastFile = files.Count - 1;

                for (int i = 0; i < files.Count; i++)
                {
                    string file = files[i];

                    BitmapEntry addbmp = new BitmapEntry();
                    string ext = Path.GetExtension(file);
                    string alphaPath = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + AlphaMapSuffix + ext);
                    string fileName = Path.GetFileName(file);

                    using (Bitmap bmp = new Bitmap(file))
                    {
                        origbmplist.Add(bmp.Clone<Bitmap>());
                        addbmp.Bitmap = bmp.Clone(PixelFormat.Format24bppRgb);

                        this.fshWriteCbGenMips = true;

                        if (File.Exists(alphaPath))
                        {
                            Bitmap alpha = new Bitmap(alphaPath);
                            addbmp.Alpha = alpha;
                            if (fileName.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                            {
                                addbmp.BmpType = FshImageFormat.ThirtyTwoBit;
                                fshTypeBox.SelectedIndex = 1;
                            }
                            else
                            {
                                addbmp.BmpType = FshImageFormat.DXT3;
                                fshTypeBox.SelectedIndex = 3;
                            }

                        }
                        else if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            Bitmap testbmp = GetAlphafromPng(bmp);
                            addbmp.Alpha = testbmp;
                            if (fileName.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                            {
                                addbmp.BmpType = FshImageFormat.ThirtyTwoBit;
                                fshTypeBox.SelectedIndex = 1;
                            }
                            else
                            {
                                addbmp.BmpType = FshImageFormat.DXT3;
                                fshTypeBox.SelectedIndex = 3;
                            }
                        }
                        else
                        {
                            addbmp.Alpha = GenerateAlpha(bmp);
                            if (fileName.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                            {
                                addbmp.BmpType = FshImageFormat.TwentyFourBit;
                                fshTypeBox.SelectedIndex = 0;
                            }
                            else
                            {
                                addbmp.BmpType = FshImageFormat.DXT1;
                                fshTypeBox.SelectedIndex = 2;
                            }
                        }

                        if (dirTxt.Text.Length == 4)
                        {
                            addbmp.DirName = dirTxt.Text;
                        }
                        else
                        {
                            addbmp.DirName = "FiSH";
                        }

                        curImage.Bitmaps.Add(addbmp);
                        if (i == lastFile)
                        {
                            colorRadio.Checked = true;
                            ReloadCurrentImage();
                            BuildMipMaps();
                            SetSaveButtonsEnabled(true);
                            listViewMain.Items[0].Selected = true;
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (ExternalException eex)
            {
                ShowErrorMessage(eex.Message);
            }
            catch (IOException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (SecurityException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowErrorMessage(ex.Message);
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
                    AddFilesToImage(files, false);
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

            bmplist.ResetImageSize();
            alphalist.ResetImageSize();
            blendlist.ResetImageSize();

            int count = image.Bitmaps.Count;

            // Enable the remove button for standalone files with more than 1 image.
            if (loadIsMip)
            {
                this.remBtn.Enabled = count > 1;
            }

            for (int i = 0; i < count; i++)
            {
                bmpEntry = image.Bitmaps[i];
                bmplist.ScaleListSize(bmpEntry.Bitmap);
                bmplist.Images.Add(bmpEntry.Bitmap);
                alphalist.ScaleListSize(bmpEntry.Alpha);
                alphalist.Images.Add(bmpEntry.Alpha);
                blendlist.ScaleListSize(bmpEntry.Bitmap);
                blendlist.Images.Add(AlphaBlend(bmpEntry, blendlist.ImageSize));
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

        /// <summary>
        /// Refreshes the listviewMain ImageLists from the current fsh 
        /// </summary>
        private void RefreshImageLists()
        {

            int count = curImage.Bitmaps.Count;
            repBtn.Enabled = true;
            remBtn.Enabled = count > 1;

            for (int i = 0; i < count; i++)
            {
                bmpEntry = curImage.Bitmaps[i];

                bitmapList.ScaleListSize(bmpEntry.Bitmap);
                bitmapList.Images.Add(bmpEntry.Bitmap);
                alphaList.ScaleListSize(bmpEntry.Alpha);
                alphaList.Images.Add(bmpEntry.Alpha);
                blendList.ScaleListSize(bmpEntry.Bitmap);
                blendList.Images.Add(AlphaBlend(bmpEntry, blendList.ImageSize));
            }

            RefreshDirectory(curImage);

            listViewMain.BeginUpdate();
            if (colorRadio.Checked)
            {
                RefreshBitmapList(curImage, listViewMain, bitmapList);
            }
            else if (alphaRadio.Checked)
            {
                RefreshAlphaList(curImage, listViewMain, alphaList);
            }
            else if (blendRadio.Checked)
            {
                RefreshBlendList(curImage, listViewMain, blendList);
            }
            listViewMain.EndUpdate();
        }
        /// <summary>
        /// Refresh the fsh size and dir name for the input image 
        /// </summary>
        /// <param name="image">The input image</param>
        private void RefreshDirectory(FSHImageWrapper image)
        {
            int count = image.Bitmaps.Count;
            dirName = new string[count];
            fshSize = new string[count];
            for (int i = 0; i < count; i++)
            {
                FSHDirEntry dir = image.GetDirectoryEntry(i);
                EntryHeader entryhead = image.GetEntryHeader(dir.Offset);
                dirName[i] = dir.Name;
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
                    ReloadCurrentImage();
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
                        ReloadCurrentImage();
                        BuildMipMaps();
                        listViewMain.Items[0].Selected = true;
                    }
                    catch (ArgumentException ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                    catch (ExternalException ex)
                    {
                        ShowErrorMessage(ex.Message);
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
                            string fileName = openBitmapDialog1.FileName;
                            if (!Path.GetFileNameWithoutExtension(fileName).Contains(AlphaMapSuffix, StringComparison.OrdinalIgnoreCase))
                            {
                                bmp = new Bitmap(fileName);
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
                            ShowErrorMessage(Resources.repBmp_NewFileSelect_Error);
                        }

                        if (bmpLoaded)
                        {
                            if (!LoadAlphaMap(bmpFileName, bmp, ref repBmp))
                            {
                                repBmp.Alpha = GenerateAlpha(bmp);
                                if (Path.GetFileName(bmpFileName).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                {
                                    repBmp.BmpType = FshImageFormat.TwentyFourBit;
                                    fshTypeBox.SelectedIndex = 0;
                                }
                                else
                                {
                                    repBmp.BmpType = FshImageFormat.DXT1;
                                    fshTypeBox.SelectedIndex = 2;
                                }
                            }
                            
                            if (dirTxt.Text.Length == 4)
                            {
                                repBmp.DirName = dirTxt.Text;
                            }
                            else
                            {
                                repBmp.DirName = "FiSH";
                            }

                            curImage.Bitmaps.RemoveAt(listViewMain.SelectedItems[0].Index);
                            curImage.Bitmaps.Insert(listViewMain.SelectedItems[0].Index, repBmp);

                            ReloadCurrentImage();
                            BuildMipMaps();
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                    catch (ExternalException eex)
                    {
                        ShowErrorMessage(eex.Message);
                    }
                    catch (IOException ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                    catch (SecurityException ex)
                    {
                        ShowErrorMessage(ex.Message);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        ShowErrorMessage(ex.Message);
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
                    ShowErrorMessage(Resources.repBmp_NoImageSelected_Error);
                }

            }
        }

        private void hdFshRadio_CheckedChanged(object sender, EventArgs e)
        {

            if (bmpEntry != null && bmpEntry.Bitmap != null)
            {
                hdBaseFshRadio.Enabled = true;
                if (hdFshRadio.Checked)
                {
                    bmpEntry.BmpType = FshImageFormat.ThirtyTwoBit;
                    fshTypeBox.SelectedIndex = 1;
                }
                else if (hdBaseFshRadio.Checked)
                {
                    bmpEntry.BmpType = FshImageFormat.TwentyFourBit;
                    fshTypeBox.SelectedIndex = 0;
                }
                else if (regFshRadio.Checked)
                {
                    if (bmpEntry.Alpha.GetPixel(0, 0) != Color.White)
                    {
                        bmpEntry.BmpType = FshImageFormat.DXT3;
                        fshTypeBox.SelectedIndex = 3;
                    }
                    else
                    {
                        bmpEntry.BmpType = FshImageFormat.DXT1;
                        fshTypeBox.SelectedIndex = 2;
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
        private void ReloadMipImage(int mipsize)
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                switch (mipsize)
                {
                    case 64:
                        SaveFsh(mstream, mip64Fsh);
                        mstream.Position = 0L;
                        mip64Fsh = new FSHImageWrapper(mstream);
                        break;
                    case 32:
                        SaveFsh(mstream, mip32Fsh);
                        mstream.Position = 0L;
                        mip32Fsh = new FSHImageWrapper(mstream);
                        break;
                    case 16:
                        SaveFsh(mstream, mip16Fsh);
                        mstream.Position = 0L;
                        mip16Fsh = new FSHImageWrapper(mstream);
                        break;
                    case 8:
                        SaveFsh(mstream, mip8Fsh);
                        mstream.Position = 0L;
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
        /// <summary>
        /// Creates the mip thumbnail using Graphics.DrawImage
        /// </summary>
        /// <param name="source">The Bitmap to draw</param>
        /// <param name="width">The width of the new bitmap</param>
        /// <param name="height">The height of the new bitmap</param>
        /// <returns>The new scaled Bitmap</returns>
        private static Bitmap GetBitmapThumbnail(Bitmap source, int width, int height)
        {
            using (Bitmap temp = SuperSample.GetBitmapThumbnail(source, width, height))
            {
                return temp.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format24bppRgb);
            }
        }

        /// <summary>
        /// Generates the mimmaps for the zoom levels
        /// </summary>
        /// <param name="index">the index of the bitmap to scale</param>
        private void GenerateMips(BitmapEntry item)
        {

            Bitmap[] bmps = new Bitmap[4];
            Bitmap[] alphas = new Bitmap[4];

            try
            {
                // 0 = 8, 1 = 16, 2 = 32, 3 = 64
                int[] sizes = new int[4] { 8, 16, 32, 64 };
                int width = item.Bitmap.Width;
                int height = item.Bitmap.Height;

                for (int i = 0; i < 4; i++)
                {
                    int sWidth = Math.Min(sizes[i], width);
                    int sHeight = Math.Min(sizes[i], height);

                    bmps[i] = GetBitmapThumbnail(item.Bitmap, sWidth, sHeight);
                    alphas[i] = GetBitmapThumbnail(item.Alpha, sWidth, sHeight);
                }

                if (mip8Fsh == null)
                {
                    mip8Fsh = new FSHImageWrapper();
                }
                if (mip16Fsh == null)
                {
                    mip16Fsh = new FSHImageWrapper();
                }
                if (mip32Fsh == null)
                {
                    mip32Fsh = new FSHImageWrapper();
                }
                if (mip64Fsh == null)
                {
                    mip64Fsh = new FSHImageWrapper();
                }

                FshImageFormat format;
                if (item.BmpType == FshImageFormat.DXT3 || item.BmpType == FshImageFormat.ThirtyTwoBit)
                {
                    format = FshImageFormat.DXT3;
                }
                else
                {
                    format = FshImageFormat.DXT1;
                }
                string name = !string.IsNullOrEmpty(item.DirName) ? item.DirName : "FiSH";

                for (int i = 0; i < 4; i++)
                {
                    if (bmps[i] != null && alphas[i] != null)
                    {
                        switch (i)
                        {
                            case 0:
                                mip8Fsh.Bitmaps.Add(new BitmapEntry(bmps[i], alphas[i], format, name));
                                break;
                            case 1:
                                mip16Fsh.Bitmaps.Add(new BitmapEntry(bmps[i], alphas[i], format, name));
                                break;
                            case 2:
                                mip32Fsh.Bitmaps.Add(new BitmapEntry(bmps[i], alphas[i], format, name));
                                break;
                            case 3:
                                mip64Fsh.Bitmaps.Add(new BitmapEntry(bmps[i], alphas[i], format, name));
                                break;
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

        private void RefreshMips()
        {
            ReloadMipImage(64);
            ReloadMipImage(32);
            ReloadMipImage(16);
            ReloadMipImage(8);
        }

        /// <summary>
        /// Builds the mip maps for the image.
        /// </summary>
        private void BuildMipMaps()
        {
            if ((curImage != null) && curImage.Bitmaps.Count >= 1)
            {
                mip64Fsh = null;
                mip32Fsh = null;
                mip16Fsh = null;
                mip8Fsh = null;
                foreach (BitmapEntry entry in curImage.Bitmaps)
                {
                    GenerateMips(entry);
                }
                RefreshMips();

                RefreshDirectory(curImage);
                mipsBuilt = true;
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
                    fs = null;

                    sw.WriteLine("7ab50e44\t\n");
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", tgiGroupTxt.Text + "\n"));
                    switch (zoom)
                    {
                        case 0:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", tgiInstanceTxt.Text.Substring(0, 7) + end8));
                            break;
                        case 1:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", tgiInstanceTxt.Text.Substring(0, 7) + end16));
                            break;
                        case 2:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", tgiInstanceTxt.Text.Substring(0, 7) + end32));
                            break;
                        case 3:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", tgiInstanceTxt.Text.Substring(0, 7) + end64));
                            break;
                        case 4:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", tgiInstanceTxt.Text.Substring(0, 7) + endreg));
                            break;

                    }
                }
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
            if (bmpEntry != null && bmpEntry.Bitmap != null)
            {
                int selectedIndex = fshTypeBox.SelectedIndex;

                switch (selectedIndex)
                {
                    case 0:
                        hdBaseFshRadio.Checked = true;
                        break;
                    case 1:
                        hdFshRadio.Checked = true;
                        break;
                    default:
                        regFshRadio.Checked = true;
                        break;
                }

                switch (selectedIndex)
                {
                    case 0:
                        bmpEntry.BmpType = FshImageFormat.TwentyFourBit;
                        break;
                    case 1:
                        bmpEntry.BmpType = FshImageFormat.ThirtyTwoBit;
                        break;
                    case 2:
                        bmpEntry.BmpType = FshImageFormat.DXT1;
                        break;
                    case 3:
                        bmpEntry.BmpType = FshImageFormat.DXT3;
                        break;
                }
            }
        }

        private static FileStream CreateMipStream(string path, string addToPath)
        {
            return new FileStream(path + addToPath, FileMode.OpenOrCreate, FileAccess.Write);
        }

        private void saveFshBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab == Maintab)
                {
                    if (curImage != null && saveFshDialog1.ShowDialog() == DialogResult.OK)
                    {

                        curImage.IsCompressed = Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase);

                        if (datListViewItems.Count == 0 && !embeddedMipmapsCb.Checked)
                        {
                            if (!mipsBuilt)
                            {
                                BuildMipMaps();
                            }
                            if (mipsBuilt && mip64Fsh != null && mip32Fsh != null && mip16Fsh != null && mip8Fsh != null)
                            {
                                string filepath = Path.Combine(Path.GetDirectoryName(saveFshDialog1.FileName), Path.GetFileName(saveFshDialog1.FileName));
                                if (curImage.IsCompressed)
                                {
                                    mip64Fsh.IsCompressed = true;
                                    mip32Fsh.IsCompressed = true;
                                    mip16Fsh.IsCompressed = true;
                                    mip8Fsh.IsCompressed = true;
                                }
                                string ext = Path.GetExtension(saveFshDialog1.FileName);
                                using (FileStream m64 = CreateMipStream(filepath, "_s3" + ext))
                                {
                                    SaveFsh(m64, mip64Fsh);
                                }
                                WriteTgi(filepath + "_s3" + ext, 3);

                                using (FileStream m32 = CreateMipStream(filepath, "_s2" + ext))
                                {
                                    SaveFsh(m32, mip32Fsh);
                                }
                                WriteTgi(filepath + "_s2" + ext, 2);

                                using (FileStream m16 = CreateMipStream(filepath, "_s1" + ext))
                                {
                                    SaveFsh(m16, mip16Fsh);
                                }
                                WriteTgi(filepath + "_s1" + ext, 1);

                                using (FileStream m8 = CreateMipStream(filepath, "_s0" + ext))
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
                        
                        mip64Fsh.IsCompressed = Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase);
                        
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
                        mip32Fsh.IsCompressed = Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase);
                        
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
                       
                        mip16Fsh.IsCompressed = Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase);
                        
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
                        
                        mip8Fsh.IsCompressed = Path.GetExtension(saveFshDialog1.FileName).Equals(".qfs", StringComparison.OrdinalIgnoreCase);

                        WriteTgi(saveFshDialog1.FileName, 0);
                        using (FileStream fs = new FileStream(saveFshDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            SaveFsh(fs, mip8Fsh);
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (IOException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (SecurityException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }
        /// <summary>
        /// Saves the bitmap.
        /// </summary>
        /// <param name="bmp">The Bitmap  to save.</param>
        /// <param name="format">The format to save in.</param>
        /// <param name="append">The suffix to append to the file name.</param>
        private void SaveBitmap(Bitmap bmp, PixelFormat format, string append)
        {
            ListView listv = null;
            FSHImageWrapper image = null;
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
                    string bitmapnum = image.Bitmaps.Count > 1 ? "-" + listv.SelectedItems[0].Index.ToString(CultureInfo.InvariantCulture) : string.Empty;

                    if (!string.IsNullOrEmpty(fshFileName))
                    {
                        string fileName = Path.Combine(Path.GetDirectoryName(fshFileName), Path.GetFileNameWithoutExtension(fshFileName) + "_fsh");

                        string name = string.Concat(fileName, bitmapnum, append, ".png");
                        using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            using (Bitmap tempbmp = bmp.Clone(format))
                            {
                                tempbmp.Save(fs, ImageFormat.Png);
                            }
                        }
                    }
                    else if (datListView.SelectedIndices.Count > 0)
                    {
                        int index = datListView.SelectedIndices[0];
                        ListViewItem item = datListViewItems[index];
                        string fshname = Path.Combine(Path.GetDirectoryName(dat.FileName), "0x" + item.SubItems[2].Text);

                        string name = string.Concat(fshname, bitmapnum, append, ".png");

                        using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            using (Bitmap tempbmp = bmp.Clone(format))
                            {
                                tempbmp.Save(fs, ImageFormat.Png);
                            }
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (ExternalException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (IOException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (SecurityException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
            else
            {
                string suffix = string.Empty;
                switch (append)
                {
                    case "_a":
                        suffix = Resources.SaveBitmap_Alpha_Suffix;
                        break;
                    case "_blend":
                        suffix = Resources.SaveBitmap_Blended_Suffix;
                        break;
                    default:
                        suffix = Resources.SaveBitmap_Bitmap_Suffix;
                        break;
                }

                ShowErrorMessage(string.Format(CultureInfo.CurrentCulture, Resources.SaveBitmap_Error_Format, suffix));
            }
        }
        private void saveBmpBlendBtn_Click(object sender, EventArgs e)
        {
            if (bmpEntry != null && bmpEntry.Bitmap != null && bmpEntry.Alpha != null)
            {
                using (Bitmap blended = BlendBitmap.BlendBmp(bmpEntry))
                {
                    SaveBitmap(blended, PixelFormat.Format32bppArgb, "_blend");
                }
            }
        }

        private void saveBmpBtn_Click(object sender, EventArgs e)
        {
            if (bmpEntry != null && bmpEntry.Bitmap != null)
            {
                SaveBitmap(bmpEntry.Bitmap, PixelFormat.Format24bppRgb, string.Empty);
            }
        }

        private void saveAlphaBtn_Click(object sender, EventArgs e)
        {
            if (bmpEntry != null && bmpEntry.Alpha != null)
            {
                SaveBitmap(bmpEntry.Alpha, PixelFormat.Format24bppRgb, "_a");
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
                string path = openBitmapDialog1.FileName;
                bmpBox.Text = path;
                string alpha = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + AlphaMapSuffix + Path.GetExtension(path));
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

        private void NewFsh(List<string> files)
        {
            ClearandReset(true);

            curImage = new FSHImageWrapper();
            if (tabControl1.SelectedTab != Maintab)
            {
                tabControl1.SelectedTab = Maintab;
            }

            if (bmpEntry != null)
            {
                bmpEntry.Dispose();
                bmpEntry = null;
            }
            bmpEntry = new BitmapEntry();
            try
            {
                files = CheckSizeAndTrimAlpha(files);

                if (files.Count > 0)
                {
                    string file = files[0];

                    using (Bitmap bmp = new Bitmap(file))
                    {
                        bmpEntry.Bitmap = bmp.Clone(PixelFormat.Format24bppRgb);
                        
                        if (!LoadAlphaMap(file, bmp, ref bmpEntry))
                        {
                            if (bmp != null)
                            {
                                bmpEntry.Alpha = GenerateAlpha(bmp);
                                if (Path.GetFileName(file).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                                {
                                    bmpEntry.BmpType = FshImageFormat.TwentyFourBit;
                                    fshTypeBox.SelectedIndex = 0;
                                }
                                else
                                {
                                    bmpEntry.BmpType = FshImageFormat.DXT1;
                                    fshTypeBox.SelectedIndex = 2;
                                }
                            }
                        }

                        if (dirTxt.Text.Length == 4)
                        {
                            bmpEntry.DirName = dirTxt.Text;
                        }
                        else
                        {
                            bmpEntry.DirName = "FiSH";
                        }

                        string fn = Path.GetFileNameWithoutExtension(file);
                        if (fn.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!ValidateHexString(fn))
                            {
                                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidInstanceFileNameFormat, fn));
                            }

                            instStr = fn.Substring(2, 8);
                            EndFormat_Refresh();
                        }
                        else if (tgiInstanceTxt.Text.Length <= 0)
                        {
                            EndFormat_Refresh();
                        }

                        if (origbmplist == null)
                        {
                            origbmplist = new BitmapCollection(files.Count);
                        }

                        this.fshWriteCbGenMips = true;
                        origbmplist.Add(bmpEntry.Bitmap.Clone<Bitmap>()); // store the original bitmap to use if switching between fshwrite and fshlib compression

                        curImage = new FSHImageWrapper();
                        curImage.Bitmaps.Add(bmpEntry);
                        if (files.Count == 1)
                        {
                            ReloadCurrentImage();
                            BuildMipMaps();
                            SetSaveButtonsEnabled(true);
                            listViewMain.Items[0].Selected = true;
                        }
                        else
                        {
                            AddFilesToImage(files.GetRange(1, files.Count - 1), true);
                        }

                    }

                }
            }
            catch (ArgumentException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (ExternalException eex)
            {
                ShowErrorMessage(eex.Message);
            }
            catch (FileNotFoundException fnfex)
            {
                ShowErrorMessage(fnfex.Message);
            }
            catch (FormatException fex)
            {
                ShowErrorMessage(fex.Message);
            }
            catch (IOException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (SecurityException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void newfshbtn_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Copy;
            }

        }

        private void newfshbtn_DragDrop(object sender, DragEventArgs e)
        {
            List<string> list = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));
            NewFsh(list);
        }

        private void LoadSettings()
        {
            try
            {
                settings = new Settings(Path.Combine(Application.StartupPath, @"Multifshview.xml"));

                bool value;
                if (bool.TryParse(settings.GetSetting("compDatcb_checked", bool.TrueString).Trim(), out value))
                {
                    compDatCb.Checked = value;
                }

                if (bool.TryParse(settings.GetSetting("genNewInstcb_checked", bool.FalseString).Trim(), out value))
                {
                    genNewInstCb.Checked = value;
                }

                string groupOverride = settings.GetSetting("GroupidOverride", string.Empty).Trim();

                if (ValidateHexString(groupOverride))
                {
                    this.groupIDOverride = groupOverride;
                }
            }
            catch (FileNotFoundException fnfex)
            {
                ShowErrorMessage(fnfex.Message);
            }
            catch (IOException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (SecurityException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private static bool ValidateHexString(string str)
        {
            if (str != null)
            {
                if (str.Length == 8 || str.Length == 10)
                {
                    if (hexadecimalRegex == null)
                    {
                        hexadecimalRegex = new Regex("^(0x|0X)?[a-fA-F0-9]+$", RegexOptions.CultureInvariant);
                    }

                    return hexadecimalRegex.IsMatch(str);
                }
            }

            return false;
        }

        private void ReloadGroupID()
        {
            string g = string.Empty;
            if (!string.IsNullOrEmpty(groupIDOverride))
            {
                g = groupIDOverride;
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
            int height = temp.Height;
            int width = temp.Width;

            Bitmap alpha = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData data = alpha.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            try
            {
                void* scan0 = data.Scan0.ToPointer();
                int stride = data.Stride;

                for (int y = 0; y < height; y++)
                {
                    byte* p = (byte*)scan0 + (y * stride);
                    for (int x = 0; x < width; x++)
                    {
                        p[0] = p[1] = p[2] = 255;
                        p += 3;
                    }
                }
            }
            finally
            {
                alpha.UnlockBits(data);
            }

            return alpha;
        }

        /// <summary>
        /// Gets the alpha map from a 32-bit png
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The resulting alpha map
        /// </returns>
        private unsafe static Bitmap GetAlphafromPng(Bitmap source)
        {
            Bitmap dest = null;

            int width = source.Width;
            int height = source.Height;
            Bitmap temp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            try
            {
                Rectangle rect = new Rectangle(0, 0, width, height);

                BitmapData srcData = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData dstData = temp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                byte* srcScan0 = (byte*)srcData.Scan0.ToPointer();
                byte* dstScan0 = (byte*)dstData.Scan0.ToPointer();
                int srcStride = srcData.Stride;
                int dstStride = dstData.Stride;

                for (int y = 0; y < height; y++)
                {
                    byte* src = srcScan0 + (y * srcStride);
                    byte* dst = dstScan0 + (y * dstStride);
                    for (int x = 0; x < width; x++)
                    {
                        dst[0] = dst[1] = dst[2] = src[3];

                        src += 4;
                        dst += 3;
                    }
                }

                temp.UnlockBits(dstData);
                source.UnlockBits(srcData);

                dest = temp.Clone(rect, temp.PixelFormat);
            }
            finally
            {
                if (temp != null)
                {
                    temp.Dispose();
                    temp = null;
                }
            }

            return dest;
        }

        private static int CountImageArgs(string[] args)
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

        protected override void OnLoad(EventArgs e)
        {
            fshTypeBox.SelectedIndex = 2;

            base.OnLoad(e);
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == mip64tab && mip64Fsh == null)
            {
                e.Cancel = true;
            }
            else if (e.TabPage == mip32tab && mip32Fsh == null)
            {
                e.Cancel = true;
            }
            else if (e.TabPage == mip16tab && mip16Fsh == null)
            {
                e.Cancel = true;
            }
            else if (e.TabPage == mip8tab && mip8Fsh == null)
            {
                e.Cancel = true;
            }
            else if (loadIsMip && e.TabPage == Maintab)
            {
                e.Cancel = true;
            }
        }

        private void RefreshBmpType()
        {
            switch (bmpEntry.BmpType)
            {
                case FshImageFormat.TwentyFourBit:
                    fshTypeBox.SelectedIndex = 0;
                    break;
                case FshImageFormat.ThirtyTwoBit:
                    fshTypeBox.SelectedIndex = 1;
                    break;
                case FshImageFormat.DXT1:
                    fshTypeBox.SelectedIndex = 2;
                    break;
                case FshImageFormat.DXT3:
                    fshTypeBox.SelectedIndex = 3;
                    break;
            }
        }

        private void DisableManageButtons(TabPage page)
        {
            if (page != Maintab && !loadIsMip)
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

        private void DisableFshWriteCheckBox(TabPage page)
        {
            if (fshWriteCompressionEnabled)
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
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!mipsBuilt && !loadIsMip)
            {
                if (tabControl1.SelectedTab == mip64tab || tabControl1.SelectedTab == mip32tab || tabControl1.SelectedTab == mip16tab || tabControl1.SelectedTab == mip8tab)
                {
                    tabControl1.SelectedTab = Maintab;
                }
            }
            else
            {
                DisableManageButtons(tabControl1.SelectedTab);
                DisableFshWriteCheckBox(tabControl1.SelectedTab);
                if (tabControl1.SelectedTab == mip64tab && mip64Fsh != null && mip64Fsh.Bitmaps.Count > 0)
                {
                    RefreshMipImageList(mip64Fsh, bmp64Mip, alpha64Mip, blend64Mip, listViewMip64);
                    listViewMip64.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(instStr, end64);
                }
                else if (tabControl1.SelectedTab == mip32tab && mip32Fsh != null && mip32Fsh.Bitmaps.Count > 0)
                {
                    RefreshMipImageList(mip32Fsh, bmp32Mip, alpha32Mip, blend32Mip, listViewMip32);
                    bmpEntry = mip32Fsh.Bitmaps[0];
                    listViewMip32.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(instStr, end32);
                }
                else if (tabControl1.SelectedTab == mip16tab && mip16Fsh != null && mip16Fsh.Bitmaps.Count > 0)
                {
                    RefreshMipImageList(mip16Fsh, bmp16Mip, alpha16Mip, blend16Mip, listViewMip16);
                    bmpEntry = mip16Fsh.Bitmaps[0];
                    listViewMip16.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(instStr, end16);
                }
                else if (tabControl1.SelectedTab == mip8tab && mip8Fsh != null && mip8Fsh.Bitmaps.Count > 0)
                {
                    RefreshMipImageList(mip8Fsh, bmp8Mip, alpha8Mip, blend8Mip, listViewMip8);
                    bmpEntry = mip8Fsh.Bitmaps[0];
                    listViewMip8.Items[0].Selected = true;
                    RefreshBmpType();
                    tgiInstanceTxt.Text = string.Concat(instStr, end8);
                }
                else if (tabControl1.SelectedTab == Maintab && curImage != null && curImage.Bitmaps.Count > 0)
                {
                    bitmapList.ResetImageSize();
                    alphaList.ResetImageSize();
                    blendList.ResetImageSize();
                    RefreshImageLists();
                    bmpEntry = curImage.Bitmaps[0];
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
                RefreshBmpType();
                sizeLbl.Text = fshSize[listViewMip8.SelectedItems[0].Index];
                dirTxt.Text = dirName[listViewMip8.SelectedItems[0].Index];
            }
        }

        private void ReadRangeTxt(string path)
        {
            if (File.Exists(path))
            {
                string[] instArray = null;
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            instArray = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        }
                    }
                }

                if (instArray != null)
                {
                    if (instArray.Length != 2)
                    {
                        throw new FormatException(Resources.InvalidInstanceRange);
                    }

                    string inst0 = instArray[0].Trim();
                    string inst1 = instArray[1].Trim();

                    if (!ValidateHexString(inst0))
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidInstanceIdFormat, inst0));
                    }
                    if (!ValidateHexString(inst1))
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidInstanceIdFormat, inst1));
                    }

                    string lowerRange, upperRange;
                    if (inst0.Length == 10 && inst0.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        lowerRange = inst0.Substring(2, 8);
                    }
                    else
                    {
                        lowerRange = inst0;
                    }

                    if (inst1.Length == 10 && inst1.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        upperRange = inst1.Substring(2, 8);
                    }
                    else
                    {
                        upperRange = inst1;
                    }

                    long lower, upper;

                    if (long.TryParse(lowerRange, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out lower) &&
                        long.TryParse(upperRange, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out upper))
                    {
                        if (lower >= upper)
                        {
                            throw new FormatException(Resources.InvalidInstanceRange);
                        }

                        lowerInstRange = lower;
                        upperInstRange = upper;
                    }
                }
            }
        }

        private string RandomHexString(int length)
        {


            if (!lowerInstRange.HasValue && upperInstRange.HasValue)
            {
                long lower = lowerInstRange.Value;
                long upper = upperInstRange.Value;

                double rn = (upper * 1.0 - lower * 1.0) * ra.NextDouble() + lower * 1.0;

                return Convert.ToInt64(rn).ToString("X", CultureInfo.InvariantCulture).Substring(0, 7);
            }

            byte[] buffer = new byte[length / 2];
            ra.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2", CultureInfo.InvariantCulture)).ToArray());
            if (length % 2 == 0)
            {
                return result;
            }

            return result + ra.Next(16).ToString("X", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Determines whether the specified key is a hexadecimal character.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="shiftPressed">Set to <c>true</c> when the shift key is pressed.</param>
        /// <returns><c>true</c> if the specified key is a valid hexadecimal character; otherwise <c>false</c></returns>
        private static bool IsHexadecimalChar(Keys key, bool shiftPressed)
        {
            bool result = false;
            if (!shiftPressed && (key >= Keys.D0 && key <= Keys.D9 || key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                result = true;
            }
            else
            {
                switch (key)
                { 
                    case Keys.A:
                    case Keys.B:
                    case Keys.C:
                    case Keys.D:
                    case Keys.E:
                    case Keys.F:
                        result = true;
                        break;
                }
            }

            return result;
        }
        
        private void tgiGrouptxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsHexadecimalChar(e.KeyCode, e.Shift) || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
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

        private void tgiInstancetxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsHexadecimalChar(e.KeyCode, e.Shift) || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
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

        private void EndFormat_Refresh()
        {
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
            else if (instStr.Length > 7)
            {
                if (tabControl1.SelectedTab == Maintab)
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
                else if (tabControl1.SelectedTab == mip64tab)
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

                instStr = instStr.Substring(0, 7);
            }

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
            bitmapList.Images.Clear();
            bmp64Mip.Images.Clear();
            bmp32Mip.Images.Clear();
            bmp16Mip.Images.Clear();
            bmp8Mip.Images.Clear();
            alphaList.Images.Clear();
            alpha64Mip.Images.Clear();
            alpha32Mip.Images.Clear();
            alpha16Mip.Images.Clear();
            alpha8Mip.Images.Clear();
            blendList.Images.Clear();
            blend64Mip.Images.Clear();
            blend32Mip.Images.Clear();
            blend16Mip.Images.Clear();
            blend8Mip.Images.Clear();
            bitmapList.ResetImageSize();
            alphaList.ResetImageSize();
            blendList.ResetImageSize();
        }

        private void LoadDat(string fileName)
        {
            try
            {
                ClearandReset(true);
                dat = new DatFile(fileName);
                this.Cursor = Cursors.WaitCursor;
                if (this.manager != null)
                {
                    this.manager.SetProgressState(TaskbarProgressBarState.Normal);
                }
                this.toolStripStatusLabel1.Text = Resources.LoadingDatText + Path.GetFileName(fileName);

                if (!loadDatWorker.IsBusy)
                {
                    loadDatWorker.RunWorkerAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void loadDatbtn_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == Maintab)
            {
                if (openDatDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        LoadDat(openDatDialog1.FileName);
                    }
                    catch (DatHeaderException dhex)
                    {
                        ShowErrorMessage(dhex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Rebuild the dat with the new items
        /// </summary>
        /// <param name="inputdat">The dat file to build</param>
        private void RebuildDat(DatFile inputdat)
        {
            uint group = uint.Parse(tgiGroupTxt.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            if (mipsBuilt && mip64Fsh != null && mip32Fsh != null && mip16Fsh != null && mip8Fsh != null && curImage != null && !embeddedMipmapsCb.Checked)
            {
                uint[] instanceIds = new uint[5];
                FSHImageWrapper[] fshimg = new FSHImageWrapper[5];
                fshimg[0] = mip8Fsh; fshimg[1] = mip16Fsh; fshimg[2] = mip32Fsh;
                fshimg[3] = mip64Fsh; fshimg[4] = curImage;

                string subString = tgiInstanceTxt.Text.Substring(0, 7);
                instanceIds[0] = uint.Parse(subString + end8, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                instanceIds[1] = uint.Parse(subString + end16, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                instanceIds[2] = uint.Parse(subString + end32, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                instanceIds[3] = uint.Parse(subString + end64, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                instanceIds[4] = uint.Parse(subString + endreg, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                if (inputdat == null)
                {
                    dat = new DatFile();
                }

                bool compress = this.compDatCb.Checked;
                bool useFshWrite = this.fshWriteCompCb.Checked;
                for (int i = 4; i >= 0; i--)
                {
                    CheckInstance(inputdat, group, instanceIds[i]);

                    inputdat.Add(new FshFileItem(fshimg[i], useFshWrite), group, instanceIds[i], compress);
                }
                datRebuilt = true;
            }
            else if (curImage != null) // the dat does not contain mipmaps
            {
                uint instance = uint.Parse(tgiInstanceTxt.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                if (this.embeddedMipmapsCb.Checked)
                {
                    foreach (var item in curImage.Bitmaps.Where(b => b.EmbeddedMipmapCount == 0))
                    {
                        item.CalculateMipmapCount();
                    }
                }


                CheckInstance(inputdat, group, instance);

                inputdat.Add(new FshFileItem(curImage), group, instance, this.compDatCb.Checked);
                datRebuilt = true;
            }
        }

        /// <summary>
        /// Checks if the Dat contains mipmaps in separate files.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        private bool DatContainsNormalMipMaps(string group, string instance)
        {
            if (curImage.Bitmaps[0].EmbeddedMipmapCount > 0)
            {
                return false;
            }

            if (instance.EndsWith("0", StringComparison.Ordinal) || instance.EndsWith("5", StringComparison.Ordinal) || instance.EndsWith("A", StringComparison.OrdinalIgnoreCase))
            {
                return false; // if the instance ends with 0, 5 or A there should not be mipmaps
            }

            uint groupID = uint.Parse(group, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            uint instanceID = uint.Parse(instance.Substring(0, 7) + end64, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            try
            {
                return !dat.CheckImageSize(groupID, instanceID); // check if the next file is a mipmap
            }
            catch (DatIndexException)
            {
            }

            return true;
        }

        /// <summary>
        /// Checks the dat for files with the same TGI id
        /// </summary>
        /// <param name="checkdat">The Dat to check</param>
        /// <param name="group">The group id to check</param>
        /// <param name="instance">The instance id to check</param>
        private static void CheckInstance(DatFile checkdat, uint group, uint instance)
        {
            var indices = checkdat.Indexes;
            for (int i = 0; i < indices.Count; i++)
            {
                DatIndex chkindex = indices[i];
                if (chkindex.Type == fshTypeID && chkindex.Group == group && chkindex.IndexState != DatIndexState.New)
                {
                    if (chkindex.Instance == instance)
                    {
                        checkdat.Remove(group, instance);
                    }
                }
            }
        }

        private void saveDatbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (curImage != null)
                {
                    string fileName = string.Empty;
                    if (this.datListViewItems.Count > 0)
                    {
                        fileName = dat.FileName;
                    }
                    else
                    {
                        if (saveDatDialog1.ShowDialog(this) == DialogResult.OK)
                        {
                            fileName = saveDatDialog1.FileName;
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (this.dat == null)
                    {
                        this.dat = new DatFile();
                        this.datRebuilt = false;
                    }

                    if ((this.datListViewItems.Count > 0 && this.dat.IsDirty) || this.dat.Indexes.Count == 0)
                    {
                        if (!mipsBuilt)
                        {
                            if (this.datListViewItems.Count > 0 && !DatContainsNormalMipMaps(this.tgiGroupTxt.Text, this.tgiInstanceTxt.Text))
                            {
                                RebuildDat(dat); // the dat does not contain mipmaps for the selected file so just rebuild it
                            }
                            else
                            {
                                BuildMipMaps();
                                RebuildDat(dat);
                            }
                        }

                        if (!this.genNewInstCb.Checked && !this.datRebuilt)
                        {
                            if (datListViewItems.Count > 0 && !DatContainsNormalMipMaps(tgiGroupTxt.Text, tgiInstanceTxt.Text))
                            {
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
                                this.mipsBuilt = false;
                            }

                            RebuildDat(dat);
                        }
                    }

                    if (dat.Indexes.Count > 0)
                    {
                        this.toolStripStatusLabel1.Text = Resources.SavingDatText + Path.GetFileName(fileName);
                        this.statusStrip1.Refresh();

                        dat.Save(fileName);

                        if (datListViewItems.Count == 0)
                        {
                            ClearandReset(true);
                        }
                        else
                        {
                            LoadDat(dat.FileName); // reload the modified dat
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (IOException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (SecurityException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void genNewInstcb_CheckedChanged(object sender, EventArgs e)
        {
            if (dat != null && datListViewItems.Count > 0)
            {
                settings.PutSetting("genNewInstcb_checked", genNewInstCb.Checked.ToString());
                if (genNewInstCb.Checked)
                {
                    var indices = dat.Indexes;

                    for (int i = 0; i < indices.Count; i++)
                    {
                        DatIndex index = indices[i];
                        if (index.Type == fshTypeID)
                        {
                            string newInstance = null;

                            string oldInstance = tgiInstanceTxt.Text.Substring(0, 7);

                            if (index.Instance == uint.Parse(oldInstance + end8, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                newInstance = RandomHexString(7);
                                instStr = newInstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (index.Instance == uint.Parse(oldInstance + end16, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                newInstance = RandomHexString(7);
                                instStr = newInstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (index.Instance == uint.Parse(oldInstance + end32, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                newInstance = RandomHexString(7);
                                instStr = newInstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (index.Instance == uint.Parse(oldInstance + end64, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                newInstance = RandomHexString(7);
                                instStr = newInstance.Substring(0, 7);
                                EndFormat_Refresh();
                            }
                            else if (index.Instance == uint.Parse(oldInstance + endreg, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                newInstance = RandomHexString(7);
                                instStr = newInstance.Substring(0, 7);
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
            if (datListViewItems.Count > 0)
            {
                ClearandReset(true);
            }
            else
            {
                ClearandReset(false);
            }
            this.dat = new DatFile();
            datRebuilt = false;
            toolStripStatusLabel1.Text = Resources.DatInMemoryText;
            SetLoadedDatEnables();
        }

        private void datListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (datListView.SelectedIndices.Count > 0)
            {
                try
                {
                    ListViewItem listItem = datListViewItems[datListView.SelectedIndices[0]];
                    string group = listItem.SubItems[1].Text;
                    string instance = listItem.SubItems[2].Text;
                    ClearFshlists();
                    tgiGroupTxt.Text = group;
                    tgiInstanceTxt.Text = instance;

                    if (!instance.EndsWith("0", StringComparison.Ordinal) || !instance.EndsWith("5", StringComparison.Ordinal) || !instance.EndsWith("A", StringComparison.Ordinal))
                    {
                        instStr = instance.Substring(0, 7);
                        origInst = instStr;
                        if (instance.EndsWith("4", StringComparison.Ordinal))
                        {
                            inst0_4Rdo.Checked = true;
                        }
                        else if (instance.EndsWith("9", StringComparison.Ordinal))
                        {
                            inst5_9Rdo.Checked = true;
                        }
                        else if (instance.EndsWith("E", StringComparison.Ordinal))
                        {
                            instA_ERdo.Checked = true;
                        }
                    }

                    if (listItem.Tag == null)
                    {
                        uint grp = uint.Parse(group, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        uint inst = uint.Parse(instance, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        FshFileItem item = dat.LoadFile(grp, inst);
                        listItem.Tag = item;
                    }

                    FshFileItem fshitem = listItem.Tag as FshFileItem;
                    if (fshitem.Compressed)
                    {
                        compDatCb.Checked = true;
                    }

                    if (fshitem.Image != null)
                    {
                        BitmapEntry tempEntry = fshitem.Image.Bitmaps[0];

                        curImage = fshitem.Image.Clone();
                        RefreshImageLists();
                        if (tabControl1.SelectedTab != Maintab)
                        {
                            tabControl1.SelectedTab = Maintab;
                        }

                        switch (tempEntry.BmpType)
                        {
                            case FshImageFormat.TwentyFourBit:
                                fshTypeBox.SelectedIndex = 0;
                                break;
                            case FshImageFormat.ThirtyTwoBit:
                                fshTypeBox.SelectedIndex = 1;
                                break;
                            case FshImageFormat.DXT1:
                                fshTypeBox.SelectedIndex = 2;
                                break;
                            case FshImageFormat.DXT3:
                                fshTypeBox.SelectedIndex = 3;
                                break;
                        }

                        if (!saveDatBtn.Enabled)
                        {
                            SetSaveButtonsEnabled(true);
                        }
                    }

                }
                catch (DatFileException dfex)
                {
                    ShowErrorMessage(dfex.Message);
                }
                catch (DirectoryNotFoundException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (IOException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (SecurityException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
        }

        private void compDatcb_CheckedChanged(object sender, EventArgs e)
        {
            settings.PutSetting("compDatcb_checked", compDatCb.Checked.ToString());
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
                toolStripStatusLabel1.Text = Resources.StatusReadyText;
            }
            if (datListViewItems.Count > 0)
            {
                datListView.SelectedIndices.Clear();
                datListView.VirtualListSize = 0;
                datListViewItems.Clear();
                SetLoadedDatEnables();
            }
            if (clearLoadedFshFiles)
            {
                ClearFshlists();
                listViewMain.Refresh();
                listViewMip64.Refresh();
                listViewMip32.Refresh();
                listViewMip16.Refresh();
                listViewMip8.Refresh();
                mipsBuilt = false;
                this.loadIsMip = false;
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
                    EndFormat_Refresh();
                    if (origbmplist != null)
                    {
                        origbmplist.Dispose();
                        origbmplist = null;
                    }
                }
                SetSaveButtonsEnabled(false);
                repBtn.Enabled = false;
                remBtn.Enabled = false;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.loadDatWorker.IsBusy)
            {
                this.loadDatWorker.CancelAsync();
                e.Cancel = true;
            }
            else if (this.dat != null)
            {
                if (this.dat.IsDirty)
                {
                    switch (MessageBox.Show(this, Resources.SaveDatChangesText, this.Text, MessageBoxButtons.YesNoCancel))
                    {
                        case DialogResult.Yes:
                            this.dat.Save();
                            this.dat.Close();
                            break;
                        case DialogResult.No:
                            this.dat.Close();
                            break;
                        case DialogResult.Cancel:
                            e.Cancel = true;
                            break;
                    }
                }
                else
                {
                    this.dat.Close();
                }
            }

            base.OnFormClosing(e);
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

        private void datListView_ColumnClick(object sender, ColumnClickEventArgs e)
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
                {
                    datListView.Sorting = SortOrder.Descending;
                }
                else
                {
                    datListView.Sorting = SortOrder.Ascending;
                }
            }

            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            datListViewItems.Sort(new ListViewItemComparer(sortColumn, datListView.Sorting));
            this.datListView.Refresh();
        }

        private void Fshwritecompcb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.curImage != null && this.curImage.Bitmaps.Count > 0 && this.origbmplist != null && this.datListViewItems.Count == 0)
                {
                    this.useOriginalImage = true;

                    ReloadCurrentImage();

                    this.useOriginalImage = false;

                    if (this.fshWriteCbGenMips && !this.embeddedMipmapsCb.Checked)
                    {
                        BuildMipMaps();
                    }

                }
            }
            catch (ArgumentException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (ExternalException ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }
        /// <summary>
        /// Set if the close dat button and fshwrite compression checkbox are enabled 
        /// </summary>
        private void SetLoadedDatEnables()
        {
            if (datListViewItems.Count == 0)
            {
                if (fshWriteCompressionEnabled)
                {
                    fshWriteCompCb.Enabled = true;
                }
                if (closeDatBtn.Enabled)
                {
                    closeDatBtn.Enabled = false;
                }
                if (!embeddedMipmapsCb.Enabled)
                {
                    embeddedMipmapsCb.Enabled = true;
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
                if (embeddedMipmapsCb.Enabled)
                {
                    embeddedMipmapsCb.Enabled = false;
                }
            }
        }

        private void closeDatbtn_Click(object sender, EventArgs e)
        {
            if (this.dat != null && this.datListViewItems.Count > 0)
            {
                if (this.dat.IsDirty)
                {
                    switch (MessageBox.Show(this, Resources.SaveDatChangesText, this.Text, MessageBoxButtons.YesNo))
                    {
                        case DialogResult.Yes:
                            this.dat.Save();
                            this.dat.Close();
                            break;
                        case DialogResult.No:
                            this.dat.Close();
                            break;
                    }
                }
                else
                {
                    this.dat.Close();
                }
                ClearandReset(true);
            }
        }

        private void listViewMain_DragEnter(object sender, DragEventArgs e)
        {
            newfshbtn_DragEnter(sender, e);
        }

        private void listViewMain_DragDrop(object sender, DragEventArgs e)
        {
            List<string> files = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));
            if (datListViewItems.Count > 0)
            {
                AddFilesToImage(files, false);
            }
            else
            {
                if (files.Count == 1)
                {
                    string ext = Path.GetExtension(files[0]);
                    if (ext.Equals(".fsh", StringComparison.OrdinalIgnoreCase) || ext.Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                    {
                        LoadFsh(files[0]);
                        return;
                    }
                }

                NewFsh(files);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            if (manager != null)
            {
                jumpList = JumpList.CreateJumpList();
            }

            if (OS.IsMicrosoftWindows && !OS.HaveSSE)
            {
                ShowErrorMessage(Resources.FshWriteSSERequired);
                this.fshWriteCompressionEnabled = false;
                this.fshWriteCompCb.Enabled = false;
            }

            LoadSettings();

            if (tgiGroupTxt.Text.Length <= 0)
            {
                ReloadGroupID();
            }

            try
            {
                ReadRangeTxt(Path.Combine(Application.StartupPath, @"instRange.txt"));
            }
            catch (FormatException fex)
            {
                ShowErrorMessage(fex.Message);
            }

            if (tgiInstanceTxt.Text.Length <= 0)
            {
                instStr = RandomHexString(7);
                EndFormat_Refresh();
            }

            ProcessCommandLineArguments();

            base.OnShown(e);
        }

        private void AddRecentFile(string path)
        {
            if (jumpList != null)
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                using (JumpListLink link = new JumpListLink(exePath, Path.GetFileName(path)))
                {
                    link.Arguments = "\"" + path + "\""; // encase the path with quotes so it will work with spaces in the path
                    link.IconReference = OS.FileIconReference;
                    link.WorkingDirectory = Path.GetDirectoryName(exePath);

                    JumpListHelper.AddToRecent(link);
                }

                jumpList.Refresh();
            }
        }

        private void ProcessCommandLineArguments()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 0)
            {
                try
                {
                    Application.DoEvents(); // finish loading the form before we read the command line arguments

                    int imageCount = CountImageArgs(args);
                    List<string> images = null;

                    for (int i = 0; i < args.Length; i++)
                    {
                        FileInfo fi = new FileInfo(args[i]);
                        string ext = fi.Extension;
                        if (fi.Exists)
                        {
                            if (ext.Equals(".fsh", StringComparison.OrdinalIgnoreCase) || ext.Equals(".qfs", StringComparison.OrdinalIgnoreCase))
                            {
                                LoadFsh(fi.FullName);
                                break; // exit the loop if a fsh or dat file has been loaded
                            }
                            else if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                            {
                                if (images == null)
                                {
                                    images = new List<string>();
                                }
                                images.Add(fi.FullName);
                                if (images.Count == imageCount)
                                {
                                    NewFsh(images);
                                    break;
                                }
                            }
                            else if (ext.Equals(".dat", StringComparison.OrdinalIgnoreCase))
                            {
                                LoadDat(fi.FullName);
                                break;
                            }
                        }
                    }
                }
                catch (DatHeaderException dhex)
                {
                    ShowErrorMessage(dhex.Message);
                }
                catch (FileNotFoundException fnfex)
                {
                    ShowErrorMessage(fnfex.Message);
                }
                catch (FormatException fex)
                {
                    ShowErrorMessage(fex.Message);
                }
                catch (IOException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (SecurityException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
        }

        private void datListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (datListViewItems.Count > 0)
            {
                e.Item = datListViewItems[e.ItemIndex];
            }
        }

        private void loadDatWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var indices = dat.Indexes;

            base.Invoke(new MethodInvoker(delegate()
                {
                    this.toolStripProgressBar1.Visible = true;
                    this.toolStripProgressBar1.Maximum = indices.Count;
                }));

            int fshNum = 0;
            int count = indices.Count;

            for (int i = 0; i < count; i++)
            {
                DatIndex index = indices[i];

                if (loadDatWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (index.Type == fshTypeID)
                {
                    uint endDiget = index.Instance & 0xff;

                    switch (endDiget)
                    {
                        case 4:
                        case 9:
                        case 0xE:
                        case 0:
                        case 5:
                        case 0xA:
                            try
                            {
                                if (dat.CheckImageSize(index))
                                {
                                    fshNum++;
                                    ListViewItem item1 = new ListViewItem(Resources.FshNumberText + fshNum.ToString(CultureInfo.CurrentCulture));

                                    item1.SubItems.Add(index.Group.ToString("X8", CultureInfo.InvariantCulture));
                                    item1.SubItems.Add(index.Instance.ToString("X8", CultureInfo.InvariantCulture));

                                    datListViewItems.Add(item1);
                                }
                            }
                            catch (FormatException)
                            {                               
                                // Invalid or unsupported file.
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("0x" + index.Instance.ToString("X8"));
#endif
                            }
                            break;
                        default:
                            break;
                    }
                }
                
                loadDatWorker.ReportProgress(i, count);
            }
        }

        private void loadDatWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            this.toolStripProgressBar1.Value = e.ProgressPercentage;
            if (manager != null)
            {
                manager.SetProgressValue(e.ProgressPercentage, (int)e.UserState);
            }
        }

        private void loadDatWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error != null)
                {
                    ShowErrorMessage(e.Error.Message);
                    ClearandReset(true);
                }
                else
                {
                    if (this.datListViewItems.Count > 0)
                    {
                        this.datListView.VirtualListSize = this.datListViewItems.Count;

                        AddRecentFile(dat.FileName);
                        datRebuilt = false;
                        SetLoadedDatEnables();
                        datListView.SelectedIndices.Add(0);
                        toolStripStatusLabel1.Text = Resources.StatusReadyText;
                    }
                    else
                    {
                        string message = string.Format(CultureInfo.CurrentCulture, Resources.NoImagesInDatFileError_Format, Path.GetFileName(dat.FileName));
                        MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, 0);
                        ClearandReset(true);
                    }
                }

                this.Cursor = Cursors.Default;
                this.toolStripProgressBar1.Visible = false;
                this.toolStripStatusLabel1.Text = Resources.StatusReadyText;
                if (this.manager != null)
                {
                    this.manager.SetProgressState(TaskbarProgressBarState.NoProgress);
                }
            }
            else
            {
                this.Close();
            }
        }

        private void SetSaveButtonsEnabled(bool enabled)
        {
            this.saveAlphaBtn.Enabled = enabled;
            this.saveBmpBtn.Enabled = enabled;
            this.saveBmpBlendBtn.Enabled = enabled;
            if (this.tabControl1.SelectedTab == Maintab)
            {
                this.saveDatBtn.Enabled = enabled; 
            }
            else
            {
                this.saveDatBtn.Enabled = false;
            }
            this.saveFshBtn.Enabled = enabled;
        }

        /// <summary>
        /// Loads the alpha map from a 32-bit PNG or an external file.
        /// </summary>
        /// <param name="file">The path to the bitmap.</param>
        /// <param name="bmp">The Bitmap that is used for the 32-bit PNG case.</param>
        /// <param name="entry">The entry so set the alpha bitmap for.</param>
        /// <returns><c>true</c> if the alpha channel was set; otherwise, <c>false</c>.</returns>
        private bool LoadAlphaMap(string file, Bitmap bmp, ref BitmapEntry entry)
        {
            bool result = false;

            string alphaMap = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + AlphaMapSuffix + Path.GetExtension(file));

            if (File.Exists(alphaMap))
            {
                entry.Alpha = new Bitmap(alphaMap);
                if (Path.GetFileName(file).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                {
                    entry.BmpType = FshImageFormat.ThirtyTwoBit;
                    this.fshTypeBox.SelectedIndex = 1;
                }
                else
                {
                    entry.BmpType = FshImageFormat.DXT3;
                    this.fshTypeBox.SelectedIndex = 3;
                }
                result = true;
            }
            else if (alphaBox.Text.Length > 0 && File.Exists(alphaBox.Text))
            {
                entry.Alpha = new Bitmap(alphaBox.Text);
                if (Path.GetFileName(file).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                {
                    entry.BmpType = FshImageFormat.ThirtyTwoBit;
                    this.fshTypeBox.SelectedIndex = 1;
                }
                else
                {
                    entry.BmpType = FshImageFormat.DXT3;
                    this.fshTypeBox.SelectedIndex = 3;
                }
                result = true;
            }
            else if (Path.GetExtension(file).Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
            {
                entry.Alpha = GetAlphafromPng(bmp);
                if (Path.GetFileName(file).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                {
                    entry.BmpType = FshImageFormat.ThirtyTwoBit;
                    this.fshTypeBox.SelectedIndex = 1;
                }
                else
                {
                    entry.BmpType = FshImageFormat.DXT3;
                    this.fshTypeBox.SelectedIndex = 3;
                }
                result = true;
            }

            return result;
        }
    }
}