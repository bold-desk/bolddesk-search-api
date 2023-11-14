//-----------------------------------------------------------------------
// <copyright file="SwaggerExtensions.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using BoldDesk.Search.DIResolver.Filters;
    using BoldDesk.Search.DIResolver.Objects.AppSettings;
    using MicroElements.Swashbuckle.FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.Filters;
    using Swashbuckle.AspNetCore.Swagger;

    /// <summary>
    /// Swagger Builder Extensions class.
    /// </summary>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Add swagger.
        /// </summary>
        /// <param name="services">Services.</param>
        [Obsolete]
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "BoldDesk Developer API Docs", Version = "1.0", Description = "Bold Desk is a Help Desk Support System for Enterprises. It is based on SaaS multi-tenant cloud-native architecture. Supports branding based customer portals with themes and settings customization. Built using the latest Microsoft and Cloud technology stack." });

                var serviceProvider = services.BuildServiceProvider().GetRequiredService<IOptions<AppSettings>>().Value;
                var idServerConfig = serviceProvider?.IdentityServerConfiguration;

                bool isValidUrl = !Uri.IsWellFormedUriString(idServerConfig?.AuthorizationUrl, UriKind.Relative);

                c.EnableAnnotations();
                c.ExampleFilters();

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "Oauth login",
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = isValidUrl ? new Uri(idServerConfig?.AuthorizationUrl ?? string.Empty) : new Uri(idServerConfig?.AuthorizationUrl ?? string.Empty, UriKind.Relative),
                            TokenUrl = isValidUrl ? new Uri(idServerConfig?.TokenUrl ?? string.Empty) : new Uri(idServerConfig?.TokenUrl ?? string.Empty, UriKind.Relative),
                            Scopes = new Dictionary<string, string>
                            {
                            { "agent.api", "Api full access" }
                            }
                        }
                    }
                });

                c.OperationFilter<SwaggerFilter>();
                c.OperationFilter<AddHasPermissionParameterOperationFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.DocInclusionPredicate((version, apiDescription) =>
                {
                    var values = apiDescription.RelativePath
                    .Split('/')
                    .Select(v => v.Replace("{version}", version, ignoreCase: false, CultureInfo.InvariantCulture));

                    apiDescription.RelativePath = apiDescription.RelativePath.Replace("{v}", version.Replace("v", string.Empty, StringComparison.InvariantCultureIgnoreCase), StringComparison.InvariantCultureIgnoreCase);

                    var versionParameter = apiDescription.ParameterDescriptions
                        .SingleOrDefault(p => p.Name == "v");

                    if (versionParameter == null)
                    {
                        return true;
                    }

                    apiDescription.ParameterDescriptions.Remove(versionParameter);

                    return true;
                });

                c.SchemaFilter<FluentValidationRules>();
                c.AddFluentValidationRules();
            });
        }

        /// <summary>
        /// Custom Swagger Middleware.
        /// </summary>
        /// <param name="app">Application builder.</param>
        public static void UseCustomSwagger(this IApplicationBuilder app)
        {
            var appSettingsValue = app?.ApplicationServices.GetService<IOptions<AppSettings>>()?.Value;
            var subpath = appSettingsValue?.ApiRoutePrefix;
            var oAuthClientId = appSettingsValue?.OAuthClientId ?? string.Empty;

            app.UseSwagger(
               c =>
               {
                   c.RouteTemplate = "/agent-api/help/swagger/{documentname}/swagger.json";
                   c.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
                   swaggerDoc.Servers = new List<OpenApiServer>
                   {
                       new OpenApiServer { Url = $"{httpRequest.Scheme}://{httpRequest.Host.Value}" }
                   });
               });

            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Working with Bold Desk API v1.0";
                c.SwaggerEndpoint(subpath + "/agent-api/help/swagger/v1.0/swagger.json", "API V1.0");
                c.RoutePrefix = "agent-api/help";
                c.OAuthClientId(oAuthClientId);
                c.OAuthAppName("Agent API");
                c.DisplayRequestDuration();
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.EnableFilter();
            });
        }
    }
}