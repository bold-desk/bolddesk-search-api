// -----------------------------------------------------------------------
// <copyright file="ApplicationInsightsCustomFilter.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Filters
{
    using System;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// Application Insights Custom Filter class.
    /// </summary>
    public class ApplicationInsightsCustomFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInsightsCustomFilter"/> class.
        /// </summary>
        /// <param name="next">Request Delegate Value</param>
        public ApplicationInsightsCustomFilter(ITelemetryProcessor next)
        {
            this.next = next;
        }

        /// <summary>
        /// Process method
        /// </summary>
        /// <param name="item">ITelemetry item</param>
        public void Process(ITelemetry item)
        {
            RequestTelemetry? telemetry = item as RequestTelemetry;
            if (telemetry?.Url.AbsoluteUri.Contains("/health", StringComparison.InvariantCultureIgnoreCase) == true || telemetry?.ResponseCode == "400" || telemetry?.ResponseCode == "404")
            {
                return;
            }

            next.Process(item);
        }
    }
}