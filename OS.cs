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

using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Runtime.InteropServices;

namespace loaddatfsh
{
    internal static class OS
    {
        static bool checkedHaveSSE = false;
        static bool checkedIsMicrosoftWindows = false;
        static bool haveSSE;
        static bool isMicrosoftWindows;
        static bool initFileIconReference = false;
        static IconReference fileIconReference;

        private static class NativeConstants
        {
            internal const uint PF_XMMI_INSTRUCTIONS_AVAILABLE = 6;
            internal const int S_OK = 0;
            internal const uint SHGSI_ICONLOCATION = 0;
            internal const uint SHGSI_SMALLICON = 1;
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool IsProcessorFeaturePresent(uint ProcessorFeature);

            [DllImport("shell32.dll", ExactSpelling = true)]
            internal static extern int SHGetStockIconInfo(NativeEnums.SHSTOCKICONID siid, uint uFlags, ref NativeStructs.SHSTOCKICONINFO psii);
        }

        private static class NativeStructs
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct SHSTOCKICONINFO
            {
                public uint cbSize;
                public IntPtr hIcon;
                public int iSysImageIndex;
                public int iIcon;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szPath;
            }
        }

        private static class NativeEnums
        {
            internal enum SHSTOCKICONID : int
            {
                SIID_DOCNOASSOC = 0,
                SIID_DOCASSOC = 1,
                SIID_APPLICATION = 2,
                SIID_FOLDER = 3,
                SIID_FOLDEROPEN = 4,
                SIID_DRIVE525 = 5,
                SIID_DRIVE35 = 6,
                SIID_DRIVEREMOVE = 7,
                SIID_DRIVEFIXED = 8,
                SIID_DRIVENET = 9,
                SIID_DRIVENETDISABLED = 10,
                SIID_DRIVECD = 11,
                SIID_DRIVERAM = 12,
                SIID_WORLD = 13,
                SIID_SERVER = 15,
                SIID_PRINTER = 16,
                SIID_MYNETWORK = 17,
                SIID_FIND = 22,
                SIID_HELP = 23,
                SIID_SHARE = 28,
                SIID_LINK = 29,
                SIID_SLOWFILE = 30,
                SIID_RECYCLER = 31,
                SIID_RECYCLERFULL = 32,
                SIID_MEDIACDAUDIO = 40,
                SIID_LOCK = 47,
                SIID_AUTOLIST = 49,
                SIID_PRINTERNET = 50,
                SIID_SERVERSHARE = 51,
                SIID_PRINTERFAX = 52,
                SIID_PRINTERFAXNET = 53,
                SIID_PRINTERFILE = 54,
                SIID_STACK = 55,
                SIID_MEDIASVCD = 56,
                SIID_STUFFEDFOLDER = 57,
                SIID_DRIVEUNKNOWN = 58,
                SIID_DRIVEDVD = 59,
                SIID_MEDIADVD = 60,
                SIID_MEDIADVDRAM = 61,
                SIID_MEDIADVDRW = 62,
                SIID_MEDIADVDR = 63,
                SIID_MEDIADVDROM = 64,
                SIID_MEDIACDAUDIOPLUS = 65,
                SIID_MEDIACDRW = 66,
                SIID_MEDIACDR = 67,
                SIID_MEDIACDBURN = 68,
                SIID_MEDIABLANKCD = 69,
                SIID_MEDIACDROM = 70,
                SIID_AUDIOFILES = 71,
                SIID_IMAGEFILES = 72,
                SIID_VIDEOFILES = 73,
                SIID_MIXEDFILES = 74,
                SIID_FOLDERBACK = 75,
                SIID_FOLDERFRONT = 76,
                SIID_SHIELD = 77,
                SIID_WARNING = 78,
                SIID_INFO = 79,
                SIID_ERROR = 80,
                SIID_KEY = 81,
                SIID_SOFTWARE = 82,
                SIID_RENAME = 83,
                SIID_DELETE = 84,
                SIID_MEDIAAUDIODVD = 85,
                SIID_MEDIAMOVIEDVD = 86,
                SIID_MEDIAENHANCEDCD = 87,
                SIID_MEDIAENHANCEDDVD = 88,
                SIID_MEDIAHDDVD = 89,
                SIID_MEDIABLURAY = 90,
                SIID_MEDIAVCD = 91,
                SIID_MEDIADVDPLUSR = 92,
                SIID_MEDIADVDPLUSRW = 93,
                SIID_DESKTOPPC = 94,
                SIID_MOBILEPC = 95,
                SIID_USERS = 96,
                SIID_MEDIASMARTMEDIA = 97,
                SIID_MEDIACOMPACTFLASH = 98,
                SIID_DEVICECELLPHONE = 99,
                SIID_DEVICECAMERA = 100,
                SIID_DEVICEVIDEOCAMERA = 101,
                SIID_DEVICEAUDIOPLAYER = 102,
                SIID_NETWORKCONNECT = 103,
                SIID_INTERNET = 104,
                SIID_ZIPFILE = 105,
                SIID_SETTINGS = 106,
                SIID_DRIVEHDDVD = 132,
                SIID_DRIVEBD = 133,
                SIID_MEDIAHDDVDROM = 134,
                SIID_MEDIAHDDVDR = 135,
                SIID_MEDIAHDDVDRAM = 136,
                SIID_MEDIABDROM = 137,
                SIID_MEDIABDR = 138,
                SIID_MEDIABDRE = 139,
                SIID_CLUSTEREDDRIVE = 140,
                SIID_MAX_ICONS = 175
            }
        }

        /// <summary>
        /// Gets a value indicating whether SSE instructions are available.
        /// </summary>
        /// <value>
        ///   <c>true</c> if SSE instructions are available; otherwise, <c>false</c>.
        /// </value>
        internal static bool HaveSSE
        {
            get
            {
                if (!checkedHaveSSE)
                {
                    haveSSE = NativeMethods.IsProcessorFeaturePresent(NativeConstants.PF_XMMI_INSTRUCTIONS_AVAILABLE);
                    checkedHaveSSE = true;
                }

                return haveSSE;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current operating system is Microsoft Windows.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the current operating system is Microsoft Windows; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsMicrosoftWindows
        {
            get
            {
                if (!checkedIsMicrosoftWindows)
                {
                    OperatingSystem os = Environment.OSVersion;
                    isMicrosoftWindows = (os.Platform == PlatformID.Win32NT || os.Platform == PlatformID.Win32Windows);
                    checkedIsMicrosoftWindows = true;
                } 

                return isMicrosoftWindows;
            }
        }

        internal static IconReference FileIconReference
        {
            get
            {
                if (!initFileIconReference)
                {
                    OperatingSystem os = Environment.OSVersion;
                    if (os.Platform == PlatformID.Win32NT && os.Version.Major >= 6)
                    {
                        NativeStructs.SHSTOCKICONINFO info = new NativeStructs.SHSTOCKICONINFO();
                        info.cbSize = (uint)Marshal.SizeOf(typeof(NativeStructs.SHSTOCKICONINFO));
                        uint flags =  NativeConstants.SHGSI_ICONLOCATION | NativeConstants.SHGSI_SMALLICON;

                        if (NativeMethods.SHGetStockIconInfo(NativeEnums.SHSTOCKICONID.SIID_DOCNOASSOC, flags, ref info) == NativeConstants.S_OK)
                        {
                            fileIconReference = new IconReference(info.szPath, info.iIcon);
                        }
                    }
                    initFileIconReference = true;
                }

                return fileIconReference;
            }
        }

    }
}
