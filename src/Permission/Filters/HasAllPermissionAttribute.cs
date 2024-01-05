//-----------------------------------------------------------------------
// <copyright file="HasAllPermissionAttribute.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Filters
{
    using BoldDesk.Permission.Enums;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Has Any Permission Attribute.
    /// </summary>
    public class HasAllPermissionAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HasAllPermissionAttribute"/> class.
        /// </summary>
        /// <param name="permissions">Permissions that we need to check.</param>
        public HasAllPermissionAttribute(PermissionEnum[] permissions)
            : base(typeof(HasAllPermissionAsyncFilter))
        {
            Arguments = new object[] { permissions };
        }
    }
}
