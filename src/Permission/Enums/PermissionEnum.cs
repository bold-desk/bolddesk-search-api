//-----------------------------------------------------------------------
// <copyright file="PermissionEnum.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Enums
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Enum of permissions
    /// </summary>
    public enum PermissionEnum
    {
        /// <summary>
        /// Permission handled inside the action
        /// </summary>
        HandledInsideMethod = -1,

        /// <summary>
        /// Any authenticated agent can access
        /// </summary>
        AnyAuthenticatedAgentCanAccess = 0,

        /// <summary>
        /// Permission to view contacts module
        /// </summary>
        [Display(GroupName = "Contacts")]
        ViewContactModule = 1,

        /// <summary>
        /// Permission to view admin module
        /// </summary>
        [Display(GroupName = "Admin")]
        ViewAdminModule = 2,

        /// <summary>
        /// Permission to reply ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        ReplyTicketViaAgentPortal = 3,

        /// <summary>
        /// Permission to edit or delete other user's private notes
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditOrDeleteOtherUsersPrivateNotes = 4,

        /// <summary>
        /// Permission to edit ticket fields
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditTicketFields = 5,

        /// <summary>
        /// Permission to change the requester of ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditRequester = 6,

        /// <summary>
        /// Permission to edit or delete other user's work log
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditOrDeleteOtherUsersWorklog = 7,

        /// <summary>
        /// Permission to create ticket views
        /// </summary>
        [Display(GroupName = "Tickets")]
        CreateTicketViews = 8,

        /// <summary>
        /// Permission to create private ticket views
        /// </summary>
        [Display(GroupName = "Tickets")]
        CreateTicketViewsPrivate = 9,

        /// <summary>
        /// Permission to create private and group ticket views
        /// </summary>
        [Display(GroupName = "Tickets")]
        CreateTicketViewsPrivateAndGroup = 10,

        /// <summary>
        /// Permission to create private, group, and public ticket views
        /// </summary>
        [Display(GroupName = "Tickets")]
        CreateTicketViewsPrivateGroupAndPublic = 11,

        /// <summary>
        /// Permission to add or remove other users as watcher
        /// </summary>
        [Display(GroupName = "Tickets")]
        AddOrRemoveOtherUsersAsWatcher = 12,

        /// <summary>
        /// Permission to manage spam tickets
        /// </summary>
        [Display(GroupName = "Tickets")]
        ManageTicketSpam = 13,

        /// <summary>
        /// Permission to manage deleted tickets
        /// </summary>
        [Display(GroupName = "Tickets")]
        ManageTicketDeletion = 14,

        /// <summary>
        /// Permission to manage suspended tickets
        /// </summary>
        [Display(GroupName = "Tickets")]
        ManageTicketSuspension = 15,

        /// <summary>
        /// Permission to create or edit contact
        /// </summary>
        [Display(GroupName = "Contacts")]
        CreateOrEditContact = 16,

        /// <summary>
        /// Permission to create or edit contact groups
        /// </summary>
        [Display(GroupName = "Contacts")]
        CreateOrEditContactGroups = 17,

        /// <summary>
        /// Permission to block or unblock contact and delete or mark as spam associated tickets
        /// </summary>
        [Display(GroupName = "Contacts")]
        BlockOrUnblockContactAndDeleteOrSpamAssociatedTickets = 18,

        /// <summary>
        /// Permission to delete or restore contact and delete or mark as spam associated tickets
        /// </summary>
        [Display(GroupName = "Contacts")]
        DeleteOrRestoreContactAndDeleteOrSpamAssociatedTickets = 19,

        /// <summary>
        /// Permission to delete contact group
        /// </summary>
        [Display(GroupName = "Contacts")]
        DeleteContactGroup = 20,

        /// <summary>
        /// Permission to manage ticket fields and forms
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageFieldsAndForms = 21,

        /// <summary>
        /// Permission to manage canned response
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageCannedResponse = 22,

        /// <summary>
        /// Permission to manage email notification and templates
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageEmailNotificationAndTemplates = 23,

        /// <summary>
        /// Permission to manage tags
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageTags = 24,

        /// <summary>
        /// Permission to manage auto assignments
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageAutoAssignments = 25,

        /// <summary>
        /// Permission to manage create and update triggers
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageCreateAndUpdateTriggers = 26,

        /// <summary>
        /// Permission to manage time triggers
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageTimeTriggers = 27,

        /// <summary>
        /// Permission to manage work schedules
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageWorkSchedules = 28,

        /// <summary>
        /// Permission to manage SLA
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageSLA = 29,

        /// <summary>
        /// Permission to manage agents
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageAgents = 30,

        /// <summary>
        /// Permission to manage groups
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageGroups = 31,

        /// <summary>
        /// Permission to manage roles and permission
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageRolesAndPermission = 32,

        /// <summary>
        /// Permission to manage brands
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageBrands = 33,

        /// <summary>
        /// Permission to view audit logs
        /// </summary>
        [Display(GroupName = "Admin")]
        ViewAuditLogs = 34,

        /// <summary>
        /// Permission to edit ticket subject
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditSubject = 35,

        /// <summary>
        /// Permission to create new tags for ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        CreateNewTagsForTicket = 36,

        /// <summary>
        /// Permission to export ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        ExportTicket = 37,

        /// <summary>
        /// Permission to remove tag from ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        RemoveTagFromTicket = 38,

        /// <summary>
        /// Permission to edit assignee field
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditAssignee = 39,

        /// <summary>
        /// Permission to edit priority field
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditPriority = 40,

        /// <summary>
        /// Permission to edit resolution due date field
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditResolutionDue = 41,

        /// <summary>
        /// Permission to change the ticket status to close
        /// </summary>
        [Display(GroupName = "Tickets")]
        ChangeTicketStatusToClose = 42,

        /// <summary>
        /// Permission to agent can reply ticket via email
        /// </summary>
        [Display(GroupName = "Tickets")]
        ReplyTicketViaEmail = 43,

        /// <summary>
        /// Permission to manage settings
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageSettings = 44,

        /// <summary>
        /// Permission to edit other fields
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditOtherFields = 45,

        /// <summary>
        /// Permission to manage ticket views
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageTicketViews = 46,

        /// <summary>
        /// Permission to impersonate contact
        /// </summary>
        [Display(GroupName = "Contacts")]
        ImpersonateContact = 47,

        /// <summary>
        /// Permission to add new contact at the time of ticket creation
        /// </summary>
        [Display(GroupName = "Tickets")]
        AddNewContactAtTheTimeOfTicketCreation = 48,

        /// <summary>
        /// Permission to view contact
        /// </summary>
        [Display(GroupName = "Contacts")]
        ViewContact = 49,

        /// <summary>
        /// Permission to view contact group
        /// </summary>
        [Display(GroupName = "Contacts")]
        ViewContactGroup = 50,

        /// <summary>
        /// Permission to manage webhook
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageWebhook = 51,

        /// <summary>
        /// Permission to manage API keys
        /// </summary>
        [Display(GroupName = "AgentProfile")]
        ManageAPIKeys = 52,

        /// <summary>
        /// Permission to manage apps
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageApps = 53,

        /// <summary>
        /// Permission to manage billing and subscription
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageBillingAndSubscription = 54,

        /// <summary>
        /// Permission to view reports module
        /// </summary>
        [Display(GroupName = "Reports")]
        ViewReportsModule = 55,

        /// <summary>
        /// Permission to create views
        /// </summary>
        [Display(GroupName = "Reports")]
        CreateViews = 56,

        /// <summary>
        /// Permission to create private views
        /// </summary>
        [Display(GroupName = "Reports")]
        CreateViewsPrivate = 57,

        /// <summary>
        /// Permission to create private and group views
        /// </summary>
        [Display(GroupName = "Reports")]
        CreateViewsPrivateAndGroup = 58,

        /// <summary>
        /// Permission to create private, group and public views
        /// </summary>
        [Display(GroupName = "Reports")]
        CreateViewsPrivateGroupAndPublic = 59,

        /// <summary>
        /// Permission to delete public comments
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeletePublicComments = 60,

        /// <summary>
        /// Permission to delete own comments
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeletePublicCommentsOwnComments = 61,

        /// <summary>
        /// Permission to delete own and any agent's comments
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeletePublicCommentsOwnAndAnyAgentComments = 62,

        /// <summary>
        /// Permission to delete own, any agent and customer's comments
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeletePublicCommentsOwnAnyAgentAndCustomerComments = 63,

        /// <summary>
        /// Permission to delete private notes
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeletePrivateNotes = 64,

        /// <summary>
        /// Permission to delete own private notes
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeletePrivateNotesOwnNotes = 65,

        /// <summary>
        /// Permission to delete own and any agent's private notes
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeletePrivateNotesOwnAndAnyAgentNotes = 66,

        /// <summary>
        /// Permission to delete own, any agent and customer's private notes
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeletePrivateNotesOwnAnyAgentAndCustomerNotes = 67,

        /// <summary>
        /// Permission to delete uploaded files
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeleteUploadedFiles = 68,

        /// <summary>
        /// Permission to delete own uploaded files
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeleteOwnUploadedFiles = 69,

        /// <summary>
        /// Permission to delete own and any agent's uploaded files
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeleteOwnAndAnyAgentUploadedFiles = 70,

        /// <summary>
        /// Permission to delete own, any agent and customer's uploaded files
        /// </summary>
        [Display(GroupName = "Tickets")]
        DeleteOwnAnyAgentAndCustomerUploadedFiles = 71,

        /// <summary>
        /// Permission to share ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        ShareTicket = 72,

        /// <summary>
        /// Permission to create article
        /// </summary>
        [Display(GroupName = "KB")]
        CreateArticle = 75,

        /// <summary>
        /// Permission to edit article
        /// </summary>
        [Display(GroupName = "KB")]
        EditArticle = 76,

        /// <summary>
        /// Permission to edit title
        /// </summary>
        [Display(GroupName = "KB")]
        EditTitle = 77,

        /// <summary>
        /// Permission to edit description
        /// </summary>
        [Display(GroupName = "KB")]
        EditDescription = 78,

        /// <summary>
        /// Permission to edit tags
        /// </summary>
        [Display(GroupName = "KB")]
        EditTags = 79,

        /// <summary>
        /// Permission to edit author
        /// </summary>
        [Display(GroupName = "KB")]
        EditAuthor = 80,

        /// <summary>
        /// Permission to edit SEO
        /// </summary>
        [Display(GroupName = "KB")]
        EditSEO = 81,

        /// <summary>
        /// Permission to edit visibility option
        /// </summary>
        [Display(GroupName = "KB")]
        EditVisibilityOption = 82,

        /// <summary>
        /// Permission to edit other fields
        /// </summary>
        [Display(GroupName = "KB")]
        EditOtherFieldsInKB = 83,

        /// <summary>
        /// Permission to publish/unpublish article
        /// </summary>
        [Display(GroupName = "KB")]
        PublishUnpublishArticle = 84,

        /// <summary>
        /// Permission to review article
        /// </summary>
        [Display(GroupName = "KB")]
        ReviewArticle = 85,

        /// <summary>
        /// Permission to delete article
        /// </summary>
        [Display(GroupName = "KB")]
        DeleteArticle = 86,

        /// <summary>
        /// Permission to restore article
        /// </summary>
        [Display(GroupName = "KB")]
        RestoreArticle = 87,

        /// <summary>
        /// Permission to restore version
        /// </summary>
        [Display(GroupName = "KB")]
        RestoreVersion = 88,

        /// <summary>
        /// Permission to update KB category
        /// </summary>
        [Display(GroupName = "KB")]
        KBCategory = 89,

        /// <summary>
        /// Permission to add KB category
        /// </summary>
        [Display(GroupName = "KB")]
        AddKBCategory = 90,

        /// <summary>
        /// Permission to edit KB category
        /// </summary>
        [Display(GroupName = "KB")]
        EditKBCategory = 91,

        /// <summary>
        /// Permission to delete KB category
        /// </summary>
        [Display(GroupName = "KB")]
        DeleteKBCategory = 92,

        /// <summary>
        /// Permission to update KB section
        /// </summary>
        [Display(GroupName = "KB")]
        KBSection = 93,

        /// <summary>
        /// Permission to add KB section
        /// </summary>
        [Display(GroupName = "KB")]
        AddKBSection = 94,

        /// <summary>
        /// Permission to edit KB section
        /// </summary>
        [Display(GroupName = "KB")]
        EditKBSection = 95,

        /// <summary>
        /// Permission to delete KB section
        /// </summary>
        [Display(GroupName = "KB")]
        DeleteKBSection = 96,

        /// <summary>
        /// Permission to update KB comments
        /// </summary>
        [Display(GroupName = "KB")]
        KBComments = 97,

        /// <summary>
        /// Permission to add KB comments
        /// </summary>
        [Display(GroupName = "KB")]
        AddKBComments = 98,

        /// <summary>
        /// Permission to edit KB comments
        /// </summary>
        [Display(GroupName = "KB")]
        EditKBComments = 99,

        /// <summary>
        /// Permission to delete KB comments
        /// </summary>
        [Display(GroupName = "KB")]
        DeleteKBComments = 100,

        /// <summary>
        /// Permission to update KB private note
        /// </summary>
        [Display(GroupName = "KB")]
        KBPrivateNote = 101,

        /// <summary>
        /// Permission to add KB private note
        /// </summary>
        [Display(GroupName = "KB")]
        AddKBPrivateNote = 102,

        /// <summary>
        /// Permission to edit KB private note
        /// </summary>
        [Display(GroupName = "KB")]
        EditKBPrivateNote = 103,

        /// <summary>
        /// Permission to delete KB private note
        /// </summary>
        [Display(GroupName = "KB")]
        DeleteKBPrivateNote = 104,

        /// <summary>
        /// Permission to update KB template
        /// </summary>
        [Display(GroupName = "KB")]
        KBTemplate = 105,

        /// <summary>
        /// Permission to add KB template
        /// </summary>
        [Display(GroupName = "KB")]
        AddKBTemplate = 106,

        /// <summary>
        /// Permission to edit KB template
        /// </summary>
        [Display(GroupName = "KB")]
        EditKBTemplate = 107,

        /// <summary>
        /// Permission to delete KB template
        /// </summary>
        [Display(GroupName = "KB")]
        DeleteKBTemplate = 108,

        /// <summary>
        /// Permission to export article
        /// </summary>
        [Display(GroupName = "KB")]
        ExportArticle = 109,

        /// <summary>
        /// Permission to edit public comments
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditPublicComments = 110,

        /// <summary>
        /// Permission to edit own comments
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditPublicCommentsOwnComments = 111,

        /// <summary>
        /// Permission to edit own and any agent's comments
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditPublicCommentsOwnAndAnyAgentComments = 112,

        /// <summary>
        /// Permission to edit own, any agent and customer's comments
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditPublicCommentsOwnAnyAgentAndCustomerComments = 113,

        /// <summary>
        /// Permission to edit private notes
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditPrivateNotes = 114,

        /// <summary>
        /// Permission to edit own private notes
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditPrivateNotesOwnNotes = 115,

        /// <summary>
        /// Permission to edit own and any agent's private notes
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditPrivateNotesOwnAndAnyAgentNotes = 116,

        /// <summary>
        /// Permission to merge tickets
        /// </summary>
        [Display(GroupName = "Tickets")]
        MergeTickets = 117,

        /// <summary>
        /// Permission to delete contact permanently
        /// </summary>
        [Display(GroupName = "Contacts")]
        DeleteContactPermanently = 118,

        /// <summary>
        /// Permission to lock or unlock ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        LockOrUnlockTicket = 119,

        /// <summary>
        /// Permission to unable to edit a locked ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        UnableToEditLockedTicket = 120,

        /// <summary>
        /// Permission to edit own locked ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        CanEditOwnLockedTicket = 121,

        /// <summary>
        /// Permission to edit any locked ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        CanEditAnyLockedTicket = 122,

        /// <summary>
        /// Permission to merge contact
        /// </summary>
        [Display(GroupName = "Contacts")]
        MergeContact = 123,

        /// <summary>
        /// Permission to bulk edit all fields
        /// </summary>
        [Display(GroupName = "Tickets")]
        BulkEditAllFields = 124,

        /// <summary>
        /// Permission to edit CC
        /// </summary>
        [Display(GroupName = "Tickets")]
        EditCc = 125,

        /// <summary>
        /// Permission to create new ticket via agent portal
        /// </summary>
        [Display(GroupName = "Tickets")]
        CreateTicketViaAgentPortal = 126,

        /// <summary>
        /// Permission to forward ticket
        /// </summary>
        [Display(GroupName = "Tickets")]
        ForwardTicket = 127,

        /// <summary>
        /// Permission to export contact
        /// </summary>
        [Display(GroupName = "Contacts")]
        ExportContact = 128,

        /// <summary>
        /// Permission to export contact group
        /// </summary>
        [Display(GroupName = "Contacts")]
        ExportContactGroup = 129,

        /// <summary>
        /// Permission to manage import
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageImport = 130,

        /// <summary>
        /// Permission to create contact views
        /// </summary>
        [Display(GroupName = "Contacts")]
        CreateContactViews = 131,

        /// <summary>
        /// Permission to create private contact views
        /// </summary>
        [Display(GroupName = "Contacts")]
        CreateContactViewsPrivate = 132,

        /// <summary>
        /// Permission to create private and group contact views
        /// </summary>
        [Display(GroupName = "Contacts")]
        CreateContactViewsPrivateAndGroup = 133,

        /// <summary>
        /// Permission to create private, group and public contact views
        /// </summary>
        [Display(GroupName = "Contacts")]
        CreateContactViewsPrivateGroupAndPublic = 134,

        /// <summary>
        /// Permission to manage agent availability status
        /// </summary>
        [Display(GroupName = "Admin")]
        ManageAgentAvailabilityStatus = 135,

        /// <summary>
        /// Permission to allow agent to change availability status
        /// </summary>
        [Display(GroupName = "AgentProfile")]
        AllowAgentToChangeAvailabilityStatus = 136,

        /// <summary>
        /// Permission to allow agent to change any availability status except offline
        /// </summary>
        [Display(GroupName = "AgentProfile")]
        AllowAgentToChangeAnyAvailabilityStatusExceptOffline = 137,

        /// <summary>
        /// Permission to allow agent to change any availability status
        /// </summary>
        [Display(GroupName = "AgentProfile")]
        AllowAgentToChangeAnyAvailabilityStatus = 138
    }
}