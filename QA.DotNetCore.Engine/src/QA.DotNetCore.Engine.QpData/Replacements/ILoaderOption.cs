using System;

namespace QA.DotNetCore.Engine.QpData.Replacements
{
    public interface ILoaderOption
    {
        Type Type { get; }
        string PropertyName { get; }
        void AttachTo(Type type, string propertyName, int contentId, int baseContentId);
        string Process(IServiceProvider serviceProvider, string value);
    }
}
