// -----------------------------------------------------------------------
// <copyright file="OrganizationStatusMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using BoldDesk.Search.DIResolver.Extensions.Core;
    using BoldDesk.Search.Localization.Resources;
    using BoldDesk.Search.Localization.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Localization;
    using Newtonsoft.Json;
    using Syncfusion.HelpDesk.Multitenant;
    using Syncfusion.HelpDesk.Multitenant.Enums;

    /// <summary>
    /// OrganizationStatusMiddleware - Restricting API based on Organization Status.
    /// </summary>
    public class OrganizationStatusMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationStatusMiddleware"/> class.
        /// </summary>
        /// <param name="next">Request Delegate Value.</param>
        public OrganizationStatusMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invoke Method.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="localizer">Agent API localizer.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, ILocalizer localizer)
        {
            if (context == null || localizer == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var tenantContext = context.GetTenantContext<OrganizationInfo>();

            if (tenantContext?.Tenant.OrgId > 0)
            {
                var currentOrgStatusId = tenantContext.Tenant.StatusId;

                var httpMethod = context.Request.Method;

                //// Trial Expired and Active and Trial Suspended API restrictions.
                if (string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase) && (tenantContext.Tenant.StatusId.HasValue
                    && (tenantContext.Tenant.StatusId.Value == (int)OrganizationStatusEnum.Suspended || (tenantContext.Tenant.StatusId.Value == (int)OrganizationStatusEnum.Expired && tenantContext.Tenant.IsTrial))))
                {
                    var pathValue = context.Request?.GetTemplateRouteValue() ?? string.Empty;
                    bool isPathRestricted = false;

                    if (tenantContext.Tenant.IsTrial && tenantContext.Tenant.StatusId.Value == (int)OrganizationStatusEnum.Suspended)
                    {
                        isPathRestricted = IsPathRestricted(RestrictedPathsForTrialSuspended(), pathValue);
                    }

                    if ((tenantContext.Tenant.StatusId.Value == (int)OrganizationStatusEnum.Suspended && !tenantContext.Tenant.IsTrial)
                        || (tenantContext.Tenant.StatusId.Value == (int)OrganizationStatusEnum.Expired && tenantContext.Tenant.IsTrial))
                    {
                        isPathRestricted = IsPathRestricted(RestrictedPathsForActivePlanSuspendedOrTrialPlanExpired(), pathValue);
                    }

                    if (isPathRestricted)
                    {
                        GetHttpContextForAgentPortal(context, tenantContext.Tenant.IsTrial, localizer);
                        return;
                    }
                }
            }

            await next(context).ConfigureAwait(false);
        }

        private void GetHttpContextForAgentPortal(HttpContext context, bool isTrial, ILocalizer localizer)
        {
            string errorMessage = isTrial ? localizer.GetLocalizerValueForSpecifiedLanguage("en-US", ResourceConstants.TrialExpiredOrSuspended) : localizer.GetLocalizerValueForSpecifiedLanguage("en-US", ResourceConstants.ActiveSuspended);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Items.Add("errorMessage", errorMessage);
        }

        private List<string> RestrictedPathsForTrialSuspended()
        {
            // Restricting ticket creation, update reply and update note process from email, agent portal and developer API.
            return new List<string>()
            {
                "/agent-api/v{v:apiVersion}/tickets/create",
                "/agent-api/v{v:apiVersion}/email/tickets/create",
                "/agent-api/v{v:apiVersion}/email/tickets/{ticketId}/notes",
                "/agent-api/v{v:apiVersion}/email/tickets/{ticketId}/updates",
                "/agent-api/v{v:apiVersion}/tickets/{ticketId}/updates",
                "/agent-api/v{v:apiVersion}/tickets/{ticketId}/notes",
                "/agent-api/v{v:apiVersion}/tickets/split_ticket/{updateid}",
                "/agent-api/v{v:apiVersion}/tickets/move_reply_to_public/{updateid}",
                "/api/v{v:apiVersion}/tickets/create",
                "/api/v{v:apiVersion}/tickets",
                "/api/v{v:apiVersion}/tickets/{ticketId}/updates",
                "/api/v{v:apiVersion}/tickets/{ticketId}/notes"
            };
        }

        private List<string> RestrictedPathsForActivePlanSuspendedOrTrialPlanExpired()
        {
            // Restricting ticket creation, update reply and update note process from agent portal and developer API.
            return new List<string>()
            {
                "/agent-api/v{v:apiVersion}/tickets/create",
                "/agent-api/v{v:apiVersion}/tickets/{ticketId}/updates",
                "/agent-api/v{v:apiVersion}/tickets/{ticketId}/notes",
                "/agent-api/v{v:apiVersion}/tickets/split_ticket/{updateid}",
                "/agent-api/v{v:apiVersion}/tickets/move_reply_to_public/{updateid}",
                "/api/v{v:apiVersion}/tickets/create",
                "/api/v{v:apiVersion}/tickets",
                "/api/v{v:apiVersion}/tickets/{ticketId}/updates",
                "/api/v{v:apiVersion}/tickets/{ticketId}/notes"
            };
        }

        private bool IsPathRestricted(List<string> restrictedPathvalues, string pathValue)
        {
            return restrictedPathvalues.Any(x => string.Equals(pathValue, x, StringComparison.OrdinalIgnoreCase));
        }
    }
}
