using System;

namespace QA.DotNetCore.Engine.QpData.Interfaces
{
    public interface IItemDefinition
    {
        string Discriminator { get; }

        Type Type { get; }
    }
}
