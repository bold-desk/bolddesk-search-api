//-----------------------------------------------------------------------
// <copyright file="ApplicationBuilderExtensions.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Extensions;

using BoldDesk.Search.DIResolver.Middleware;
using BoldDesk.Search.DIResolver.Objects;
using BoldDesk.Search.DIResolver.Objects.AppSettings;
using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Syncfusion.HelpDesk.Core.Middlewares;
using Syncfusion.HelpDesk.Core.Objects.Hosting;
using Syncfusion.HelpDesk.Encryption;
using Syncfusion.HelpDesk.Multitenant.Builder;
using Syncfusion.HelpDesk.Multitenant;
using Microsoft.AspNetCore.Hosting;

/// <summary>
/// class for application builder Service
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns result as IApplicationBuilder.</returns>
    public static IApplicationBuilder BuildApplication(this WebApplication app)
    {
        var appSettings = app.Services.GetService<IOptions<AppSettings>>();
        bool enableHTTPS = true;
        string? enableHTTPSValue = appSettings?.Value.EnableHTTPS;
        if (!string.IsNullOrWhiteSpace(enableHTTPSValue))
        {
            enableHTTPS = bool.Parse(enableHTTPSValue);
        }

        app.UseRequestHeaderLogging();
        app.UseSecurityHeaders();
        app.UseHTTPSAndReverseProxyHost(enableHTTPS);
        app.UseResponseHeaderMiddleware();
        app.UseDecryptorService();
        app.UseExceptionlessClientConfiguration();
        app.UseSyncfusionLicenseKey();

        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment(HostingEnvironmentName.IntegrationTesting))
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            if (enableHTTPS)
            {
                app.UseHsts();
            }
        }

        app.UseCustomExceptionHandler();
        app.UseFluentValidationExceptionHandler();
        app.UseHealthChecks("/health");

        string? subpath = appSettings?.Value.ApiRoutePrefix;
        if (!string.IsNullOrWhiteSpace(subpath))
        {
            app.UseApiRoutePrefix(subpath);
        }

        bool enableSwagger = false;
        string? enableSwaggerValue = appSettings?.Value.EnableSwagger;
        if (!string.IsNullOrWhiteSpace(enableSwaggerValue))
        {
            enableSwagger = bool.Parse(enableSwaggerValue);
        }

        if (enableSwagger)
        {
            app.UseCustomSwagger();
        }

        app.UseRouting();

        var corsValue = app.Services.GetRequiredService<IOptions<CORSPolicy>>().Value;
        var whitelistedDomainsValue = corsValue?.WhitelistedDomains;
        if (whitelistedDomainsValue?.Length > 0)
        {
            app.UseCors("CorsPolicyWhitelistedDomains");
        }
        else
        {
            app.UseCors();
        }

        app.UseMultitenancy<OrganizationInfo>();
        app.UseUnresolvedTenantHandler();

        app.UseWhitelistedIP();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseUserTypeValidator();

        app.OrganizationStatusValidator();
        app.UseOrganizationSettings();

        if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment(HostingEnvironmentName.IntegrationTesting))
        {
            app.UseErrorContextSettings();
        }

        app.UseApplicationInsightsTelemetry(app.Environment); // Application insights

        // Get the Rate Limiting Rules from Redis Cache.
        app.UseAgentAPICustomRateLimitingRedisCache();
        app.UseAgentAPIClientRateLimiting();
        app.UseTenantLocalization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        return app;
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure Host value based on Reverse Proxy in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder? UseRequestHeaderLogging(this IApplicationBuilder app)
    {
        var appSettingsValue = app.ApplicationServices.GetService<IOptions<AppSettings>>();
        bool enableRequestHeadersLogging = false;
        string? enableRequestHeadersLoggingValue = appSettingsValue?.Value.EnableRequestHeadersLogging;
        if (!string.IsNullOrWhiteSpace(enableRequestHeadersLoggingValue))
        {
            enableRequestHeadersLogging = bool.Parse(enableRequestHeadersLoggingValue);
        }

        if (enableRequestHeadersLogging)
        {
            try
            {
                app.Use(async (context, next) =>
                {
                    string headers = string.Empty;
                    foreach (var key in context.Request.Headers.Keys)
                    {
                        headers += key + "==>" + context.Request.Headers[key] + Environment.NewLine;
                    }

                    Console.WriteLine("Request Headers Log from Search API:");
                    Console.WriteLine(headers);
                    await next().ConfigureAwait(false);
                });
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception from in-line middleware in Search API Startup class for getting Request Headers: " + Environment.NewLine + "Error message: " + exp.Message + Environment.NewLine + "Inner exception: " + exp.InnerException);
            }
        }

        return app;
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure Host value based on Reverse Proxy in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <param name="enableHTTPS">enable HTTPS.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder? UseHTTPSAndReverseProxyHost(this IApplicationBuilder app, bool enableHTTPS)
    {
        app.UseMiddleware<ReverseProxyHostMiddleware>();

        if (enableHTTPS)
        {
            app.UseEnforceHTTPS();
            app.UseHttpsRedirection();
        }

        return app;
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to add Security headers in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        // if you want to update the Content-Security-Policy or Cache-Control or Access-Control-Allow-Origin security headers in Core project,
        // just pass that string value in below extension method call as an argument. Else default value will be applied for those security headers.
        // for example, if you want to update Content-Security-Policy alone, do like this => app.UseMiddleware<SecurityHeadersMiddleware>(cspStringValue);
        // if you want to update Cache-Control alone, do like this => app.UseMiddleware<SecurityHeadersMiddleware>(string.Empty, cacheControlStringValue);
        // if you want to update Access-Control-Allow-Origin alone, do like this => app.UseMiddleware<SecurityHeadersMiddleware>(string.Empty, string.Empty, allowOriginStringValue);
        // if you want to update all, do like this => app.UseMiddleware<SecurityHeadersMiddleware>(cspStringValue, cacheControlStringValue, allowOriginStringValue);
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure Request Header Scheme.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseEnforceHTTPS(this IApplicationBuilder app)
    {
        return app.UseMiddleware<EnforceHTTPSMiddleware>();
    }

    /// <summary>
    /// UseResponseHeaderMiddleware
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder? UseResponseHeaderMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ResponseHeaderHandlerMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to decrypt the encrypted values in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns result as IApplicationBuilder.</returns>
    public static IApplicationBuilder UseDecryptorService(this IApplicationBuilder app)
    {
        app.UseDecryptorAppSettings();
        app.UseDecryptorServiceBusSettings();

        return app;
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to decrypt the encrypted values in appSettings.json file in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns result as IApplicationBuilder.</returns>
    public static IApplicationBuilder? UseDecryptorAppSettings(this IApplicationBuilder app)
    {
        var rijndaelEncryption = app.ApplicationServices.GetService<IRijndaelEncryption>();
        var appSettingsValue = app?.ApplicationServices.GetService<IOptions<AppSettings>>()?.Value;

        if (appSettingsValue != null)
        {
            appSettingsValue.ExceptionLess.ApiKey = rijndaelEncryption?.Decrypt(appSettingsValue.ExceptionLess.ApiKey) ?? string.Empty;
            appSettingsValue.ExceptionLess.ServerName = rijndaelEncryption?.Decrypt(appSettingsValue.ExceptionLess.ServerName) ?? string.Empty;
            appSettingsValue.ConnectionStrings.CatalogEntity = rijndaelEncryption?.Decrypt(appSettingsValue.ConnectionStrings.CatalogEntity) ?? string.Empty;
            appSettingsValue.RedisConnection = rijndaelEncryption?.Decrypt(appSettingsValue.RedisConnection) ?? string.Empty;
            appSettingsValue.SyncfusionLicense.Key = rijndaelEncryption?.Decrypt(appSettingsValue.SyncfusionLicense.Key) ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(appSettingsValue.OAuthClientId))
            {
                appSettingsValue.OAuthClientId = rijndaelEncryption?.Decrypt(appSettingsValue.OAuthClientId) ?? string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(appSettingsValue.OAuthValidAudience))
            {
                appSettingsValue.OAuthValidAudience = rijndaelEncryption?.Decrypt(appSettingsValue.OAuthValidAudience) ?? string.Empty;
            }
        }

        return app;
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to decrypt the encrypted values for Service Bus Settings in appSettings.json file in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns result as IApplicationBuilder.</returns>
    public static IApplicationBuilder? UseDecryptorServiceBusSettings(this IApplicationBuilder app)
    {
        var rijndaelEncryption = app.ApplicationServices.GetService<IRijndaelEncryption>();
        var serviceBusSettingsValue = app?.ApplicationServices.GetService<IOptions<ServiceBusSettings>>()?.Value;

        if (serviceBusSettingsValue != null)
        {
            serviceBusSettingsValue.ConnectionString = rijndaelEncryption?.Decrypt(serviceBusSettingsValue.ConnectionString) ?? string.Empty;
        }

        return app;
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure Exceptionless Client configuration in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns result as IApplicationBuilder.</returns>
    public static IApplicationBuilder? UseExceptionlessClientConfiguration(this IApplicationBuilder app)
    {
        var appSettingsValue = app?.ApplicationServices.GetService<IOptions<AppSettings>>();
        var exceptionLess = appSettingsValue?.Value.ExceptionLess;

        ExceptionlessClient.Default.Configuration.ApiKey = exceptionLess?.ApiKey ?? string.Empty;
        ExceptionlessClient.Default.Configuration.ServerUrl = exceptionLess?.ServerName ?? string.Empty;

        return app;
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure Syncfusion License Key in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder? UseSyncfusionLicenseKey(this IApplicationBuilder app)
    {
        var appSettingsValue = app?.ApplicationServices.GetService<IOptions<AppSettings>>();
        var syncfusionLicense = appSettingsValue?.Value.SyncfusionLicense;

        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionLicense?.Key);

        return app;
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure Custom Exception Handler Globally in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure Custom Exception Handler Globally in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseFluentValidationExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<FluentValidationExceptionHandlerMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure Api Route Prefix in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <param name="subpath">Sub path value.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseApiRoutePrefix(this IApplicationBuilder app, string subpath)
    {
        return app.UseMiddleware<ApiRoutePrefixMiddleware>(subpath);
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure whitelisted IP.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseWhitelistedIP(this IApplicationBuilder app)
    {
        return app.UseMiddleware<WhitelistedIPMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to validate User type of the user logged in.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseUserTypeValidator(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserTypeValidatorMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to restrict some APIs based on organization Status.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder OrganizationStatusValidator(this IApplicationBuilder app)
    {
        return app.UseMiddleware<OrganizationStatusMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure refresh the organization settings globally in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseOrganizationSettings(this IApplicationBuilder app)
    {
        return app.UseMiddleware<OrganizationSettingsMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to configure Error Context settings in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseErrorContextSettings(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorContextSettingsMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to gather additional insights.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <param name="env">Environment value</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseApplicationInsightsTelemetry(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment() && !env.IsEnvironment(HostingEnvironmentName.IntegrationTesting))
        {
            return app.UseMiddleware<ApplicationInsightsTelemetryMiddleware>();
        }

        return app;
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to apply limit for the API usage.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseAgentAPICustomRateLimitingRedisCache(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AgentApiCustomRateLimitingMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to apply limit for the API usage.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder UseAgentAPIClientRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AgentAPIClientRateLimitMiddleware>();
    }

    /// <summary>
    /// This is an extension method of Configure(). Used to create cloud storage for the tenant in the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Application Builder.</param>
    /// <returns>Returns Application Builder.</returns>
    public static IApplicationBuilder? UseTenantLocalization(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantRequestLocalizationMiddleware>();
    }
}
