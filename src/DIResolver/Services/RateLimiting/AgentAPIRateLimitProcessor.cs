// -----------------------------------------------------------------------
// <copyright file="AgentAPIRateLimitProcessor.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Services;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using BoldDesk.Search.DIResolver.Objects.AppSettings;
using Microsoft.Extensions.Options;
using Syncfusion.HelpDesk.Core.Objects;

/// <summary>
/// AgentAPIRateLimitiProcessor - Class Declaration.
/// </summary>
public class AgentAPIRateLimitProcessor : RateLimitProcessor, IRateLimitProcessor
{
    private readonly ClientRateLimitOptions options;
    private readonly IRateLimitStore<ClientRateLimitPolicy> policyStore;
    private readonly IOptions<CustomPostRateLimitEndPoints> customPostEndPoints;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentAPIRateLimitProcessor"/> class.
    /// Agent API rate limit Processor.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="counterStore">Counter Store.</param>
    /// <param name="policyStore">Client Policy Store.</param>
    /// <param name="config">Rate Limit Configuration.</param>
    /// <param name="customPostEndPoints">Custom Post End Points.</param>
    public AgentAPIRateLimitProcessor(
       ClientRateLimitOptions options,
       IRateLimitCounterStore counterStore,
       IClientPolicyStore policyStore,
       IRateLimitConfiguration config,
       IOptions<CustomPostRateLimitEndPoints> customPostEndPoints)
    : base(options, counterStore, new ClientCounterKeyBuilder(options), config)
    {
        this.options = options;
        this.policyStore = policyStore;
        this.customPostEndPoints = customPostEndPoints;
    }

    /// <summary>
    /// Get Matching Rules.
    /// </summary>
    /// <param name="identity">Client Request Identity.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>Returns the list of matching rules for the API call.</returns>
    public async Task<IEnumerable<RateLimitRule>> GetMatchingRulesAsync(ClientRequestIdentity identity, CancellationToken cancellationToken = default)
    {
        var policy = await policyStore.GetAsync($"{options.ClientPolicyPrefix}_{identity.ClientId}", cancellationToken).ConfigureAwait(false);

        return GetMatchingRules(identity, policy?.Rules);
    }

    /// <summary>
    /// Get End Points List For Custom Http Verb.
    /// </summary>
    /// <returns>returns list of end points for custom http verb.</returns>
    public override List<string> GetEndPointsListForCustomHttpVerb()
    {
        return customPostEndPoints.Value.EndPoints?.Count > 0 ? customPostEndPoints.Value.EndPoints : new List<string>();
    }
}
