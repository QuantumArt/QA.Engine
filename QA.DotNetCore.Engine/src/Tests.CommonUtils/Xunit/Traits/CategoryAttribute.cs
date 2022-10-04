using System;
using Xunit.Sdk;

namespace Tests.CommonUtils.Xunit.Traits;

/// <summary>
/// Apply this attribute to your test method to specify a category.
/// </summary>
[TraitDiscoverer(nameof(CategoryDiscoverer), Constants.AssemblyName)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CategoryAttribute : Attribute, ITraitAttribute
{
#pragma warning disable IDE0060 // Remove unused parameter
    public CategoryAttribute(CategoryType category)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }
}
