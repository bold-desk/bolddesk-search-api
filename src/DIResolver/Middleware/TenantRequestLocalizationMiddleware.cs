// -----------------------------------------------------------------------
// <copyright file="TenantRequestLocalizationMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoldDesk.Search.Core.Objects.Common;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Syncfusion.HelpDesk.Core.Enums;
using Syncfusion.HelpDesk.Core.Language;
using Syncfusion.HelpDesk.Core.LocalizationServices;
using Syncfusion.HelpDesk.Core.Objects;
using Syncfusion.HelpDesk.Multitenant;

/// <summary>
/// Tenant Requst Localization Middleware
/// </summary>
public class TenantRequestLocalizationMiddleware
{
    private readonly RequestDelegate next;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantRequestLocalizationMiddleware"/> class.
    /// Creates a new <see cref="TenantRequestLocalizationMiddleware"/>.
    /// </summary>
    /// <param name="next">The <see cref="RequestDelegate"/> representing the next middleware in the pipeline.</param>
    public TenantRequestLocalizationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    /// <summary>
    /// Invokes the logic of the middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <param name="organization">Organization Info.</param>
    /// <param name="userInfo">User Info.</param>
    /// <param name="localizationService">Localization Service.</param>
    /// <param name="requestDetails">Request Details.</param>
    /// <returns>A <see cref="Task"/> that completes when the middleware has completed processing.</returns>
    public Task Invoke(HttpContext context, OrganizationInfo organization, UserInfo userInfo, ILocalizationService localizationService, RequestDetails requestDetails)
    {
        if (context != null && organization != null && userInfo != null && localizationService != null && requestDetails != null)
        {
            var defaultLanguage = string.Empty;
            if (requestDetails.SourceId == (int)TicketSourceEnum.MobileApp)
            {
                defaultLanguage = "en-Us";
            }
            else
            {
                defaultLanguage = GetDefaultLanguage(organization, userInfo);
            }

            CultureInfo? cultureInfo = new CultureInfo(defaultLanguage);
            CultureInfo? uiCultureInfo = new CultureInfo(defaultLanguage);
            ValidatorOptions.Global.LanguageManager.Enabled = true;

            context.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(new RequestCulture(cultureInfo, uiCultureInfo), new CookieRequestCultureProvider()));

            context.Items.Add("LanguageId", SupportedLanguages.GetIdFromLangCode(defaultLanguage));

            SetCurrentThreadCulture(new RequestCulture(cultureInfo, uiCultureInfo));
        }

        return next(context);
    }

    private static void SetCurrentThreadCulture(RequestCulture requestCulture)
    {
        CultureInfo.CurrentCulture = requestCulture.Culture;
        CultureInfo.CurrentUICulture = requestCulture.UICulture;
    }

    private string GetDefaultLanguage(OrganizationInfo organization, UserInfo userInfo)
    {
        return organization.OrganizationSetting.IsMultiLanguageEnabled && userInfo.AgentLanguage > 0 ? SupportedLanguages.GetLangCodeFromId(userInfo.AgentLanguage.Value)
                : !string.IsNullOrWhiteSpace(organization.OrganizationSetting.DefaultLanguage) ? organization.OrganizationSetting.DefaultLanguage : "en-US";
    }
}