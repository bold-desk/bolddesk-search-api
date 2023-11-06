//-----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Class for the service collection extensions.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Get Configuration details from local json files like appSetting.json and apisettings.json.
    /// </summary>
    /// <param name="env">Hosting Environment.</param>
    /// <returns>Returns result as IConfiguration.</returns>
    public static IConfiguration GetApiConfiguration(this IWebHostEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env?.ContentRootPath)
            .AddJsonFile("apisettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"apisettings.{env?.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
