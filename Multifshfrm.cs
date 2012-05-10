﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FshDatIO;
using loaddatfsh.Properties;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Threading;

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

			if (Type.GetType("Mono.Runtime") == null) // skip the Windows 7 code if we are on mono 
			{
				if (TaskbarManager.IsPlatformSupported)
				{
					manager = TaskbarManager.Instance;
					manager.ApplicationId = "MultiFshTool";
				} 
			}
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
		private TaskbarManager manager;
		private JumpList jumpList;

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
					FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);
					try
					{
						bool success = false;

						using (FSHImageWrapper tempimg = new FSHImageWrapper(fs))
						{
							fs = null;
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

							if (tempitem.Bitmap.Width >= 128 || tempitem.Bitmap.Height >= 128)
							{
								curImage = tempimg.Clone();
								RefreshImageLists();
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
											if (line.StartsWith("7ab50e44", StringComparison.OrdinalIgnoreCase))
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

				if (image.IsDXTFsh() && fshWriteCompression && useOriginalImage)
				{
					BitmapEntryCollection entries = image.Bitmaps;
					int count = entries.Count;
					using (FSHImageWrapper fsh = new FSHImageWrapper())
					{
						for (int i = 0; i < count; i++)
						{
							BitmapEntry item = entries[i];

							BitmapEntry entry = new BitmapEntry();
							entry.Bitmap = origbmplist[i].Clone(PixelFormat.Format24bppRgb);
							entry.Alpha = item.Alpha.Clone<Bitmap>();
							entry.BmpType = item.BmpType;
							entry.DirName = item.DirName;

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
		/// Sets the IsDirty flag on the loaded dat if it has changed
		/// </summary>
		private void SetLoadedDatIsDirty()
		{
			if (dat != null && datListView.SelectedIndices.Count > 0)
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
						RefreshBitmapList(curImage, listViewMain, bitmapList);
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
						RefreshAlphaList(curImage, listViewMain, alphaList);
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
						RefreshBlendList(curImage, listViewMain, blendList);
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
			int count = image.Bitmaps.Count;
			for (int i = 0; i < count; i++)
			{
				ListViewItem alpha = new ListViewItem(Resources.BitmapNumberText + i.ToString(), i);
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

			int count = image.Bitmaps.Count;
			for (int i = 0; i < count; i++)
			{
				ListViewItem alpha = new ListViewItem(Resources.AlphaNumberText + i.ToString(), i);
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
			
			int count = image.Bitmaps.Count;
			for (int i = 0; i < count; i++)
			{
				ListViewItem blend = new ListViewItem(Resources.BlendNumberText + i.ToString(), i);
				listview.Items.Add(blend);
			}
		}
		private Bitmap Alphablend(BitmapEntry item, Size displaySize)
		{
			Bitmap blendbmp = new Bitmap(displaySize.Width, displaySize.Height);
			using(Graphics g = Graphics.FromImage(blendbmp))
			{
				Rectangle rect = new Rectangle(0, 0, blendbmp.Width, blendbmp.Height);
				using (HatchBrush brush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.FromArgb(192, 192, 192)))
				{
					g.FillRectangle(brush, rect);
				}
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				using (Bitmap blended = BlendBitmap.BlendBmp(bmpEntry))
				{ 
					g.DrawImage(blended, rect);
				}
			}
			return blendbmp;
		}
		private void listViewmain_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewMain.SelectedItems.Count > 0)
			{
				bmpEntry = curImage.Bitmaps[listViewMain.SelectedItems[0].Index];
				RefreshBmpType();

				sizeLbl.Text = fshSize[listViewMain.SelectedItems[0].Index];
				dirTxt.Text = dirName[listViewMain.SelectedItems[0].Index];
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
					using (FileStream fs = new FileStream(list[i],FileMode.Open, FileAccess.Read, FileShare.None))
					{
						using (Bitmap b = new Bitmap(fs))
						{
							if (b.Width < 128 && b.Height < 128)
							{
								list.Remove(list[i]);
							}
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
					int count = files.Count;
					for (int i = 0; i < count; i++)
					{
						FileInfo fi = new FileInfo(files[i]);

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
								addbmp.Bitmap = bmp.Clone(PixelFormat.Format24bppRgb);

								this.fshWriteCbGenMips = true;

								if (File.Exists(alphaPath))
								{
									Bitmap alpha = new Bitmap(alphaPath);
									addbmp.Alpha = alpha;
									if (fi.Name.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
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
								else if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
								{
									Bitmap testbmp = GetAlphafromPng(bmp);
									addbmp.Alpha = testbmp;
									if (fi.Name.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
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
									if (fi.Name.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
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
								if ((dirTxt.Text.Length > 0) && dirTxt.Text.Length == 4)
								{
									addbmp.DirName = dirTxt.Text;
								}
								else
								{
									addbmp.DirName = "FiSH";
								}

								if (bmpEntry.BmpType == FshImageFormat.ThirtyTwoBit)
								{
									hdFshRadio.Checked = true;
								}
								else if (bmpEntry.BmpType == FshImageFormat.TwentyFourBit)
								{
									hdBaseFshRadio.Checked = true;
								}
								else
								{
									regFshRadio.Checked = true;
								}


								if (tabControl1.SelectedTab == Maintab)
								{
									colorRadio.Checked = true;
									if (curImage == null)
									{
										curImage = new FSHImageWrapper();
									}
									curImage.Bitmaps.Add(addbmp);
									if (i == files.Count - 1)
									{
										Temp_fsh();
										BuildMipMaps();
										listViewMain.Items[0].Selected = true;
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

				int count = image.Bitmaps.Count;
				for (int i = 0; i < count; i++)
				{
					bmpEntry = image.Bitmaps[i];
					bmplist.Images.Add(bmpEntry.Bitmap);
					alphalist.Images.Add(bmpEntry.Alpha);
					blendlist.Images.Add(Alphablend(bmpEntry, blendlist.ImageSize));
				}

			}
			else
			{
				bmpEntry = image.Bitmaps[0];
				bmplist.Images.Add(bmpEntry.Bitmap);
				alphalist.Images.Add(bmpEntry.Alpha);
				blendlist.Images.Add(Alphablend(bmpEntry, blendlist.ImageSize));
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
			if (curImage.Bitmaps.Count > 1)
			{

				remBtn.Enabled = true;
				int count = curImage.Bitmaps.Count;
				for (int i = 0; i < count; i++)
				{
					bmpEntry = curImage.Bitmaps[i];

					bitmapList.ScaleListSize(bmpEntry.Bitmap);
					bitmapList.Images.Add(bmpEntry.Bitmap);
					alphaList.ScaleListSize(bmpEntry.Alpha);
					alphaList.Images.Add(bmpEntry.Alpha);
					blendList.ScaleListSize(bmpEntry.Bitmap);
					blendList.Images.Add(Alphablend(bmpEntry, blendList.ImageSize));
				}
				  
			}
			else
			{
				remBtn.Enabled = false;
				bmpEntry = curImage.Bitmaps[0];
				bitmapList.ScaleListSize(bmpEntry.Bitmap);
				bitmapList.Images.Add(bmpEntry.Bitmap);
				alphaList.ScaleListSize(bmpEntry.Alpha);
				alphaList.Images.Add(bmpEntry.Alpha);
				blendList.ScaleListSize(bmpEntry.Bitmap);
				blendList.Images.Add(Alphablend(bmpEntry, blendList.ImageSize));
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
						BuildMipMaps();
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

								if (Path.GetFileName(bmpFileName).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
								{
									repBmp.BmpType = FshImageFormat.ThirtyTwoBit;
									fshTypeBox.SelectedIndex = 1;
								}
								else
								{
									repBmp.BmpType = FshImageFormat.DXT3;
									fshTypeBox.SelectedIndex = 3;
								}
							}
							else if (!string.IsNullOrEmpty(alphaMap) && File.Exists(alphaMap))
							{
								using (Bitmap alpha = new Bitmap(alphaMap))
								{
									repBmp.Alpha = alpha.Clone(PixelFormat.Format24bppRgb); 
								}
								if (Path.GetFileName(bmpFileName).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
								{
									repBmp.BmpType = FshImageFormat.ThirtyTwoBit;
									fshTypeBox.SelectedIndex = 1;
								}
								else
								{
									repBmp.BmpType = FshImageFormat.DXT3;
									fshTypeBox.SelectedIndex = 3;
								}
							}
							else if (Path.GetExtension(bmpFileName).Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
							{
								repBmp.Alpha = GetAlphafromPng(bmp);
								if (Path.GetFileName(bmpFileName).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
								{
									repBmp.BmpType = FshImageFormat.ThirtyTwoBit;
									fshTypeBox.SelectedIndex = 1;
								}
								else
								{
									repBmp.BmpType = FshImageFormat.DXT3;
									fshTypeBox.SelectedIndex = 3;
								}
							}
							else
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
							BuildMipMaps();
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
		private void Temp_Mips(int mipsize)
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
		/// <summary>
		/// Creates the mip thumbnail using Graphics.DrawImage
		/// </summary>
		/// <param name="source">The Bitmap to draw</param>
		/// <param name="width">The width of the new bitmap</param>
		/// <param name="height">The height of the new bitmap</param>
		/// <returns>The new scaled Bitmap</returns>
		private Bitmap GetBitmapThumbnail(Bitmap source, int width, int height)
		{
			return SuperSample.GetBitmapThumbnail(source, width, height);
		}
		private bool mipsbtn_clicked = false;

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
				bmps[0] = GetBitmapThumbnail(item.Bitmap, 8, 8);
				bmps[1] = GetBitmapThumbnail(item.Bitmap, 16, 16);
				bmps[2] = GetBitmapThumbnail(item.Bitmap, 32, 32);
				bmps[3] = GetBitmapThumbnail(item.Bitmap, 64, 64);
				//alpha
				alphas[0] = GetBitmapThumbnail(item.Alpha, 8, 8);
				alphas[1] = GetBitmapThumbnail(item.Alpha, 16, 16);
				alphas[2] = GetBitmapThumbnail(item.Alpha, 32, 32);
				alphas[3] = GetBitmapThumbnail(item.Alpha, 64, 64);

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

						mipitm.BmpType = item.BmpType;

						switch (i)
						{
							case 0:
								mip8Fsh.Bitmaps.Add(mipitm);
								break;
							case 1:
								mip16Fsh.Bitmaps.Add(mipitm);
								break;
							case 2:
								mip32Fsh.Bitmaps.Add(mipitm);
								break;
							case 3:
								mip64Fsh.Bitmaps.Add(mipitm);
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

		private void ReloadMips()
		{
			Temp_Mips(64);
			Temp_Mips(32);
			Temp_Mips(16);
			Temp_Mips(8);
		}

		/// <summary>
		/// Builds the mip maps for the image.
		/// </summary>
		private void BuildMipMaps()
		{
			if ((curImage != null) && curImage.Bitmaps.Count >= 1)
			{
				try
				{
					mip64Fsh = null;
					mip32Fsh = null;
					mip16Fsh = null;
					mip8Fsh = null;
					foreach (BitmapEntry entry in curImage.Bitmaps)
					{
						GenerateMips(entry);
					}
					ReloadMips();

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
					fs = null;

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
					case 0 :
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

					if (!loadedDat && datListViewItems.Count == 0)
					{
						if (!mipsbtn_clicked)
						{
							BuildMipMaps();
						}
						if (mipsbtn_clicked && mip64Fsh != null && mip32Fsh != null && mip16Fsh != null && mip8Fsh != null)
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
					string bitmapnum = image.Bitmaps.Count > 1 ? "-" + listv.SelectedItems[0].Index.ToString() : string.Empty;

					if (!string.IsNullOrEmpty(fshFileName))
					{
						string name = string.Concat(fshFileName, bitmapnum, append, ".png");
						using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write))
						{
							using (Bitmap tempbmp = bmp.Clone(format))
							{
								tempbmp.Save(fs, ImageFormat.Png);
							}
						}

					}
					else if (loadedDat && datListView.SelectedIndices.Count > 0)
					{
						if (!string.IsNullOrEmpty(dat.FileName))
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

					string message = string.Format(Resources.SaveBitmap_Error_Format, suffix);
					MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
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
				bmpBox.Text = openBitmapDialog1.FileName;
				string dirpath = Path.GetDirectoryName(openBitmapDialog1.FileName);
				string filename = Path.GetFileNameWithoutExtension(openBitmapDialog1.FileName);
				string alpha = Path.Combine(dirpath, filename + "_a" + Path.GetExtension(openBitmapDialog1.FileName));
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
			}
			ClearandReset(true);

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
							bmpEntry.Bitmap = bmp.Clone(PixelFormat.Format24bppRgb);

							if (alphaBox.Text.Length > 0 && File.Exists(alphaBox.Text))
							{
								bmpEntry.Alpha = new Bitmap(alphaBox.Text);
								if (Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
								{
									bmpEntry.BmpType = FshImageFormat.ThirtyTwoBit;
									fshTypeBox.SelectedIndex = 1;
								}
								else
								{
									bmpEntry.BmpType = FshImageFormat.DXT3;
									fshTypeBox.SelectedIndex = 3;
								}
							}
							else if (!string.IsNullOrEmpty(alphaMap) && File.Exists(alphaMap))
							{
								bmpEntry.Alpha = new Bitmap(alphaMap);
								if (Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
								{
									bmpEntry.BmpType = FshImageFormat.ThirtyTwoBit;
									fshTypeBox.SelectedIndex = 1;
								}
								else
								{
									bmpEntry.BmpType = FshImageFormat.DXT3;
									fshTypeBox.SelectedIndex = 3;
								}
							}
							else if (Path.GetExtension(files[0]).Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
							{

								bmpEntry.Alpha = GetAlphafromPng(bmp);
								if (Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
								{
									bmpEntry.BmpType = FshImageFormat.ThirtyTwoBit;
									fshTypeBox.SelectedIndex = 1;
								}
								else
								{
									bmpEntry.BmpType = FshImageFormat.DXT3;
									fshTypeBox.SelectedIndex = 3;
								}
							}
							else
							{
								if (bmp != null)
								{
									bmpEntry.Alpha = GenerateAlpha(bmp);
									if (Path.GetFileName(files[0]).StartsWith("hd", StringComparison.OrdinalIgnoreCase))
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

							if (bmpEntry.BmpType == FshImageFormat.ThirtyTwoBit)
							{
								hdFshRadio.Checked = true;
							}
							else if (bmpEntry.BmpType == FshImageFormat.TwentyFourBit)
							{
								hdBaseFshRadio.Checked = true;
							}
							else
							{
								regFshRadio.Checked = true;
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
									BuildMipMaps();
									listViewMain.Items[0].Selected = true;
								}
							}
						}

						if ((files.Count - 1) > 0)
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
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
			datNameTxt.Text = Resources.NoDatLoadedText;
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
			if (!mipsbtn_clicked && !loadIsMip)
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
		private Random ra = new Random(); 
		private string lowerInstRange = string.Empty;
		private string upperInstRange = string.Empty;

		private string RandomHexString(int length)
		{
			string rangepath = Path.Combine(Application.StartupPath, @"instRange.txt");
			if (File.Exists(rangepath) && string.IsNullOrEmpty(lowerInstRange) && string.IsNullOrEmpty(upperInstRange))
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
					if (inst0.Length == 10 && inst0.ToUpperInvariant().StartsWith("0X"))
					{
						lowerInstRange = inst0.Substring(2, 8);
					}
					else if (inst0.Length == 8)
					{
						lowerInstRange = inst0;
					}
					if (inst1.Length == 10 && inst1.ToUpperInvariant().StartsWith("0X"))
					{
						upperInstRange = inst1.Substring(2, 8);
					}
					else if (inst1.Length == 8)
					{
						upperInstRange = inst1;
					}
				}

			}

			if (!string.IsNullOrEmpty(lowerInstRange) && !string.IsNullOrEmpty(upperInstRange))
			{
				long lower = long.Parse(lowerInstRange, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				long upper = long.Parse(upperInstRange, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				double rn = (upper * 1.0 - lower * 1.0) * ra.NextDouble() + lower * 1.0;

				return Convert.ToInt64(rn).ToString("X").Substring(0,7);
			}

			const string numbers = "0123456789";
			const string hexcode = "ABCDEF";
			char[] charArray = new char[length];
			string hexstring = string.Empty;

			hexstring += numbers;
			hexstring += hexcode;
				
			int index; 
			// generate a random hex string
			for (int c = 0; c < length; c++)
			{
				index = ra.Next(0, 16);
				charArray[c] = hexstring[index];
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
		}



		private DatFile dat = null;
		private bool compress_datmips = false;
		private string origInst = null;
		private bool loadedDat = false;
		private List<ListViewItem> datListViewItems = new List<ListViewItem>(); 
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Fsh")]
		private void Load_Dat(string fileName)
		{
			try
			{                
				ClearandReset(true);
				dat = new DatFile(fileName);
				this.Cursor = Cursors.WaitCursor;
				if (manager != null)
				{
					this.manager.SetProgressState(TaskbarProgressBarState.Normal);
				}

				if (!loadDatWorker.IsBusy)
				{
					loadDatWorker.RunWorkerAsync();
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
					AddRecentFile(openDatDialog1.FileName);
				}
			}
		}
		private bool datRebuilt = false;
		/// <summary>
		/// Rebuild the dat with the new items
		/// </summary>
		/// <param name="inputdat">The dat file to build</param>
		private void RebuildDat(DatFile inputdat)
		{                
			uint group = uint.Parse(tgiGroupTxt.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

			if (mipsbtn_clicked && mip64Fsh != null && mip32Fsh != null && mip16Fsh != null && mip8Fsh != null && curImage != null)
			{
				uint[] instanceIds = new uint[5];
				FshWrapper[] fshwrap = new FshWrapper[5];
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

				bool useFshWrite = this.fshWriteCompCb.Checked;
				for (int i = 4; i >= 0; i--)
				{

					fshwrap[i] = new FshWrapper(fshimg[i]) { UseFshWrite = useFshWrite };

					CheckInstance(inputdat, group, instanceIds[i]);


					inputdat.Add(fshwrap[i], group, instanceIds[i], compress_datmips);
					
				}
				datRebuilt = true;
			}
			else if (curImage != null) // the dat does not contain mipmaps
			{
				uint instance = uint.Parse(tgiInstanceTxt.Text, NumberStyles.HexNumber);

				FshWrapper wrap = new FshWrapper(curImage);

				CheckInstance(inputdat, group, instance);

				inputdat.Add(wrap, group, instance, compress_datmips);
				datRebuilt = true;
			}
		}

		private bool CheckDatForMipMaps(string group, string instance)
		{
			uint groupID = uint.Parse(group, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			uint instanceID = uint.Parse(instance.Substring(0, 7) + end64, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

			try
			{
				return !dat.CheckImageSize(groupID, instanceID); // check if the next file is a mipmap
			}
			catch (DatFileException)
			{
			} 
			
			if (instance.EndsWith("0", StringComparison.Ordinal) || instance.EndsWith("5", StringComparison.Ordinal) || 
				instance.EndsWith("A", StringComparison.OrdinalIgnoreCase))
			{
				return false; // if the instance ends with 0, 5 or A there should not be mipmaps
			}

			return true; 
		}

		/// <summary>
		/// Checks the dat for files with the same TGI id
		/// </summary>
		/// <param name="checkdat">The Dat to check</param>
		/// <param name="group">The group id to check</param>
		/// <param name="instance">The instance id to check</param>
		private void CheckInstance(DatFile checkdat, uint group, uint instance)
		{
			int count = checkdat.Indexes.Count;
			for (int n = 0; n < count; n++)
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
		private void SaveDat(string fileName)
		{
			try
			{
				dat.Save(fileName);   
				
				datNameTxt.Text = Path.GetFileName(dat.FileName);

				dat.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{

				if (!loadedDat && datListViewItems.Count == 0)
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
				datRebuilt = false;
			}
			if (compDatCb.Checked && !compress_datmips)
			{
				compress_datmips = true; // compress the dat items
			}

			if ((datListViewItems.Count > 0 && dat.IsDirty) || dat.Indexes.Count == 0)
			{
				if (!mipsbtn_clicked)
				{
					if ((loadedDat && datListViewItems.Count > 0) && !CheckDatForMipMaps(tgiGroupTxt.Text, tgiInstanceTxt.Text))
					{
						RebuildDat(dat); // the dat does not contain mipmaps for the selected file so just rebuild it
					}
					else
					{
						BuildMipMaps();
						RebuildDat(dat);
					}
				}

				if (!genNewInstCb.Checked && !datRebuilt)
				{
					if ((loadedDat && datListViewItems.Count > 0) && !CheckDatForMipMaps(tgiGroupTxt.Text, tgiInstanceTxt.Text))
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
						mipsbtn_clicked = false;
					}

					RebuildDat(dat);
				} 
			}
		   
			if (dat.Indexes.Count > 0)
			{
				if (!loadedDat && datListViewItems.Count == 0)
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
		   if (loadedDat && datListViewItems.Count > 0)
		   {
			   ClearandReset(true);
		   }
		   else
		   {
			   ClearandReset(false);
		   }
		   this.dat = new DatFile();
		   datRebuilt = false;
		   datNameTxt.Text = Resources.DatInMemoryText;       
		   SetLoadedDatEnables();
		}
		
		private void DatlistView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (datListView.SelectedIndices.Count > 0)
			{
				try
				{
					ListViewItem listItem =  datListViewItems[datListView.SelectedIndices[0]];
					string group = listItem.SubItems[1].Text;
					string instance = listItem.SubItems[2].Text;
					ClearFshlists();
					tgiGroupTxt.Text = group;
					tgiInstanceTxt.Text = instance;

					  if (!instance.EndsWith("0", StringComparison.Ordinal) || !instance.EndsWith("5", StringComparison.Ordinal) ||
						!instance.EndsWith("A", StringComparison.Ordinal))
					{
						instStr = instance.Substring(0, 7);
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
						FshWrapper item = dat.LoadFile(grp, inst);
						listItem.Tag  = item;
					}

					FshWrapper fshitem = listItem.Tag as FshWrapper;
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

						listViewMain.Items[0].Selected = true;
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
			if (datListViewItems.Count > 0)
			{               
				datListView.SelectedIndices.Clear();          
				datListView.VirtualListSize = 0;
				datListViewItems.Clear();
				loadedDat = false;
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
				remBtn.Enabled = false;
				hdFshRadio.Enabled = true;
				hdBaseFshRadio.Enabled = true;
			}
		}

		private void Multifshfrm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (loadDatWorker.IsBusy)
			{
				loadDatWorker.CancelAsync();
				e.Cancel = true;
			}
			else if (dat != null)
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

			// Set the ListViewItemSorter property to a new ListViewItemComparer
			// object.
			datListViewItems.Sort(new ListViewItemComparer(e.Column, datListView.Sorting));
			this.datListView.Refresh();
		}
		private bool useOriginalImage = false;
		/// <summary>
		/// Used to disable the fshwrite mipmap generation for loaded fsh images.
		/// </summary>
		private bool fshWriteCbGenMips;
		private void Fshwritecompcb_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (curImage != null && curImage.Bitmaps.Count > 0 && origbmplist != null && !loadedDat && datListViewItems.Count == 0)
				{
					useOriginalImage = true;

					Temp_fsh();					
					
					useOriginalImage = false; // reset it to false

					if (fshWriteCbGenMips)
					{
						BuildMipMaps();
					}

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
			if (!loadedDat && datListViewItems.Count == 0)
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
			}
		}

		private void listViewMain_DragEnter(object sender, DragEventArgs e)
		{
			newfshbtn_DragEnter(sender, e);
		}

		private void listViewMain_DragDrop(object sender, DragEventArgs e)
		{               
			List<string> files = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));
			if (loadedDat && datListViewItems.Count > 0)
			{
				AddbtnFiles(files, false);
			}
			else
			{
				NewFsh(files);
			}
		}

		private void Multifshfrm_Shown(object sender, EventArgs e)
		{
			if (manager != null)
			{
				jumpList = JumpList.CreateJumpList();
			}

			ProcessCommandLineArguments();
		}

		private void AddRecentFile(string path)
		{
			if (jumpList != null)
			{
				string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
				using (JumpListLink link = new JumpListLink(exePath, Path.GetFileName(path)))
				{
					link.Arguments = "\"" + path + "\""; // encase the path with quotes so it will work with spaces in the path
					link.IconReference = new Microsoft.WindowsAPICodePack.Shell.IconReference("shell32.dll", 0);
					link.WorkingDirectory = Path.GetDirectoryName(exePath);

					JumpListHelper.AddToRecent(link, manager.ApplicationId);
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

					int pngcnt = CountPngArgs(args);
					List<string> pnglist = null;
					for (int i = 0; i < args.Length; i++)
					{
						FileInfo fi = new FileInfo(args[i]);
						string ext = fi.Extension;
						if (fi.Exists)
						{
							if (ext.Equals(".fsh", StringComparison.OrdinalIgnoreCase) || ext.Equals(".qfs", StringComparison.OrdinalIgnoreCase))
							{
								Load_Fsh(fi.FullName);
								break; // exit the loop if a fsh or dat file has been loaded
							}
							else if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
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
							else if (ext.Equals(".dat", StringComparison.OrdinalIgnoreCase))
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

		private void datListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			if (datListViewItems.Count > 0)
			{
				e.Item = datListViewItems[e.ItemIndex];
			}
		}
		
		private void loadDatWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			int count = dat.Indexes.Count;
			int fshNum = 0;
			for (int i = 0; i < count; i++)
			{
				DatIndex index = dat.Indexes[i];

				if (loadDatWorker.CancellationPending)
				{
					e.Cancel = true;
					return;
				}

				if (index.Type == fshTypeID)
				{
					string iStr = index.Instance.ToString("X8", CultureInfo.InvariantCulture);
					if (iStr.EndsWith("4", StringComparison.Ordinal) || iStr.EndsWith("9", StringComparison.Ordinal)
						|| iStr.EndsWith("E", StringComparison.Ordinal) || iStr.EndsWith("0", StringComparison.Ordinal) ||
						iStr.EndsWith("5", StringComparison.Ordinal) || iStr.EndsWith("A", StringComparison.Ordinal))
					{
						try
						{
							if (dat.CheckImageSize(index))
							{
								fshNum++;
								ListViewItem item1 = new ListViewItem(Resources.FshNumberText + fshNum.ToString(CultureInfo.CurrentCulture));

								item1.SubItems.Add(index.Group.ToString("X8", CultureInfo.InvariantCulture));
								item1.SubItems.Add(iStr);

								datListViewItems.Add(item1);
							}
							else
							{
								continue; // skip the images that are 64x64 or smaller
							}
						}
						catch (FormatException)
						{
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("0x" + index.Instance.ToString("X8"));
#endif
							// Invalid or unsupported file, skip it
							continue;
						}
					}
				}

				if (manager != null)
				{
					loadDatWorker.ReportProgress(i, count);
				}
			}
		}

		private void loadDatWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
		{
			manager.SetProgressValue(e.ProgressPercentage, (int)e.UserState);
		}

		private void loadDatWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(this, e.Error.Message + Environment.NewLine + e.Error.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				ClearandReset(true);
			}
			else if (e.Cancelled)
			{
				this.Close();
			}
			else
			{
				if (datListViewItems.Count > 0)
				{
					datListView.VirtualListSize = datListViewItems.Count;

					loadedDat = true;
					datRebuilt = false;
					SetLoadedDatEnables();
					datListView.SelectedIndices.Add(0);
					datNameTxt.Text = Path.GetFileName(dat.FileName);
				}
				else
				{
					string message = string.Format(Resources.NoImagesInDatFileError_Format, Path.GetFileName(dat.FileName));
					MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					ClearandReset(true);
				}

				this.Cursor = Cursors.Default;
				if (manager != null)
				{
					this.manager.SetProgressState(TaskbarProgressBarState.NoProgress);
				}
			}
				
		}
	}
}