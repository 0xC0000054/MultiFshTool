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
