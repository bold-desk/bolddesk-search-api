//-----------------------------------------------------------------------
// <copyright file="ModulePermissionObject.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Objects
{
    using System.Collections.Generic;

    /// <summary>
    /// ModulePermissionObject - Class Declaration.
    /// </summary>
    public class ModulePermissionObject
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
        /// Gets or sets the module sort order number.
        /// </summary>
        public int ModuleSortOrder { get; set; }

        /// <summary>
        /// Gets or sets the permission Id.
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the permission name.
        /// </summary>
        public string PermissionName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of permission.
        /// </summary>
        public string PermissionDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent permission Id.
        /// </summary>
        public int? ParentPermissionId { get; set; }

        /// <summary>
        /// Gets or sets the sort order number.
        /// </summary>
        public int PermissionSortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether permission is enabled or not.
        /// </summary>
        public bool PermissionIsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the selection type.
        /// </summary>
        public string PermissionSelectionType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the child permissions.
        /// </summary>
        public List<ModulePermissionObject> ChildPermissions { get; set; } = new List<ModulePermissionObject>();
    }
}
