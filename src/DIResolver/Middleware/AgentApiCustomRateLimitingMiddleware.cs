// -----------------------------------------------------------------------
// <copyright file="AgentApiCustomRateLimitingMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using BoldDesk.Search.Core.Objects;
using BoldDesk.Search.Core.Utilities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Syncfusion.Caching.Services;
using Syncfusion.HelpDesk.Catalog.Constants;
using Syncfusion.HelpDesk.Catalog.Services.FeatureManagement;
using Syncfusion.HelpDesk.Core.Enums;
using Syncfusion.HelpDesk.Core.Objects;
using Syncfusion.HelpDesk.Multitenant;
using Syncfusion.HelpDesk.Multitenant.Enums;

/// <summary>
/// Agent API Custom Rate Limiting Redis Cache
/// </summary>
public class AgentApiCustomRateLimitingMiddleware
{
    private readonly RequestDelegate next;

    private readonly IRateLimitStore<ClientRateLimitPolicy> policyStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentApiCustomRateLimitingMiddleware"/> class.
    /// </summary>
    /// <param name="next">Next</param>
    /// <param name="policyStore">Policy Store</param>
    public AgentApiCustomRateLimitingMiddleware(RequestDelegate next, IClientPolicyStore policyStore)
    {
        this.next = next;
        this.policyStore = policyStore;
    }

    /// <summary>
    /// Invoke method.
    /// </summary>
    /// <param name="httpContext">HttpContext</param>
    /// <param name="cacheService">CacheService</param>
    /// <param name="organization">Organization</param>
    /// <param name="featureManagementService"> featureManagementService. </param>
    /// <param name="user">User Info.</param>
    /// <param name="requestDetails">Request Details.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext httpContext, IDistributedCacheService cacheService, OrganizationInfo organization, IFeatureManagementService featureManagementService, UserInfo user, RequestDetails requestDetails)
    {
        if (httpContext == null || cacheService == null || organization == null || organization.OrgId <= 0 || featureManagementService == null || requestDetails == null)
        {
            return;
        }

        if (user?.UserId > 0 && httpContext.User.Identity.IsAuthenticated && requestDetails.SourceId == (int)TicketSourceEnum.AgentPortal)
        {
            var isTrial = organization.IsTrial || organization.StatusId == (int)OrganizationStatusEnum.Trial;

            string rateLimitingOrgId = isTrial ? RateLimitingConstants.SearchAPITrialRateLimitingKey
                : RateLimitingConstants.SearchAPIRateLimitingKey;

            rateLimitingOrgId = rateLimitingOrgId.Replace("{OrgId}", organization.OrgId.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase)
                .Replace("{FeatureVersionId}", organization.FeatureVersionId.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase)
                .Replace("{UserId}", user.UserId.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase);

            string? agentApiRateLimitRule = string.Empty;
            bool isFetchedFromCache = false;

            var rateLimitingRule = await policyStore.GetAsync($"crlp_{rateLimitingOrgId}").ConfigureAwait(false);

            if (rateLimitingRule == null)
            {
                ClientRateLimitPolicy clientRateLimit = new ClientRateLimitPolicy { ClientId = rateLimitingOrgId };
                agentApiRateLimitRule = isTrial ? await featureManagementService.GetValueBasedOnFeatureKeyAsync(FeatureKeyConstants.AgentApiRateOrgTrialLimit, organization.OrgId, featureVersionId: null).ConfigureAwait(false)
                    : await featureManagementService.GetValueBasedOnFeatureKeyAsync(FeatureKeyConstants.AgentApiUserRateLimit, organization.OrgId, organization.FeatureVersionId).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(agentApiRateLimitRule))
                {
                    clientRateLimit.Rules = agentApiRateLimitRule != "{}" ? JsonConvert.DeserializeObject<List<RateLimitRule>>(agentApiRateLimitRule) : new List<RateLimitRule>();

                    // Add Rate Limit Rule cache with 6 hours expiry.
                    await policyStore.SetAsync($"crlp_{rateLimitingOrgId}", clientRateLimit, new TimeSpan(6, 0, 0)).ConfigureAwait(false);
                }
            }
            else
            {
                isFetchedFromCache = true;
            }

            if ((isFetchedFromCache && (rateLimitingRule?.Rules.Any(x => x != new RateLimitRule()) == true))
                || (!isFetchedFromCache && (!string.IsNullOrWhiteSpace(agentApiRateLimitRule) && agentApiRateLimitRule != "{}")))
            {
                httpContext.Request.Headers["X-ClientId"] = rateLimitingOrgId; // Assign RateLimitingOrgId value to 'X-ClientId' header for API Rate Limiting process.
            }
        }

        await next(httpContext).ConfigureAwait(false); // calling next middleware
    }
}