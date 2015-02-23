using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace loaddatfsh
{
    /// <summary>
    /// Delays the SelectedIndexChanged event to suppress false deselection events.
    /// </summary>
    /// <remarks>
    /// As current item in the ListView is deselected before selecting the new item.
    /// This class uses a timer to ensure that the only deselection event we receive is from the user.
    /// </remarks>
    internal sealed class DelayIndexChangedListView : ListView
    {
        private Timer indexChangedTimer;

        public DelayIndexChangedListView()
        {
            this.indexChangedTimer = new Timer();
            this.indexChangedTimer.Interval = 10;
            this.indexChangedTimer.Tick += eventDelayTimer_Tick;

            base.DoubleBuffered = true;
        }

        /// <summary>
        /// Enables the border select mode for the native Win32 list view.
        /// </summary>
        public void EnableBorderSelect()
        {
            // Set the border select extended style to prevent the underlying Win32 list view from changing the color of the thumbnails when an item is selected.
            NativeEnums.ListViewExtendedStyles style = NativeEnums.ListViewExtendedStyles.BorderSelect;

            NativeMethods.SendMessageW(this.Handle, NativeConstants.LVM_SETEXTENDEDLISTVIEWSTYLE, (UIntPtr)style, (IntPtr)style);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            this.indexChangedTimer.Start();
        }

        private void eventDelayTimer_Tick(object sender, EventArgs e)
        {
            this.indexChangedTimer.Stop();
            base.OnSelectedIndexChanged(EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.indexChangedTimer != null)
                {
                    this.indexChangedTimer.Dispose();
                    this.indexChangedTimer = null;
                }
            }

            base.Dispose(disposing);
        }

        private static class NativeConstants
        {
             internal const uint LVM_FIRST = 0x1000;
             internal const uint LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
             internal const uint LVM_GETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 55;
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll", ExactSpelling = true)]
            public static extern IntPtr SendMessageW([In()] IntPtr hWnd, [In()] uint Msg, [In()] UIntPtr wParam, [In()] IntPtr lParam);
        }

        private static class NativeEnums
        {
            public enum ListViewExtendedStyles
            {
                /// <summary>
                /// LVS_EX_GRIDLINES
                /// </summary>
                GridLines = 0x00000001,
                /// <summary>
                /// LVS_EX_SUBITEMIMAGES
                /// </summary>
                SubItemImages = 0x00000002,
                /// <summary>
                /// LVS_EX_CHECKBOXES
                /// </summary>
                CheckBoxes = 0x00000004,
                /// <summary>
                /// LVS_EX_TRACKSELECT
                /// </summary>
                TrackSelect = 0x00000008,
                /// <summary>
                /// LVS_EX_HEADERDRAGDROP
                /// </summary>
                HeaderDragDrop = 0x00000010,
                /// <summary>
                /// LVS_EX_FULLROWSELECT
                /// </summary>
                FullRowSelect = 0x00000020,
                /// <summary>
                /// LVS_EX_ONECLICKACTIVATE
                /// </summary>
                OneClickActivate = 0x00000040,
                /// <summary>
                /// LVS_EX_TWOCLICKACTIVATE
                /// </summary>
                TwoClickActivate = 0x00000080,
                /// <summary>
                /// LVS_EX_FLATSB
                /// </summary>
                FlatsB = 0x00000100,
                /// <summary>
                /// LVS_EX_REGIONAL
                /// </summary>
                Regional = 0x00000200,
                /// <summary>
                /// LVS_EX_INFOTIP
                /// </summary>
                InfoTip = 0x00000400,
                /// <summary>
                /// LVS_EX_UNDERLINEHOT
                /// </summary>
                UnderlineHot = 0x00000800,
                /// <summary>
                /// LVS_EX_UNDERLINECOLD
                /// </summary>
                UnderlineCold = 0x00001000,
                /// <summary>
                /// LVS_EX_MULTIWORKAREAS
                /// </summary>
                MultilWorkAreas = 0x00002000,
                /// <summary>
                /// LVS_EX_LABELTIP
                /// </summary>
                LabelTip = 0x00004000,
                /// <summary>
                /// LVS_EX_BORDERSELECT
                /// </summary>
                BorderSelect = 0x00008000,
                /// <summary>
                /// LVS_EX_DOUBLEBUFFER
                /// </summary>
                DoubleBuffer = 0x00010000,
                /// <summary>
                /// LVS_EX_HIDELABELS
                /// </summary>
                HideLabels = 0x00020000,
                /// <summary>
                /// LVS_EX_SINGLEROW
                /// </summary>
                SingleRow = 0x00040000,
                /// <summary>
                /// LVS_EX_SNAPTOGRID
                /// </summary>
                SnapToGrid = 0x00080000,
                /// <summary>
                /// LVS_EX_SIMPLESELECT
                /// </summary>
                SimpleSelect = 0x00100000
            }

        }
    }
}
