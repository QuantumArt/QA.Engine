using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions
{
    public interface ITypeFinder
    {
        Dictionary<string, Type> GetTypesOf<T>();
    }
}
