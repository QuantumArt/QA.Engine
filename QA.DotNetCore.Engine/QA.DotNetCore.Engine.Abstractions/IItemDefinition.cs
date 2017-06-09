using System;

namespace QA.DotNetCore.Engine.Abstractions
{
    public interface IItemDefinition
    {
        string Discriminator { get; }

        Type Type { get; }
    }
}
