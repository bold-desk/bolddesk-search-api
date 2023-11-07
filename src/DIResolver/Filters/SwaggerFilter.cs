// -----------------------------------------------------------------------
// <copyright file="SwaggerFilter.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Filters;

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Swagger filter class.
/// </summary>
public class SwaggerFilter : IOperationFilter
{
    /// <summary>
    /// Apply method.
    /// </summary>
    /// <param name="operation">Operation.</param>
    /// <param name="context">Context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation == null)
        {
            return;
        }

        var oAuthScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
        };

        operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [oAuthScheme] = new string[] { "agent.api" }
                }
            };
    }
}
