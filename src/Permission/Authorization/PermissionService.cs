//-----------------------------------------------------------------------
// <copyright file="PermissionService.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using BoldDesk.Permission.Enums;
    using BoldDesk.Permission.Objects;
    using BoldDesk.Search.Core.Objects.Common;
    using BoldDesk.Search.Localization.Resources;
    using Dapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Localization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Syncfusion.Caching.Services;
    using Syncfusion.HelpDesk.Core.CustomException;
    using Syncfusion.HelpDesk.Core.Enums;
    using Syncfusion.HelpDesk.Core.Extensions;
    using Syncfusion.HelpDesk.Core.Language;
    using Syncfusion.HelpDesk.Core.Localization;
    using Syncfusion.HelpDesk.Core.Utilities;
    using Syncfusion.HelpDesk.Core.ValidationErrors;
    using Syncfusion.HelpDesk.Multitenant;
    using Syncfusion.HelpDesk.Organization.Data.Entity;

    /// <summary>
    /// Permission service
    /// </summary>
    public class PermissionService : IPermissionService
    {
        /// <summary>
        /// Base String Localizer object.
        /// </summary>
        private readonly IStringLocalizer<SharedResource> baseLocalizer;

        /// <summary>
        /// Localizer.
        /// </summary>
        private readonly IStringLocalizer<Resource> localizers;

        /// <summary>
        /// Agent Db Context.
        /// </summary>
        private readonly SearchDbContext context;

        /// <summary>
        /// Organization Info object.
        /// </summary>
        private readonly OrganizationInfo organization;

        /// <summary>
        /// User Info object.
        /// </summary>
        private readonly UserInfo user;

        /// <summary>
        /// IDistributed cache service.
        /// </summary>
        private readonly IDistributedCacheService distributedCacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionService"/> class.
        /// </summary>
        /// <param name="context">Agent Db Context.</param>
        /// <param name="organization">Organization Info.</param>
        /// <param name="user">User Info.</param>
        /// <param name="distributedCacheService">IDistributed Cache Service</param>
        /// <param name="baseLocalizer">Base String Localizer value.</param>
        /// <param name="localizers">IstringLocalizers</param>
        public PermissionService(SearchDbContext context, OrganizationInfo organization, UserInfo user, IDistributedCacheService distributedCacheService, IStringLocalizer<SharedResource> baseLocalizer, IStringLocalizer<Resource> localizers)
        {
            this.context = context;
            this.organization = organization;
            this.user = user;
            this.distributedCacheService = distributedCacheService;
            this.baseLocalizer = baseLocalizer;
            this.localizers = localizers;
        }

        /// <summary>
        /// Get the list of settings available for specific user.
        /// </summary>
        /// <returns>Returns list of settings available for specific user.</returns>
        public async Task<AgentSettings> GetAgentSettingsAsync()
        {
            AgentAvailabilityStatusObject agentAvailabilityStatus = await GetUserAvailabilityAsync().ConfigureAwait(false);
            string agentSettingsFromCache = await distributedCacheService.GetAsync("agent_setting_" + organization.OrgId.ToString(CultureInfo.InvariantCulture) + "_" + user.UserId.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
            var organizationSettings = organization.OrganizationSetting;
            if (!string.IsNullOrWhiteSpace(agentSettingsFromCache))
            {
                AgentSettings agentSetting = JsonConvert.DeserializeObject<AgentSettings>(agentSettingsFromCache) !;
                agentSetting.IsAvailable = agentAvailabilityStatus.AgentSupportChannelAvailability.StatusCategoryId != (int)AgentAvailabilityStatusCategoryEnum.Offline;

                if (agentSetting.LanguageShortCode != null)
                {
                    var langList = new List<string>();
                    if (organizationSettings.IsMultiLanguageEnabled)
                    {
                        langList = organizationSettings.SupportedLanguages?.Split(",").ToList();
                    }

                    langList?.Add(organizationSettings.DefaultLanguage);

                    if (langList?.Contains(agentSetting.LanguageShortCode) == false)
                    {
                        agentSetting.LanguageShortCode = null;
                        agentSetting.LanguageId = 0;
                    }
                }

                agentSetting.AgentAvailabilityStatus = agentAvailabilityStatus;
                return agentSetting;
            }

            DefaultTypeMap.MatchNamesWithUnderscores = true;
            var agentSettingsQueryList = await context.Database.GetDbConnection().QueryAsync<AgentSettingsFromQuery>(GetAgentSettingsDapperQuery()).ConfigureAwait(false);
            AgentSettingsFromQuery agentSettingsQueryObject = agentSettingsQueryList.SingleOrDefault();
            if (agentSettingsQueryObject == null)
            {
                throw new ValidationException(
                    localizers[ResourceConstants.FieldUser],
                    baseLocalizer[SharedResourceConstants.NoRecords],
                    ErrorTypeEnum.InvalidValue);
            }

            if (!string.IsNullOrEmpty(agentSettingsQueryObject.TicketFields))
            {
                agentSettingsQueryObject.TicketFields = JsonConvert.DeserializeObject<string>(agentSettingsQueryObject.TicketFields);
            }

            if (!string.IsNullOrEmpty(agentSettingsQueryObject.Brands))
            {
                agentSettingsQueryObject.Brands = JsonConvert.DeserializeObject<string>(agentSettingsQueryObject.Brands);
            }

            var agentSettings = new AgentSettings
            {
                DisplayName = agentSettingsQueryObject.DisplayName,
                Email = agentSettingsQueryObject.Email,
                ShortCode = agentSettingsQueryObject.ShortCode,
                ColorCode = agentSettingsQueryObject.ColorCode,
                IsAvailable = agentAvailabilityStatus.AgentSupportChannelAvailability.StatusCategoryId != 3,
                HasAllBrandAccess = agentSettingsQueryObject.HasAllBrandAccess,
                TicketAccessScopeId = agentSettingsQueryObject.AgentTicketAccessScopeId,
                TicketFields = agentSettingsQueryObject.TicketFields,
                TimeZoneId = agentSettingsQueryObject.TimeZoneId,
                TimeZoneName = agentSettingsQueryObject.TimeZoneName,
                TimeZoneOffset = agentSettingsQueryObject.TimeZoneOffset,
                TimeZoneShortCode = agentSettingsQueryObject.TimeZoneShortCode,
                IANATimeZoneName = agentSettingsQueryObject.WindowsTimeZoneId?.GetIANATimeZoneName(),
                LanguageId = agentSettingsQueryObject.LanguageId,
                TicketLayoutId = agentSettingsQueryObject.TicketLayoutId,
                SortReplyId = agentSettingsQueryObject.SortReplyId,
                EnableShortcut = agentSettingsQueryObject.EnableShortcut,
                Brands = agentSettingsQueryObject.Brands,
                LanguageShortCode = agentSettingsQueryObject.LanguageShortCode,
                AgentAvailabilityStatus = agentAvailabilityStatus
            };

            if (agentSettings.LanguageShortCode != null)
            {
                var langList = new List<string>();
                if (organizationSettings.IsMultiLanguageEnabled)
                {
                    langList = organizationSettings.SupportedLanguages?.Split(",").ToList();
                }

                langList?.Add(organizationSettings.DefaultLanguage);

                if (langList?.Contains(agentSettings.LanguageShortCode) == false)
                {
                    agentSettings.LanguageShortCode = null;
                    agentSettings.LanguageId = 0;
                }
            }

            if (agentSettings.TimeZoneId == null || agentSettings.TimeZoneId == 0
                || agentSettings.SortReplyId == null || agentSettings.SortReplyId == 0
                || agentSettings.TicketLayoutId == null || agentSettings.TicketLayoutId == 0
                || agentSettings.DefaultTicketViewId == null || agentSettings.DefaultTicketViewId == 0
                || agentSettings.TicketPerPageCount == null || agentSettings.TicketPerPageCount == 0
                || agentSettings.EnableShortcut == null || agentSettings.Brands == null
                || agentSettings.DefaultMessageFilterId == null || agentSettings.DefaultMessageFilterId == 0)
            {
                if (agentSettings.TimeZoneId == null || agentSettings.TimeZoneId == 0)
                {
                    agentSettings.TimeZoneId = organizationSettings.TimeZoneId;
                    agentSettings.TimeZoneName = organizationSettings.TimeZoneName;
                    agentSettings.TimeZoneOffset = organizationSettings.TimeZoneOffset;
                    agentSettings.TimeZoneShortCode = organizationSettings.TimeZoneShortCode;
                    agentSettings.IANATimeZoneName = organizationSettings.IANATimeZoneName;
                }

                if (agentSettings.SortReplyId == null || agentSettings.SortReplyId == 0)
                {
                    agentSettings.SortReplyId = organizationSettings.SortReplyId;
                }

                if (agentSettings.TicketLayoutId == null || agentSettings.TicketLayoutId == 0)
                {
                    agentSettings.TicketLayoutId = organizationSettings.TicketLayoutId;
                }

                if (agentSettings.DefaultTicketViewId == null || agentSettings.DefaultTicketViewId == 0)
                {
                    agentSettings.DefaultTicketViewId = organizationSettings.DefaultTicketViewId;
                }

                if (agentSettings.TicketPerPageCount == null || agentSettings.TicketPerPageCount == 0)
                {
                    agentSettings.TicketPerPageCount = organizationSettings.TicketPerPageCount;
                }

                if (agentSettings.EnableShortcut == null)
                {
                    agentSettings.EnableShortcut = true;
                }

                if (agentSettings.Brands == null)
                {
                    agentSettings.Brands = "-1";
                }

                if (agentSettings.DefaultMessageFilterId == null || agentSettings.DefaultMessageFilterId == 0)
                {
                    agentSettings.DefaultMessageFilterId = organizationSettings.DefaultMessageFilterId;
                }
            }

            string agentSettingsString = JsonConvert.SerializeObject(agentSettings);
            await distributedCacheService.SetAsync("agent_setting_" + organization.OrgId.ToString(CultureInfo.InvariantCulture) + "_" + user.UserId.ToString(CultureInfo.InvariantCulture), agentSettingsString, TimeSpan.FromMinutes(15)).ConfigureAwait(false);
            return agentSettings;
        }

        /// <summary>
        /// Method to check the user's permission
        /// </summary>
        /// <param name="permissionIds">Permission Ids to check.</param>
        /// <param name="hasAllPermissionCheck">Boolean value to specify whether all/any permissions to check</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <returns>Returns true if user has that permission else false.</returns>
        public async Task<bool> HasPermissionAsync(int[] permissionIds, bool hasAllPermissionCheck = false, long? currentUserId = null)
        {
            bool hasPermission = false;
            (hasPermission, _) = await TryGetPermissionAsync(permissionIds, hasAllPermissionCheck, currentUserId).ConfigureAwait(false);

            return hasPermission;
        }

        /// <summary>
        /// Method to check the user's permission and return matching permissions if any
        /// </summary>
        /// <param name="permissionIds">Permission Ids to check.</param>
        /// <param name="hasAllPermissionCheck">Boolean value to specify whether all/any permissions to check</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <returns>Returns true if user has that permission else false along with list of matching permissions if any.</returns>
        public async Task<(bool, List<int>?)> TryGetPermissionAsync(int[] permissionIds, bool hasAllPermissionCheck, long? currentUserId = null)
        {
            bool hasPermission = false;

            // if 'AnyAuthenticatedAgentCanAccess' or 'HandledInsideMethod' filter is set, then we can simply return true before checking with DB as it meant to not need any permission for that action.
            if (permissionIds.Contains(0) || permissionIds.Contains(-1))
            {
                return (true, permissionIds.Contains(0) ? new List<int> { 0 } : new List<int> { -1 });
            }

            AgentPermissionSettings agentSettings = await GetAgentPermissionsAsync(currentUserId).ConfigureAwait(false);
            List<int>? permissionList = agentSettings?.Permissions;
            if (permissionList?.Count > 0)
            {
                if (hasAllPermissionCheck)
                {
                    if (permissionList.Intersect(permissionIds).ToList().Count == permissionIds?.Length)
                    {
                        hasPermission = true;
                    }
                }
                else
                {
                    if (permissionList.Intersect(permissionIds).Any())
                    {
                        hasPermission = true;
                    }
                }
            }

            return (hasPermission, permissionList?.Intersect(permissionIds).ToList());
        }

        /// <summary>
        /// Check whether requested user has access to reply ticket from email or not
        /// </summary>
        /// <param name="userId">User ID to check access</param>
        /// <param name="permissionId">Permission ID to check access</param>
        /// <returns>User availability</returns>
        public Task<bool> CheckUserAccessToReplyTicket(long userId, int permissionId)
        {
            return (from userRoleMapper in context.UserRoleMapper
                    join userRole in context.UserRole on userRoleMapper.RoleId equals userRole.Id
                    join userRolePermissionMapper in context.UserRolePermissionMapper on userRoleMapper.RoleId equals userRolePermissionMapper.RoleId
                    join permission in context.Permission on userRolePermissionMapper.PermissionId equals permission.Id
                    where userRoleMapper.UserId == userId && permission.Id == permissionId
                    && userRoleMapper.IsActive && userRole.IsActive
                    && userRolePermissionMapper.IsActive && permission.IsActive
                    select userRoleMapper.Id).AnyAsync();
        }

        /// <summary>
        /// Get the list of permission available for specific user.
        /// </summary>
        /// <param name="currentUserId">Current User Id.</param>
        /// <returns>Returns list of permission available for specific user.</returns>
        public async Task<AgentPermissionSettings> GetAgentPermissionsAsync(long? currentUserId = null)
        {
            var userId = currentUserId > 0 ? currentUserId.Value : user.UserId;
            string agentSettingsFromCache = await distributedCacheService.GetAsync("agent_permission_" + organization.OrgId.ToString(CultureInfo.InvariantCulture) + "_" + userId.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(agentSettingsFromCache))
            {
                return JsonConvert.DeserializeObject<AgentPermissionSettings>(agentSettingsFromCache) !;
            }

            DefaultTypeMap.MatchNamesWithUnderscores = true;
            var agentSettingsQueryList = await context.Database.GetDbConnection().QueryAsync<AgentPermissionSettingsFromQuery>(GetAgentPermissionsDapperQuery(userId.ToString(CultureInfo.InvariantCulture))).ConfigureAwait(false);
            AgentPermissionSettingsFromQuery agentSettingsQueryObject = agentSettingsQueryList.SingleOrDefault();

            if (agentSettingsQueryObject == null || string.IsNullOrWhiteSpace(agentSettingsQueryObject.Permissions))
            {
                throw new ValidationException(
                    "userId",
                    baseLocalizer[SharedResourceConstants.AccessDenied],
                    ErrorTypeEnum.AccessDenied,
                    commonMessage: baseLocalizer[SharedResourceConstants.AccessDenied],
                    StatusCodes.Status401Unauthorized);
            }

            var agentSettings = new AgentPermissionSettings
            {
                Permissions = agentSettingsQueryObject.Permissions.Split(',').Select(int.Parse).ToList()
            };

            agentSettings.Permissions = agentSettings.Permissions.Distinct().ToList();
            string agentSettingsString = JsonConvert.SerializeObject(agentSettings);
            await distributedCacheService.SetAsync("agent_permission_" + organization.OrgId.ToString(CultureInfo.InvariantCulture) + "_" + userId.ToString(CultureInfo.InvariantCulture), agentSettingsString, TimeSpan.FromMinutes(15)).ConfigureAwait(false);
            return agentSettings;
        }

        /// <summary>
        /// Get the list of permission available based on role or user
        /// </summary>
        /// <param name="roleId">Role id when it role based</param>
        /// <param name="requestType">Request type to get the permission list. It may be user based or role based.</param>
        /// <param name="userId">User Id</param>
        /// <returns>Returns list of permission available based on role or user.</returns>
        public async Task<List<PermissionListObject>> GetPermissionsAsync(int? roleId = null, string? requestType = null, long? userId = null)
        {
            var modulePermissions = await (from applicationModuleMapper in context.ApplicationModuleMapper
                                           join applicationModule in context.ApplicationModule on applicationModuleMapper.ApplicationModuleId equals applicationModule.Id
                                           join permission in context.Permission on applicationModule.Id equals permission.ModuleId
                                           where applicationModuleMapper.OrgId == organization.OrgId && applicationModule.IsActive
                                           && applicationModuleMapper.IsActive && permission.IsActive
                                           && applicationModule.ApplicationId == (int)ApplicationTypeEnum.AgentPortal
                                           orderby applicationModule.SortOrder
                                           select new ModulePermissionObject
                                           {
                                               ModuleId = applicationModule.Id,
                                               ModuleName = applicationModule.Name,
                                               ModuleSortOrder = applicationModule.SortOrder,
                                               IsModuleEnabled = applicationModule.IsEnabled,
                                               PermissionId = permission.Id,
                                               PermissionName = permission.PermissionScheme,
                                               PermissionDescription = permission.Description,
                                               ParentPermissionId = permission.ParentPermissionId,
                                               PermissionSortOrder = permission.SortOrder,
                                               PermissionIsEnabled = permission.IsDefault,
                                               PermissionSelectionType = !string.IsNullOrWhiteSpace(permission.SelectionType) ? permission.SelectionType : string.Empty
                                           }).ToListAsync().ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(requestType) && requestType == "role")
            {
                if (roleId != null && roleId > 0)
                {
                    var permissionList = await (from userRolePermissionMapper in context.UserRolePermissionMapper
                                                where userRolePermissionMapper.OrgId == organization.OrgId
                                                && userRolePermissionMapper.RoleId == roleId
                                                && userRolePermissionMapper.IsActive
                                                select userRolePermissionMapper.PermissionId).ToListAsync().ConfigureAwait(false);

                    if (permissionList.Count > 0)
                    {
                        foreach (var modulePermission in modulePermissions)
                        {
                            bool roleHasPermission = false;
                            foreach (int permissionId in permissionList)
                            {
                                if (modulePermission.PermissionId == permissionId)
                                {
                                    modulePermission.PermissionIsEnabled = true;
                                    roleHasPermission = true;
                                    break;
                                }
                            }

                            if (!roleHasPermission)
                            {
                                modulePermission.PermissionIsEnabled = false;
                            }
                        }
                    }
                }
            }
            else
            {
                long userDetailsId = user.UserId;
                if (userId != null && userId > 0)
                {
                    var isUserAgent = (from user in context.Users.AsNoTracking()
                                       where user.Id == userId
                                       && user.UserTypeId == (int)UserTypeEnum.Agent
                                       && user.UserStatusId == (int)UserStatusEnum.Active
                                       select user).Any();

                    if (!isUserAgent)
                    {
                        throw new ValidationException(
                            nameof(userId),
                            localizers[ResourceConstants.UserNotValidAgent],
                            ErrorTypeEnum.InvalidValue);
                    }

                    userDetailsId = (long)userId;
                }

                DefaultTypeMap.MatchNamesWithUnderscores = true;
                var agentSettingsQueryList = await context.Database.GetDbConnection().QueryAsync<AgentPermissionSettingsFromQuery>(GetAgentPermissionsDapperQuery(userDetailsId.ToString(CultureInfo.InvariantCulture))).ConfigureAwait(false);
                AgentPermissionSettingsFromQuery agentSettingsQueryObject = agentSettingsQueryList.SingleOrDefault();
                if (agentSettingsQueryObject == null || string.IsNullOrWhiteSpace(agentSettingsQueryObject.Permissions))
                {
                    throw new ValidationException(
                        nameof(userId),
                        localizers[ResourceConstants.NoRoleForTheAgent],
                        ErrorTypeEnum.InvalidValue);
                }

                var agentSettings = new AgentPermissionSettings
                {
                    Permissions = agentSettingsQueryObject.Permissions.Split(',').Select(int.Parse).ToList()
                };

                agentSettings.Permissions = agentSettings.Permissions.Distinct().ToList();
                if (agentSettings.Permissions.Count > 0)
                {
                    foreach (var modulePermission in modulePermissions)
                    {
                        bool userHasPermission = false;
                        foreach (int permissionId in agentSettings.Permissions)
                        {
                            if (modulePermission.PermissionId == permissionId)
                            {
                                modulePermission.PermissionIsEnabled = true;
                                userHasPermission = true;
                                break;
                            }
                        }

                        if (!userHasPermission)
                        {
                            modulePermission.PermissionIsEnabled = false;
                        }
                    }
                }
            }

            // checking if contact/admin modules are enabled based on the module permission id in permission table.
            var isContactModuleEnabled = modulePermissions.Any(x => x.PermissionId == (int)ModulePermissionEnum.CanViewContactsModule && x.PermissionIsEnabled);
            var isAdminModuleEnabled = modulePermissions.Any(x => x.PermissionId == (int)ModulePermissionEnum.CanViewAdminModule && x.PermissionIsEnabled);
            var isKBModuleEnabled = modulePermissions.Any(x => x.PermissionId == (int)ModulePermissionEnum.CanViewKBModule && x.PermissionIsEnabled);
            var isReportsModuleEnabled = modulePermissions.Any(x => x.PermissionId == (int)ModulePermissionEnum.CanViewReportsModule && x.PermissionIsEnabled);
            modulePermissions = modulePermissions.Where(x => x.PermissionId != (int)ModulePermissionEnum.CanViewContactsModule
                                && x.PermissionId != (int)ModulePermissionEnum.CanViewAdminModule
                                && x.PermissionId != (int)ModulePermissionEnum.CanViewKBModule
                                && x.PermissionId != (int)ModulePermissionEnum.CanViewReportsModule).ToList();

            List<ModulePermissionObject> permissions = new List<ModulePermissionObject>();
            foreach (var permission in modulePermissions.Where(x => x.ParentPermissionId == null).ToList())
            {
                ModulePermissionObject per = new ModulePermissionObject
                {
                    ModuleId = permission.ModuleId,
                    ModuleName = permission.ModuleName,
                    ModuleSortOrder = permission.ModuleSortOrder,
                    IsModuleEnabled = permission.IsModuleEnabled,
                    PermissionId = permission.PermissionId,
                    PermissionName = permission.PermissionName,
                    PermissionDescription = permission.PermissionDescription,
                    PermissionSortOrder = permission.PermissionSortOrder,
                    PermissionIsEnabled = permission.PermissionIsEnabled,
                    PermissionSelectionType = permission.PermissionSelectionType,
                    ChildPermissions = modulePermissions.Where(x => x.ParentPermissionId == permission.PermissionId).ToList()
                };
                permissions.Add(per);
            }

            var modules = permissions.GroupBy(x => x.ModuleId).Select(grp => grp.ToList()).ToList();
            List<PermissionListObject> permissionlist = new List<PermissionListObject>();
            foreach (var module in modules)
            {
                int? modulePermissionId = null;
                var moduleId = module.Select(x => x.ModuleId).FirstOrDefault();
                int moduleSortOrder = module.Select(x => x.ModuleSortOrder).FirstOrDefault();

                // based on contacts/admin module enabled/disabled value, we are setting the 'IsModuleEnabled' property in final output.
                bool isModuleEnabled = true;
                if (moduleId != (int)ApplicationsModuleEnum.Tickets && moduleId != (int)ApplicationsModuleEnum.AgentProfile)
                {
                    isModuleEnabled = (isContactModuleEnabled && moduleId == (int)ApplicationsModuleEnum.Contacts)
                        || (isAdminModuleEnabled && moduleId == (int)ApplicationsModuleEnum.Admin)
                        || (isKBModuleEnabled && moduleId == (int)ApplicationsModuleEnum.KB)
                        || (isReportsModuleEnabled && moduleId == (int)ApplicationsModuleEnum.Reports);

                    int permissionId = GetPermissionIdFromModuleId<ModulePermissionEnum>(moduleId);
                    if (permissionId > 0)
                    {
                        modulePermissionId = permissionId;
                    }
                }

                PermissionListObject permissionlistobj = new PermissionListObject
                {
                    ModuleId = moduleId,
                    ModuleName = module.Select(x => x.ModuleName).FirstOrDefault(),
                    ModuleSortOrder = moduleSortOrder,
                    IsModuleEnabled = isModuleEnabled,
                    PermissionId = modulePermissionId,
                    Permissions = module.Select(x => new PermissionObject()
                    {
                        Id = x.PermissionId,
                        Name = x.PermissionName,
                        Description = x.PermissionDescription,
                        SortOrder = x.PermissionSortOrder,
                        IsEnabled = isModuleEnabled && x.PermissionIsEnabled,
                        SelectionType = x.PermissionSelectionType,
                        ChildPermissions = x.ChildPermissions.Select(y => new PermissionObject()
                        {
                            Id = y.PermissionId,
                            Name = y.PermissionName,
                            Description = y.PermissionDescription,
                            SortOrder = y.PermissionSortOrder,
                            IsEnabled = isModuleEnabled && y.PermissionIsEnabled,
                            SelectionType = y.PermissionSelectionType,
                        }).OrderBy(x => x.SortOrder).ToList()
                    }).OrderBy(x => x.SortOrder).ToList()
                };
                permissionlist.Add(permissionlistobj);
            }

            return permissionlist.OrderBy(x => x.ModuleSortOrder).ToList();
        }

        /// <summary>
        /// Checking impersonating agent has any permission from admin module
        /// </summary>
        /// <param name="userId">Impersonating agent Id</param>
        /// <returns>Returns 'true' if impersonating agent has any one permission from admin module, else 'false'</returns>
        public async Task<bool> HasAnyAdminModulePermission(long userId)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            var agentPermissionsQueryList = await context.Database.GetDbConnection().QueryAsync<AgentPermissionSettingsFromQuery>(GetAgentPermissionsDapperQuery(userId.ToString(CultureInfo.InvariantCulture))).ConfigureAwait(false);
            AgentPermissionSettingsFromQuery agentPermissionsQueryObject = agentPermissionsQueryList.SingleOrDefault();
            if (agentPermissionsQueryObject == null || string.IsNullOrWhiteSpace(agentPermissionsQueryObject.Permissions))
            {
                throw new ValidationException(
                    localizers[ResourceConstants.FieldUser],
                    baseLocalizer[SharedResourceConstants.NoRecords],
                    ErrorTypeEnum.InvalidValue);
            }

            var agentSettings = new AgentPermissionSettings
            {
                Permissions = agentPermissionsQueryObject.Permissions.Split(',').Select(int.Parse).ToList()
            };

            agentSettings.Permissions = agentSettings.Permissions.Distinct().Where(x => x != (int)ModulePermissionEnum.CanViewContactsModule
                                && x != (int)ModulePermissionEnum.CanViewAdminModule
                                && x != (int)ModulePermissionEnum.CanViewKBModule
                                && x != (int)ModulePermissionEnum.CanViewReportsModule).ToList();
            return await (from permission in context.Permission
                          where permission.ModuleId == (int)ApplicationsModuleEnum.Admin
                          && agentSettings.Permissions.Contains(permission.Id)
                          select permission.Id).AnyAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Method to return the permission id from module id
        /// </summary>
        /// <typeparam name="T">PermissionEnum</typeparam>
        /// <param name="description">Module id is mentioned as description in enum</param>
        /// <returns>Permission Id</returns>
        private int GetPermissionIdFromModuleId<T>(int description)
        {
            var permissionId = 0;
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute && attribute.Description == description.ToString(CultureInfo.InvariantCulture))
                {
#pragma warning disable CS8601 // Possible null reference assignment.
                    T enumValue = (T)field.GetValue(null);
#pragma warning restore CS8601 // Possible null reference assignment.
                    if (enumValue != null)
                    {
                        permissionId = Convert.ToInt32(enumValue, CultureInfo.InvariantCulture);
                        break;
                    }
                }
            }

            return permissionId;
        }

        /// <summary>
        /// Method to return the query of agent settings
        /// </summary>
        /// <returns>SQL query to get agent settings</returns>
        private string GetAgentSettingsDapperQuery()
        {
            return @"select agnt.has_all_brand_access, usr.agent_ticket_access_scope_id, 
                    timezoneDetails.id as timeZoneId, timezoneDetails.time_zone_id as windowsTimeZoneId,
                    timezoneDetails.offset as timeZoneOffset, timezoneDetails.standard_name as timeZoneName, timezoneDetails.short_code as timeZoneShortCode,
                    usr.language as languageId, 
                    CASE WHEN usr.settings->>'TicketLayoutId' = '' THEN 0 ELSE (usr.settings->>'TicketLayoutId')::integer END as ticketLayoutId,
                    CASE WHEN usr.settings->>'SortReplyId' = '' THEN 0 ELSE (usr.settings->>'SortReplyId')::integer END as sortReplyId,
                    CASE WHEN usr.settings->>'DefaultTicketViewId' = '' THEN 0 ELSE (usr.settings->>'DefaultTicketViewId')::integer END AS defaultTicketViewId,
                    CASE WHEN usr.settings->>'TicketPerPageCount' = '' THEN 0 ELSE (usr.settings->>'TicketPerPageCount')::integer END AS ticketPerPageCount,
                    CASE WHEN usr.settings->>'EnableShortcut' = '' THEN false ELSE (usr.settings->>'EnableShortcut')::boolean END AS enableShortcut,
                    usr.settings->'TicketFields' as ticketFields,
                    usr.settings->'Brands' as brands,
                    CASE WHEN usr.settings->>'DefaultMessageFilterId' = '' THEN 0 ELSE (usr.settings->>'DefaultMessageFilterId')::integer END AS defaultMessageFilterId,
                    usr.email as email,
                    usr.display_name as displayName,
                    usr.short_code as shortCode,
                    usr.color_code as colorCode,
                    languageDetails.short_code as languageShortCode
                    from users usr join agent agnt on usr.id=agnt.user_id
                    left join timezone timezoneDetails on usr.time_zone_id=timezoneDetails.id
                    left join language languageDetails on usr.language=languageDetails.id
                    where usr.id=" + user.UserId.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Method to return the query of agent permissions
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>SQL query to get agent permissions</returns>
        private string GetAgentPermissionsDapperQuery(string userId)
        {
            return @"select (select string_agg(permisn.id::text, ',' ORDER by permisn.id) from user_role_mapper userRoleMapper
                    join user_role userRole on userRoleMapper.role_id=userRole.id
                    join user_role_permission_mapper userRolePermissionMapper on userRoleMapper.role_id=userRolePermissionMapper.role_id
                    join permission permisn on userRolePermissionMapper.permission_id=permisn.id
                    join application_module appModule on permisn.module_id=appModule.id
                    join application_module_mapper applicationModuleMapper on appModule.id=applicationModuleMapper.application_module_id
                    where userRoleMapper.org_id=" + organization.OrgId.ToString(CultureInfo.InvariantCulture) + @"
                    and userRoleMapper.user_id=" + userId + @"
                    and appModule.is_enabled and appModule.is_active and userRoleMapper.is_active
                    and userRolePermissionMapper.is_active and permisn.is_active and userRole.is_active
                    and applicationModuleMapper.is_active and applicationModuleMapper.org_id=" + organization.OrgId.ToString(CultureInfo.InvariantCulture) + @"
                    and appModule.application_id=" + ((int)ApplicationTypeEnum.AgentPortal).ToString(CultureInfo.InvariantCulture) + @") as permissions
                    from users usr
                    where usr.id=" + userId + " and usr.user_status_id = " + ((int)UserStatusEnum.Active).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get availability of current user
        /// </summary>
        /// <returns>User availability</returns>
        private async Task<AgentAvailabilityStatusObject> GetUserAvailabilityAsync()
        {
            string userAvailabilityFromCache = await distributedCacheService.GetAsync("agent_availability_" + organization.OrgId.ToString(CultureInfo.InvariantCulture) + "_" + user.UserId.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(userAvailabilityFromCache) && !bool.TryParse(userAvailabilityFromCache, out bool availabilityStatus))
            {
                return JsonConvert.DeserializeObject<AgentAvailabilityStatusObject>(userAvailabilityFromCache) ?? new AgentAvailabilityStatusObject();
            }

            AgentAvailabilityStatusObject agentAvailabilityStatus = await (from agent in context.Agent
                                      join agentSupportChannelStatus in context.AgentAvailabilityStatus
                                      on agent.SupportChannelStatusId equals agentSupportChannelStatus.Id
                                      where agent.UserId == user.UserId && agent.IsActive
                                      select new AgentAvailabilityStatusObject
                                      {
                                          AgentSupportChannelAvailability = new AgentAvailabilityObject()
                                          {
                                              Id = agentSupportChannelStatus.Id,
                                              StatusCategoryId = agentSupportChannelStatus.AgentStatusCategoryId,
                                              ColorCode = agentSupportChannelStatus.ColorCode,
                                              Name = agentSupportChannelStatus.Name,
                                              StatusCategory = agentSupportChannelStatus.AgentStatusCategory.Name
                                          }
                                      }).FirstOrDefaultAsync().ConfigureAwait(false);

            agentAvailabilityStatus ??= new AgentAvailabilityStatusObject();
            string userAvailabilityString = JsonConvert.SerializeObject(agentAvailabilityStatus);
            await distributedCacheService.SetAsync("agent_availability_" + organization.OrgId.ToString(CultureInfo.InvariantCulture) + "_" + user.UserId.ToString(CultureInfo.InvariantCulture), userAvailabilityString, TimeSpan.FromMinutes(30)).ConfigureAwait(false);
            return agentAvailabilityStatus;
        }
    }
}
