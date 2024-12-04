using System;

namespace QA.DotNetCore.Caching
{
    public record CacheTagModification
    {
        public string Name { get; private set; }
        public DateTime Modified { get; private set; }

        public CacheTagModification(string name, DateTime modified)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Modified = modified;
        }

        public override string ToString() => Name + " - " + Modified.ToLongTimeString();
    }
}
