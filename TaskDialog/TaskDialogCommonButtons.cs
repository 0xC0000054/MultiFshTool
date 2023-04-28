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
    internal enum TaskDialogCommonButtons : uint
    {
        None = 0,
        Ok = 0x0001, // selected control return value IDOK
        Yes = 0x0002, // selected control return value IDYES
        No = 0x0004, // selected control return value IDNO
        Cancel = 0x0008, // selected control return value IDCANCEL
        Retry = 0x0010, // selected control return value IDRETRY
        Close = 0x0020  // selected control return value IDCLOSE
    }
}
