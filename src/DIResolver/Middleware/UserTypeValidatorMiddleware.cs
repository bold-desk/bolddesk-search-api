// -----------------------------------------------------------------------
// <copyright file="UserTypeValidatorMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using BoldDesk.Search.Core.Objects;
    using BoldDesk.Search.Localization.Services;
    using Microsoft.AspNetCore.Http;
    using Syncfusion.HelpDesk.Core.CustomException;
    using Syncfusion.HelpDesk.Core.Enums;
    using Syncfusion.HelpDesk.Core.Localization;

    /// <summary>
    /// UserTypeValidatorMiddleware - Middleware for validating user type validation.
    /// </summary>
    public class UserTypeValidatorMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTypeValidatorMiddleware"/> class.
        /// </summary>
        /// <param name="next">Request Delegate Value.</param>
        public UserTypeValidatorMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invoke method for validating user type logged in.
        /// </summary>
        /// <param name="context">Http context.</param>
        /// <param name="user">Logged in User info.</param>
        /// <param name="localizer">Localizer.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task InvokeAsync(HttpContext context, UserInfo user, ILocalizer localizer)
        {
            if (context != null)
            {
                var errorMessage = localizer != null ? localizer.GetBaseLocalizerValue(SharedResourceConstants.UnknownError) : string.Empty;

                if (!context.User.Identity.IsAuthenticated || (context.User.Identity.IsAuthenticated && user?.UserTypeId == (int)UserTypeEnum.Agent))
                {
                    return next(context);
                }

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }

            return Task.FromResult(context);
        }
    }
}
