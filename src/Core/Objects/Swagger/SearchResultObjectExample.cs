//-----------------------------------------------------------------------
// <copyright file="SearchResultObjectExample.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Objects.Swagger;

using BoldDesk.Search.Core.Objects.Search;
using Swashbuckle.AspNetCore.Filters;

/// <summary>
/// Agent Details Objects Example.
/// </summary>
public class SearchResultObjectExample : IExamplesProvider<SearchResultObject>
{
    /// <summary>
    /// GetExamples.
    /// </summary>
    /// <returns>Return class object.</returns>
    public SearchResultObject GetExamples()
    {
        return new SearchResultObject() { Result = new List<object>(), Count = 0, Took = 0 };
    }
}
