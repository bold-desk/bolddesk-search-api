//-----------------------------------------------------------------------
// <copyright file="AddHasPermissionParameterOperationFilter.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Filters
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using BoldDesk.Permission.Enums;
    using BoldDesk.Permission.Filters;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Add Has Permission Parameter Operation Filter
    /// </summary>
    public class AddHasPermissionParameterOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Apply method.
        /// </summary>
        /// <param name="operation">Operation.</param>
        /// <param name="context">Context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null)
            {
                return;
            }

            string hasAuthorizeAttribute = HasAttribute(context.MethodInfo, typeof(HasPermissionAttribute), true);
            if (string.IsNullOrWhiteSpace(hasAuthorizeAttribute))
            {
                hasAuthorizeAttribute = HasAttribute(context.MethodInfo, typeof(HasAnyPermissionAttribute), true);
                if (string.IsNullOrWhiteSpace(hasAuthorizeAttribute))
                {
                    hasAuthorizeAttribute = HasAttribute(context.MethodInfo, typeof(HasAllPermissionAttribute), true);
                    if (string.IsNullOrWhiteSpace(hasAuthorizeAttribute))
                    {
                        return;
                    }
                }
            }

            operation.Summary = hasAuthorizeAttribute;
        }

        /// <summary>
        /// Method to get the attribute value of different permission filters
        /// </summary>
        /// <param name="methodInfo">Method info</param>
        /// <param name="type">Permission filter type</param>
        /// <param name="inherit">Whether to check inherited method also</param>
        /// <returns>Attribute value as string</returns>
        private string HasAttribute(MethodInfo methodInfo, Type type, bool inherit)
        {
            var actionAttributes = methodInfo.GetCustomAttributes(inherit);
            bool hasType = actionAttributes.Any(attr => attr.GetType() == type);
            if (hasType)
            {
                if (actionAttributes.SingleOrDefault(attr => attr.GetType() == type) is HasPermissionAttribute hasPermissionAttribute)
                {
                    return "[HasPermission: " + hasPermissionAttribute.Arguments[0].ToString() + "]";
                }

                if (type.Name == "HasAnyPermissionAttribute")
                {
                    HasAnyPermissionAttribute? hasAnyPermissionAttribute = actionAttributes.SingleOrDefault(attr => attr.GetType() == type) as HasAnyPermissionAttribute;
                    if (hasAnyPermissionAttribute?.Arguments[0] is PermissionEnum[] permissions)
                    {
                        return "[HasAnyPermission: " + string.Join(", ", permissions) + "]";
                    }
                }
                else
                {
                    HasAllPermissionAttribute? hasAllPermissionAttribute = actionAttributes.SingleOrDefault(attr => attr.GetType() == type) as HasAllPermissionAttribute;
                    if (hasAllPermissionAttribute?.Arguments[0] is PermissionEnum[] permissions)
                    {
                        return "[HasAllPermission: " + string.Join(", ", permissions) + "]";
                    }
                }
            }

            return string.Empty;
        }
    }
}