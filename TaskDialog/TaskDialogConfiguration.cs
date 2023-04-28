/*
* This file is part of MultiFshTool, a tool for creating and editing FSH
* files that contain multiple images.
*
* Copyright (C) 2010-2017, 2023 Nicholas Hayes
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*
*/

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
