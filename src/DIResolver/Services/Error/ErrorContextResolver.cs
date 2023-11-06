// -----------------------------------------------------------------------
// <copyright file="ErrorContextResolver.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Services.Error;

using Microsoft.AspNetCore.Http;
using Syncfusion.HelpDesk.Catalog.Data.Entity;
using Syncfusion.HelpDesk.Core.Enums;
using Syncfusion.HelpDesk.Core.Objects;
using Syncfusion.HelpDesk.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Error Context Details Class.
/// </summary>
public class ErrorContextResolver : IErrorContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorContextResolver"/> class.
    /// </summary>
    /// <param name="contextAccessor"> Context Accessor.</param>
    /// <param name="requestDetails"> Request Details.</param>
    public ErrorContextResolver(IHttpContextAccessor contextAccessor, RequestDetails requestDetails)
    {
        GetCustomErrorDetailsFromContext(contextAccessor?.HttpContext);
        GetCustomErrorDetailsFromRequestDetails(requestDetails);
    }

    /// <summary>
    /// Gets or sets
    /// </summary>
    public IDictionary<string, string>? ErrorDictionary { get; set; }

    private void GetCustomErrorDetailsFromContext(HttpContext? context)
    {
        if (context is null)
        {
            return;
        }

        ErrorDictionary ??= new Dictionary<string, string>();
        ErrorDictionary.Add(nameof(HttpMethods), context.Request.Method);
        ErrorDictionary.Add(nameof(context.Request.QueryString), context.Request.QueryString.Value.ToString(CultureInfo.InvariantCulture));
        ErrorDictionary.Add(nameof(context.Request.RouteValues), context.Request.Path.Value.ToString(CultureInfo.InvariantCulture));
        ErrorDictionary.Add(nameof(context.Request.Host), context.Request.Host.Value.ToString(CultureInfo.InvariantCulture));
    }

    private void GetCustomErrorDetailsFromRequestDetails(RequestDetails requestDetails)
    {
        if (requestDetails is null)
        {
            return;
        }

        ErrorDictionary ??= new Dictionary<string, string>();
        ErrorDictionary.Add(nameof(RequestDetails.IpAddress), requestDetails.IpAddress ?? string.Empty);
        ErrorDictionary.Add(nameof(Syncfusion.HelpDesk.Organization.Data.Entity.TicketSource), ((TicketSourceEnum)requestDetails.SourceId).ToString());
    }
}
