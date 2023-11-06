//-----------------------------------------------------------------------
// <copyright file="ApplicationBuilderExtensions.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

/// <summary>
/// class for application builder Service
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <param name="configuration">configuration details.</param>
    /// <returns>Returns result as IApplicationBuilder.</returns>
    public static IApplicationBuilder BuildApplication(this WebApplication app, IConfiguration configuration)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
