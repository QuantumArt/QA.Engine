namespace QA.DotNetCore.Engine.QpData.Replacements
{
    public interface IQpUrlResolver
    {
        string UploadUrl(bool removeScheme = false);
        string UrlForImage(int contentId, string fieldName, bool removeScheme = false);
    }
}
