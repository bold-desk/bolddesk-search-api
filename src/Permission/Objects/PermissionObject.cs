//-----------------------------------------------------------------------
// <copyright file="PermissionObject.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Objects
{
    using System.Collections.Generic;

    /// <summary>
    /// PermissionObject - Class Declaration.
    /// </summary>
    public class PermissionObject
    {
        /// <summary>
        /// Gets or sets the permission Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the permission name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of permission.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sort order number.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether permission is enabled or not.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the selection type.
        /// </summary>
        public string SelectionType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the child permissions.
        /// </summary>
        public List<PermissionObject> ChildPermissions { get; set; } = new List<PermissionObject>();
    }
}
