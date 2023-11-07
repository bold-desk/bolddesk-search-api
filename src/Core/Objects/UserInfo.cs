//-----------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Objects;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Syncfusion.HelpDesk.Core.Utilities;
using Syncfusion.HelpDesk.Multitenant;

/// <summary>
/// User Info Class.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserInfo"/> class.
    /// </summary>
    /// <param name="contextAccessor">Context Accessor.</param>
    /// <param name="organization">Organization Info.</param>
    public UserInfo(IHttpContextAccessor contextAccessor, OrganizationInfo organization)
    {
        var user = contextAccessor?.HttpContext?.User;
        GetUserInfo(user, organization);
    }

    /// <summary>
    /// Gets or sets Display Name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Time Zone.
    /// </summary>
    public string TimeZone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets IANA time zone name.
    /// </summary>
    public string IANATimeZoneName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Short Code.
    /// </summary>
    public string ShortCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Color Code.
    /// </summary>
    public string ColorCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets User Id.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Gets or sets Role Id Collection.
    /// </summary>
    public List<int> RoleIds { get; set; } = new List<int>();

    /// <summary>
    /// Gets or sets Brand Id Collection.
    /// </summary>
    public List<int> BrandIds { get; set; } = new List<int>();

    /// <summary>
    /// Gets or sets Org Brand Id Collection.
    /// </summary>
    public List<int> OrgBrandIds { get; set; } = new List<int>();

    /// <summary>
    /// Gets or sets Default Brand Id.
    /// </summary>
    public int DefaultBrandId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether user is authenticated or not.
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether access to all brands or not.
    /// </summary>
    public bool HasFullAccess { get; set; }

    /// <summary>
    /// Gets or sets access scope Id.
    /// </summary>
    public int AccessScopeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets Is Impersonated.
    /// </summary>
    public bool IsImpersonated { get; set; }

    /// <summary>
    /// Gets or sets User Type Id.
    /// </summary>
    public int UserTypeId { get; set; }

    /// <summary>
    /// Gets or sets Id Server User Id.
    /// </summary>
    public string IdServerUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets ATAS.
    /// </summary>
    public string? ATAS { get; set; }

    /// <summary>
    /// Gets or sets RID.
    /// </summary>
    public string RID { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets CIDWS.
    /// </summary>
    public string? CIDWS { get; set; }

    /// <summary>
    /// Gets or sets Role.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user time zone details
    /// </summary>
    public UserTimeZoneInfo UserTimeZoneInfo { get; set; } = new UserTimeZoneInfo();

    /// <summary>
    /// Gets or Sets Language Short Code.
    /// </summary>
    public int? AgentLanguage { get; set; }

    /// <summary>
    /// Gets or sets permission ID.
    /// </summary>
    public string PermissionId { get; set; } = string.Empty;

    /// <summary>
    /// Get User details from Claims.
    /// </summary>
    /// <param name="user">User Claims Principal.</param>
    /// <param name="organization">Organization Info.</param>
    /// <returns>Returns User Info.</returns>
    public UserInfo GetUserInfo(ClaimsPrincipal? user, OrganizationInfo? organization)
    {
        if (user is null)
        {
            return this;
        }

        IsAuthenticated = user.Identity.IsAuthenticated;
        var claims = user.Claims?.ToList();

        if (!IsAuthenticated)
        {
            return this;
        }

        foreach (var property in GetType().GetProperties())
        {
            if (string.IsNullOrEmpty(property.Name))
            {
                continue;
            }

            var propertyValue = claims?.Find(c => c.Type.ToUpper(CultureInfo.CurrentCulture) == property.Name.ToUpper(CultureInfo.CurrentCulture).Trim())?.Value;

            if (string.IsNullOrEmpty(propertyValue))
            {
                continue;
            }

            var propertyType = property.PropertyType;

            if (propertyType == typeof(int))
            {
                property.SetValue(this, propertyValue.ToInt());
            }
            else if (propertyType == typeof(long))
            {
                property.SetValue(this, propertyValue.ToLong());
            }
            else if (propertyType == typeof(List<int>))
            {
                property.SetValue(this, propertyValue.ToIntList());
            }
            else if (propertyType == typeof(bool))
            {
                property.SetValue(this, propertyValue.ToNullableBool());
            }
            else if (propertyType == typeof(int?))
            {
                property.SetValue(this, propertyValue.ToInt());
            }
            else
            {
                property.SetValue(this, propertyValue);
            }
        }

        UserTimeZoneInfo = new UserTimeZoneInfo
        {
            UtcOffset = GetTimeZoneValue(TimeZone, organization?.OrganizationSetting?.TimeZoneOffset).ToUtcOffset(),
            TimeZoneOffset = GetTimeZoneValue(TimeZone, organization?.OrganizationSetting?.TimeZoneOffset),
            IANATimeZoneName = GetTimeZoneValue(IANATimeZoneName, organization?.OrganizationSetting?.IANATimeZoneName)
        };

        return this;
    }

    private string GetTimeZoneValue(string? userValue, string? organizationValue)
    {
        var value = string.IsNullOrWhiteSpace(userValue) ? organizationValue : userValue;
        return value ?? string.Empty;
    }
}
