// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon.CDK;

namespace Aspire.Hosting.AWS.CDK;

/// <summary>
///
/// </summary>
public interface ICDKResource : IStackResource
{
    /// <summary>
    ///
    /// </summary>
    App App { get; }
}
