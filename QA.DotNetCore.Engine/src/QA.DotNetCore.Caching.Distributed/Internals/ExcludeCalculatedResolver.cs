using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace QA.DotNetCore.Caching.Distributed.Internals;

public class ExcludeCalculatedResolver : DefaultContractResolver
{
   protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(p => p.SetMethod != null)
            .Select(p => base.CreateProperty(p, memberSerialization))
            .ToList();
        props.ForEach(p => { p.Writable = true; p.Readable = true; });
        return props;
    }
}
