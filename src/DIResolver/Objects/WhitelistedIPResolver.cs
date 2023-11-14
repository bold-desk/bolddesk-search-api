// -----------------------------------------------------------------------
// <copyright file="WhitelistedIPResolver.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using BoldDesk.Search.DIResolver.Objects.AppSettings;
    using Microsoft.Extensions.Options;
    using StackExchange.Redis;
    using Syncfusion.HelpDesk.Core.Enums;
    using Syncfusion.HelpDesk.Core.Middlewares;
    using Syncfusion.HelpDesk.Multitenant;

    /// <summary>
    /// Whitelisted IP resolver class.
    /// </summary>
    public class WhitelistedIPResolver : WhitelistedIPObjects
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhitelistedIPResolver"/> class.
        /// </summary>
        /// <param name="organization">Organization Info.</param>
        /// <param name="appSettings">Application settings</param>
        public WhitelistedIPResolver(OrganizationInfo organization, IOptions<AppSettings.AppSettings> appSettings)
        {
            if (organization == null || appSettings == null || organization.OrganizationSetting == null
                || organization.OrganizationSetting.WhitelistedIP == null)
            {
                return;
            }

            OrgId = organization.OrgId;
            BrandId = organization.BrandId;

            WhitelistedIPDetails whitelistedIP = organization.OrganizationSetting.WhitelistedIP;

            if (whitelistedIP != null)
            {
                whitelistedIP.IP.AddRange(appSettings.Value.WhitelistedIP.IP);
                whitelistedIP.RoleId.AddRange(appSettings.Value.WhitelistedIP.RoleId);
            }
            else
            {
                whitelistedIP = appSettings.Value.WhitelistedIP;
            }

            IP = whitelistedIP.IP ?? new List<string>();
            RoleId = whitelistedIP.RoleId ?? new List<int>();
            IsEnabled = IP.Count > 0 && whitelistedIP.IsEnabled;
            ApplicationTypeId = (int)ApplicationsTypeEnum.CustomerPortal;
        }
    }
}