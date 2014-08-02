using loaddatfsh.Properties;
namespace loaddatfsh
{
    partial class Multifshfrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
            {
                if (dat != null)
                {
                    dat.Dispose();
                    dat = null; 
                }

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
                }

            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "qfs"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Fsh"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "diget"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "fsh")]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bitmapList = new System.Windows.Forms.ImageList(this.components);
            this.listViewMain = new System.Windows.Forms.ListView();
            this.loadFshBtn = new System.Windows.Forms.Button();
            this.blendRadio = new System.Windows.Forms.RadioButton();
            this.alphaRadio = new System.Windows.Forms.RadioButton();
            this.colorRadio = new System.Windows.Forms.RadioButton();
            this.openFshDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fshTypeBox = new System.Windows.Forms.ComboBox();
            this.alphaList = new System.Windows.Forms.ImageList(this.components);
            this.blendList = new System.Windows.Forms.ImageList(this.components);
            this.sizeLbl = new System.Windows.Forms.Label();
            this.imgSizeLbl = new System.Windows.Forms.Label();
            this.dirNameLbl = new System.Windows.Forms.Label();
            this.openBitmapDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.remBtn = new System.Windows.Forms.Button();
            this.addBtn = new System.Windows.Forms.Button();
            this.openAlphaDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.dirTxt = new System.Windows.Forms.TextBox();
            this.repBtn = new System.Windows.Forms.Button();
            this.saveFshBtn = new System.Windows.Forms.Button();
            this.saveFshDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.newFshBtn = new System.Windows.Forms.Button();
            this.bmpmanBox1 = new System.Windows.Forms.GroupBox();
            this.fshDefbox = new System.Windows.Forms.GroupBox();
            this.regFshRadio = new System.Windows.Forms.RadioButton();
            this.hdBaseFshRadio = new System.Windows.Forms.RadioButton();
            this.hdFshRadio = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Maintab = new System.Windows.Forms.TabPage();
            this.mip64tab = new System.Windows.Forms.TabPage();
            this.listViewMip64 = new System.Windows.Forms.ListView();
            this.mip32tab = new System.Windows.Forms.TabPage();
            this.listViewMip32 = new System.Windows.Forms.ListView();
            this.mip16tab = new System.Windows.Forms.TabPage();
            this.listViewMip16 = new System.Windows.Forms.ListView();
            this.bmp16Mip = new System.Windows.Forms.ImageList(this.components);
            this.mip8tab = new System.Windows.Forms.TabPage();
            this.listViewMip8 = new System.Windows.Forms.ListView();
            this.bmp8Mip = new System.Windows.Forms.ImageList(this.components);
            this.bmp64Mip = new System.Windows.Forms.ImageList(this.components);
            this.alpha64Mip = new System.Windows.Forms.ImageList(this.components);
            this.blend64Mip = new System.Windows.Forms.ImageList(this.components);
            this.bmp32Mip = new System.Windows.Forms.ImageList(this.components);
            this.alpha32Mip = new System.Windows.Forms.ImageList(this.components);
            this.blend32Mip = new System.Windows.Forms.ImageList(this.components);
            this.alpha16Mip = new System.Windows.Forms.ImageList(this.components);
            this.blend16Mip = new System.Windows.Forms.ImageList(this.components);
            this.alpha8Mip = new System.Windows.Forms.ImageList(this.components);
            this.blend8Mip = new System.Windows.Forms.ImageList(this.components);
            this.tgiGroupTxt = new System.Windows.Forms.TextBox();
            this.tgiInstanceTxt = new System.Windows.Forms.TextBox();
            this.tgiGroupLbl = new System.Windows.Forms.Label();
            this.tgiInstLbl = new System.Windows.Forms.Label();
            this.InstendBox1 = new System.Windows.Forms.GroupBox();
            this.instA_ERdo = new System.Windows.Forms.RadioButton();
            this.inst5_9Rdo = new System.Windows.Forms.RadioButton();
            this.inst0_4Rdo = new System.Windows.Forms.RadioButton();
            this.datListView = new System.Windows.Forms.ListView();
            this.NameHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.GroupHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InstanceHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loadDatBtn = new System.Windows.Forms.Button();
            this.openDatDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveDatDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.saveDatBtn = new System.Windows.Forms.Button();
            this.newDatBtn = new System.Windows.Forms.Button();
            this.DatfuncBox1 = new System.Windows.Forms.GroupBox();
            this.closeDatBtn = new System.Windows.Forms.Button();
            this.genNewInstCb = new System.Windows.Forms.CheckBox();
            this.compDatCb = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.fshWriteCompCb = new System.Windows.Forms.CheckBox();
            this.saveAlphaBtn = new System.Windows.Forms.Button();
            this.saveBmpBlendBtn = new System.Windows.Forms.Button();
            this.saveBmpBtn = new System.Windows.Forms.Button();
            this.expbmpBox1 = new System.Windows.Forms.GroupBox();
            this.alphaLbl = new System.Windows.Forms.Label();
            this.bmpLbl = new System.Windows.Forms.Label();
            this.Alphabtn = new System.Windows.Forms.Button();
            this.alphaBox = new System.Windows.Forms.TextBox();
            this.bmpbtn = new System.Windows.Forms.Button();
            this.bmpBox = new System.Windows.Forms.TextBox();
            this.loadDatWorker = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.embeddedMipmapsCb = new System.Windows.Forms.CheckBox();
            this.bmpmanBox1.SuspendLayout();
            this.fshDefbox.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.Maintab.SuspendLayout();
            this.mip64tab.SuspendLayout();
            this.mip32tab.SuspendLayout();
            this.mip16tab.SuspendLayout();
            this.mip8tab.SuspendLayout();
            this.InstendBox1.SuspendLayout();
            this.DatfuncBox1.SuspendLayout();
            this.expbmpBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bitmapList
            // 
            this.bitmapList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.bitmapList.ImageSize = new System.Drawing.Size(96, 96);
            this.bitmapList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // listViewMain
            // 
            this.listViewMain.AllowDrop = true;
            this.listViewMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMain.BackgroundImageTiled = true;
            this.listViewMain.HideSelection = false;
            this.listViewMain.LargeImageList = this.bitmapList;
            this.listViewMain.Location = new System.Drawing.Point(-7, -8);
            this.listViewMain.MultiSelect = false;
            this.listViewMain.Name = "listViewMain";
            this.listViewMain.Size = new System.Drawing.Size(523, 150);
            this.listViewMain.SmallImageList = this.bitmapList;
            this.listViewMain.TabIndex = 0;
            this.listViewMain.TileSize = new System.Drawing.Size(184, 34);
            this.listViewMain.UseCompatibleStateImageBehavior = false;
            this.listViewMain.SelectedIndexChanged += new System.EventHandler(this.listViewmain_SelectedIndexChanged);
            this.listViewMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewMain_DragDrop);
            this.listViewMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewMain_DragEnter);
            // 
            // loadFshBtn
            // 
            this.loadFshBtn.AllowDrop = true;
            this.loadFshBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadFshBtn.Location = new System.Drawing.Point(349, 571);
            this.loadFshBtn.Name = "loadFshBtn";
            this.loadFshBtn.Size = new System.Drawing.Size(75, 23);
            this.loadFshBtn.TabIndex = 1;
            this.loadFshBtn.Text = "Load fsh...";
            this.toolTip1.SetToolTip(this.loadFshBtn, global::loaddatfsh.Properties.Resources.loadFshBtn_ToolTip);
            this.loadFshBtn.UseVisualStyleBackColor = true;
            this.loadFshBtn.Click += new System.EventHandler(this.loadfsh_Click);
            // 
            // blendRadio
            // 
            this.blendRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.blendRadio.AutoSize = true;
            this.blendRadio.Location = new System.Drawing.Point(12, 367);
            this.blendRadio.Name = "blendRadio";
            this.blendRadio.Size = new System.Drawing.Size(82, 17);
            this.blendRadio.TabIndex = 45;
            this.blendRadio.TabStop = true;
            this.blendRadio.Text = "Alpha Blend";
            this.blendRadio.UseVisualStyleBackColor = true;
            this.blendRadio.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            // 
            // alphaRadio
            // 
            this.alphaRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.alphaRadio.AutoSize = true;
            this.alphaRadio.Location = new System.Drawing.Point(12, 344);
            this.alphaRadio.Name = "alphaRadio";
            this.alphaRadio.Size = new System.Drawing.Size(52, 17);
            this.alphaRadio.TabIndex = 44;
            this.alphaRadio.Text = "Alpha";
            this.alphaRadio.UseVisualStyleBackColor = true;
            this.alphaRadio.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            // 
            // colorRadio
            // 
            this.colorRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.colorRadio.AutoSize = true;
            this.colorRadio.Checked = true;
            this.colorRadio.Location = new System.Drawing.Point(12, 321);
            this.colorRadio.Name = "colorRadio";
            this.colorRadio.Size = new System.Drawing.Size(49, 17);
            this.colorRadio.TabIndex = 43;
            this.colorRadio.TabStop = true;
            this.colorRadio.Text = "Color";
            this.colorRadio.UseVisualStyleBackColor = true;
            this.colorRadio.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            // 
            // openFshDialog1
            // 
            this.openFshDialog1.Filter = global::loaddatfsh.Properties.Resources.FshFiles_Filter;
            // 
            // fshTypeBox
            // 
            this.fshTypeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fshTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fshTypeBox.FormattingEnabled = true;
            this.fshTypeBox.Items.AddRange(new object[] {
            "24 Bit RGB (0:8:8:8)",
            "32 Bit ARGB (8:8:8:8)",
            "DXT1 Compressed, no Alpha",
            "DXT3 Compressed, with Alpha"});
            this.fshTypeBox.Location = new System.Drawing.Point(12, 294);
            this.fshTypeBox.Name = "fshTypeBox";
            this.fshTypeBox.Size = new System.Drawing.Size(164, 21);
            this.fshTypeBox.TabIndex = 46;
            this.toolTip1.SetToolTip(this.fshTypeBox, global::loaddatfsh.Properties.Resources.fshTypeBox_ToolTip);
            this.fshTypeBox.SelectedIndexChanged += new System.EventHandler(this.FshtypeBox_SelectedIndexChanged);
            // 
            // alphaList
            // 
            this.alphaList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.alphaList.ImageSize = new System.Drawing.Size(96, 96);
            this.alphaList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // blendList
            // 
            this.blendList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.blendList.ImageSize = new System.Drawing.Size(96, 96);
            this.blendList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // sizeLbl
            // 
            this.sizeLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sizeLbl.AutoSize = true;
            this.sizeLbl.Location = new System.Drawing.Point(142, 325);
            this.sizeLbl.Name = "sizeLbl";
            this.sizeLbl.Size = new System.Drawing.Size(0, 13);
            this.sizeLbl.TabIndex = 48;
            // 
            // imgSizeLbl
            // 
            this.imgSizeLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.imgSizeLbl.AutoSize = true;
            this.imgSizeLbl.Location = new System.Drawing.Point(100, 325);
            this.imgSizeLbl.Name = "imgSizeLbl";
            this.imgSizeLbl.Size = new System.Drawing.Size(33, 13);
            this.imgSizeLbl.TabIndex = 47;
            this.imgSizeLbl.Text = "Size: ";
            // 
            // dirNameLbl
            // 
            this.dirNameLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dirNameLbl.AutoSize = true;
            this.dirNameLbl.Location = new System.Drawing.Point(95, 346);
            this.dirNameLbl.Name = "dirNameLbl";
            this.dirNameLbl.Size = new System.Drawing.Size(81, 13);
            this.dirNameLbl.TabIndex = 50;
            this.dirNameLbl.Text = "Directory name:";
            // 
            // openBitmapDialog1
            // 
            this.openBitmapDialog1.Filter = global::loaddatfsh.Properties.Resources.ImageFiles_Filter;
            // 
            // remBtn
            // 
            this.remBtn.Enabled = false;
            this.remBtn.Location = new System.Drawing.Point(169, 20);
            this.remBtn.Name = "remBtn";
            this.remBtn.Size = new System.Drawing.Size(75, 23);
            this.remBtn.TabIndex = 54;
            this.remBtn.Text = "Remove";
            this.toolTip1.SetToolTip(this.remBtn, global::loaddatfsh.Properties.Resources.remBtn_ToolTip);
            this.remBtn.UseVisualStyleBackColor = true;
            this.remBtn.Click += new System.EventHandler(this.remBtn_Click);
            // 
            // addBtn
            // 
            this.addBtn.AllowDrop = true;
            this.addBtn.Location = new System.Drawing.Point(7, 20);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(75, 23);
            this.addBtn.TabIndex = 53;
            this.addBtn.Text = "Add...";
            this.toolTip1.SetToolTip(this.addBtn, global::loaddatfsh.Properties.Resources.addBtn_ToolTip);
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.Click += new System.EventHandler(this.addbtn_Click);
            this.addBtn.DragDrop += new System.Windows.Forms.DragEventHandler(this.addbtn_DragDrop);
            this.addBtn.DragEnter += new System.Windows.Forms.DragEventHandler(this.addbtn_DragEnter);
            // 
            // openAlphaDialog1
            // 
            this.openAlphaDialog1.Filter = global::loaddatfsh.Properties.Resources.ImageFiles_Filter;
            // 
            // dirTxt
            // 
            this.dirTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dirTxt.Location = new System.Drawing.Point(182, 343);
            this.dirTxt.MaxLength = 4;
            this.dirTxt.Name = "dirTxt";
            this.dirTxt.Size = new System.Drawing.Size(45, 20);
            this.dirTxt.TabIndex = 57;
            this.toolTip1.SetToolTip(this.dirTxt, global::loaddatfsh.Properties.Resources.dirTxt_ToolTip);
            this.dirTxt.TextChanged += new System.EventHandler(this.dirTxt_TextChanged);
            // 
            // repBtn
            // 
            this.repBtn.Location = new System.Drawing.Point(88, 20);
            this.repBtn.Name = "repBtn";
            this.repBtn.Size = new System.Drawing.Size(75, 23);
            this.repBtn.TabIndex = 58;
            this.repBtn.Text = "Replace...";
            this.toolTip1.SetToolTip(this.repBtn, global::loaddatfsh.Properties.Resources.repBtn_ToolTip);
            this.repBtn.UseVisualStyleBackColor = true;
            this.repBtn.Click += new System.EventHandler(this.repBtn_Click);
            // 
            // saveFshBtn
            // 
            this.saveFshBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveFshBtn.Location = new System.Drawing.Point(430, 571);
            this.saveFshBtn.Name = "saveFshBtn";
            this.saveFshBtn.Size = new System.Drawing.Size(75, 23);
            this.saveFshBtn.TabIndex = 62;
            this.saveFshBtn.Text = "Save fsh...";
            this.toolTip1.SetToolTip(this.saveFshBtn, global::loaddatfsh.Properties.Resources.saveFshBtn_ToolTip);
            this.saveFshBtn.UseVisualStyleBackColor = true;
            this.saveFshBtn.Click += new System.EventHandler(this.saveFshBtn_Click);
            // 
            // saveFshDialog1
            // 
            this.saveFshDialog1.DefaultExt = "fsh";
            this.saveFshDialog1.Filter = global::loaddatfsh.Properties.Resources.FshFiles_Filter;
            // 
            // newFshBtn
            // 
            this.newFshBtn.AllowDrop = true;
            this.newFshBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newFshBtn.Location = new System.Drawing.Point(268, 571);
            this.newFshBtn.Name = "newFshBtn";
            this.newFshBtn.Size = new System.Drawing.Size(75, 23);
            this.newFshBtn.TabIndex = 66;
            this.newFshBtn.Text = "New fsh...";
            this.toolTip1.SetToolTip(this.newFshBtn, global::loaddatfsh.Properties.Resources.newFshBtn_ToolTip);
            this.newFshBtn.UseVisualStyleBackColor = true;
            this.newFshBtn.Click += new System.EventHandler(this.newFshBtn_Click);
            this.newFshBtn.DragDrop += new System.Windows.Forms.DragEventHandler(this.newfshbtn_DragDrop);
            this.newFshBtn.DragEnter += new System.Windows.Forms.DragEventHandler(this.newfshbtn_DragEnter);
            // 
            // bmpmanBox1
            // 
            this.bmpmanBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bmpmanBox1.Controls.Add(this.addBtn);
            this.bmpmanBox1.Controls.Add(this.remBtn);
            this.bmpmanBox1.Controls.Add(this.repBtn);
            this.bmpmanBox1.Location = new System.Drawing.Point(261, 392);
            this.bmpmanBox1.Name = "bmpmanBox1";
            this.bmpmanBox1.Size = new System.Drawing.Size(260, 52);
            this.bmpmanBox1.TabIndex = 67;
            this.bmpmanBox1.TabStop = false;
            this.bmpmanBox1.Text = "Manage Bitmaps ";
            // 
            // fshDefbox
            // 
            this.fshDefbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fshDefbox.Controls.Add(this.regFshRadio);
            this.fshDefbox.Controls.Add(this.hdBaseFshRadio);
            this.fshDefbox.Controls.Add(this.hdFshRadio);
            this.fshDefbox.Location = new System.Drawing.Point(3, 409);
            this.fshDefbox.Name = "fshDefbox";
            this.fshDefbox.Size = new System.Drawing.Size(130, 93);
            this.fshDefbox.TabIndex = 69;
            this.fshDefbox.TabStop = false;
            this.fshDefbox.Text = "Fsh Definition";
            // 
            // regFshRadio
            // 
            this.regFshRadio.AutoSize = true;
            this.regFshRadio.Checked = true;
            this.regFshRadio.Location = new System.Drawing.Point(7, 20);
            this.regFshRadio.Name = "regFshRadio";
            this.regFshRadio.Size = new System.Drawing.Size(79, 17);
            this.regFshRadio.TabIndex = 39;
            this.regFshRadio.TabStop = true;
            this.regFshRadio.Text = "Regular fsh";
            this.regFshRadio.UseVisualStyleBackColor = true;
            this.regFshRadio.CheckedChanged += new System.EventHandler(this.hdFshRadio_CheckedChanged);
            // 
            // hdBaseFshRadio
            // 
            this.hdBaseFshRadio.AutoSize = true;
            this.hdBaseFshRadio.Location = new System.Drawing.Point(6, 66);
            this.hdBaseFshRadio.Name = "hdBaseFshRadio";
            this.hdBaseFshRadio.Size = new System.Drawing.Size(103, 17);
            this.hdBaseFshRadio.TabIndex = 38;
            this.hdBaseFshRadio.Text = "HD Base texture";
            this.hdBaseFshRadio.UseVisualStyleBackColor = true;
            this.hdBaseFshRadio.CheckedChanged += new System.EventHandler(this.hdFshRadio_CheckedChanged);
            // 
            // hdFshRadio
            // 
            this.hdFshRadio.AutoSize = true;
            this.hdFshRadio.Location = new System.Drawing.Point(6, 42);
            this.hdFshRadio.Name = "hdFshRadio";
            this.hdFshRadio.Size = new System.Drawing.Size(111, 17);
            this.hdFshRadio.TabIndex = 37;
            this.hdFshRadio.Text = "High Definition fsh";
            this.hdFshRadio.UseVisualStyleBackColor = true;
            this.hdFshRadio.CheckedChanged += new System.EventHandler(this.hdFshRadio_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.Maintab);
            this.tabControl1.Controls.Add(this.mip64tab);
            this.tabControl1.Controls.Add(this.mip32tab);
            this.tabControl1.Controls.Add(this.mip16tab);
            this.tabControl1.Controls.Add(this.mip8tab);
            this.tabControl1.Location = new System.Drawing.Point(15, 118);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.ShowToolTips = true;
            this.tabControl1.Size = new System.Drawing.Size(523, 164);
            this.tabControl1.TabIndex = 70;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // Maintab
            // 
            this.Maintab.Controls.Add(this.listViewMain);
            this.Maintab.Location = new System.Drawing.Point(4, 22);
            this.Maintab.Name = "Maintab";
            this.Maintab.Padding = new System.Windows.Forms.Padding(3);
            this.Maintab.Size = new System.Drawing.Size(515, 138);
            this.Maintab.TabIndex = 0;
            this.Maintab.Text = "Main";
            this.Maintab.UseVisualStyleBackColor = true;
            // 
            // mip64tab
            // 
            this.mip64tab.Controls.Add(this.listViewMip64);
            this.mip64tab.Location = new System.Drawing.Point(4, 22);
            this.mip64tab.Name = "mip64tab";
            this.mip64tab.Padding = new System.Windows.Forms.Padding(3);
            this.mip64tab.Size = new System.Drawing.Size(515, 138);
            this.mip64tab.TabIndex = 1;
            this.mip64tab.Text = "64x64";
            this.mip64tab.UseVisualStyleBackColor = true;
            // 
            // listViewMip64
            // 
            this.listViewMip64.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMip64.BackgroundImageTiled = true;
            this.listViewMip64.HideSelection = false;
            this.listViewMip64.LargeImageList = this.bitmapList;
            this.listViewMip64.Location = new System.Drawing.Point(-7, -2);
            this.listViewMip64.MultiSelect = false;
            this.listViewMip64.Name = "listViewMip64";
            this.listViewMip64.Size = new System.Drawing.Size(523, 143);
            this.listViewMip64.SmallImageList = this.bitmapList;
            this.listViewMip64.TabIndex = 1;
            this.listViewMip64.TileSize = new System.Drawing.Size(184, 34);
            this.listViewMip64.UseCompatibleStateImageBehavior = false;
            this.listViewMip64.SelectedIndexChanged += new System.EventHandler(this.listViewMip64_SelectedIndexChanged);
            // 
            // mip32tab
            // 
            this.mip32tab.Controls.Add(this.listViewMip32);
            this.mip32tab.Location = new System.Drawing.Point(4, 22);
            this.mip32tab.Name = "mip32tab";
            this.mip32tab.Size = new System.Drawing.Size(515, 138);
            this.mip32tab.TabIndex = 2;
            this.mip32tab.Text = "32x32";
            this.mip32tab.UseVisualStyleBackColor = true;
            // 
            // listViewMip32
            // 
            this.listViewMip32.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMip32.BackgroundImageTiled = true;
            this.listViewMip32.HideSelection = false;
            this.listViewMip32.LargeImageList = this.bitmapList;
            this.listViewMip32.Location = new System.Drawing.Point(-4, -2);
            this.listViewMip32.MultiSelect = false;
            this.listViewMip32.Name = "listViewMip32";
            this.listViewMip32.Size = new System.Drawing.Size(523, 143);
            this.listViewMip32.SmallImageList = this.bitmapList;
            this.listViewMip32.TabIndex = 2;
            this.listViewMip32.TileSize = new System.Drawing.Size(184, 34);
            this.listViewMip32.UseCompatibleStateImageBehavior = false;
            this.listViewMip32.SelectedIndexChanged += new System.EventHandler(this.listViewMip32_SelectedIndexChanged);
            // 
            // mip16tab
            // 
            this.mip16tab.Controls.Add(this.listViewMip16);
            this.mip16tab.Location = new System.Drawing.Point(4, 22);
            this.mip16tab.Name = "mip16tab";
            this.mip16tab.Size = new System.Drawing.Size(515, 138);
            this.mip16tab.TabIndex = 3;
            this.mip16tab.Text = "16x16";
            this.mip16tab.UseVisualStyleBackColor = true;
            // 
            // listViewMip16
            // 
            this.listViewMip16.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMip16.BackgroundImageTiled = true;
            this.listViewMip16.HideSelection = false;
            this.listViewMip16.LargeImageList = this.bmp16Mip;
            this.listViewMip16.Location = new System.Drawing.Point(-4, -2);
            this.listViewMip16.MultiSelect = false;
            this.listViewMip16.Name = "listViewMip16";
            this.listViewMip16.Size = new System.Drawing.Size(523, 143);
            this.listViewMip16.SmallImageList = this.bmp16Mip;
            this.listViewMip16.TabIndex = 2;
            this.listViewMip16.TileSize = new System.Drawing.Size(184, 34);
            this.listViewMip16.UseCompatibleStateImageBehavior = false;
            this.listViewMip16.SelectedIndexChanged += new System.EventHandler(this.listViewMip16_SelectedIndexChanged);
            // 
            // bmp16Mip
            // 
            this.bmp16Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.bmp16Mip.ImageSize = new System.Drawing.Size(16, 16);
            this.bmp16Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // mip8tab
            // 
            this.mip8tab.Controls.Add(this.listViewMip8);
            this.mip8tab.Location = new System.Drawing.Point(4, 22);
            this.mip8tab.Name = "mip8tab";
            this.mip8tab.Size = new System.Drawing.Size(515, 138);
            this.mip8tab.TabIndex = 4;
            this.mip8tab.Text = "8x8";
            this.mip8tab.UseVisualStyleBackColor = true;
            // 
            // listViewMip8
            // 
            this.listViewMip8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMip8.BackgroundImageTiled = true;
            this.listViewMip8.HideSelection = false;
            this.listViewMip8.LargeImageList = this.bmp8Mip;
            this.listViewMip8.Location = new System.Drawing.Point(-4, -2);
            this.listViewMip8.MultiSelect = false;
            this.listViewMip8.Name = "listViewMip8";
            this.listViewMip8.Size = new System.Drawing.Size(523, 143);
            this.listViewMip8.SmallImageList = this.bitmapList;
            this.listViewMip8.TabIndex = 2;
            this.listViewMip8.TileSize = new System.Drawing.Size(184, 34);
            this.listViewMip8.UseCompatibleStateImageBehavior = false;
            this.listViewMip8.SelectedIndexChanged += new System.EventHandler(this.listViewMip8_SelectedIndexChanged);
            // 
            // bmp8Mip
            // 
            this.bmp8Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.bmp8Mip.ImageSize = new System.Drawing.Size(8, 8);
            this.bmp8Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // bmp64Mip
            // 
            this.bmp64Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.bmp64Mip.ImageSize = new System.Drawing.Size(64, 64);
            this.bmp64Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // alpha64Mip
            // 
            this.alpha64Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.alpha64Mip.ImageSize = new System.Drawing.Size(64, 64);
            this.alpha64Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // blend64Mip
            // 
            this.blend64Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.blend64Mip.ImageSize = new System.Drawing.Size(64, 64);
            this.blend64Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // bmp32Mip
            // 
            this.bmp32Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.bmp32Mip.ImageSize = new System.Drawing.Size(32, 32);
            this.bmp32Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // alpha32Mip
            // 
            this.alpha32Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.alpha32Mip.ImageSize = new System.Drawing.Size(32, 32);
            this.alpha32Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // blend32Mip
            // 
            this.blend32Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.blend32Mip.ImageSize = new System.Drawing.Size(32, 32);
            this.blend32Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // alpha16Mip
            // 
            this.alpha16Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.alpha16Mip.ImageSize = new System.Drawing.Size(16, 16);
            this.alpha16Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // blend16Mip
            // 
            this.blend16Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.blend16Mip.ImageSize = new System.Drawing.Size(16, 16);
            this.blend16Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // alpha8Mip
            // 
            this.alpha8Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.alpha8Mip.ImageSize = new System.Drawing.Size(8, 8);
            this.alpha8Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // blend8Mip
            // 
            this.blend8Mip.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.blend8Mip.ImageSize = new System.Drawing.Size(8, 8);
            this.blend8Mip.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tgiGroupTxt
            // 
            this.tgiGroupTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiGroupTxt.Location = new System.Drawing.Point(145, 366);
            this.tgiGroupTxt.MaxLength = 8;
            this.tgiGroupTxt.Name = "tgiGroupTxt";
            this.tgiGroupTxt.Size = new System.Drawing.Size(82, 20);
            this.tgiGroupTxt.TabIndex = 71;
            this.toolTip1.SetToolTip(this.tgiGroupTxt, global::loaddatfsh.Properties.Resources.tgiGroupTxt_ToolTip);
            this.tgiGroupTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TgiGrouptxt_KeyDown);
            // 
            // tgiInstanceTxt
            // 
            this.tgiInstanceTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiInstanceTxt.Location = new System.Drawing.Point(145, 392);
            this.tgiInstanceTxt.MaxLength = 8;
            this.tgiInstanceTxt.Name = "tgiInstanceTxt";
            this.tgiInstanceTxt.Size = new System.Drawing.Size(82, 20);
            this.tgiInstanceTxt.TabIndex = 72;
            this.toolTip1.SetToolTip(this.tgiInstanceTxt, global::loaddatfsh.Properties.Resources.tgiInstanceTxt_ToolTip);
            this.tgiInstanceTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TgiGrouptxt_KeyDown);
            // 
            // tgiGroupLbl
            // 
            this.tgiGroupLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiGroupLbl.AutoSize = true;
            this.tgiGroupLbl.Location = new System.Drawing.Point(103, 372);
            this.tgiGroupLbl.Name = "tgiGroupLbl";
            this.tgiGroupLbl.Size = new System.Drawing.Size(36, 13);
            this.tgiGroupLbl.TabIndex = 73;
            this.tgiGroupLbl.Text = "Group";
            // 
            // tgiInstLbl
            // 
            this.tgiInstLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiInstLbl.AutoSize = true;
            this.tgiInstLbl.BackColor = System.Drawing.SystemColors.Control;
            this.tgiInstLbl.Location = new System.Drawing.Point(91, 395);
            this.tgiInstLbl.Name = "tgiInstLbl";
            this.tgiInstLbl.Size = new System.Drawing.Size(48, 13);
            this.tgiInstLbl.TabIndex = 74;
            this.tgiInstLbl.Text = "Instance";
            // 
            // InstendBox1
            // 
            this.InstendBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.InstendBox1.Controls.Add(this.instA_ERdo);
            this.InstendBox1.Controls.Add(this.inst5_9Rdo);
            this.InstendBox1.Controls.Add(this.inst0_4Rdo);
            this.InstendBox1.Location = new System.Drawing.Point(145, 419);
            this.InstendBox1.Name = "InstendBox1";
            this.InstendBox1.Size = new System.Drawing.Size(82, 80);
            this.InstendBox1.TabIndex = 75;
            this.InstendBox1.TabStop = false;
            this.InstendBox1.Text = "End format";
            // 
            // instA_ERdo
            // 
            this.instA_ERdo.AutoSize = true;
            this.instA_ERdo.Location = new System.Drawing.Point(6, 55);
            this.instA_ERdo.Name = "instA_ERdo";
            this.instA_ERdo.Size = new System.Drawing.Size(42, 17);
            this.instA_ERdo.TabIndex = 2;
            this.instA_ERdo.Text = "A-E";
            this.toolTip1.SetToolTip(this.instA_ERdo, global::loaddatfsh.Properties.Resources.InstanceIDRadioButtons_ToolTip);
            this.instA_ERdo.UseVisualStyleBackColor = true;
            this.instA_ERdo.CheckedChanged += new System.EventHandler(this.EndFormat_CheckedChanged);
            // 
            // inst5_9Rdo
            // 
            this.inst5_9Rdo.AutoSize = true;
            this.inst5_9Rdo.Location = new System.Drawing.Point(6, 36);
            this.inst5_9Rdo.Name = "inst5_9Rdo";
            this.inst5_9Rdo.Size = new System.Drawing.Size(40, 17);
            this.inst5_9Rdo.TabIndex = 1;
            this.inst5_9Rdo.Text = "5-9";
            this.toolTip1.SetToolTip(this.inst5_9Rdo, global::loaddatfsh.Properties.Resources.InstanceIDRadioButtons_ToolTip);
            this.inst5_9Rdo.UseVisualStyleBackColor = true;
            // 
            // inst0_4Rdo
            // 
            this.inst0_4Rdo.AutoSize = true;
            this.inst0_4Rdo.Checked = true;
            this.inst0_4Rdo.Location = new System.Drawing.Point(6, 19);
            this.inst0_4Rdo.Name = "inst0_4Rdo";
            this.inst0_4Rdo.Size = new System.Drawing.Size(40, 17);
            this.inst0_4Rdo.TabIndex = 0;
            this.inst0_4Rdo.TabStop = true;
            this.inst0_4Rdo.Text = "0-4";
            this.toolTip1.SetToolTip(this.inst0_4Rdo, global::loaddatfsh.Properties.Resources.InstanceIDRadioButtons_ToolTip);
            this.inst0_4Rdo.UseVisualStyleBackColor = true;
            this.inst0_4Rdo.CheckedChanged += new System.EventHandler(this.EndFormat_CheckedChanged);
            // 
            // datListView
            // 
            this.datListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameHeader1,
            this.GroupHeader,
            this.InstanceHeader});
            this.datListView.FullRowSelect = true;
            this.datListView.HideSelection = false;
            this.datListView.Location = new System.Drawing.Point(15, 12);
            this.datListView.MultiSelect = false;
            this.datListView.Name = "datListView";
            this.datListView.Size = new System.Drawing.Size(523, 100);
            this.datListView.TabIndex = 76;
            this.toolTip1.SetToolTip(this.datListView, global::loaddatfsh.Properties.Resources.datListView_ToolTip);
            this.datListView.UseCompatibleStateImageBehavior = false;
            this.datListView.View = System.Windows.Forms.View.Details;
            this.datListView.VirtualMode = true;
            this.datListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.datListView_ColumnClick);
            this.datListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.datListView_RetrieveVirtualItem);
            this.datListView.SelectedIndexChanged += new System.EventHandler(this.datListView_SelectedIndexChanged);
            // 
            // NameHeader1
            // 
            this.NameHeader1.Text = "Fsh #";
            this.NameHeader1.Width = 92;
            // 
            // GroupHeader
            // 
            this.GroupHeader.Text = "Group";
            this.GroupHeader.Width = 155;
            // 
            // InstanceHeader
            // 
            this.InstanceHeader.Text = "Instance";
            this.InstanceHeader.Width = 140;
            // 
            // loadDatBtn
            // 
            this.loadDatBtn.Location = new System.Drawing.Point(93, 42);
            this.loadDatBtn.Name = "loadDatBtn";
            this.loadDatBtn.Size = new System.Drawing.Size(75, 23);
            this.loadDatBtn.TabIndex = 77;
            this.loadDatBtn.Text = "Load...";
            this.toolTip1.SetToolTip(this.loadDatBtn, global::loaddatfsh.Properties.Resources.loadDatBtn_ToolTip);
            this.loadDatBtn.UseVisualStyleBackColor = true;
            this.loadDatBtn.Click += new System.EventHandler(this.loadDatbtn_Click);
            // 
            // openDatDialog1
            // 
            this.openDatDialog1.Filter = global::loaddatfsh.Properties.Resources.DatFiles_Filter;
            // 
            // saveDatDialog1
            // 
            this.saveDatDialog1.DefaultExt = "dat";
            this.saveDatDialog1.Filter = global::loaddatfsh.Properties.Resources.DatFiles_Filter;
            // 
            // saveDatBtn
            // 
            this.saveDatBtn.Location = new System.Drawing.Point(174, 42);
            this.saveDatBtn.Name = "saveDatBtn";
            this.saveDatBtn.Size = new System.Drawing.Size(75, 23);
            this.saveDatBtn.TabIndex = 78;
            this.saveDatBtn.Text = "Save...";
            this.toolTip1.SetToolTip(this.saveDatBtn, global::loaddatfsh.Properties.Resources.saveDatBtn_ToolTip);
            this.saveDatBtn.UseVisualStyleBackColor = true;
            this.saveDatBtn.Click += new System.EventHandler(this.saveDatbtn_Click);
            // 
            // newDatBtn
            // 
            this.newDatBtn.Location = new System.Drawing.Point(12, 42);
            this.newDatBtn.Name = "newDatBtn";
            this.newDatBtn.Size = new System.Drawing.Size(75, 23);
            this.newDatBtn.TabIndex = 81;
            this.newDatBtn.Text = "New";
            this.toolTip1.SetToolTip(this.newDatBtn, global::loaddatfsh.Properties.Resources.newDatBtn_ToolTip);
            this.newDatBtn.UseVisualStyleBackColor = true;
            this.newDatBtn.Click += new System.EventHandler(this.newDatbtn_Click);
            // 
            // DatfuncBox1
            // 
            this.DatfuncBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DatfuncBox1.Controls.Add(this.closeDatBtn);
            this.DatfuncBox1.Controls.Add(this.newDatBtn);
            this.DatfuncBox1.Controls.Add(this.loadDatBtn);
            this.DatfuncBox1.Controls.Add(this.saveDatBtn);
            this.DatfuncBox1.Location = new System.Drawing.Point(256, 456);
            this.DatfuncBox1.Name = "DatfuncBox1";
            this.DatfuncBox1.Size = new System.Drawing.Size(265, 75);
            this.DatfuncBox1.TabIndex = 82;
            this.DatfuncBox1.TabStop = false;
            this.DatfuncBox1.Text = "Dat Functions";
            // 
            // closeDatBtn
            // 
            this.closeDatBtn.Enabled = false;
            this.closeDatBtn.Location = new System.Drawing.Point(174, 13);
            this.closeDatBtn.Name = "closeDatBtn";
            this.closeDatBtn.Size = new System.Drawing.Size(75, 23);
            this.closeDatBtn.TabIndex = 84;
            this.closeDatBtn.Text = "Close";
            this.toolTip1.SetToolTip(this.closeDatBtn, global::loaddatfsh.Properties.Resources.closeDatBtn_ToolTip);
            this.closeDatBtn.UseVisualStyleBackColor = true;
            this.closeDatBtn.Click += new System.EventHandler(this.closeDatbtn_Click);
            // 
            // genNewInstCb
            // 
            this.genNewInstCb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.genNewInstCb.AutoSize = true;
            this.genNewInstCb.Location = new System.Drawing.Point(3, 508);
            this.genNewInstCb.Name = "genNewInstCb";
            this.genNewInstCb.Size = new System.Drawing.Size(141, 17);
            this.genNewInstCb.TabIndex = 80;
            this.genNewInstCb.Text = "Generate new instances";
            this.toolTip1.SetToolTip(this.genNewInstCb, global::loaddatfsh.Properties.Resources.genNewInstCb_ToolTip);
            this.genNewInstCb.UseVisualStyleBackColor = true;
            this.genNewInstCb.CheckedChanged += new System.EventHandler(this.genNewInstcb_CheckedChanged);
            // 
            // compDatCb
            // 
            this.compDatCb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.compDatCb.AutoSize = true;
            this.compDatCb.Checked = true;
            this.compDatCb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.compDatCb.Location = new System.Drawing.Point(3, 531);
            this.compDatCb.Name = "compDatCb";
            this.compDatCb.Size = new System.Drawing.Size(120, 17);
            this.compDatCb.TabIndex = 84;
            this.compDatCb.Text = "Compress dat items ";
            this.toolTip1.SetToolTip(this.compDatCb, global::loaddatfsh.Properties.Resources.compDatCb_ToolTip);
            this.compDatCb.UseVisualStyleBackColor = true;
            this.compDatCb.CheckedChanged += new System.EventHandler(this.compDatcb_CheckedChanged);
            // 
            // fshWriteCompCb
            // 
            this.fshWriteCompCb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fshWriteCompCb.AutoSize = true;
            this.fshWriteCompCb.Location = new System.Drawing.Point(3, 554);
            this.fshWriteCompCb.Name = "fshWriteCompCb";
            this.fshWriteCompCb.Size = new System.Drawing.Size(127, 17);
            this.fshWriteCompCb.TabIndex = 91;
            this.fshWriteCompCb.Text = "Fshwrite compression";
            this.toolTip1.SetToolTip(this.fshWriteCompCb, global::loaddatfsh.Properties.Resources.fshWriteCompCb_ToolTip);
            this.fshWriteCompCb.UseVisualStyleBackColor = true;
            this.fshWriteCompCb.CheckedChanged += new System.EventHandler(this.Fshwritecompcb_CheckedChanged);
            // 
            // saveAlphaBtn
            // 
            this.saveAlphaBtn.Location = new System.Drawing.Point(87, 18);
            this.saveAlphaBtn.Name = "saveAlphaBtn";
            this.saveAlphaBtn.Size = new System.Drawing.Size(75, 23);
            this.saveAlphaBtn.TabIndex = 65;
            this.saveAlphaBtn.Text = "Alpha";
            this.saveAlphaBtn.UseVisualStyleBackColor = true;
            this.saveAlphaBtn.Click += new System.EventHandler(this.saveAlphaBtn_Click);
            // 
            // saveBmpBlendBtn
            // 
            this.saveBmpBlendBtn.Location = new System.Drawing.Point(168, 18);
            this.saveBmpBlendBtn.Name = "saveBmpBlendBtn";
            this.saveBmpBlendBtn.Size = new System.Drawing.Size(106, 23);
            this.saveBmpBlendBtn.TabIndex = 63;
            this.saveBmpBlendBtn.Text = "Blended Bitmap ";
            this.saveBmpBlendBtn.UseVisualStyleBackColor = true;
            this.saveBmpBlendBtn.Click += new System.EventHandler(this.saveBmpBlendBtn_Click);
            // 
            // saveBmpBtn
            // 
            this.saveBmpBtn.Location = new System.Drawing.Point(6, 18);
            this.saveBmpBtn.Name = "saveBmpBtn";
            this.saveBmpBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBmpBtn.TabIndex = 64;
            this.saveBmpBtn.Text = "Bitmap";
            this.saveBmpBtn.UseVisualStyleBackColor = true;
            this.saveBmpBtn.Click += new System.EventHandler(this.saveBmpBtn_Click);
            // 
            // expbmpBox1
            // 
            this.expbmpBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.expbmpBox1.Controls.Add(this.saveBmpBtn);
            this.expbmpBox1.Controls.Add(this.saveBmpBlendBtn);
            this.expbmpBox1.Controls.Add(this.saveAlphaBtn);
            this.expbmpBox1.Location = new System.Drawing.Point(256, 336);
            this.expbmpBox1.Name = "expbmpBox1";
            this.expbmpBox1.Size = new System.Drawing.Size(278, 50);
            this.expbmpBox1.TabIndex = 68;
            this.expbmpBox1.TabStop = false;
            this.expbmpBox1.Text = "Export bitmap";
            // 
            // alphaLbl
            // 
            this.alphaLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaLbl.AutoSize = true;
            this.alphaLbl.Location = new System.Drawing.Point(230, 315);
            this.alphaLbl.Name = "alphaLbl";
            this.alphaLbl.Size = new System.Drawing.Size(46, 13);
            this.alphaLbl.TabIndex = 90;
            this.alphaLbl.Text = "Alpha = ";
            // 
            // bmpLbl
            // 
            this.bmpLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bmpLbl.AutoSize = true;
            this.bmpLbl.Location = new System.Drawing.Point(225, 290);
            this.bmpLbl.Name = "bmpLbl";
            this.bmpLbl.Size = new System.Drawing.Size(51, 13);
            this.bmpLbl.TabIndex = 89;
            this.bmpLbl.Text = "Bitmap = ";
            // 
            // Alphabtn
            // 
            this.Alphabtn.AllowDrop = true;
            this.Alphabtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Alphabtn.Location = new System.Drawing.Point(510, 311);
            this.Alphabtn.Name = "Alphabtn";
            this.Alphabtn.Size = new System.Drawing.Size(24, 21);
            this.Alphabtn.TabIndex = 88;
            this.Alphabtn.Text = "...";
            this.Alphabtn.UseVisualStyleBackColor = true;
            this.Alphabtn.Click += new System.EventHandler(this.alphaBtn_Click);
            // 
            // alphaBox
            // 
            this.alphaBox.AllowDrop = true;
            this.alphaBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaBox.Location = new System.Drawing.Point(282, 312);
            this.alphaBox.Name = "alphaBox";
            this.alphaBox.Size = new System.Drawing.Size(223, 20);
            this.alphaBox.TabIndex = 87;
            // 
            // bmpbtn
            // 
            this.bmpbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bmpbtn.Location = new System.Drawing.Point(510, 286);
            this.bmpbtn.Name = "bmpbtn";
            this.bmpbtn.Size = new System.Drawing.Size(24, 21);
            this.bmpbtn.TabIndex = 86;
            this.bmpbtn.Text = "...";
            this.bmpbtn.UseVisualStyleBackColor = true;
            this.bmpbtn.Click += new System.EventHandler(this.bmpBtn_Click);
            // 
            // bmpBox
            // 
            this.bmpBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bmpBox.Location = new System.Drawing.Point(282, 287);
            this.bmpBox.Name = "bmpBox";
            this.bmpBox.Size = new System.Drawing.Size(223, 20);
            this.bmpBox.TabIndex = 85;
            // 
            // loadDatWorker
            // 
            this.loadDatWorker.WorkerReportsProgress = true;
            this.loadDatWorker.WorkerSupportsCancellation = true;
            this.loadDatWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loadDatWorker_DoWork);
            this.loadDatWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.loadDatWorker_ProgressChanged);
            this.loadDatWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loadDatWorker_RunWorkerCompleted);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 598);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(545, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // embeddedMipmapsCb
            // 
            this.embeddedMipmapsCb.Location = new System.Drawing.Point(3, 575);
            this.embeddedMipmapsCb.Name = "embeddedMipmapsCb";
            this.embeddedMipmapsCb.Size = new System.Drawing.Size(125, 17);
            this.embeddedMipmapsCb.TabIndex = 0;
            this.embeddedMipmapsCb.Text = "Embedded Mipmaps";
            this.embeddedMipmapsCb.UseVisualStyleBackColor = true;
            // 
            // Multifshfrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 620);
            this.Controls.Add(this.embeddedMipmapsCb);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.fshWriteCompCb);
            this.Controls.Add(this.alphaLbl);
            this.Controls.Add(this.bmpLbl);
            this.Controls.Add(this.Alphabtn);
            this.Controls.Add(this.alphaBox);
            this.Controls.Add(this.bmpbtn);
            this.Controls.Add(this.bmpBox);
            this.Controls.Add(this.compDatCb);
            this.Controls.Add(this.DatfuncBox1);
            this.Controls.Add(this.genNewInstCb);
            this.Controls.Add(this.datListView);
            this.Controls.Add(this.InstendBox1);
            this.Controls.Add(this.tgiInstLbl);
            this.Controls.Add(this.tgiGroupLbl);
            this.Controls.Add(this.tgiInstanceTxt);
            this.Controls.Add(this.tgiGroupTxt);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.expbmpBox1);
            this.Controls.Add(this.bmpmanBox1);
            this.Controls.Add(this.newFshBtn);
            this.Controls.Add(this.saveFshBtn);
            this.Controls.Add(this.dirTxt);
            this.Controls.Add(this.dirNameLbl);
            this.Controls.Add(this.sizeLbl);
            this.Controls.Add(this.imgSizeLbl);
            this.Controls.Add(this.fshTypeBox);
            this.Controls.Add(this.blendRadio);
            this.Controls.Add(this.alphaRadio);
            this.Controls.Add(this.colorRadio);
            this.Controls.Add(this.loadFshBtn);
            this.Controls.Add(this.fshDefbox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MinimumSize = new System.Drawing.Size(555, 591);
            this.Name = "Multifshfrm";
            this.Text = "Multi Fsh tool";
            this.bmpmanBox1.ResumeLayout(false);
            this.fshDefbox.ResumeLayout(false);
            this.fshDefbox.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.Maintab.ResumeLayout(false);
            this.mip64tab.ResumeLayout(false);
            this.mip32tab.ResumeLayout(false);
            this.mip16tab.ResumeLayout(false);
            this.mip8tab.ResumeLayout(false);
            this.InstendBox1.ResumeLayout(false);
            this.InstendBox1.PerformLayout();
            this.DatfuncBox1.ResumeLayout(false);
            this.expbmpBox1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ImageList bitmapList;
        private System.Windows.Forms.ListView listViewMain;
        private System.Windows.Forms.Button loadFshBtn;
        private System.Windows.Forms.RadioButton blendRadio;
        private System.Windows.Forms.RadioButton alphaRadio;
        private System.Windows.Forms.RadioButton colorRadio;
        private System.Windows.Forms.OpenFileDialog openFshDialog1;
        private System.Windows.Forms.ComboBox fshTypeBox;
        private System.Windows.Forms.ImageList alphaList;
        private System.Windows.Forms.ImageList blendList;
        private System.Windows.Forms.Label sizeLbl;
        private System.Windows.Forms.Label imgSizeLbl;
        private System.Windows.Forms.Label dirNameLbl;
        private System.Windows.Forms.OpenFileDialog openBitmapDialog1;
        private System.Windows.Forms.Button remBtn;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.OpenFileDialog openAlphaDialog1;
        private System.Windows.Forms.TextBox dirTxt;
        private System.Windows.Forms.Button repBtn;
        private System.Windows.Forms.Button saveFshBtn;
        private System.Windows.Forms.SaveFileDialog saveFshDialog1;
        private System.Windows.Forms.Button newFshBtn;
        private System.Windows.Forms.GroupBox bmpmanBox1;
        private System.Windows.Forms.GroupBox fshDefbox;
        private System.Windows.Forms.RadioButton regFshRadio;
        private System.Windows.Forms.RadioButton hdBaseFshRadio;
        private System.Windows.Forms.RadioButton hdFshRadio;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Maintab;
        private System.Windows.Forms.TabPage mip64tab;
        private System.Windows.Forms.ListView listViewMip64;
        private System.Windows.Forms.TabPage mip32tab;
        private System.Windows.Forms.TabPage mip16tab;
        private System.Windows.Forms.TabPage mip8tab;
        private System.Windows.Forms.ListView listViewMip32;
        private System.Windows.Forms.ListView listViewMip16;
        private System.Windows.Forms.ListView listViewMip8;
        private System.Windows.Forms.ImageList bmp64Mip;
        private System.Windows.Forms.ImageList alpha64Mip;
        private System.Windows.Forms.ImageList blend64Mip;
        private System.Windows.Forms.ImageList bmp32Mip;
        private System.Windows.Forms.ImageList alpha32Mip;
        private System.Windows.Forms.ImageList blend32Mip;
        private System.Windows.Forms.ImageList bmp16Mip;
        private System.Windows.Forms.ImageList alpha16Mip;
        private System.Windows.Forms.ImageList blend16Mip;
        private System.Windows.Forms.ImageList bmp8Mip;
        private System.Windows.Forms.ImageList alpha8Mip;
        private System.Windows.Forms.ImageList blend8Mip;
        private System.Windows.Forms.TextBox tgiGroupTxt;
        private System.Windows.Forms.TextBox tgiInstanceTxt;
        private System.Windows.Forms.Label tgiGroupLbl;
        private System.Windows.Forms.Label tgiInstLbl;
        private System.Windows.Forms.GroupBox InstendBox1;
        private System.Windows.Forms.RadioButton inst5_9Rdo;
        private System.Windows.Forms.RadioButton inst0_4Rdo;
        private System.Windows.Forms.RadioButton instA_ERdo;
        private System.Windows.Forms.ListView datListView;
        private System.Windows.Forms.Button loadDatBtn;
        private System.Windows.Forms.OpenFileDialog openDatDialog1;
        private System.Windows.Forms.ColumnHeader GroupHeader;
        private System.Windows.Forms.ColumnHeader InstanceHeader;
        private System.Windows.Forms.ColumnHeader NameHeader1;
        private System.Windows.Forms.SaveFileDialog saveDatDialog1;
        private System.Windows.Forms.Button saveDatBtn;
        private System.Windows.Forms.Button newDatBtn;
        private System.Windows.Forms.GroupBox DatfuncBox1;
        private System.Windows.Forms.CheckBox genNewInstCb;
        private System.Windows.Forms.CheckBox compDatCb;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button saveAlphaBtn;
        private System.Windows.Forms.Button saveBmpBlendBtn;
        private System.Windows.Forms.Button saveBmpBtn;
        private System.Windows.Forms.GroupBox expbmpBox1;
        private System.Windows.Forms.Label alphaLbl;
        private System.Windows.Forms.Label bmpLbl;
        private System.Windows.Forms.Button Alphabtn;
        private System.Windows.Forms.TextBox alphaBox;
        private System.Windows.Forms.Button bmpbtn;
        private System.Windows.Forms.TextBox bmpBox;
        private System.Windows.Forms.CheckBox fshWriteCompCb;
        private System.Windows.Forms.Button closeDatBtn;
        private System.ComponentModel.BackgroundWorker loadDatWorker;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.CheckBox embeddedMipmapsCb;
    }
}

