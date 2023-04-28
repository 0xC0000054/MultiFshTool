using loaddatfsh.TaskDialog;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace loaddatfsh
{
    internal static class UIUtil
    {
        private static readonly bool TaskDialogSupported = CheckTaskDialogSupported();

        private static bool CheckTaskDialogSupported()
        {
            OperatingSystem os = Environment.OSVersion;

            return (os.Platform == PlatformID.Win32NT && os.Version.Major >= 6);
        }

        private static unsafe DialogResult SaveChangesTaskDialog(IWin32Window owner, string message, string caption, MessageBoxButtons buttons)
        {
            if (buttons != MessageBoxButtons.YesNo && buttons != MessageBoxButtons.YesNoCancel)
            {
                throw new ArgumentOutOfRangeException("buttons");
            }

            DialogResult result = DialogResult.Cancel;

            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(caption))
            {
                fixed (char* pMessage = message)
                fixed (char* pCaption = caption)
                fixed (char* pSaveMessage = Properties.Resources.SaveButtonText)
                fixed (char* pDontSaveMessage = Properties.Resources.DontSaveButtonText)
                {
                    const int SaveButtonId = 101;
                    const int DontSaveButtonId = 102;

                    const int CustomButtonCount = 2;

                    TaskDialogButton* customButtons = stackalloc TaskDialogButton[CustomButtonCount];

                    customButtons[0].nButtonID = SaveButtonId;
                    customButtons[0].pszButtonText = (ushort*)pSaveMessage;
                    customButtons[1].nButtonID = DontSaveButtonId;
                    customButtons[1].pszButtonText = (ushort*)pDontSaveMessage;

                    TaskDialogConfiguration config = new TaskDialogConfiguration()
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(TaskDialogConfiguration)),
                        pszWindowTitle = (ushort*)pCaption,
                        pszMainInstruction = (ushort*)pMessage,
                        cButtons = CustomButtonCount,
                        pButtons = customButtons
                    };

                    if (owner != null)
                    {
                        config.hwndParent = owner.Handle;
                        config.dwFlags |= TaskDialogOptions.PositionRelativeToWindow;
                    }

                    if (buttons == MessageBoxButtons.YesNoCancel)
                    {
                        config.dwFlags |= TaskDialogOptions.AllowCancel;
                        config.dwCommonButtons = TaskDialogCommonButtons.Cancel;
                    }

                    int hr = UnsafeNativeMethods.TaskDialogIndirect(&config, out int selectedButton, out int _, out int _);

                    if (UnsafeNativeMethods.SUCCEEDED(hr))
                    {
                        switch (selectedButton)
                        {
                            case SaveButtonId:
                                result = DialogResult.Yes;
                                break;
                            case DontSaveButtonId:
                                result = DialogResult.No;
                                break;
                        }
                    }
                }
            }

            return result;
        }

        public static DialogResult ShowSaveChangesDialog(IWin32Window owner, string message, string caption, MessageBoxButtons buttons)
        {
            if (TaskDialogSupported)
            {
                return SaveChangesTaskDialog(owner, message, caption, buttons);
            }
            else
            {
                return MessageBox.Show(owner, message, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
            }
        }

        private static unsafe DialogResult ErrorTaskDialog(IWin32Window owner, string message, string caption, TaskDialogIcon icon)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string dialogCaption = !string.IsNullOrEmpty(caption) ? caption : "Error";

                fixed (char* pMessage = message)
                fixed (char* pCaption = dialogCaption)
                {
                    TaskDialogConfiguration config = new TaskDialogConfiguration()
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(TaskDialogConfiguration)),
                        dwFlags = TaskDialogOptions.AllowCancel,
                        dwCommonButtons = TaskDialogCommonButtons.Ok,
                        pszWindowTitle = (ushort*)pCaption,
                        pszMainIcon = UnsafeNativeMethods.MAKEINTRESOURCE((ushort)icon),
                        pszMainInstruction = (ushort*)pMessage
                    };

                    if (owner != null)
                    {
                        config.hwndParent = owner.Handle;
                        config.dwFlags |= TaskDialogOptions.PositionRelativeToWindow;
                    }

                    _ = UnsafeNativeMethods.TaskDialogIndirect(&config, out int _, out int _, out int _);
                }
            }

            return DialogResult.OK;
        }

        public static DialogResult ShowErrorMessage(IWin32Window owner, string message, string caption)
        {
            if (TaskDialogSupported)
            {
                return ErrorTaskDialog(owner, message, caption, TaskDialogIcon.Error);
            }
            else
            {
                return MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
            }
        }

        public static DialogResult ShowWarningMessage(IWin32Window owner, string message, string caption)
        {
            if (TaskDialogSupported)
            {
                return ErrorTaskDialog(owner, message, caption, TaskDialogIcon.Warning);
            }
            else
            {
                return MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, 0);
            }
        }
    }
}
