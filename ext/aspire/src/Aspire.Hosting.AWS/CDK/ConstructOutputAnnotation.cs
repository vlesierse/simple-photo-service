// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon.CDK;
using Constructs;

namespace Aspire.Hosting.AWS.CDK;

internal sealed class ConstructOutputAnnotation<T>(string name, ConstructOutputDelegate<T> output)
    : IConstructModifierAnnotation, IConstructOutputAnnotation
    where T : IConstruct
{
    public string OutputName { get; } = name;

    public void ChangeConstruct(IConstruct construct)
    {
        if (construct is not Stack stack)
        {
            stack = construct.Node.Scopes.OfType<Stack>().FirstOrDefault() ?? throw new InvalidOperationException("Construct is not part of a Stack");
        }

        _ = new CfnOutput(stack, OutputName, new CfnOutputProps
        {
            Key = $"{construct.StackUniqueId()}{OutputName}",
            Value = output((T)construct)
        });
    }
}
