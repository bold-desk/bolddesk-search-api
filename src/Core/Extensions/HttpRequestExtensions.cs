// -----------------------------------------------------------------------
// <copyright file="HttpRequestExtensions.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Extensions;

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Http Request Extensions - Class Declaration
/// </summary>
public static partial class CommonExtensions
{
    /// <summary>
    /// Gets the route value as route template which contains the placeholder by replacing the action parameters.
    /// </summary>
    /// <param name="request">Http Request.</param>
    /// <returns>A <see cref="string"/> containing the route template of the request.</returns>
    public static string GetTemplateRouteValue(this HttpRequest request)
    {
        if (request is null)
        {
            return string.Empty;
        }

        var pathValue = request.Path.Value;
        pathValue = GetTemplatePathValue(pathValue, request.RouteValues, "v", "{v:apiVersion}");
        pathValue = GetTemplatePathValue(pathValue, request.RouteValues, "TICKETID", "{ticketId}");
        pathValue = GetTemplatePathValue(pathValue, request.RouteValues, "UPDATEID", "{updateid}");

        return pathValue;
    }

    private static string GetTemplatePathValue(string pathValue, RouteValueDictionary routeValues, string routeKeyValue, string routetemplateValue)
    {
        var routeValue = GetValue(routeValues, routeKeyValue);

        if (!string.IsNullOrWhiteSpace(routeValue))
        {
            pathValue = pathValue.Replace(routeValue, routetemplateValue, StringComparison.OrdinalIgnoreCase);
        }

        return pathValue;
    }

    private static string GetValue(RouteValueDictionary routeValues, string key)
    {
        if (routeValues.TryGetValue(key, out object? value))
        {
            return value?.ToString() ?? string.Empty;
        }

        return string.Empty;
    }
}