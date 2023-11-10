// -----------------------------------------------------------------------
// <copyright file="AgentAPIClientRateLimitMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware;

using System;
using System.Collections.Generic;
using System.Text;
using AspNetCoreRateLimit;
using BoldDesk.Search.DIResolver.Objects.AppSettings;
using BoldDesk.Search.DIResolver.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// AgentAPIClientRateLimitMiddleware - Client Rate limit Middleware
/// </summary>
public class AgentAPIClientRateLimitMiddleware : RateLimitMiddleware<AgentAPIRateLimitProcessor>
{
    private readonly ILogger<AgentAPIClientRateLimitMiddleware> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentAPIClientRateLimitMiddleware"/> class.
    /// </summary>
    /// <param name="next">Request Delegate.</param>
    /// <param name="options">Options.</param>
    /// <param name="counterStore">Rate limit counter store.</param>
    /// <param name="policyStore">Policy store.</param>
    /// <param name="config">Rate limit configuration.</param>
    /// <param name="logger">Logger.</param>
    /// <param name="customPostEndPoints">Custom End Points.</param>
    public AgentAPIClientRateLimitMiddleware(
        RequestDelegate next,
        IOptions<ClientRateLimitOptions> options,
        IRateLimitCounterStore counterStore,
        IClientPolicyStore policyStore,
        IRateLimitConfiguration config,
        ILogger<AgentAPIClientRateLimitMiddleware> logger,
        IOptions<CustomPostRateLimitEndPoints> customPostEndPoints)
    : base(next, options?.Value, new AgentAPIRateLimitProcessor(options?.Value ?? new ClientRateLimitOptions(), counterStore, policyStore, config, customPostEndPoints), config)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Log Blocked Request
    /// </summary>
    /// <param name="httpContext">Http Context.</param>
    /// <param name="identity">Identity.</param>
    /// <param name="counter">Rate limit count.</param>
    /// <param name="rule">Rules.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "RCS1198:Avoid unnecessary boxing of value type.", Justification = "OK")]
    protected override void LogBlockedRequest(HttpContext httpContext, ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule)
    {
        if (httpContext == null || identity == null || rule == null)
        {
            return;
        }

        logger.LogInformation($"Request {identity.HttpVerb}:{identity.Path} from ClientId {identity.ClientId} has been blocked, quota {rule.Limit}/{rule.Period} exceeded by {counter.Count - rule.Limit}. Blocked by rule {rule.Endpoint}, TraceIdentifier {httpContext.TraceIdentifier}.");
    }
}
