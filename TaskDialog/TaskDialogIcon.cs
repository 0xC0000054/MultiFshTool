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

namespace loaddatfsh.TaskDialog
{
    internal enum TaskDialogIcon : ushort
    {
        /// <summary>
        /// Displays no icons (default).
        /// </summary>
        None = 0,

        /// <summary>
        /// Displays the warning icon.
        /// </summary>
        Warning = 65535,

        /// <summary>
        /// Displays the error icon.
        /// </summary>
        Error = 65534,

        /// <summary>
        /// Displays the Information icon.
        /// </summary>
        Information = 65533,

        /// <summary>
        /// Displays the User Account Control shield.
        /// </summary>
        Shield = 65532
    }
}
