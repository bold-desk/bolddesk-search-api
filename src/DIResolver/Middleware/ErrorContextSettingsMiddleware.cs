// -----------------------------------------------------------------------
// <copyright file="ErrorContextSettingsMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using BoldDesk.Search.Core.Objects.Common;
    using Microsoft.AspNetCore.Http;
    using Syncfusion.HelpDesk.Logger;
    using Syncfusion.HelpDesk.Multitenant;

    /// <summary>
    /// Error Context Settings Middleware.
    /// </summary>
    public class ErrorContextSettingsMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorContextSettingsMiddleware"/> class.
        /// </summary>
        /// <param name="next">Request Delegate Value.</param>
        public ErrorContextSettingsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invoke Method.
        /// </summary>
        /// <param name="context">HTTP Context.</param>
        /// <param name="errorContext">Error Context.</param>
        /// <param name="organization">Organization Info.</param>
        /// <param name="user">User Info.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task InvokeAsync(HttpContext context, IErrorContext errorContext, OrganizationInfo organization, UserInfo user)
        {
            if (context != null && errorContext != null)
            {
                GetCustomErrorDetailsFromOrganizationInfo(errorContext, organization);
                GetCustomErrorDetailsFromUserInfo(errorContext, user);
            }

            return next(context);
        }

        private void GetCustomErrorDetailsFromOrganizationInfo(IErrorContext errorContext, OrganizationInfo? organization)
        {
            if (organization is null)
            {
                return;
            }

            errorContext.ErrorDictionary ??= new Dictionary<string, string>();
            errorContext.ErrorDictionary.Add(nameof(OrganizationInfo.OrgId), organization.OrgId.ToString(CultureInfo.InvariantCulture));
            errorContext.ErrorDictionary.Add(nameof(OrganizationInfo.BrandId), organization.BrandId.ToString(CultureInfo.InvariantCulture));
            errorContext.ErrorDictionary.Add("IsTrial", organization.IsTrial.ToString(CultureInfo.InvariantCulture));
        }

        private void GetCustomErrorDetailsFromUserInfo(IErrorContext errorContext, UserInfo user)
        {
            if (user is null)
            {
                return;
            }

            errorContext.ErrorDictionary ??= new Dictionary<string, string>();
            errorContext.ErrorDictionary.Add(nameof(UserInfo.UserId), user.UserId.ToString(CultureInfo.InvariantCulture));
        }
    }
}
