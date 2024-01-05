//-----------------------------------------------------------------------
// <copyright file="DontLogError.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Services.Logger;

using Syncfusion.HelpDesk.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Logger class which do not log any errors.
/// </summary>
public class DontLogError : ILogs
{
    /// <summary>
    /// Log Error.
    /// </summary>
    /// <param name="logObjects">Log Objects.</param>
    public void LogError(LogObjects logObjects)
    {
    }

    /// <summary>
    /// Log Message.
    /// </summary>
    /// <param name="logMessageObjects">Log Message Objects.</param>
    public void LogMessageInExceptionless(LogMessageObjects logMessageObjects)
    {
    }

    /// <summary>
    /// Log Feature usage.
    /// </summary>
    /// <param name="logFeatureUsageObject">Log Feature Usage Objects.</param>
    public void LogFeatureUsageInExceptionless(LogFeatureUsageObject logFeatureUsageObject)
    {
    }
}
