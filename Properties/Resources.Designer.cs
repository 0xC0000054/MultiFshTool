﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace loaddatfsh.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("loaddatfsh.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fsh files (*.fsh;*.qfs)|*.fsh;*.qfs|All files (*.*)|*.*.
        /// </summary>
        internal static string FshFiles_Filter {
            get {
                return ResourceManager.GetString("FshFiles_Filter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bitmap files (*.png;*.bmp)|*.png;*.bmp|All files (*.*)|*.*.
        /// </summary>
        internal static string ImageFiles_Filter {
            get {
                return ResourceManager.GetString("ImageFiles_Filter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must select a Bitmap to replace the selected image..
        /// </summary>
        internal static string repbmp_NewFileSelect_Error {
            get {
                return ResourceManager.GetString("repbmp_NewFileSelect_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must select an image to replace..
        /// </summary>
        internal static string repbmp_NoImageSelected_Error {
            get {
                return ResourceManager.GetString("repbmp_NoImageSelected_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to alpha.
        /// </summary>
        internal static string saveBitmap_Alpha_Prefix {
            get {
                return ResourceManager.GetString("saveBitmap_Alpha_Prefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to blended.
        /// </summary>
        internal static string saveBitmap_Blended_Prefix {
            get {
                return ResourceManager.GetString("saveBitmap_Blended_Prefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An image must be selected to save the {0}.
        /// </summary>
        internal static string saveBitmap_Error_Format {
            get {
                return ResourceManager.GetString("saveBitmap_Error_Format", resourceCulture);
            }
        }
    }
}
