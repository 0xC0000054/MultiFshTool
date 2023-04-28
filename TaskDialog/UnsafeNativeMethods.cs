using System;
using System.Runtime.InteropServices;
using System.Security;

namespace loaddatfsh.TaskDialog
{
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        [DllImport("Comctl32.dll", SetLastError = true)]
        internal static extern unsafe int TaskDialogIndirect(TaskDialogConfiguration* pTaskConfig,
                                                             out int nButton,
                                                             out int nRadioButton,
                                                             out int fVerificationFlagChecked);
        internal static unsafe ushort* MAKEINTRESOURCE(ushort value) => (ushort*)new UIntPtr(value);

        internal static bool SUCCEEDED(int hr) => hr >= 0;
    }
}
