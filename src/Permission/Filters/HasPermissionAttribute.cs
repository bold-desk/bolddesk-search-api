//-----------------------------------------------------------------------
// <copyright file="HasPermissionAttribute.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Filters
{
    using BoldDesk.Permission.Enums;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Has Permission Attribute.
    /// </summary>
    public class HasPermissionAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HasPermissionAttribute"/> class.
        /// </summary>
        /// <param name="permission">Permission that we need to check.</param>
        public HasPermissionAttribute(PermissionEnum permission)
            : base(typeof(HasPermissionAsyncFilter))
        {
            Arguments = new object[] { permission };
        }
    }
}
