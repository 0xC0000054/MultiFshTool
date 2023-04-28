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

namespace loaddatfsh.TaskDialog
{
    [Flags]
    internal enum TaskDialogOptions : uint
    {
        None = 0,
        EnableHyperlinks = 0x0001,
        UseMainIcon = 0x0002,
        UseFooterIcon = 0x0004,
        AllowCancel = 0x0008,
        UseCommandLinks = 0x0010,
        UseNoIconCommandLinks = 0x0020,
        ExpandFooterArea = 0x0040,
        ExpandedByDefault = 0x0080,
        CheckVerificationFlag = 0x0100,
        ShowProgressBar = 0x0200,
        ShowMarqueeProgressBar = 0x0400,
        UseCallbackTimer = 0x0800,
        PositionRelativeToWindow = 0x1000,
        RightToLeftLayout = 0x2000,
        NoDefaultRadioButton = 0x4000
    }
}
