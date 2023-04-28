using System;
using System.Runtime.InteropServices;

namespace loaddatfsh.TaskDialog
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct TaskDialogConfiguration
    {
        public uint cbSize;
        public IntPtr hwndParent;
        public IntPtr hInstance;
        public TaskDialogOptions dwFlags;
        public TaskDialogCommonButtons dwCommonButtons;
        public ushort* pszWindowTitle;
        public ushort* pszMainIcon;
        public ushort* pszMainInstruction;
        public ushort* pszContent;
        public uint cButtons;
        public TaskDialogButton* pButtons;
        public int nDefaultButton;
        public uint cRadioButtons;
        public TaskDialogButton* pRadioButtons;
        public int nDefaultRadioButton;
        public ushort* pszVerificationText;
        public ushort* pszExpandedInformation;
        public ushort* pszExpandedControlText;
        public ushort* pszCollapsedControlText;
        public ushort* pszFooterIcon;
        public ushort* pszFooter;
        public IntPtr pfCallback;
        public IntPtr lpCallbackData;
        public uint cxWidth;
    }

}
