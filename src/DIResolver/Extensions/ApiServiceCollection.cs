//-----------------------------------------------------------------------
// <copyright file="ApiServiceCollection.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Extensions;

using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AspNetCoreRateLimit;
using AutoMapper;
using BoldDesk.Search.DIResolver.Objects;
using BoldDesk.Search.DIResolver.Objects.AppSettings;
using BoldDesk.Search.DIResolver.Objects.AutoMapperProfile;
using BoldDesk.Search.DIResolver.Objects.Settings;
using BoldDesk.Search.DIResolver.Services.Error;
using BoldDesk.Search.DIResolver.Services.General;
using BoldDesk.Search.DIResolver.Services.Logger;
using Humanizer.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Syncfusion.HelpDesk.Catalog.Objects;
using Syncfusion.HelpDesk.Core.LocalizationServices;
using Syncfusion.HelpDesk.Core.Objects.Hosting;
using Syncfusion.HelpDesk.Encryption;
using Syncfusion.HelpDesk.Logger;
using Syncfusion.HelpDesk.Multitenant;
using System.Configuration;

/// <summary>
/// Class for the service collection extensions.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Add necessary services to the container.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    /// <returns>Returns result as IServiceCollection.</returns>
    public static IServiceCollection AddApiServiceCollection(this IServiceCollection services)
    {
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddScoped<IUrlHelper>(x =>
        {
            var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
            var factory = x.GetRequiredService<IUrlHelperFactory>();
#pragma warning disable CS8604 // Possible null reference argument.
            return factory.GetUrlHelper(actionContext);
#pragma warning restore CS8604 // Possible null reference argument.
        });

        services.AddApiVersioningServices();
        services.AddHealthChecks();

        var serviceProvider = services.BuildServiceProvider();
        var appSettingsValue = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
        bool enableSwagger = false;
        string? enableSwaggerValue = appSettingsValue?.Value.EnableSwagger;
        if (!string.IsNullOrWhiteSpace(enableSwaggerValue))
        {
            enableSwagger = bool.Parse(enableSwaggerValue);
        }

        if (enableSwagger)
        {
            services.AddSwaggerGen();
        }

        return services;
    }

    /// <summary>
    /// Add Api versioning service.
    /// </summary>
    /// <param name="services">service collection.</param>
    /// <returns>Returns Iservice collection.</returns>
    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        var apiVersioningBuilder = services.AddApiVersioning(x =>
        {
            x.AssumeDefaultVersionWhenUnspecified = true;
            x.ReportApiVersions = true;
            x.DefaultApiVersion = new ApiVersion(1, 0);
        });

        apiVersioningBuilder.AddApiExplorer(x => x.GroupNameFormat = "'v'VVV");

        return services;
    }
}
