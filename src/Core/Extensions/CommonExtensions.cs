// -----------------------------------------------------------------------
// <copyright file="FluentValidationExtensions.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation.Results;
    using Syncfusion.HelpDesk.Core.ValidationErrors;

    /// <summary>
    /// FluentValidation Extensions - Class Declaration
    /// </summary>
    public static partial class CommonExtensions
    {
        /// <summary>
        /// Creates a list of field errors from Fluent Validation Result Errors.
        /// </summary>
        /// <param name="errors">A collection of errors from <see cref="ValidationResult.Errors"/></param>
        /// <returns>A list contains elements of <see cref="FieldErrors"/>.</returns>
        public static List<FieldErrors> ToFieldErrors(this IList<ValidationFailure> errors)
        {
            return errors.Select(i => new FieldErrors(i.PropertyName, i.ErrorMessage)).ToList();
        }
    }
}
