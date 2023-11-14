//-----------------------------------------------------------------------
// <copyright file="Localizer.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Localization.Services
{
    using BoldDesk.Search.Localization.Resources;
    using Microsoft.Extensions.Localization;
    using Syncfusion.HelpDesk.Core.Extensions;
    using Syncfusion.HelpDesk.Core.Localization;

    /// <summary>
    /// Localizer Class.
    /// </summary>
    public class Localizer : ILocalizer
    {
        private readonly IStringLocalizer<Resource> localizer;
        private readonly IStringLocalizer<SharedResource> baseLocalizer;
        private readonly IStringLocalizer<WebhookPlaceholderResource> placeholderLocalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Localizer"/> class.
        /// </summary>
        /// <param name="localizer">String Localizer value.</param>
        /// <param name="baseLocalizer">Base String Localizer value.</param>
        /// <param name="placeholderLocalizer">Placeholder Localizer value</param>
        public Localizer(IStringLocalizer<Resource> localizer, IStringLocalizer<SharedResource> baseLocalizer, IStringLocalizer<WebhookPlaceholderResource> placeholderLocalizer)
        {
            this.localizer = localizer;
            this.baseLocalizer = baseLocalizer;
            this.placeholderLocalizer = placeholderLocalizer;
        }

        /// <summary>
        /// Get the value based on the given resource name from Localization.
        /// </summary>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the resource value.</returns>
        [System.Obsolete]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message", Justification = "OK")]
        public string GetLocalizerValue(string resourceName)
        {
            var resourceValue = localizer[resourceName];

            return resourceValue.ResourceNotFound ? localizer.WithCulture(new System.Globalization.CultureInfo("en-Us"))[resourceName] : resourceValue;
        }

        /// <summary>
        /// Get the value based on the given resource name from Localization.
        /// </summary>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the resource value.</returns>
        [System.Obsolete]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message", Justification = "OK")]
        public string GetLocalizerValueForUnitTest(string resourceName)
        {
            return localizer[resourceName];
        }

        /// <summary>
        /// Get the value based on the given resource name from Base Localization.
        /// </summary>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the resource value.</returns>
        [System.Obsolete]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message", Justification = "OK")]
        public string GetBaseLocalizerValue(string resourceName)
        {
            var resourceValue = baseLocalizer[resourceName];

            return resourceValue.ResourceNotFound ? baseLocalizer.WithCulture(new System.Globalization.CultureInfo("en-Us"))[resourceName] : resourceValue;
        }

        /// <summary>
        /// Returns the error message for string field whose length exceeds.
        /// </summary>
        /// <param name="lengthValue">Maximum characters allowed.</param>
        /// <returns>Returns the error message.</returns>
        [System.Obsolete]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message", Justification = "OK")]
        public string GetErrorMessageForStringLengthExceeds(int lengthValue)
        {
            var resourceValue = baseLocalizer[SharedResourceConstants.ErrorMessageForStringLengthExceeds];

            return resourceValue.ResourceNotFound ? baseLocalizer.WithCulture(new System.Globalization.CultureInfo("en-Us"))[SharedResourceConstants.ErrorMessageForStringLengthExceeds] + " " + lengthValue.ParseToString() : resourceValue + " " + lengthValue.ParseToString();
        }

        /// <summary>
        /// Returns the error message for string field whose length exceeds.
        /// </summary>
        /// <param name="langShortCode">Language Short Code.</param>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the error message.</returns>
        [System.Obsolete]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message", Justification = "OK")]
        public string GetLocalizerValueForSpecifiedLanguage(string langShortCode, string resourceName)
        {
            var resourceValue = localizer.WithCulture(new System.Globalization.CultureInfo(langShortCode))[resourceName];

            return resourceValue.ResourceNotFound ? localizer.WithCulture(new System.Globalization.CultureInfo("en-Us"))[resourceName] : resourceValue;
        }

        /// <summary>
        /// Returns the localized value for the specified language.
        /// </summary>
        /// <param name="langShortCode">Language Short Code.</param>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the error message.</returns>
        [System.Obsolete]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message", Justification = "OK")]
        public string GetBaseLocalizerValueForSpecifiedLanguage(string langShortCode, string resourceName)
        {
            var resourceValue = baseLocalizer.WithCulture(new System.Globalization.CultureInfo(langShortCode))[resourceName];

            return resourceValue.ResourceNotFound ? baseLocalizer.WithCulture(new System.Globalization.CultureInfo("en-Us"))[resourceName] : resourceValue;
        }

        /// <summary>
        /// Returns the webhook placeholder localized value for the specified language.
        /// </summary>
        /// <param name="langShortCode">Language Short Code.</param>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the placeholder localized value.</returns>
        [System.Obsolete]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message", Justification = "OK")]
        public string GetPlaceholderLocalizerValueForSpecifiedLanguage(string langShortCode, string resourceName)
        {
            var resourceValue = placeholderLocalizer.WithCulture(new System.Globalization.CultureInfo(langShortCode))[resourceName];

            return resourceValue.ResourceNotFound ? placeholderLocalizer.WithCulture(new System.Globalization.CultureInfo("en-Us"))[resourceName] : resourceValue;
        }
    }
}