using PSTaskDialog;
using System;
using System.Collections.Generic;
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

        private static DialogResult SaveChangesTaskDialog(IWin32Window owner, string message, string caption, MessageBoxButtons buttons)
        {
            if (buttons != MessageBoxButtons.YesNo && buttons != MessageBoxButtons.YesNoCancel)
            {
                throw new ArgumentOutOfRangeException("buttons");
            }

            VistaTaskDialog saveChangesDialog = new VistaTaskDialog();

            List<VistaTaskDialogButton> buttonList = new List<VistaTaskDialogButton>();
            const int SaveButtonId = 101;
            const int DontSaveButtonId = 102;
            const int CancelButtonId = 103;

            VistaTaskDialogButton saveBtn = new VistaTaskDialogButton();
            saveBtn.ButtonId = SaveButtonId;           
            saveBtn.ButtonText = Properties.Resources.SaveButtonText;
            buttonList.Add(saveBtn);

            VistaTaskDialogButton dontSaveBtn = new VistaTaskDialogButton();
            dontSaveBtn.ButtonId = DontSaveButtonId;
            dontSaveBtn.ButtonText = Properties.Resources.DontSaveButtonText;
            buttonList.Add(dontSaveBtn);
            
            if (buttons == MessageBoxButtons.YesNoCancel)
            {
                VistaTaskDialogButton cancelButton = new VistaTaskDialogButton();
                cancelButton.ButtonId = CancelButtonId;
                cancelButton.ButtonText = Properties.Resources.CancelButtonText;
                buttonList.Add(cancelButton);
                
                saveChangesDialog.AllowDialogCancellation = true;
            }
            saveChangesDialog.Buttons = buttonList.ToArray();
            saveChangesDialog.DefaultButton = SaveButtonId;
            saveChangesDialog.WindowTitle = caption;
            saveChangesDialog.MainInstruction = message;
            saveChangesDialog.PositionRelativeToWindow = true;
 
            int res = saveChangesDialog.Show(owner);

            DialogResult result = DialogResult.None;

            switch (res)
            {
                case SaveButtonId:
                    result = DialogResult.Yes;
                    break;
                case DontSaveButtonId:
                    result = DialogResult.No;
                    break;
                case CancelButtonId:
                default:
                    result = DialogResult.Cancel;
                    break;
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

        private static DialogResult ErrorTaskDialog(IWin32Window owner, string message, string caption, VistaTaskDialogIcon icon)
        {

            VistaTaskDialog errorDialog = new VistaTaskDialog()
            {
                AllowDialogCancellation = true,
                CommonButtons = VistaTaskDialogCommonButtons.Ok,
                MainIcon = icon,
                MainInstruction = message,
                PositionRelativeToWindow = true,
                WindowTitle = caption
            };

            errorDialog.Show(owner);

            return DialogResult.OK;
        }

        public static DialogResult ShowErrorMessage(IWin32Window owner, string message, string caption)
        {
            if (TaskDialogSupported)
            {
                return ErrorTaskDialog(owner, message, caption, VistaTaskDialogIcon.Error);
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
                return ErrorTaskDialog(owner, message, caption, VistaTaskDialogIcon.Warning);
            }
            else
            {
                return MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, 0);
            }
        }
    }
}
