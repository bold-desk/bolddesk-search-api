//-----------------------------------------------------------------------
// <copyright file="ModulePermissionEnum.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Enums
{
    using System.ComponentModel;

    /// <summary>
    /// Enum of module permissions
    /// </summary>
    public enum ModulePermissionEnum
    {
        /// <summary>
        /// Permission to view contacts module
        /// </summary>
        [Description("2")] // 2 is module id for this permission
        CanViewContactsModule = 1,

        /// <summary>
        /// Permission to view admin module
        /// </summary>
        [Description("4")] // 4 is module id for this permission
        CanViewAdminModule = 2,

        /// <summary>
        /// Permission to view KB module
        /// </summary>
        [Description("5")] // 5 is module id for this permission
        CanViewKBModule = 73, // 73 is permission id

        /// <summary>
        /// Permission to view reports module
        /// </summary>
        [Description("12")] // 12 is module id for this permission
        CanViewReportsModule = 55 // 55 is permission id
    }
}
