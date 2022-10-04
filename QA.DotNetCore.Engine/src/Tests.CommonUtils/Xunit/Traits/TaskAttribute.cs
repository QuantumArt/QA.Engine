using System;
using Xunit.Sdk;

namespace Tests.CommonUtils.Xunit.Traits;

/// <summary>
/// Apply this attribute to your test method to specify a task information.
/// </summary>
[TraitDiscoverer(nameof(TaskDiscoverer), Constants.AssemblyName)]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TaskAttribute : Attribute, ITraitAttribute
{
#pragma warning disable IDE0060 // Remove unused parameter
    public TaskAttribute(TaskSource source, int taskNumber)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }
}
