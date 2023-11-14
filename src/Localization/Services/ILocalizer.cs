//-----------------------------------------------------------------------
// <copyright file="ILocalizer.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Localization.Services
{
    /// <summary>
    /// Localizer Interface.
    /// </summary>
    public interface ILocalizer
    {
        /// <summary>
        /// Get the value based on the given resource name from Localization.
        /// </summary>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the resource value.</returns>
        string GetLocalizerValue(string resourceName);

        /// <summary>
        /// Get the value based on the given resource name from Localization.
        /// </summary>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the resource value.</returns>
        string GetLocalizerValueForUnitTest(string resourceName);

        /// <summary>
        /// Get the value based on the given resource name from Base Localization.
        /// </summary>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the resource value.</returns>
        string GetBaseLocalizerValue(string resourceName);

        /// <summary>
        /// Returns the error message for string field whose length exceeds.
        /// </summary>
        /// <param name="lengthValue">Maximum characters allowed.</param>
        /// <returns>Returns the error message.</returns>
        string GetErrorMessageForStringLengthExceeds(int lengthValue);

        /// <summary>
        /// Returns the error message for string field whose length exceeds.
        /// </summary>
        /// <param name="langShortCode">Language Short Code.</param>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the error message.</returns>
        public string GetLocalizerValueForSpecifiedLanguage(string langShortCode, string resourceName);

        /// <summary>
        /// Returns the localized value for the specified language.
        /// </summary>
        /// <param name="langShortCode">Language Short Code.</param>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the error message.</returns>
        public string GetBaseLocalizerValueForSpecifiedLanguage(string langShortCode, string resourceName);

        /// <summary>
        /// Returns the placeholder localized value for the specified language.
        /// </summary>
        /// <param name="langShortCode">Language Short Code.</param>
        /// <param name="resourceName">Resource Name.</param>
        /// <returns>Returns the placeholder localized value.</returns>
        public string GetPlaceholderLocalizerValueForSpecifiedLanguage(string langShortCode, string resourceName);
    }
}