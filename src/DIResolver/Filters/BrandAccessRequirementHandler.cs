//-----------------------------------------------------------------------
// <copyright file="BrandAccessRequirementHandler.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Filters
{
    using System.Threading.Tasks;
    using BoldDesk.Search.Core.Objects.Common;
    using BoldDesk.Search.DIResolver.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Syncfusion.HelpDesk.Core.ValidationErrors;
    using Syncfusion.HelpDesk.Multitenant;

    /// <summary>
    /// Brand Access Requirement Handler Class.
    /// </summary>
    public class BrandAccessRequirementHandler : AuthorizationHandler<BrandAccessRequirement>
    {
        private readonly OrganizationInfo organization;
        private readonly UserInfo user;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrandAccessRequirementHandler"/> class.
        /// </summary>
        /// <param name="organization">Organization.</param>
        /// <param name="user">Current User.</param>
        public BrandAccessRequirementHandler(OrganizationInfo organization, UserInfo user)
        {
            this.organization = organization;
            this.user = user;
        }

        /// <summary>
        /// Authorization Handler.
        /// </summary>
        /// <param name="context">Authorization Handler Context.</param>
        /// <param name="requirement">Brand Access Requirement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BrandAccessRequirement requirement)
        {
            var user = this.user.GetUserInfo(context?.User, organization);

            if (user.IsAuthenticated)
            {
                context?.Succeed(requirement);
            }
            else if (organization?.BrandId == -1)
            {
                if (context?.Resource is AuthorizationFilterContext resultContext)
                {
                    var response = resultContext.HttpContext.Response;

                    response?.OnStarting(() =>
                    {
                        resultContext.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                        return Task.CompletedTask;
                    });
                }
            }

            return Task.CompletedTask;
        }
    }
}
