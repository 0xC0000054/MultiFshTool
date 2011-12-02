using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace loaddatfsh
{
    /* The following code was adapted from
     * http://remyblok.tweakblogs.net/blog/6606/win7-recent-jumplist-without-associating-a-filetype.html
     */
    class JumpListHelper
    {

        private static class NativeMethods
        {
            [ComImport,
                Guid("000214F9-0000-0000-C000-000000000046"),
                InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            internal interface IShellLinkW { }

            internal struct SHARDAPPIDINFOLINK
            {
                public IShellLinkW isl;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pszAppID;
            }

            [DllImport("shell32.dll")]
            internal static extern void SHAddToRecentDocs(
                ShellAddToRecentDocs flags,
                SHARDAPPIDINFOLINK link);

        }

        internal enum ShellAddToRecentDocs
        {
            Pidl = 0x1,
            PathA = 0x2,
            PathW = 0x3,
            AppIdInfo = 0x4,       // indicates the data type is a pointer to a SHARDAPPIDINFO structure
            AppIdInfoIdList = 0x5, // indicates the data type is a pointer to a SHARDAPPIDINFOIDLIST structure
            Link = 0x6,            // indicates the data type is a pointer to an IShellLink instance
            AppIdInfoLink = 0x7,   // indicates the data type is a pointer to a SHARDAPPIDINFOLINK structure 
        }

        private static MethodInfo nativeShellLinkGetMethod;
        public static void AddToRecent(JumpListLink link, string appID)
        {
            if (nativeShellLinkGetMethod == null)
            {
                //find the NativeShellLink property on the JumpListLink
                Type jumpListLinkType = typeof(JumpListLink);
                PropertyInfo nativeShellLinkProperty = jumpListLinkType.GetProperty("NativeShellLink",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                if (nativeShellLinkProperty == null)
                    throw new InvalidOperationException();

                //Save the Method info for later use, so we have to do the reflection only once.
                nativeShellLinkGetMethod = nativeShellLinkProperty.GetGetMethod(true);
            }

            //Get the value of the NativeShellLink property.
            //Cast this to our own implementation of IShellLinkW because it is using COM interop.
            NativeMethods.IShellLinkW nativeShellLink = (NativeMethods.IShellLinkW)nativeShellLinkGetMethod.Invoke(link, null);

            NativeMethods.SHARDAPPIDINFOLINK appInfo = new NativeMethods.SHARDAPPIDINFOLINK();
            appInfo.isl = nativeShellLink;
            appInfo.pszAppID = appID;

            // Now make the call to Win32 to add the link to the recent items
            NativeMethods.SHAddToRecentDocs(ShellAddToRecentDocs.AppIdInfoLink, appInfo);

        }
        
    }
}
