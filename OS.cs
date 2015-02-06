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

        private static class NativeConstants
        {
            internal const uint PF_XMMI_INSTRUCTIONS_AVAILABLE = 6;
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool IsProcessorFeaturePresent(uint ProcessorFeature);
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

    }
}
