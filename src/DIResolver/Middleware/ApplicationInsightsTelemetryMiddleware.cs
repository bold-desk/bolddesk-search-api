// -----------------------------------------------------------------------
// <copyright file="ApplicationInsightsTelemetryMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;
    using BoldDesk.Search.Core.Objects;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Http;
    using Syncfusion.HelpDesk.Core.Enums;
    using Syncfusion.HelpDesk.Core.Objects;
    using Syncfusion.HelpDesk.Logger;
    using Syncfusion.HelpDesk.Multitenant;

    /// <summary>
    /// Send Telemetry data to Application Insights
    /// </summary>
    public class ApplicationInsightsTelemetryMiddleware
    {
        private const string EventName = "Middleware_tracking";
        private readonly RequestDelegate next;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInsightsTelemetryMiddleware"/> class.
        /// Creates a new <see cref="ApplicationInsightsTelemetryMiddleware"/>.
        /// </summary>
        /// <param name="next">Next</param>
        /// <param name="telemetryClient">TelemetryClient</param>
        public ApplicationInsightsTelemetryMiddleware(RequestDelegate next, TelemetryClient telemetryClient)
        {
            this.next = next;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Invoke method.
        /// </summary>
        /// <param name="httpContext">httpContext</param>
        /// <param name="organization">Organisation information</param>
        /// <param name="user">User information</param>
        /// <param name="requestDetails">Request Details</param>
        /// <param name="logs">Logger service.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext httpContext, OrganizationInfo organization, UserInfo user, RequestDetails requestDetails, ILogs logs)
        {
            if (httpContext == null || organization == null || organization.OrgId <= 0 || requestDetails == null)
            {
                return;
            }

            Dictionary<string, string> data = new Dictionary<string, string>();

            try
            {
                if (user?.UserId == 0 || user?.UserId == null)
                {
                    telemetryClient.Context.User.Id = null;
                    data.Add("UserId", "0");
                }
                else
                {
                    telemetryClient.Context.User.Id = user.UserId.ToString(CultureInfo.InvariantCulture); // Sets UserId
                    data.Add("UserId", user.UserId.ToString(CultureInfo.InvariantCulture));
                }

                data.Add("OrgId", organization.OrgId.ToString(CultureInfo.InvariantCulture));
                data.Add("OrgBrandId", organization.OrgBrandId.ToString(CultureInfo.InvariantCulture));
                data.Add("BrandId", organization.BrandId.ToString(CultureInfo.InvariantCulture));

                string? sourceName = Enum.GetName(typeof(TicketSourceEnum), requestDetails.SourceId);
                if (sourceName != null)
                {
                    data.Add("SourceName", sourceName);
                }
            }
            catch (Exception exception)
            {
                logs?.LogError(new LogObjects()
                {
                    Exception = exception,
                    FileName = exception.TargetSite?.DeclaringType?.Name ?? string.Empty,
                    MethodName = exception.TargetSite?.Name ?? string.Empty,
                    Source = exception.Source ?? string.Empty,
                    Tag = nameof(ApplicationTypeEnum.AgentPortal)
                });
            }
            finally
            {
                await next(httpContext).ConfigureAwait(false); // calling next middleware
            }

            if (httpContext.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
            {
                data.Add("StatusCode", httpContext.Response.StatusCode.ToString(CultureInfo.InvariantCulture));
            }

            if (data.Count <= 0)
            {
                return;
            }

            telemetryClient.TrackEvent(EventName, data);
        }
    }
}
