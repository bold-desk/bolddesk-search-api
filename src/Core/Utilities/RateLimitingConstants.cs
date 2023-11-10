// -----------------------------------------------------------------------
// <copyright file="RateLimitingConstants.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Utilities
{
    /// <summary>
    /// RateLimiting Constants
    /// </summary>
    public static class RateLimitingConstants
    {
        /// <summary>
        /// RateLimitingKey
        /// </summary>
        public const string RateLimitingKey = "Rule_";

        /// <summary>
        /// RateLimitingKey
        /// </summary>
        public const string SearchAPIRateLimitingKey = "Agent_API_Rule_Org_{OrgId}_VersionId_{FeatureVersionId}_UserId_{UserId}";

        /// <summary>
        /// RateLimitingKey
        /// </summary>
        public const string SearchAPITrialRateLimitingKey = "Trial_Agent_API_Rule_Org_{OrgId}";
    }
}