//-----------------------------------------------------------------------
// <copyright file="JwtTokenDataSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects
{
    using Syncfusion.HelpDesk.Core.Enums;
    using Syncfusion.HelpDesk.Core.Tokens;
    using Syncfusion.HelpDesk.Multitenant;

    /// <summary>
    /// Jwt Token Data Settings class.
    /// </summary>
    public class JwtTokenDataSettings : JwtTokenData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenDataSettings"/> class.
        /// </summary>
        /// <param name="organization">Organization Info.</param>
        public JwtTokenDataSettings(OrganizationInfo organization)
        {
            if (organization == null)
            {
                return;
            }

            OrgId = organization.OrgId;
            BrandId = organization.BrandId;
            Issuer = organization.HostName;
            OrganizationHostName = organization.HostName;
            Key = organization.JwtKey;
            ApplicationType = ApplicationsTypeEnum.AgentPortal;
        }
    }
}