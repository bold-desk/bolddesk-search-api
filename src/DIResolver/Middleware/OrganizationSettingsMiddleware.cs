//-----------------------------------------------------------------------
// <copyright file="OrganizationSettingsMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware
{
    using System;
    using System.Threading.Tasks;
    using BoldDesk.Search.Core.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Localization;
    using Newtonsoft.Json;
    using Syncfusion.Caching.Services;
    using Syncfusion.HelpDesk.Core.Enums;
    using Syncfusion.HelpDesk.Core.Localization;
    using Syncfusion.HelpDesk.Multitenant;
    using Syncfusion.HelpDesk.Organization.Data.Entity;

    /// <summary>
    /// OrganizationSettingsMiddleware - Class Declaration.
    /// </summary>
    public class OrganizationSettingsMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationSettingsMiddleware"/> class.
        /// </summary>
        /// <param name="next">Request Delegate Value.</param>
        public OrganizationSettingsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invoke Method.
        /// </summary>
        /// <param name="context">HTTP Context.</param>
        /// <param name="organization">Organization Info.</param>
        /// <param name="searchDbContext">Search DB Context.</param>
        /// <param name="distributedCacheService">Distributed Cache service.</param>
        /// <param name="baseLocalizer">Base String Localizer value.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task InvokeAsync(HttpContext context, OrganizationInfo organization, SearchDbContext searchDbContext, IDistributedCacheService distributedCacheService, IStringLocalizer<SharedResource> baseLocalizer)
        {
            if (context != null)
            {
                var errorMessage = baseLocalizer != null ? baseLocalizer[SharedResourceConstants.UnknownError] : string.Empty;
                try
                {
                    var setting = new SettingService(organization, searchDbContext, distributedCacheService);
                    var settingsJson = setting.GetSettingsAsync((int)SettingTypeEnum.AgentPortalSettings).Result;
                    if (!settingsJson.IsSuccess)
                    {
                        throw new InvalidOperationException(errorMessage);
                    }

                    if (organization == null)
                    {
                        organization = new OrganizationInfo();
                    }

                    organization.OrganizationSetting = JsonConvert.DeserializeObject<OrganizationSetting>(settingsJson.Result) !;
                }
                catch (Exception)
                {
                    throw new InvalidOperationException(errorMessage);
                }
            }

            return next(context);
        }
    }
}
