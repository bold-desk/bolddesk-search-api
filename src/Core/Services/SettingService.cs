//-----------------------------------------------------------------------
// <copyright file="SettingService.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Services
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using BoldDesk.Search.Core.Objects;
    using BoldDesk.Search.Core.Objects.Settings;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using Syncfusion.Caching.Services;
    using Syncfusion.HelpDesk.Core.Enums;
    using Syncfusion.HelpDesk.Multitenant;
    using Syncfusion.HelpDesk.Organization.Data.Entity;

    /// <summary>
    /// SettingService - Class Declaration.
    /// </summary>
    public class SettingService
    {
        private readonly SearchDbContext context;
        private readonly OrganizationInfo organization;
        private readonly IDistributedCacheService distributedCacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingService"/> class.
        /// </summary>
        /// <param name="organization">organization.</param>
        /// <param name="context">db context.</param>
        /// <param name="distributedCacheService">distributed Cache Service.</param>
        public SettingService(
            OrganizationInfo organization,
            SearchDbContext context,
            IDistributedCacheService distributedCacheService)
        {
            this.organization = organization;
            this.context = context;
            this.distributedCacheService = distributedCacheService;
        }

        /// <summary>
        /// Get organization settings.
        /// </summary>
        /// <param name="id">key as integer.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Status> GetSettingsAsync(int id)
        {
            var status = new Status();
            var cacheKey = GetRedisKeyValue((SettingTypeEnum)id, organization);
            status.Result = await distributedCacheService.GetAsync(cacheKey).ConfigureAwait(false);

            if (string.IsNullOrEmpty(status.Result))
            {
                var orgId = organization.OrgId;

                var settingIdList = new List<int>
                {
                    id
                };

                if (id == (int)SettingTypeEnum.AgentPortalSettings)
                {
                    settingIdList.Add((int)SettingTypeEnum.GeneralSettings);
                }

                var settingsList = await (from setting in context.Setting.AsNoTracking()
                                          where settingIdList.Contains(setting.SettingTypeId)
                                          && setting.OrgId == orgId
                                          select setting).OrderBy(x => x.SettingTypeId).ToListAsync().ConfigureAwait(false);

                var settingsValueList = settingsList.ConvertAll(x => x.Value);

                if (settingsValueList?.Count > 1)
                {
                    JObject object1 = JObject.Parse(settingsValueList[0]);
                    JObject object2 = JObject.Parse(settingsValueList[1]);

                    object2.Merge(object1, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Merge
                    });

                    status.Result = object2.ToString();
                }
                else
                {
                    status.Result = settingsValueList.FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(status.Result))
                {
                    await distributedCacheService.SetAsync(cacheKey, status.Result).ConfigureAwait(false);
                }
            }

            status.IsSuccess = true;

            var defaultLanguageName = await (from language in context.Language
                                             where language.IsActive && language.IsDefault == true
                                             select new
                                             {
                                                 language.Id,
                                                 language.Name
                                             }).FirstOrDefaultAsync().ConfigureAwait(false);

            var apiSettings = new object();
            if (id == (int)SettingTypeEnum.AgentPortalSettings)
            {
                var apiSettingsForAPI = JsonConvert.DeserializeObject<OrganizationSearchSettings>(status.Result)!;
                apiSettingsForAPI.BrandId = organization.BrandId;
                apiSettingsForAPI.DefaultLanguageName = defaultLanguageName.Name;
                apiSettingsForAPI.DefaultLanguageId = defaultLanguageName.Id;
                apiSettingsForAPI.OrgId = organization.OrgId;
                apiSettings = apiSettingsForAPI;
            }

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            status.Result = JsonConvert.SerializeObject(apiSettings, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            return status;
        }

        /// <summary>
        /// GetGeneralSettingsAsync method.
        /// </summary>
        /// <typeparam name="T">Object type class.</typeparam>
        /// <returns>returns settings.</returns>
        public async Task<Status> GetGeneralSettingsAsync<T>()
            where T : new()
        {
            var status = new Status();
            var settingsValue = await GetSettingsDataAsync((int)SettingTypeEnum.GeneralSettings).ConfigureAwait(false);
            status.Result = settingsValue.Value;

            status.IsSuccess = true;
            if (status.IsSuccess && !string.IsNullOrWhiteSpace(status.Result))
            {
                var apiSettings = JsonConvert.DeserializeObject<T>(status.Result);

                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };

                status.Result = JsonConvert.SerializeObject(apiSettings, new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented
                });
            }

            return status;
        }

        /// <summary>
        /// Get General settings value from database.
        /// </summary>
        /// <param name="settingTypeId">Setting Type Id.</param>
        /// <param name="brandId">Brand Id.</param>
        /// <returns>value.</returns>
        public Task<Setting?> GetSettingsDataAsync(int settingTypeId, int brandId = 0)
        {
            var orgId = organization.OrgId;
            return (from setting in context.Setting
                    where setting.SettingTypeId == settingTypeId
                    && setting.OrgId == orgId
                    && (brandId == 0 || (brandId > 0 && setting.BrandId.HasValue && setting.BrandId.Value == brandId))
                    select setting).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get Redis Key for settings.
        /// </summary>
        /// <param name="type">settings type.</param>
        /// <param name="orgInfo">org info.</param>
        /// <param name="brandId">Brand Id.</param>
        /// <returns>returns the settings key for redis.</returns>
        public string GetRedisKeyValue(SettingTypeEnum type, OrganizationInfo? orgInfo = null, int? brandId = null)
        {
            var orgId = orgInfo != null ? orgInfo.OrgId : organization.OrgId;
            if (brandId > 0)
            {
                return type.GetType()?.GetMember(type.ToString())?.First()?.GetCustomAttribute<DescriptionAttribute>()?.Description + "_" + orgId.ToString(CultureInfo.CurrentCulture) + "_" + brandId.Value.ToString(CultureInfo.CurrentCulture);
            }

            return type.GetType()?.GetMember(type.ToString()).First()?.GetCustomAttribute<DescriptionAttribute>()?.Description + "_" + orgId.ToString(CultureInfo.CurrentCulture);
        }
    }
}
