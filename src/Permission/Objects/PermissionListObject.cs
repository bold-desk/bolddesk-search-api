//-----------------------------------------------------------------------
// <copyright file="PermissionListObject.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Objects
{
    using System.Collections.Generic;

    /// <summary>
    /// PermissionListObject - Class Declaration.
    /// </summary>
    public class PermissionListObject
    {
        /// <summary>
        /// Gets or sets the module Id.
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the module name.
        /// </summary>
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the module is enabled or not.
        /// </summary>
        public bool IsModuleEnabled { get; set; }

        /// <summary>
        /// Gets or sets the permission Id.
        /// </summary>
        public int? PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the module sort order number.
        /// </summary>
        public int ModuleSortOrder { get; set; }

        /// <summary>
        /// Gets or sets list of permission.
        /// </summary>
        public List<PermissionObject> Permissions { get; set; } = new List<PermissionObject>();
    }
}
