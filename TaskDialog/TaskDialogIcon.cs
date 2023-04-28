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
