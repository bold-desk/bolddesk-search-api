//-----------------------------------------------------------------------
// <copyright file="LoginDetails.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Services.UserManagement
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using BoldDesk.Search.Core.Objects.Common;
    using BoldDesk.Search.Core.Services.UserManagement;
    using BoldDesk.Search.Localization.Resources;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Localization;
    using Microsoft.IdentityModel.Tokens;
    using Syncfusion.HelpDesk.Catalog.Utilities;
    using Syncfusion.HelpDesk.Core.Enums;
    using Syncfusion.HelpDesk.Core.Localization;
    using Syncfusion.HelpDesk.Core.Objects;
    using Syncfusion.HelpDesk.Core.Utilities;
    using Syncfusion.HelpDesk.Encryption;
    using Syncfusion.HelpDesk.Multitenant;
    using Syncfusion.HelpDesk.Organization.Data.Entity;

    /// <summary>
    /// Login details class.
    /// </summary>
    public class LoginDetails : ILoginDetails
    {
        private readonly SearchDbContext context;
        private readonly UserInfo user;
        private readonly IStringLocalizer<Resource> localizer;
        private readonly IBCryptHasher bCrypt;
        private readonly RequestDetails requestDetails;
        private readonly OrganizationInfo organization;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginDetails"/> class.
        /// </summary>
        /// <param name="context">Agent Database Context.</param>
        /// <param name="user">Current User.</param>
        /// <param name="organization">Organization Info.</param>
        /// <param name="bCryptHasher">BCrypt hasher.</param>
        /// <param name="stringLocalizer">Localizer.</param>
        /// <param name="environment">Environment.</param>
        /// <param name="requestDetails">Request Details.</param>
        public LoginDetails(SearchDbContext context, UserInfo user, OrganizationInfo organization, IBCryptHasher bCryptHasher, IStringLocalizer<Resource> stringLocalizer, IWebHostEnvironment environment, RequestDetails requestDetails)
        {
            this.context = context;
            this.user = user;
            this.organization = organization;
            bCrypt = bCryptHasher;
            localizer = stringLocalizer;
            Environment = environment;
            this.requestDetails = requestDetails;
        }

        private IWebHostEnvironment Environment { get; }

        /// <summary>
        /// Get Agent's claim details.
        /// </summary>
        /// <param name="email">Agent email.</param>
        /// <returns>Claim details.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Ok")]
        public async Task<Claim[]> GetClaimsAsync(string email = "")
        {
            var emailId = email?.TrimIfNotNull()?.ToLower(CultureInfo.CurrentCulture);

            var query = await (from user in context.Users.Where(i => i.IsActive && i.NormalizedEmailId == emailId)
                               from agent in context.Agent.Where(i => i.IsActive && i.UserId == user.Id)
                               select new
                               {
                                   User = user,
                                   HasFullAccess = agent.HasAllBrandAccess,
                                   RoleList = context.UserRoleMapper.Where(i => context.UserRole.Any(j => j.IsActive && j.Id == i.RoleId)
                                                                                && i.IsActive && i.UserId == user.Id).Select(i => i.RoleId).ToList(),
                                   Brands = agent.HasAllBrandAccess
                                                    ? context.Brand.Where(i => i.IsActive).Select(i => i.BrandId).ToList()
                                                    : context.UserBrandAccess.Where(i => context.Brand.Any(j => i.IsActive && j.BrandId == i.BrandId)
                                                                                            && i.IsActive && i.UserId == user.Id).Select(i => i.BrandId).ToList()
                               }).FirstOrDefaultAsync().ConfigureAwait(false);

            List<int> brandids = query.Brands;
            List<int> orgBrandIds = new List<int>();
            if (brandids?.Count > 0)
            {
                foreach (var id in brandids)
                {
                    orgBrandIds.Add(DBUtils.GetOrgBrandId(query.User.OrgId, id));
                }
            }

            var userDetails = new
            {
                query.User,
                query.HasFullAccess,
                Roles = string.Join(",", query.RoleList.Where(i => i.IsNotDefault()).OrderBy(roleId => roleId).Distinct()),
                Brands = string.Join(",", query.Brands.Where(i => i.IsNotDefault()).OrderBy(brandId => brandId).Distinct()),
                OrgBrandIds = string.Join(",", orgBrandIds.Where(i => i.IsNotDefault()).OrderBy(orgBrandIds => orgBrandIds).Distinct()),
            };

            if (userDetails.User == null || userDetails.User.Id <= 0 || string.IsNullOrEmpty(userDetails.Roles))
            {
                return Array.Empty<Claim>();
            }

            var agentTicketAccessScope = string.Empty;
            if (userDetails.User.AgentTicketAccessScopeId != null)
            {
                agentTicketAccessScope = userDetails.User.AgentTicketAccessScopeId.Value.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                var defaultAccessScope = (int)AgentTicketAccessScopeEnum.Restricted;
                agentTicketAccessScope = defaultAccessScope.ToString(CultureInfo.CurrentCulture);
            }

            return new Claim[]
            {
                new Claim(nameof(UserInfo.DisplayName).ToLower(CultureInfo.CurrentCulture), userDetails.User.DisplayName ?? string.Empty),
                new Claim(nameof(UserInfo.TimeZone).ToLower(CultureInfo.CurrentCulture), userDetails.User.TimeZoneId > 0 ? userDetails.User.TimeZone.Offset : string.Empty),
                new Claim(nameof(UserInfo.UserId).ToLower(CultureInfo.CurrentCulture), userDetails.User.Id.ToString(CultureInfo.CurrentCulture)),
                new Claim(nameof(UserInfo.RoleIds).ToLower(CultureInfo.CurrentCulture), userDetails.Roles),
                new Claim(nameof(UserInfo.ShortCode).ToLower(CultureInfo.CurrentCulture), userDetails.User.ShortCode ?? string.Empty),
                new Claim(nameof(UserInfo.ColorCode).ToLower(CultureInfo.CurrentCulture), userDetails.User.ColorCode ?? string.Empty),
                new Claim(nameof(UserInfo.BrandIds).ToLower(CultureInfo.CurrentCulture), !string.IsNullOrEmpty(userDetails.Brands) ? userDetails.Brands : string.Empty),
                new Claim(nameof(UserInfo.OrgBrandIds).ToLower(CultureInfo.CurrentCulture), !string.IsNullOrEmpty(userDetails.OrgBrandIds) ? userDetails.OrgBrandIds : string.Empty),
                new Claim(nameof(UserInfo.DefaultBrandId).ToLower(CultureInfo.CurrentCulture), userDetails.User.OrginatedFromBrandId.HasValue ? userDetails.User.OrginatedFromBrandId.Value.ToString(CultureInfo.CurrentCulture) : string.Empty),
                new Claim(nameof(UserInfo.HasFullAccess).ToLower(CultureInfo.CurrentCulture), userDetails.HasFullAccess.ToString(CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentCulture)),
                new Claim(nameof(UserInfo.AccessScopeId).ToLower(CultureInfo.CurrentCulture), agentTicketAccessScope)
            };
        }

        /// <summary>
        /// Get JWT Tokenx
        /// </summary>
        /// <param name="email">Email id.</param>
        /// <returns>return JWT token.</returns>
        public string GetJwToken(string email = "")
        {
            var key = Encoding.ASCII.GetBytes(organization.JwtKey);

            HttpContextAccessor contextAccessor = new HttpContextAccessor();

            var claims = string.IsNullOrEmpty(email) ? contextAccessor.HttpContext.User.Claims : GetClaimsAsync(email).Result;

            // Generate Token for user
            var jwtToken = new JwtSecurityToken(
                claims: claims,
                notBefore: requestDetails.DateTime,
                expires: Environment.IsDevelopment() ? requestDetails.DateTime.AddDays(2) : requestDetails.DateTime.AddDays(2),
                signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)); // Using HS256 Algorithm to encrypt Token

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        /// <summary>
        /// Generate token.
        /// </summary>
        /// <param name="emailId">Email Id.</param>
        /// <param name="apiKey">API Key.</param>
        /// <returns>return JWt token.</returns>
        public async Task<string> GenerateTokenAsync(string emailId, string apiKey)
        {
            if (!user.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(emailId) && !string.IsNullOrEmpty(apiKey))
                {
                    var query = (from user in context.Users
                                 join userapikey in context.UserApiKey on user.Id equals userapikey.UserId
                                 where user.NormalizedEmailId == emailId.ToLower(CultureInfo.CurrentCulture).Trim() && user.IsActive && (user.UserStatusId == (int)UserStatusEnum.Active)
                                 && (userapikey.ExpiryDate == null || userapikey.ExpiryDate >= DateTime.UtcNow.Date) && userapikey.IsActive
                                 select userapikey).AsQueryable();

                    bool isValidKey = false;

                    foreach (var keys in query)
                    {
                        if (bCrypt.VerifyHash(keys.ApiKey, apiKey))
                        {
                            isValidKey = true;
                            break;
                        }
                    }

                    if (!isValidKey)
                    {
                        return string.Empty;
                    }

                    return GetJwToken(emailId);
                }

                return string.Empty;
            }

            // update last activity date for user.
            if (user.UserId > 0)
            {
                var userDetails = (from users in context.Users
                                   where users.Id == user.UserId
                                   select users).FirstOrDefault();
                userDetails.LastActivityOn = requestDetails.DateTime;
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            return GetJwToken();
        }
    }
}
