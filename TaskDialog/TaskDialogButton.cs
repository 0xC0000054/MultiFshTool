using System.Runtime.InteropServices;

namespace loaddatfsh.TaskDialog
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct TaskDialogButton
    {
        public int nButtonID;
        public ushort* pszButtonText;
    }
}
